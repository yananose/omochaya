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
            public int Add(int index)
            {
                var count = this.count;
                if (this.array == null) { Story.Pool.Create(ref this.array); }
                else if (0 < count && this.array[count - 1] == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
                else if (count == this.array.Length) { Story.Pool.Expand(ref this.array); }
                this.array[count] = index;
                this.count = count + 1;
                return count;
            }
            public void SetCount(int count) => this.count = count;
            public void Expand(int count) => Story.Pool.Expand(ref this.array, count);
        }
        struct ManualTopArray
        {
            // inner classes
            public struct Slot
            {
                public Story.Task Caller;
                public int Index;
                // 4 バイトの空き
            }

            // fields
            Slot[] array;
            int count;

            // overrides
            public ref Slot this[int index] => ref this.array[index];

            // properties
            public readonly int Count => this.count;

            // methods
            public int Add(int index)
            {
                var count = this.count;
                if (this.array == null) { Story.Pool.Create(ref this.array); }
                else if (0 < count && this.array[count - 1].Index == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
                else if (count == this.array.Length) { Story.Pool.Expand(ref this.array); }
                this.array[count] = new Slot { Index = index };
                this.count = count + 1;
                return count;
            }
            public void SetCount(int count) => this.count = count;
            public void Expand(int count) => Story.Pool.Expand(ref this.array, count);
        }

        // const
        const int UPDATE_TYPE_SHIFT = 32 - 3; // なので rawOffset の有効範囲は 1 << 29 まで。
        public const int UPDATE_TYPE_MASK = -1 << UPDATE_TYPE_SHIFT;
        public const int UPDATE_TYPE_MANUAL = 0 << UPDATE_TYPE_SHIFT; // 手動更新 & 初期位置
        public const int UPDATE_TYPE_AUTO = 1 << UPDATE_TYPE_SHIFT; // Update更新 & 削除位置
        public const int UPDATE_TYPE_LATE = 2 << UPDATE_TYPE_SHIFT; // LateUpdate更新
        public const int UPDATE_TYPE_FIXED = 3 << UPDATE_TYPE_SHIFT; // FixedUpdate更新

        // fields
        ManualTopArray manualTopArray;
        TopArray autoTopArray;
        TopArray lateTopArray;
        TopArray fixedTopArray;
        int frameCount;
        int updateOffset;
        int runningIndex = -1;
        Exception runningException = null;
        Story.PoolMemory runningResult;
        public int LastAwaitType; // 一番最後に設定された type。タスクが終了したときは参照しない。つまりゴミを気にする必要はない。
        public bool IsResultError;

        // properties
        public bool IsRunningValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 <= runningIndex;
        }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public int ManualTopCount => this.manualTopArray.Count;
        public int AutoTopCount => this.autoTopArray.Count;
        public int LateTopCount => this.lateTopArray.Count;
        public int FixedTopCount => this.fixedTopArray.Count;
