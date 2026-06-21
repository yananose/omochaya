// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTask.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Implements the centralized task manager engine and framework updater, controlling 
//   automated execution loops, lifecycle resolution, and task cancellation packing.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;

    // タスクマネージャ（アップデータ）
    /// <summary>Don't touch! Only for system.</summary>
    class TaskManager
    {
        // static

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskManager Shared { get; } = new TaskManager();

        /// <summary>Thrown internally when a task is forcibly freed or canceled.</summary>
        class CanceledException : Exception
        {
            public static readonly CanceledException Shared = new CanceledException();
            CanceledException() : base(Messages.Exceptions.TaskCanceled) { }
        }

        // inner classes
        struct TopArray
        {
            // fields
            int[] array;
            int count;

            // overrides
            public ref int this[int index] => ref this.array[index];

            // properties
            public readonly int Count => this.count;

            // methods
            public int Add(int offset)
            {
                var count = this.count;
                if (this.array == null) { Story.Pool.Create(ref this.array); }
                else if (0 < count && this.array[count - 1] == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
                else if (count == this.array.Length) { Story.Pool.Expand(ref this.array); }
                this.array[count] = offset;
                this.count = count + 1;
                return count;
            }
            public bool IsInRange(int index) => (uint)index < this.count;
            public void SetCount(int count) => this.count = count;
            public void Expand(int count) => Story.Pool.Expand(ref this.array, count);
        }

        // const
        public const int UPDATE_TYPE_MAIN = 0;
        public const int UPDATE_TYPE_LATE = 1 << (32 - 3); // なので rawOffset の有効範囲は 1 << 29 まで。
        public const int UPDATE_TYPE_FIXED = 1 << (32 - 2);
        const int UPDATE_TYPE_MASK = UPDATE_TYPE_LATE | UPDATE_TYPE_FIXED;

        // fields
        TopArray mainTopArray;
        TopArray lateTopArray;
        TopArray fixedTopArray;
        int frameCount;
        int updateOffset;
        int autoOffset;
        int runningIndex = -1;
        Exception runningException = null;
        Story.PoolMemory runningResult;
        Story.Pool.Id runningResultId;
        public int LastAwaitType; // 一番最後に設定された type。タスクが終了したときは参照しない。つまりゴミを気にする必要はない。

        // properties
        public bool IsRunningValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 <= runningIndex;
        }
        Story.Task UnsafeRunningTask
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsRunningValid) { return Story.Task.UnsafeCreate(runningIndex); }
                else { return default; }
            }
        }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public int AutoOffset => this.autoOffset;
        public int MainTopCount => this.mainTopArray.Count;
        public int LateTopCount => this.lateTopArray.Count;
        public int FixedTopCount => this.fixedTopArray.Count;
