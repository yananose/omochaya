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
            public void Init(int length)
            {
                Dev.Assert(this.array == null);
                this.array = new int[length];
            }
            public void Add(int offset)
            {
                if (this.array == null) { Story.Pool.Create(ref this.array); }
                if (this.count == this.array.Length) { Story.Pool.Expand(ref this.array); }
                this.array[this.count++] = offset;
            }
            public bool IsInRange(int index) => (uint)index < this.count;
            public void SetCount(int count) => this.count = count;
        }

        // fields
        TopArray topArray;
        int frameCount;
        int updateOffset;
        int autoOffset;
        int runningIndex = -1;
        Exception runningException = null;
        Story.PoolMemory runningResult;
        Story.Pool.Id runningResultId;

        // properties
        int endOffset => this.topArray.Count;
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
        public int EndOffset => this.endOffset;
#endif

        // constructors
        TaskManager()
        {
            this.topArray.Init(1024);
            this.frameCount = Time.frameCount;
            this.updateOffset = -1;
            this.autoOffset = 0;
        }

        // methods
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsAuto(int offset) => offset < this.autoOffset;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsManual(int offset) => !IsAuto(offset);

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
            var offset = this.endOffset;
            this.topArray.Add(id.Index);
            pool.UnsafeGet(id.Index).Entry(in stateMachine, offset, id.Index);
            pool.UnsafeGet2(id.Index).Entry();
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
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            while (0 < this.updateOffset)
            {
                var index = this.topArray[--this.updateOffset]; // タスクの実行順を後着優先にしたいので逆順
                if (index < 0) { continue; } // ほぼ false
                if (UnsafeInvoke(index)) { continue; }
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 「(自身以外も含めて)手動タスクの offset が変わっている」
                    // 可能性がある
                TryLogException();
            }
            this.updateOffset = -1;

            // 最初の隙間
            var end = this.endOffset;
            var offset = 0;
            while (offset < end)
            {
                var index = this.topArray[offset];
                if (index < 0) { break; }
                else { offset++; }
            }

            // 自動タスクを詰める
            end = this.autoOffset;
            for (var i=offset+1; i<end; ++i)
            {
                var index = this.topArray[i];
                if (index < 0) { continue; }
                pool.UnsafeGet(index).Offset = offset;
                this.topArray[offset++] = index;
            }
            if (offset < this.autoOffset) { this.autoOffset = offset; }
            var freeStart = this.autoOffset;

            // 自動タスクの範囲内に隙間がなかった場合、手動タスクの先頭から最初の隙間までの間を生存確認
            for (var i=end; i<offset; ++i) { UnsafeCheckAlive(this.topArray[i]); }

            // 残りの手動タスクを詰めならが生存確認
            var start = Mathf.Max(end, offset);
            end = this.endOffset;
            for (var i=start; i<end; ++i)
            {
                var index = this.topArray[i];
                if (index < 0) { continue; }
                ref var info = ref pool.UnsafeGet(index);
                info.Offset = offset;
                this.topArray[offset++] = index;
                CheckAlive(ref info, index);
            }
            this.topArray.SetCount(offset);

            // キャンセル用に移動したタスク（不要になった手動タスク）を解放
            // （finallyでタスクの追加や起動がされてもいいように最後に自動タスクの範囲で行う）
            var freeEnd = this.autoOffset;
            for (var i=freeStart; i<freeEnd; ++i)
            {
                var index = this.topArray[i];
Dev.LoopBreak.Init();
                while (0 <= index) {
Dev.LoopBreak.Check(i.ToString());
                    index = UnsafeCancelOne(index); }
            }
            TryLogException();
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
        void UnsafeCheckAlive(int topIndex) => CheckAlive(ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex), topIndex);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void CheckAlive(ref TaskInfo topInfo, int topIndex)
        {
            if (topInfo.IsPinned) { return; }
            var node = Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet2(topIndex).ManualNode;
            if (node.IsEmpty)
            {
                if (!topInfo.IsOrphaned) { return; }
            }
            else if (node.IsValid) { return; }

            // キャンセル用に移動
            MakeAuto(ref topInfo);
        }

        public bool Boot(Story.Task task)
        {
            Dev.Assert(task.IsValid, Messages.Exceptions.InvalidTaskOnBoot);

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);

            TryKeep(ref rootInfo);

            var topIndex = rootInfo.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.AlreadyBooted);

            // 自動実行状態へ
            MakeAuto(ref topInfo);

            // 実行
            if (Invoke(ref topInfo, topIndex)) { return true; }
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 「(自身以外も含めて)手動タスクの offset が変わっている」
                // 可能性がある

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

        void MakeAuto(ref TaskInfo topInfo)
        {
            var swapOffset = this.autoOffset++;
            var offset = topInfo.Offset;
            if (offset != swapOffset)
            {
                var swapIndex = this.topArray[swapOffset];
                if (0 <= swapIndex) { Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(swapIndex).Offset = offset; }
                topInfo.Offset = swapOffset;
                this.topArray[swapOffset] = this.topArray[offset];
                this.topArray[offset] = swapIndex;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext(Story.Task task)
        {
            if (!task.IsValid) { return false; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);

            var topIndex = rootInfo.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.CannotMoveNextAutoTask);

            // 呼び出し元を設定（結局復元してる...）
            pool.UnsafeGet2(topIndex).ManualNode = IsRunningValid ? new Story.Task(pool.UnsafeGetId(this.runningIndex)) : default;

            // 実行
            if (Invoke(ref topInfo, topIndex)) { return true; }
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

            TryKeep(ref rootInfo);

            var topIndex = rootInfo.Next;
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
                UnsafePushTaskChain(rootIndex, this.runningIndex);

                // 継続
                return true;
            }

            // await なので、この後の GetResult で処理させる
            // TryLogException();

            return false;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void UnsafePushTaskChain(int prevRootIndex, int nextIndex)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var prevRootInfo = ref pool.UnsafeGet(prevRootIndex);
            ref var nextInfo = ref pool.UnsafeGet(nextIndex);

            // await される側
            var prevActiveIndex = prevRootInfo.Next;
            ref var prevActiveInfo = ref pool.UnsafeGet(prevActiveIndex);
            var oldOffset = prevActiveInfo.Offset;
            Dev.Assert(0 <= oldOffset, string.Format(Messages.Exceptions.DoubleAwait, prevRootInfo.GetMethodName(), nextInfo.GetMethodName()));

            // await する側
            var newOffset = nextInfo.Offset;
            Dev.Assert(0 <= newOffset, string.Format(Messages.Exceptions.AwaitingWhileAwaited, prevRootInfo.GetMethodName(), nextInfo.GetMethodName()));
            nextInfo.IsWaiting = true; // 待機状態に

            // 繋ぎ変え
            this.topArray[oldOffset] = -1;
            this.topArray[newOffset] = prevActiveIndex;
            prevActiveInfo.Offset = newOffset;
            nextInfo.SetPrev(prevRootIndex);
            prevRootInfo.Next = nextIndex;

            // 生存チェック情報を先頭へ反映
            prevActiveInfo.IsPinned = nextInfo.IsPinned;
            if (IsManual(newOffset)) { pool.UnsafeGet2(prevActiveIndex).ManualNode = pool.UnsafeGet2(nextIndex).ManualNode; }

            // 最後の Next に先頭を入れる（PrevとOffsetを共存させてるのでこんなことに...）
Dev.LoopBreak.Init();
            while(nextInfo.Next != nextIndex) {
Dev.LoopBreak.Check(prevRootInfo.GetMethodName());
                nextInfo = ref pool.UnsafeGet(nextInfo.Next); }
            nextInfo.Next = prevActiveIndex;
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int UnsafeFreeCore(int topIndex) => FreeCore(ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(topIndex), topIndex);
        int FreeCore(ref TaskInfo topInfo, int topIndex)
        {
            // Dev.Log($"free - {Task.UnsafeCreate(topIndex)}");
            var nextIndex = Unlink(ref topInfo, topIndex);
            topInfo.Free();
            Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet2(topIndex).Free();
            Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeFree(topIndex);
            return nextIndex;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int Unlink(ref TaskInfo info, int index) // 削除前提でリンクを切る
        {
            var offset = info.Offset;
            var nextIndex = info.Next;

            // 自身のみの場合
            if (index == nextIndex)
            {
                this.topArray[offset] = -1;
                return -1;
            }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var nextInfo = ref pool.UnsafeGet(nextIndex);

            // 先頭ではない場合（先頭のタスクに対してしか呼ばれないはずだが一応）
            if (offset < 0)
            {
                var prevIndex = info.Prev;
                ref var prevInfo = ref pool.UnsafeGet(prevIndex);
                prevInfo.Next = nextIndex;
                if (nextInfo.HasOffset) { return -1; }
                nextInfo.Prev = prevIndex;
                return nextIndex;
            }

            // 先頭の場合
            // 次を先頭へ
            PopTopTask(offset, ref info, index, ref nextInfo, nextIndex);
            return nextIndex;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void PopTopTask(int offset, ref TaskInfo oldInfo, int oldIndex, ref TaskInfo newInfo, int newIndex) // old と new は連続していること！
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;

            // 繋ぎ変え
            this.topArray[offset] = newIndex;
            newInfo.SetOffset(offset);

            // 生存チェック情報を先頭へ反映
            newInfo.IsPinned = oldInfo.IsPinned;
            if (IsManual(offset)) { pool.UnsafeGet2(newIndex).ManualNode = pool.UnsafeGet2(oldIndex).ManualNode; }

            // 最後の Next に先頭を入れる（PrevとOffsetを共存させてるのでこんなことに...）
Dev.LoopBreak.Init();
            while(newInfo.Next != oldIndex) {
Dev.LoopBreak.Check(Story.Task.UnsafeCreate(oldIndex).ToString());
                newInfo = ref pool.UnsafeGet(newInfo.Next); }
            newInfo.Next = newIndex;
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
                return PrepareCancel(ref topInfo, topIndex);
            }

            // 他を待ってない（初期状態）ならそのまま解放（多分このケースはないが一応）
            if (!topInfo.IsWaiting)
            {
                var offset = topInfo.Offset;
                FreeCore(ref topInfo, topIndex);
                return this.topArray[offset];
            }

            // キャンセル準備
            var nextIndex = PrepareCancel(ref topInfo, topIndex);
            if (this.runningException != null) { Dev.LogException(this.runningException); }
            this.runningException = CanceledException.Shared;

            // キャンセル実行
            if (InvokeCore(ref topInfo, topIndex))
            {
                Dev.LogWarning(string.Format(Messages.Warnings.CancelFailedTaskRunning, topInfo.GetMethodName()));
            }
            else
            {
                UnsafeFreeCore(topIndex); // nextIndex に受け取っちゃダメ
            }

            return nextIndex;
        }

        int PrepareCancel(ref TaskInfo topInfo, int topIndex)
        {
            var offset = topInfo.Offset;
            var nextIndex = topInfo.Next;
            if (topIndex == nextIndex) // 単独
            {
                nextIndex = -1;
                if (IsManual(offset)) { MakeAuto(ref topInfo); }
            }
            else
            {
                var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
                // 切り離して
                PopTopTask(offset, ref topInfo, topIndex, ref pool.UnsafeGet(nextIndex), nextIndex);

                // 自動タスクへ
                var swapOffset = this.autoOffset++; // topInfo の移動先
                if (swapOffset == this.endOffset) { this.topArray.Add(-1); }
                else
                {
                    // 移動先にタスクがあればそれを末尾に移動
                    var swapIndex = this.topArray[swapOffset];
                    if (0 <= swapIndex)
                    {
                        pool.UnsafeGet(swapIndex).Offset = this.endOffset;
                        this.topArray.Add(swapIndex);
                    }
                }
                this.topArray[swapOffset] = topIndex;
                topInfo.Offset = swapOffset;
                topInfo.Next = topIndex;
            }
            topInfo.IsPinned = true;
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
            while (info.HasPrev)
            {
Dev.LoopBreak.Check(task.ToString());
                index = info.Prev;
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
        public int IndexForDebug(int offset) => this.topArray[offset];
#endif

    }
}