#endif

        // constructors
        TaskManager()
        {
            this.frameCount = Time.frameCount;
            this.updateOffset = -1;
        }

        // methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int count)
        {
            this.manualTopArray.Expand(count);
            this.autoTopArray.Expand(count);
            this.lateTopArray.Expand(count);
            this.fixedTopArray.Expand(count);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsManual(int offset) => (offset & UPDATE_TYPE_MASK) == 0;

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
            var offset = this.manualTopArray.Add(id.Index) | UPDATE_TYPE_MANUAL;
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
            this.updateOffset = this.autoTopArray.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            // auto：実行
            FrameCheck();
            TopArrayInvoke(ref this.autoTopArray, UPDATE_TYPE_AUTO, this.updateOffset);
            this.updateOffset = -1;

            // auto：詰める
            TopArrayCompact(ref this.autoTopArray, UPDATE_TYPE_AUTO);

            // manual：生存チェックしながら詰める
            TopArrayCheckNecessity(ref this.manualTopArray, UPDATE_TYPE_MANUAL);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateUpdate() => SimpleUpdate(ref this.lateTopArray, UPDATE_TYPE_LATE);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FixedUpdate() => SimpleUpdate(ref this.fixedTopArray, UPDATE_TYPE_FIXED);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SimpleUpdate(ref TopArray topArray, int updateType)
        {
            TopArrayInvoke(ref topArray, updateType, topArray.Count);
            TopArrayCompact(ref topArray, updateType);
        }

        void TopArrayInvoke(ref TopArray topArray, int updateType, int rawOffset)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            while (0 < rawOffset)
            {
                var topIndex = topArray[--rawOffset]; // タスクの実行順を後着優先にしたいので逆順
                if (topIndex < 0) { continue; } // ほぼ false
                if (UnsafeInvokeChain(topIndex))
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 可能性がある
                {
                    // また、topIndex が指すタスクは
                    // 「await Story.Task して top ではなくなっている」
                    // 可能性がある

                    // switch する（ TopArraySwitch と同じだが最速で回すためにベタ書き）
                    if (LastAwaitType != updateType)
                    {
                        var index = topArray[rawOffset];
                        topArray[rawOffset] = -1;
                        TopArrayAdd(ref pool.UnsafeGet(index), index, LastAwaitType);
                    }
                }
                else
                {
                    // また、解放されてなければ topIndex が指すタスクは
                    // 「孤立して自動タスク化する」
                    // 加えて
                    // 「await Story.Task して top ではなくなっている」
                    // 可能性がある
                }
            }
        }

        void TopArrayCompact(ref TopArray topArray, int updateType)
        {
            // 最初の隙間
            var rawOffset = 0;
            var end = topArray.Count;
            while (rawOffset < end)
            {
                var index = topArray[rawOffset];
                if (index < 0) { break; }
                else { rawOffset++; }
            }

            // 自動タスクを詰める
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            for (var i=rawOffset+1; i<end; ++i)
            {
                var index = topArray[i];
                if (index < 0) { continue; }
                pool.UnsafeGet(index).Offset = rawOffset | updateType;
                topArray[rawOffset++] = index;
            }
            topArray.SetCount(rawOffset);
        }

        void TopArrayCheckNecessity(ref ManualTopArray topArray, int updateType)
        {
            var rawOffset = 0;
            var end = topArray.Count;
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            for (var i=0; i<end; ++i)
            {
                ref var slot = ref topArray[i]; // 微妙なサイズ
                if (UnsafeCheckNecessity(slot.Index, slot.Caller)) { continue; }
                pool.UnsafeGet(slot.Index).Offset = rawOffset | updateType;
                topArray[rawOffset++] = slot;
            }
            topArray.SetCount(rawOffset);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool UnsafeCheckNecessity(int topIndex, Story.Task caller)
        {
            if (topIndex < 0) { return true; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            if (caller.IsEmpty)
            {
                if (!pool.UnsafeGet(topIndex).IsOrphaned) { return false; } // 完全に未使用ならここで IsOrphaned をチェックして孤立していれば削除する。未使用なのでチェインにはなってない。
            }
            else if (caller.IsValid) { return false; } // 一度でも MoveNext されていれば次の MoveNext もあるはずなので、IsOrphaned はそこでチェックする。仮に放置されても呼び出したタスクが消えれば消える

            // チェインをたどりながらバラしてキャンセル処理する。
            // チェイン状態になってるのは MoveNext の呼び出し元が解放されたときなので残す必要はない。
            // finally で await してもそれまでの id は無効になる。
            var offset = pool.UnsafeGet(topIndex).Offset;
Dev.LoopBreak.Init();
            while (0 <= topIndex) {
Dev.LoopBreak.Check(topIndex.ToString());
                UnsafeCancel(topIndex);
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 可能性がある。また、解放されてなければ
                    // 「孤立して自動タスク化する」
                    // さらに
                    // 「await Story.Task して top ではなくなっている」
                    // 可能性がある

                topIndex = TopArrayGet(offset);
            }

            return true;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void TopArraySwitch(int fromOffset, int toUpdateType)
        {
            if ((fromOffset & UPDATE_TYPE_MASK) == toUpdateType) { return; }
            var index = TopArrayGet(fromOffset);
            ref var info = ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(index);
            TopArrayAdd(ref info, index, toUpdateType);
            TopArraySet(fromOffset, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void TopArrayAdd(ref TaskInfo info, int index, int toUpdateType)
        {
            var toOffset = GetTopArrayCore(toUpdateType).Add(-1) | toUpdateType;
            info.Offset = toOffset;
            TopArraySet(toOffset, index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int TopArrayGet(int offset)
        {
            if ((offset & UPDATE_TYPE_MASK) == UPDATE_TYPE_MANUAL) { return this.manualTopArray[offset].Index; }
            else { return GetTopArray(offset)[offset & ~UPDATE_TYPE_MASK]; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void TopArraySet(int offset, int index)
        {
            if ((offset & UPDATE_TYPE_MASK) == UPDATE_TYPE_MANUAL) { this.manualTopArray[offset].Index = index; }
            else { GetTopArray(offset)[offset & ~UPDATE_TYPE_MASK] = index; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref TopArray GetTopArray(int offset)
        {
            Dev.Assert(0 <= offset);
            return ref GetTopArrayCore(offset & UPDATE_TYPE_MASK);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        ref TopArray GetTopArrayCore(int updateType)
        {
            Dev.Assert(updateType != UPDATE_TYPE_MANUAL);
            switch (updateType)
            {
                case UPDATE_TYPE_LATE : return ref this.lateTopArray;
                case UPDATE_TYPE_FIXED : return ref this.fixedTopArray;
                default : return ref this.autoTopArray;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CaptureResult()
        {
            // 受け皿のない結果があれば解放する
            if (this.runningResult.IsValid)
            {
                Dev.Log($"結果が受け取られませんでした：{this.runningResult}");
                this.runningResult.Free();
                this.runningResult = default;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CaptureException()
        {
            // 受け皿のない例外があればログを出す
            if (this.runningException != null)
            {
                Dev.LogException(this.runningException);
                this.runningException = null;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void TryThrowException()
        {
            var e = this.runningException;
            if (e != null)
            {
                this.runningException = null;
                Dev.Assert(e != CanceledException.Shared);
                System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw();
            }
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
            TopArraySwitch(topInfo.Offset, UPDATE_TYPE_AUTO);
            var fromOffset = topInfo.Offset;

            // 実行
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // switch する
                TopArraySwitch(fromOffset, LastAwaitType);

                return true;
            }
            else
            {
                // また、解放されてなければ topIndex が指すタスクは
                // 「孤立して自動タスク化する」
                // 加えて
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 例外が残っていたら投げる
                TryThrowException();

                return false;
            }
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
            this.manualTopArray[topInfo.Offset].Caller = IsRunningValid ? new Story.Task(pool.UnsafeGetId(this.runningIndex)) : default;

            // 実行
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // ここでは switch しない（呼び出し元で await Yield して上書きされるハズなので）
                // TopArraySwitch(offset, LastAwaitType);

                return true;
            }
            else
            {
                // また、解放されてなければ topIndex が指すタスクは
                // 「孤立して自動タスク化する」
                // 加えて
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 例外が残っていたら投げる
                TryThrowException();

                return false;
            }
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
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 待たせる状態へ
                UnsafeLink(rootIndex, this.runningIndex);

                // ここでは switch しない（呼び出し元で switch させる）
                // TopArraySwitch(offset, LastAwaitType);

                return true;
            }
            else
            {
                // また、解放されてなければ topIndex が指すタスクは
                // 「孤立して自動タスク化する」
                // 加えて
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                return false;
            }
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
            TopArraySet(prevOffset, -1);
            TopArraySet(nextOffset, prevTopIndex);
            prevTopInfo.Offset = nextOffset;
            nextTopInfo.Offset = -1;

            // 生存チェック情報を先頭へ反映
            prevTopInfo.IsPinned = nextTopInfo.IsPinned;
        }

        void UnsafeUnlink(int index) // 削除前提。配列拡張などは発生しない
        {
            // 必要な情報取得
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var info = ref pool.UnsafeGet(index);
            ref var info2 = ref pool.UnsafeGet2(index);
            var offset = info.Offset;

            // 単体なら topArray から外すだけ
            if (info2.Next == index)
            {
                TopArraySet(offset, -1);
                return;
            }

            // リングから外す
            var prevIndex = info2.Prev;
            var nextIndex = info2.Next;
            ref var prevInfo2 = ref pool.UnsafeGet2(prevIndex);
            ref var nextInfo2 = ref pool.UnsafeGet2(nextIndex);
            prevInfo2.Next = nextIndex;
            nextInfo2.Prev = prevIndex;

            // 先頭じゃないならここまで
            if (offset < 0) { return; }

            // 次を topArtray に設定する
            TopArraySet(offset, nextIndex);
            ref var nextInfo = ref pool.UnsafeGet(nextIndex);
            nextInfo.Offset = offset;

            // 次へ生存チェック情報を渡す
            nextInfo.IsPinned = info.IsPinned;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UnsafeFreeCore(int index)
        {
            UnsafeUnlink(index);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            pool.UnsafeGet(index).Free();
            pool.UnsafeGet2(index).Free();
            pool.UnsafeFree(index);
        }


        public void SetResult()
        {
            Dev.Assert(this.IsRunningValid);
            this.GetRunningInfo().IsRunning = false;
        }

        public void SetResult<R>(R result)
        {
            SetResult();

            // 結果格納
            this.runningResult = Story.PoolMemory.Alloc<R>(in result);
        }

        public void SetException(Exception e)
        {
            SetResult();

            // 例外格納
            if (e != CanceledException.Shared)
            {
                CaptureException();
                this.runningException = e;
            }
        }

        public void GetResult()
        {
            CaptureResult();
            var e = this.runningException;
            if (e != null)
            {
                this.runningException = null;
                // ここで投げた例外は（利用者に握りつぶされなければ）SetException で受け取る
                if (e == CanceledException.Shared) { throw e; }
                else { System.Runtime.ExceptionServices.ExceptionDispatchInfo.Capture(e).Throw(); }
            }
        }

        public R GetResult<R>()
        {
            R result = default;
            IsResultError = this.runningResult.IsFailed<R>();
            if (!IsResultError)
            {
                result = this.runningResult.Get<R>();
                this.runningResult.Free();
                this.runningResult = default;
            }

            GetResult();

            return result;
        }

        // この中では
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
        // 「await Story.Task して top ではなくなっている」
        // 「孤立して自動タスク化する」
        // 可能性がある
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool UnsafeInvokeChain(int topIndex)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var topInfo = ref pool.UnsafeGet(topIndex);
            var offset = topInfo.Offset;
            Dev.Assert(0 <= offset);

            // 初期化
            CaptureResult();
            CaptureException();

Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(topInfo.GetMethodName());

                Dev.Assert(!topInfo.IsRunning);

                if (topInfo.IsOrphaned)
                {
                    UnsafeCancel(topIndex);
                        // ここで
                        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
                        // 「孤立して自動タスク化する」
                        // 加えて
                        // 「await Story.Task して top ではなくなっている」
                        // 可能性がある
                }
                else
                {
                    if (Invoke(ref topInfo, topIndex))
                    {
                        // ここで
                        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                        // 可能性がある。また、topIndex が指すタスクは
                        // 「await Story.Task して top ではなくなっている」
                        // 可能性がある

                        // ここでは switch しない（呼び出し元まで伝播させる）
                        // TopArraySwitch(offset, LastAwaitType);

                         return true;
                    }
                    else
                    {
                        // ここで
                        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
                        // 「孤立して自動タスク化する」
                        // 加えて
                        // 「await Story.Task して top ではなくなっている」
                        // 可能性がある
                    }
                }

                // 全て実行し終わった（この時点で全てFreeされている）
                topIndex = TopArrayGet(offset);
                if (topIndex < 0) { return false; }

                // 次へ
                topInfo = ref pool.UnsafeGet(topIndex);
            }
        }

        // この中で
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
        // 「await Story.Task して top ではなくなっている」
        // 「孤立して自動タスク化する」
        // 可能性がある
        bool Invoke(ref TaskInfo topInfo, int topIndex)
        {
            // 一時停止中
            if (this.runningException == null && topInfo.IsPaused)
            {
                LastAwaitType = topInfo.Offset & UPDATE_TYPE_MASK; // 呼び出し元で間違って swith しないように
                return true; // 継続
            }

            var offset = topInfo.Offset; 

            // 実行
            InvokeCore(ref topInfo, topIndex);
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある
            ref var info = ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex);

            // info.IsRunning / this.runningException != null
            // true  / true  => ありえない（GetResult が呼ばれなかったということなので、未実行のタスクに例外を処理させようとしたことになる）
            // true  / false => 継続（利用者に例外 or キャンセルを握りつぶされた可能性はある）
            // false / true  => 例外 or キャンセルが発生したが全て実行し終えた
            // false / false => 全て実行し終えた（利用者に例外 or キャンセルを握りつぶされた可能性はある）

            // 実行中にキャンセルされた
            if (info.WillCancel)
            {
                info.WillCancel = false;

                // **** この時点で孤立して自動タスク化している ****
                topInfo = ref info; // 返り咲き

                if (topInfo.IsRunning)
                {
                    topInfo.IsRunning = false;
                    Dev.Assert(this.runningException == null);
                    Dev.Assert(!this.runningResult.IsValid);

                    // 再実行でキャンセル
                    CancelInvoke(ref topInfo, topIndex);

                    return false; // 終了！！！
                }
                else { Dev.Log(string.Format(Messages.Warnings.CancelAbortedFinishedTask, topInfo.GetMethodName())); }
            }
            else if (info.IsRunning)
            {
                info.IsRunning = false;
                Dev.Assert(this.runningException == null);
                Dev.Assert(!this.runningResult.IsValid);

                // ここでは switch しない（呼び出し元まで伝播させる）
                // TopArraySwitch(offset, LastAwaitType);

                return true; // 継続
            }

            // 終了したので解放
            UnsafeFreeCore(topIndex);

            return false; // 終了
        }

        // この中で
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 「await Story.Task して top ではなくなっている」
        // 可能性がある
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void InvokeCore(ref TaskInfo topInfo, int topIndex)
        {
            Dev.Assert(!topInfo.IsRunning);
            topInfo.IsRunning = true;
            var parentIndex = this.runningIndex;
            this.runningIndex = topIndex;
            topInfo.Run();
            this.runningIndex = parentIndex; // 終わる前にかならず戻す
            // topInfo.IsRunning / this.runningException != null
            // true  / true  => ありえない（GetResult が呼ばれなかったということなので、未実行のタスクに例外を処理させようとしたことになる）
            // true  / false => 継続（利用者に例外 or キャンセルを握りつぶされた可能性はある）
            // false / true  => 例外 or キャンセルが発生したが全て実行し終えた
            // false / false => 全て実行し終えた（利用者に例外 or キャンセルを握りつぶされた可能性はある）
        }

        // この中で
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
        // 「孤立して自動タスク化する」
        // 加えて
        // 「await Story.Task して top ではなくなっている」
        // 可能性がある
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UnsafeCancel(int topIndex)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var topInfo = ref pool.UnsafeGet(topIndex);
            Dev.Assert(!topInfo.IsRunning);

            // 未使用ならそのまま解放
            if (!topInfo.IsUsing)
            {
                UnsafeFreeCore(topIndex);
                return;
            }

            // キャンセル準備（「孤立して自動タスク化する」）
            CancelPrepare(ref topInfo, topIndex);

            // キャンセル実行
            CancelInvoke(ref topInfo, topIndex);
        }

        // この中で topIndex が指すタスクは
        // 「孤立して自動タスク化する」
        void CancelPrepare(ref TaskInfo topInfo, int topIndex)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var topInfo2 = ref pool.UnsafeGet2(topIndex);

            UnsafeUnlink(topIndex); // ここで topInfo が無効化することはない

            // 自動タスク化
            topInfo2.Prev = topInfo2.Next = topIndex; // 親も子もいない
            TopArrayAdd(ref topInfo, topIndex, UPDATE_TYPE_AUTO); // 自動タスクへ

            // キャンセル属性
            topInfo.IsPinned = true;
            pool.UnsafeReborn(topIndex);
        }

        // この中で
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
        // 「await Story.Task して top ではなくなっている」
        // 可能性がある
        void CancelInvoke(ref TaskInfo topInfo, int topIndex)
        {
            // 初期化
            CaptureResult();
            CaptureException();

            // キャンセルの実行準備
            this.runningException = CanceledException.Shared;
            var offset = topInfo.Offset; 

            // finally を実行
            InvokeCore(ref topInfo, topIndex);
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある
            ref var info = ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex);
            info.WillCancel = false; // 再度消そうとされても、もう finally は実行しない

            // info.IsRunning / this.runningException != null
            // true  / true  => ありえない（GetResult が呼ばれなかったということなので、未実行のタスクに例外を処理させようとしたことになる）
            // true  / false => 継続（利用者に例外 or キャンセルを握りつぶされた可能性はある）
            // false / true  => 例外 or キャンセルが発生したが全て実行し終えた
            // false / false => 全て実行し終えた（利用者に例外 or キャンセルを握りつぶされた可能性はある）

            if (info.IsRunning)
            {
                info.IsRunning = false;
                Dev.Assert(this.runningException == null);
                Dev.Assert(!this.runningResult.IsValid);

                Dev.LogWarning(string.Format(Messages.Warnings.CancelFailedTaskRunning, info.GetMethodName()));

                // キャンセル先の処理を必要に応じて switch
                TopArraySwitch(offset, LastAwaitType);

                // 他所でやれ
                if (offset == info.Offset)
                {
                    info.Offset = this.autoTopArray.Add(topIndex);
                    this.autoTopArray[offset & ~UPDATE_TYPE_MASK] = -1;
                }
            }
            else
            {
                // 終了したので解放
                UnsafeFreeCore(topIndex);
            }
        }

        public void Free(Story.Task task)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            if (!pool.IsValid(task.Id)) { return; }

            // 先頭から
            var topIndex = task.Id.Index;
            ref var topInfo = ref pool.UnsafeGet(topIndex);
Dev.LoopBreak.Init();
            while (!topInfo.HasOffset)
            {
Dev.LoopBreak.Check(task.ToString());
                topIndex = pool.UnsafeGet2(topIndex).Prev;
                topInfo = ref pool.UnsafeGet(topIndex);
            }
            var offset = topInfo.Offset;

            // 自身まで順番にキャンセルする
Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(task.ToString());
                topInfo = ref pool.UnsafeGet(topIndex);
                if (topInfo.IsRunning)
                {
                    // 実行中なので終わってからキャンセルする
                    topInfo.WillCancel = true;
                    Dev.Log(string.Format(Messages.Warnings.CancelPendingWhileRunning, topInfo.GetMethodName()));

                    // キャンセル準備（「孤立して自動タスク化する」）
                    CancelPrepare(ref topInfo, topIndex);

                    // 他所でやれ
                    if (offset == topInfo.Offset)
                    {
                        topInfo.Offset = this.autoTopArray.Add(topIndex);
                        this.autoTopArray[offset & ~UPDATE_TYPE_MASK] = -1;
                    }
                }
                else
                {
                    UnsafeCancel(topIndex);
                        // この中で
                        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
                        // 「孤立して自動タスク化する」
                        // 加えて
                        // 「await Story.Task して top ではなくなっている」
                        // 可能性がある
                }

                if (topIndex == task.Id.Index)
                {
                    // 例外が残っていたら投げる
                    TryThrowException();
                    break;
                }
                topIndex = TopArrayGet(offset);
            }
        }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public int ManualIndexForDebug(int rawOffset) => this.manualTopArray[rawOffset].Index;
        public int AutoIndexForDebug(int rawOffset) => this.autoTopArray[rawOffset];
        public int LateIndexForDebug(int rawOffset) => this.lateTopArray[rawOffset];
        public int FixedIndexForDebug(int rawOffset) => this.fixedTopArray[rawOffset];
#endif

    }
}