#endif

        // constructors
        TaskManager()
        {
            this.frameCount = Time.frameCount;
            this.updateOffset = -1;
            this.autoOffset = 0;
        }

        // methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int count)
        {
            this.mainTopArray.Expand(count);
            this.lateTopArray.Expand(count);
            this.fixedTopArray.Expand(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsManual(int offset) => this.autoOffset <= offset && offset < this.mainTopArray.Count;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TaskInfo GetRunningInfo()
        {
            if (runningIndex < 0) { return ref TaskInfo.Invalid; }
            else { return ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(runningIndex); }
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref TaskInfo2 GetRunningInfo2()
        {
            if (runningIndex < 0) { return ref TaskInfo2.Invalid; }
            else { return ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet2(runningIndex); }
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // ジェネリクスによるコードブロート防止のため明示的にインライン化しない
        public Story.Task Entry(in StateMachine stateMachine) // TaskMethodBuilder からのみ呼ばれる
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var id = pool.Alloc();
            var offset = this.mainTopArray.Add(id.Index) | UPDATE_TYPE_MAIN;
            pool.UnsafeGet(id.Index).Entry(in stateMachine, offset);
            pool.UnsafeGet2(id.Index).Entry(id.Index);
            FrameCheck();
            // Dev.Log($"entry - {Task.UnsafeCreate(id.Index)}");
            return new Story.Task(id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Entry をインライン化しないのでこっちはインライン化
        public Story.Task<R> Entry<R>(in StateMachine stateMachine) // TaskMethodBuilder からのみ呼ばれる
        {
            var task = Entry(stateMachine);
            return new Story.Task<R>(task);
        }

        void FrameCheck()
        {
            // if (0 <= this.updateOffset) { return; } // Time.frameCount が重いらしいので設定済みかもチェック → キャッシュされるようになったらしい
            var frameCount = Time.frameCount;
            if (this.frameCount == frameCount) { return; }
            this.frameCount = frameCount;
            this.updateOffset = this.autoOffset;
        }

        public void Update()
        {
            // 自動実行
            FrameCheck();
            ref var topArray = ref this.mainTopArray;
            InvokeArray(ref topArray, this.updateOffset, UPDATE_TYPE_MAIN);
            this.updateOffset = -1;

            // 自動タスクを詰める
            var end = this.autoOffset;
            var offset = CompactArray(ref topArray, end, UPDATE_TYPE_MAIN);
            if (offset < this.autoOffset) { this.autoOffset = offset; }
            var freeStart = this.autoOffset;

            // 自動タスクの範囲内に隙間がなかった場合、手動タスクの先頭から最初の隙間までの間を生存確認
            for (var i=end; i<offset; ++i) { UnsafeCheckManualTaskNecessity(topArray[i]); } // top が不要になってたらチェインごと自動タスクにする

            // 残りの手動タスクを詰めならが生存確認
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var start = Mathf.Max(end, offset);
            end = topArray.Count;
            for (var i=start; i<end; ++i)
            {
                var index = topArray[i];
                if (index < 0) { continue; }
                ref var info = ref pool.UnsafeGet(index);
                info.Offset = offset;
                topArray[offset++] = index;
                CheckManualTaskNecessity(ref info, index); // top が不要になってたらチェインごと自動タスクにする
            }
            topArray.SetCount(offset);

            // キャンセル用に移動したタスク（不要になった手動タスク）を解放
            // （finallyでタスクの追加や起動がされてもいいように最後に自動タスクの範囲で行う）
            var freeEnd = this.autoOffset;
            for (var i=freeStart; i<freeEnd; ++i)
            {
                var index = topArray[i];
                // チェインをたどりながらバラしてキャンセル処理する。
                // チェイン状態になってるのは MoveNext の呼び出し元が解放されたときなので残す必要はない。
                // finally で await してもそれまでの id は無効になる。
Dev.LoopBreak.Init();
                while (0 <= index) {
Dev.LoopBreak.Check(i.ToString());
                    index = UnsafeCancelOne(index); }
            }
            TryLogException();
        }

        public void LateUpdate() => SimpleUpdate(ref this.lateTopArray, UPDATE_TYPE_LATE);
        public void FixedUpdate() => SimpleUpdate(ref this.fixedTopArray, UPDATE_TYPE_FIXED);
        void SimpleUpdate(ref TopArray topArray, int updateType)
        {
            InvokeArray(ref topArray, topArray.Count, updateType);
            var count = CompactArray(ref topArray, topArray.Count, updateType);
            topArray.SetCount(count);
        }

        void InvokeArray(ref TopArray topArray, int rawOffset, int updateType)
        {
            while (0 < rawOffset)
            {
                var index = topArray[--rawOffset]; // タスクの実行順を後着優先にしたいので逆順
                if (index < 0) { continue; } // ほぼ false
                if (UnsafeInvoke(index))
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある
                {
                    if (LastAwaitType != updateType) { UnsafeSwitchUpdate(topArray[rawOffset]); }
                    continue;
                }
                TryLogException();
            }
        }
        void UnsafeSwitchUpdate(int topIndex)
        {
            Dev.Assert(0 <= topIndex);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var info = ref pool.UnsafeGet(topIndex);
            var toUpdateType = LastAwaitType;

            if (toUpdateType == UPDATE_TYPE_MAIN)
            {
                // 自動タスクの位置へ
                MakeAuto(ref info);
            }
            else
            {
                // 各 Update の最後へ
                var toRawOffset = GetTopArrayCore(toUpdateType).Add(-1);
                MoveIndex(ref info, toRawOffset | toUpdateType);
            }
        }

        int CompactArray(ref TopArray topArray, int endOffset, int updateType)
        {
            // 最初の隙間
            var offset = 0;
            var count = topArray.Count;
            while (offset < count)
            {
                var index = topArray[offset];
                if (index < 0) { break; }
                else { offset++; }
            }

            // 自動タスクを詰める
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            for (var i=offset+1; i<endOffset; ++i)
            {
                var index = topArray[i];
                if (index < 0) { continue; }
                pool.UnsafeGet(index).Offset = offset | updateType;
                topArray[offset++] = index;
            }
            return offset;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetTaskIndex(int offset) => GetTopArray(offset)[offset & ~UPDATE_TYPE_MASK];

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetTaskIndex(int offset, int index) => GetTopArray(offset)[offset & ~UPDATE_TYPE_MASK] = index;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref TopArray GetTopArray(int offset)
        {
            Dev.Assert(0 <= offset);
            return ref GetTopArrayCore(offset & UPDATE_TYPE_MASK);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref TopArray GetTopArrayCore(int updateType)
        {
            switch (updateType)
            {
                case UPDATE_TYPE_LATE : return ref this.lateTopArray;
                case UPDATE_TYPE_FIXED : return ref this.fixedTopArray;
                default : return ref this.mainTopArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void TryLogException()
        {
            // 受け皿のない例外があればログを出す
            if (this.runningException != null)
            {
                Dev.LogException(this.runningException);
                this.runningException = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UnsafeCheckManualTaskNecessity(int topIndex) => CheckManualTaskNecessity(ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex), topIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckManualTaskNecessity(ref TaskInfo topInfo, int topIndex)
        {
            if (topInfo.IsPinned) { return; }
            var node = Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet2(topIndex).ManualNode;
            if (node.IsEmpty)
            {
                if (!topInfo.IsOrphaned) { return; } // 完全に未使用ならここで IsOrphaned をチェックして孤立していれば削除する。未使用なのでチェインにはなってない。
            }
            else if (node.IsValid) { return; } // 一度でも MoveNext されていれば次の MoveNext もあるはずなので、IsOrphaned はそこでチェックする。仮に放置されても呼び出したタスクが消えれば消える

            // キャンセル用に自動タスク化
            MakeAuto(ref topInfo);
        }

        public bool Boot(Story.Task task)
        {
            Dev.Assert(task.IsValid, Messages.Exceptions.InvalidTaskOnBoot);

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);
            ref var rootInfo2 = ref pool.UnsafeGet2(rootIndex);

            TryKeep(ref rootInfo);

            var topIndex = rootInfo2.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.AlreadyBooted);

            // 自動実行状態へ
            MakeAuto(ref topInfo);

            // 実行
            if (Invoke(ref topInfo, topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「(自身以外も含めて)手動タスクの offset が変わっている」
                // 可能性がある
            {
                if (LastAwaitType != UPDATE_TYPE_MAIN) { UnsafeSwitchUpdate(pool.UnsafeGet2(rootIndex).Next); }
                return true;
            }

            TryLogException();

            return false;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void TryKeep(ref TaskInfo info)
        {
            if (info.Master is null)
            {
                ref var runningInfo = ref GetRunningInfo();
                if (runningInfo.IsPinned) // ピン留めされてるタスクから起動されたピン留めする
                {
                    info.IsPinned = true;
                    return;
                }
                var master = runningInfo.Master;
                Dev.Assert(!(master is null));
                info.Keep(master);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetSwapRawOffset()
        {
            // 自動タスクしかなかったら追加
            if (this.autoOffset == this.mainTopArray.Count) { this.autoOffset = this.mainTopArray.Add(-1); }
            // 最後が空いてたらそこを使う（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
            if (0 < this.autoOffset && this.mainTopArray[this.autoOffset - 1] == -1) // うまく行けば↑と合わせて２つ節約
            {
                return this.autoOffset - 1;
            }
            return this.autoOffset++;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void MakeAuto(ref TaskInfo topInfo)
        {
            if (this.autoOffset <= topInfo.Offset) // 手動タスク & Late & Fixed
            {
                MoveIndex(ref topInfo, GetSwapRawOffset() | UPDATE_TYPE_MAIN);
            }
        }

        void MoveIndex(ref TaskInfo topInfo, int toOffset) // topOffset ではない
        {
            var fromOffset = topInfo.Offset;
            if (fromOffset == toOffset) { return; }

            // 元の位置の情報
            Dev.Assert(0 <= fromOffset);
            var fromUpdateType = fromOffset & UPDATE_TYPE_MASK;
            ref var fromTopArray = ref GetTopArrayCore(fromUpdateType); // Addの後は参照しないように注意！
            var fromRawOffset = fromOffset & ~UPDATE_TYPE_MASK;
            var fromIndex = fromTopArray[fromRawOffset];

            // 移動先の情報
            Dev.Assert(0 <= toOffset);
            var toUpdateType = toOffset & UPDATE_TYPE_MASK;
            ref var toTopArray = ref GetTopArrayCore(toUpdateType); // Addの後は参照しないように注意！
            var toRawOffset = toOffset & ~UPDATE_TYPE_MASK;
            var toIndex = toTopArray[toRawOffset];

            // topInfo を to の位置に入れる
            topInfo.Offset = toOffset;
            toTopArray[toRawOffset] = fromIndex;

            // 同タイプなら
            if (fromUpdateType == toUpdateType)
            {
                // 入れ替える（to の位置にあったタスクを topInfo があった位置へ）
                fromTopArray[fromRawOffset] = toIndex;
                if (0 <= toIndex) { Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(toIndex).Offset = fromOffset; }
                return;
            }

            // topInfo があった位置は空に
            fromTopArray[fromRawOffset] = -1;

            // to の位置にタスクが存在していたらそれを末尾へ追加
            if (0 <= toIndex) { Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(toIndex).Offset = toTopArray.Add(toIndex) | toUpdateType; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(Story.Task task)
        {
            if (!task.IsValid) { return false; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);
            ref var rootInfo2 = ref pool.UnsafeGet2(rootIndex);

            var topIndex = rootInfo2.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.CannotMoveNextAutoTask);

            // 呼び出し元を設定（結局復元してる...）
            pool.UnsafeGet2(topIndex).ManualNode = IsRunningValid ? new Story.Task(pool.UnsafeGetId(this.runningIndex)) : default;

            // 実行
            if (Invoke(ref topInfo, topIndex)) { return true; } // 呼び出し元で await Yield してるはずなのでそこで AwaitType が上書きされる
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「(自身以外も含めて)手動タスクの offset が変わっている」
                // 可能性がある

            TryLogException();

            return false;
        }

        public bool IsNotCompleted(Story.Task task)
        {
            Dev.Assert(IsRunningValid);

            if (!task.IsValid) { return false; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);
            ref var rootInfo2 = ref pool.UnsafeGet2(rootIndex);

            TryKeep(ref rootInfo);

            var topIndex = rootInfo2.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.CannotAwaitAutoTask);

            // 実行
            if (Invoke(ref topInfo, topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「(自身以外も含めて)手動タスクの offset が変わっている」
                // 可能性がある
            {
                // 待たせる状態へ
                UnsafeLink(rootIndex, this.runningIndex);

                // Invoke 内で設定された AwaitType をそのまま呼び出し元へ渡す。呼び出し元が自動タスクならそこで SwitchUpdate される。
                // Boot で AwaitType が書き換わっていてもその後で await Yield されている（されてなければタスクが終了しているのでここにはこない）。

                // 継続
                return true;
            }

            // await なので、この後の GetResult で処理させる
            // TryLogException();

            return false;
        }

        void UnsafeLink(int prevRootIndex, int nextTopIndex)
        {
            // 必要な情報取得
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var prevRootInfo2 = ref pool.UnsafeGet2(prevRootIndex);
            ref var nextTopInfo2 = ref pool.UnsafeGet2(nextTopIndex);
            var prevTopIndex = prevRootInfo2.Next;
            var nextRootIndex = nextTopInfo2.Prev;

            // 隣同士をつなぐ
            prevRootInfo2.Next = nextTopIndex;
            nextTopInfo2.Prev = prevRootIndex;

            // リングをつなぐ
            pool.UnsafeGet2(prevTopIndex).Prev = nextRootIndex;
            pool.UnsafeGet2(nextRootIndex).Next = prevTopIndex;

            // topArray の繋ぎ変え：情報取得
            ref var prevTopInfo = ref pool.UnsafeGet(prevTopIndex);
            ref var nextTopInfo = ref pool.UnsafeGet(nextTopIndex);
            Dev.Assert(prevTopInfo.HasOffset, string.Format(Messages.Exceptions.DoubleAwait, pool.UnsafeGet(prevRootIndex).GetMethodName(), nextTopInfo.GetMethodName()));
            Dev.Assert(nextTopInfo.HasOffset, string.Format(Messages.Exceptions.AwaitingWhileAwaited, pool.UnsafeGet(prevRootIndex).GetMethodName(), nextTopInfo.GetMethodName()));
            var prevOffset = prevTopInfo.Offset;
            var nextOffset = nextTopInfo.Offset;

            // topArray の繋ぎ変え：実行
            SetTaskIndex(prevOffset, -1);
            SetTaskIndex(nextOffset, prevTopIndex);
            prevTopInfo.Offset = nextOffset;
            nextTopInfo.Offset = -1;

            // 待機状態に
            nextTopInfo.IsWaiting = true;

            // 生存チェック情報を先頭へ反映
            prevTopInfo.IsPinned = nextTopInfo.IsPinned;
            if (IsManual(nextOffset)) { pool.UnsafeGet2(prevTopIndex).ManualNode = pool.UnsafeGet2(nextTopIndex).ManualNode; }
        }

        int UnsafeUnlink(int index) // 削除前提
        {
            // 必要な情報取得
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var info = ref pool.UnsafeGet(index);
            ref var info2 = ref pool.UnsafeGet2(index);
            var offset = info.Offset;
            int prevIndex = info2.Prev;

            // 単体なら topArray から外すだけ
            if (prevIndex == index)
            {
                SetTaskIndex(offset, -1);
                return -1;
            }

            // リングから外す
            int nextIndex = info2.Next;
            ref var prevInfo2 = ref pool.UnsafeGet2(prevIndex);
            ref var nextInfo2 = ref pool.UnsafeGet2(nextIndex);
            prevInfo2.Next = nextIndex;
            nextInfo2.Prev = prevIndex;

            // Invoke連鎖で次に実行すべき対象（それ以外から呼ばれたときは不要）
            ref var nextInfo = ref pool.UnsafeGet(nextIndex);
            var ret = nextInfo.HasOffset ? -1 : nextIndex;

            // 先頭の責務
            if (0 <= offset)
            {
                // topArtray に反映
                SetTaskIndex(offset, nextIndex);
                nextInfo.Offset = offset;

                // 生存チェック情報を渡す
                nextInfo.IsPinned = info.IsPinned;
                if (IsManual(offset)) { nextInfo2.ManualNode = info2.ManualNode; }
            }

            return ret;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult()
        {
            Dev.Assert(this.IsRunningValid);
            this.GetRunningInfo().IsRunning = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetResult<R>(R result)
        {
            SetResult();

            // 結果格納
            if (this.runningResult.IsValid)
            {
                Dev.LogWarning(string.Format(Messages.Warnings.ResultOverwritten, new Story.Task(this.runningResultId)));
                this.runningResult.Free();
            }
            this.runningResultId = this.UnsafeRunningTask.Id;
            this.runningResult = Story.PoolMemory.Alloc<R>(in result);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void SetException(Exception e)
        {
            SetResult();

            // 例外格納
            TryLogException();
            if (e != CanceledException.Shared) { this.runningException = e; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetResult()
        {
            var e = this.runningException;
            if (e != null)
            {
                this.runningException = null;
                if (e == CanceledException.Shared) { throw e; }
                else { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw(); }
            }

            // 必要？
            Dev.Assert(this.IsRunningValid);
            GetRunningInfo().IsWaiting = false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R GetResult<R>(Story.Task task)
        {
            R result = default;
            if (this.runningResultId.Matches(task.Id))
            {
                result = this.runningResult.Get<R>();
                this.runningResult.Free();
                this.runningResult = default;
                this.runningResultId = default;
            }
            else { Dev.LogWarning(string.Format(Messages.Warnings.ResultNotFound, task)); }
            GetResult();
            return result;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool UnsafeInvoke(int index) => Invoke(ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(index), index);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool Invoke(ref TaskInfo info, int index)
        {
            Dev.Assert(info.HasOffset);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(info.GetMethodName());

                if (!info.IsPinned && info.IsOrphaned) { index = CancelOne(ref info, index); } // これ以降 info 参照禁止
                else if (InvokeCore(ref info, index)) { return true; } // Yield 時
                else
                {
                    index = UnsafeFreeCore(index); }
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある

                // 全て実行し終わった
                if (index < 0) { return false; }

                // 次へ
                info = ref pool.UnsafeGet(index);
            }
        }

        bool InvokeCore(ref TaskInfo info, int index)
        {
            if (info.IsRunning) // ここが true になることはないはずだが...
            {
                Dev.LogError(string.Format(Messages.Warnings.RecursiveInvokeIgnored, info.GetMethodName()));
                if (this.runningException == CanceledException.Shared)
                {
                    Dev.LogWarning(Messages.Warnings.CancelPending);
                    this.runningException = null;
                    info.WillCancel = true;
                }
                return true; // 継続
            }

            var prev = this.runningIndex;
            this.runningIndex = index;

            info.IsRunning = true;
            if (this.runningException != null || !info.IsPaused)
            {
                info.Run();
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある
                info = ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(index);
            }

            var isNotCompleted = info.IsRunning;
            info.IsRunning = false;

            if (info.WillCancel)
            {
                info.WillCancel = false;
                TryLogException();
                if (isNotCompleted)
                {
                    Dev.Log(string.Format(Messages.Warnings.CancelExecutedAfterRun, info.GetMethodName()));
                    this.runningException = CanceledException.Shared;

                    info.IsRunning = true;
                    info.Run();
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある

                    info = ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(index);
                    isNotCompleted = info.IsRunning;
                    info.IsRunning = false;
                }
                else { Dev.Log(string.Format(Messages.Warnings.CancelAbortedFinishedTask, info.GetMethodName())); }
            }

            this.runningIndex = prev;
            return isNotCompleted;
        }

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int UnsafeFreeCore(int index)
        {
            // Dev.Log($"free - {Task.UnsafeCreate(topIndex)}");
            var nextIndex = UnsafeUnlink(index);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            pool.UnsafeGet(index).Free();
            pool.UnsafeGet2(index).Free();
            pool.UnsafeFree(index);
            return nextIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int UnsafeCancelOne(int topIndex) => CancelOne(ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex), topIndex);
        int CancelOne(ref TaskInfo topInfo, int topIndex)
        {
            // InvokeCore と同様にこのメソッドを呼ぶと以下の可能性がある
            // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
            // 「(自身以外も含めて)手動タスクの offset が変わっている」

            // 実行中なので終わってからキャンセルする
            if (topInfo.IsRunning)
            {
                topInfo.WillCancel = true;
                Dev.Log(string.Format(Messages.Warnings.CancelPendingWhileRunning, topInfo.GetMethodName()));
                // キャンセル準備（単体で切り離して自動タスク化）だけはしておく
                return UnsafePrepareCancel(topIndex);
            }

            // 他を待ってない（初期状態）ならそのまま解放（多分このケースはないが一応）
            if (!topInfo.IsWaiting)
            {
                var offset = topInfo.Offset;
                UnsafeFreeCore(topIndex);
                return GetTaskIndex(offset);
            }

            // キャンセル準備
            var nextIndex = UnsafePrepareCancel(topIndex);
            if (this.runningException != null) { Dev.LogException(this.runningException); }
            this.runningException = CanceledException.Shared;

            // キャンセル実行
            if (InvokeCore(ref topInfo, topIndex))
            {
                var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
                pool.UnsafeReborn(topIndex); // これまでの id を無効にする（これ以降、外部からこのタスクへはアクセスできない）
                Dev.LogWarning(string.Format(Messages.Warnings.CancelFailedTaskRunning, pool.UnsafeGet(topIndex).GetMethodName()));
            }
            else
            {
                UnsafeFreeCore(topIndex); // nextIndex に受け取っちゃダメ
            }

            return nextIndex;
        }

        int UnsafePrepareCancel(int index)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var info = ref pool.UnsafeGet(index);
            ref var info2 = ref pool.UnsafeGet2(index);
            var offset = info.Offset;
            int nextIndex = info2.Next;

            if (index == nextIndex) // 単独
            {
                nextIndex = -1;
                MakeAuto(ref info);
            }
            else
            {
                // topInfo を単体で切り離して（topInfoが宙ぶらりんになる）
                UnsafeUnlink(index);
                info2.Next = index; // PopTopTask は削除前提なので全メンバが更新されない。topInfoはこの後も使うので更新しておく。

                // 自動タスクへ
                var swapRawOffset = GetSwapRawOffset(); // topInfo の移動先
                var swapIndex = this.mainTopArray[swapRawOffset];
                if (0 <= swapIndex) { pool.UnsafeGet(swapIndex).Offset = this.mainTopArray.Add(swapIndex) | UPDATE_TYPE_MAIN; } // 移動先にタスクがあればそれを末尾に移動
                this.mainTopArray[swapRawOffset] = index;
                info.Offset = swapRawOffset | UPDATE_TYPE_MAIN;
            }
            info.IsPinned = true;
            return nextIndex;
        }

        public void Free(Story.Task task)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            if (!pool.IsValid(task.Id)) { return; }

            var index = task.Id.Index;
            ref var info = ref pool.UnsafeGet(index);

            // 先頭から
Dev.LoopBreak.Init();
            while (!info.HasOffset)
            {
Dev.LoopBreak.Check(task.ToString());
                index = pool.UnsafeGet2(index).Prev;
                info = ref pool.UnsafeGet(index);
            }

            // 自身まで順番にキャンセルする
Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(task.ToString());
                var nextIndex = UnsafeCancelOne(index);
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある
                if (index == task.Id.Index) { break; }
                index = nextIndex;
            }
        }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public int MainIndexForDebug(int rawOffset) => this.mainTopArray[rawOffset];
        public int LateIndexForDebug(int rawOffset) => this.lateTopArray[rawOffset];
        public int FixedIndexForDebug(int rawOffset) => this.fixedTopArray[rawOffset];
        public bool IsAutoForDebug(int offset) => offset < this.autoOffset;
#endif

    }
}
