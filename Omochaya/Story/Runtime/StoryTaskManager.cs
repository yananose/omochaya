// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskManager.cs" company="Omochaya">
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
    partial class TaskManager
    {
        // static

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskManager Shared { get; } = new TaskManager(3);

        /// <summary>Thrown internally when a task is forcibly freed or canceled.</summary>
        class CanceledException : Exception
        {
            public static readonly CanceledException Shared = new CanceledException();
            CanceledException() : base(Messages.Exceptions.TaskCanceled) { }
        }

        // const
        const int BAND_TYPE_SHIFT = 32 - 4; // なので rawOffset の有効範囲は 1 << 28 まで。
        public const int BAND_TYPE_MASK = -1 << BAND_TYPE_SHIFT;
        public const int BAND_TYPE_MANUAL = 0 << BAND_TYPE_SHIFT; // 手動更新 & 初期位置
        public const int BAND_TYPE_AUTO = 1 << BAND_TYPE_SHIFT; // Update更新 & 削除位置

        // fields
        Band<ManualTop> manualBand;
        Band<Top>[] bandArray;
        int frameCount;
        int updateOffset;
        int runningIndex = -1;
        Exception runningException = null;
        Story.PoolMemory runningResult;
        public int LastAwaitBandNo; // 一番最後に設定された type。タスクが終了したときは参照しない。つまりゴミを気にする必要はない。
        public bool IsResultError;

        // properties
        public bool IsRunningValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 <= runningIndex;
        }
        ref Band<Top> AutoBand
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => ref this.bandArray[0];
        }

        // constructors
        TaskManager(int bandCount)
        {
            Dev.Assert(0 < bandCount);
            this.frameCount = Time.frameCount;
            this.manualBand = new (BAND_TYPE_MANUAL);
            this.bandArray = new Band<Top>[bandCount];
            for (int i=0; i<bandCount; ++i) { this.bandArray[i] = new (GetBandType(i)); }
            this.updateOffset = -1;
        }

        // methods
        [MethodImpl(MethodImplOptions.NoInlining)] // ジェネリクスによるコードブロート防止のため明示的にインライン化しない
        public Story.Task Entry(in StateMachine stateMachine) // TaskMethodBuilder からのみ呼ばれる
        {
            if (GetRunningInfo().WillCancel) { return default; } // 削除要求されたタスク内では作成できない
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var id = pool.Alloc();
            var offset = this.manualBand.Add(id.Index);
            pool.UnsafeGet(id.Index).Entry(in stateMachine, offset);
            pool.UnsafeGet2(id.Index).Entry(id.Index);
            FrameCheck();
            return new Story.Task(id);
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // Entry をインライン化しないのでこっちはインライン化
        public Story.Task<R> Entry<R>(in StateMachine stateMachine) // TaskMethodBuilder からのみ呼ばれる
        {
            if (GetRunningInfo().WillCancel) { return default; } // 削除要求されたタスク内では作成できない
            var task = Entry(stateMachine);
            return new Story.Task<R>(task);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int count)
        {
            this.manualBand.Expand(count);
            for (var i=0; i<this.bandArray.Length; ++i) { this.bandArray[i].Expand(count); }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool IsManualBand(int offset) => (offset & BAND_TYPE_MASK) == BAND_TYPE_MANUAL;

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

        void FrameCheck()
        {
            // if (0 <= this.updateOffset) { return; } // Time.frameCount が重いらしいので設定済みかもチェック → キャッシュされるようになったらしい
            var frameCount = Time.frameCount;
            if (this.frameCount == frameCount) { return; }
            this.frameCount = frameCount;
            this.updateOffset = AutoBand.Count;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Update()
        {
            // auto：実行
            FrameCheck();
            BandInvoke(0, this.updateOffset);
            this.updateOffset = -1;

            // auto：詰める
            AutoBand.Compact();

            // manual：生存チェックしながら詰める
            this.manualBand.Compact();

            // 次が遠いのでここで放す
            CaptureResult();
            CaptureException();
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetBandNo(int offset) => (offset >> BAND_TYPE_SHIFT) - 1;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static int GetBandType(int bandNo) => (bandNo + 1) << BAND_TYPE_SHIFT;
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static bool IsMatchBand(int offset, int bandNo) => offset >> BAND_TYPE_SHIFT == bandNo + 1;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void BandUpdate(int bandNo)
        {
            Dev.Assert(0 < bandNo, string.Format(Messages.Exceptions.NotSupportedBandUpdate, bandNo));

            BandInvoke(bandNo, this.bandArray[bandNo].Count);
            this.bandArray[bandNo].Compact();

            // 次が遠いのでここで放す
            CaptureResult();
            CaptureException();
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void LateUpdate() => BandUpdate(1);
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void FixedUpdate() => BandUpdate(2);

        void BandInvoke(int bandNo, int rawOffset)
        {
            ref var band = ref this.bandArray[bandNo];
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            while (0 < rawOffset)
            {
                var topIndex = band[--rawOffset].Index; // タスクの実行順を後着優先にしたいので逆順
                if (topIndex < 0) { continue; } // ほぼ false
                if (UnsafeInvokeChain(topIndex))
                    // ここで
                    // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                    // 可能性がある
                {
                    // また、topIndex が指すタスクは
                    // 「await Story.Task して top ではなくなっている」
                    // 可能性がある

                    // switch する（ SwitchBand と同じだが最速で回すためにベタ書き）
                    if (LastAwaitBandNo != bandNo)
                    {
                        topIndex = band[rawOffset].Index; // 同じとは限らない
                        band[rawOffset].Index = -1;
                        AddTopOnBand(ref pool.UnsafeGet(topIndex), topIndex, LastAwaitBandNo);
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

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void UnsafeCancelManualChain(int topIndex)
        {
            // チェインをたどりながら全てキャンセル要求
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var topInfo = ref pool.UnsafeGet(topIndex);
Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(topInfo.GetMethodName());
                topInfo.WillCancel = true; // 削除要求
                topIndex = pool.UnsafeGet2(topIndex).Next;
                topInfo = ref pool.UnsafeGet(topIndex);
                if (topInfo.HasOffset) { break; }
            }

            // チェインで解放
            var offset = topInfo.Offset;
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 残ったら switch する（元が手動タスクなので必ず他所へ行く）
                SwitchBand(offset, LastAwaitBandNo);
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SwitchBand(int fromOffset, int toBandNo)
        {
            if (IsMatchBand(fromOffset, toBandNo)) { return; }
            var topIndex = GetIndexOnBand(fromOffset);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            AddTopOnBand(ref pool.UnsafeGet(topIndex), topIndex, toBandNo);
            SetIndexOnBand(fromOffset, -1);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void AddTopOnBand(ref TaskInfo topInfo, int topIndex, int toBandNo) => topInfo.Offset = this.bandArray[toBandNo].Add(topIndex);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        int GetIndexOnBand(int offset)
        {
            Dev.Assert(0 <= offset);
            if (IsManualBand(offset)) { return this.manualBand[offset].Index; }
            else { return this.bandArray[GetBandNo(offset)].Get(offset).Index; }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void SetIndexOnBand(int offset, int index)
        {
            Dev.Assert(0 <= offset);
            if (IsManualBand(offset)) { this.manualBand[offset].Index = index; }
            else { this.bandArray[GetBandNo(offset)].Get(offset).Index = index; }
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

        // この中では
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 可能性がある。また、解放されてなければ topIndex が指すタスクは
        // 「await Story.Task して top ではなくなっている」
        // 可能性がある
        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
        bool UnsafeInvokeChain(int topIndex)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            if (topInfo.IsRunning) { return true; } // 実行中ならスキップ（Freeから呼ばれた時にあり得る）

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
                    // 使用中のみ
                    if (topInfo.IsUsing)
                    {
                        // キャンセル要求を下ろしてピン留め
                        topInfo.WillCancel = false;
                        topInfo.IsPinned = true;

                        // 結果とキャンセルは受け取らない
                        CaptureResult();
                        CaptureException();

                        // finally を実行
                        this.runningException = CanceledException.Shared;
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

                        if (info.IsRunning)
                        {
                            info.IsRunning = false;
                            Dev.Assert(this.runningException == null);
                            Dev.Assert(!this.runningResult.IsValid);
                            return true; // 継続（つまり finally で await したらそれが終わるまで親の解放も処理されない）
                        }
                    }

                    // 終了した（開始してなかった）ので解放
                    UnsafeFreeCore(topIndex);
                }
                // 一時停止中（直前に終わった子が例外を吐いていたら止めない）
                else if (this.runningException == null && topInfo.IsPaused)
                {
                    LastAwaitBandNo = GetBandNo(topInfo.Offset); // 呼び出し元で間違って swith しないように
                    return true; // 継続
                }
                else
                {
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

                    if (info.IsRunning)
                    {
                        info.IsRunning = false;
                        Dev.Assert(this.runningException == null);
                        Dev.Assert(!this.runningResult.IsValid);

                        // 実行中にキャンセルされていない
                        if (!info.WillCancel) { return true; } // 継続

                        // 先頭までキャンセル指示（※キャンセルされたタスクは await できないので不要）
//                         var index = topIndex;
// Dev.LoopBreak.Init();
//                         while (!info.HasOffset)
//                         {
// Dev.LoopBreak.Check(info.GetMethodName());
//                             if (info.HasOffset) { break; }
//                             index = pool.UnsafeGet2(index).Prev;
//                             info = ref pool.UnsafeGet(index);
//                             info.WillCancel = true;
//                         }
                    }
                    else
                    {
                        // 終了したので解放
                        UnsafeFreeCore(topIndex);
                    }
                }

                // 次
                topIndex = GetIndexOnBand(offset);
                if (topIndex < 0) { return false; }
                topInfo = ref pool.UnsafeGet(topIndex);
            }
        }

        // この中で
        // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
        // 「await Story.Task して top ではなくなっている」
        // 可能性がある
        // し、
        // 途中が Free で抜けてるかもしれない。それはスタック上かもしれないし、チェイン上かもしれない。
        // どっちにしても抜けたタスクの親には ResultError を返す必要がある
        // よ。
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        void InvokeCore(ref TaskInfo topInfo, int topIndex)
        {
            Dev.Assert(!topInfo.IsRunning);
            topInfo.IsRunning = true; // InvokeCore の後で見て下ろす
            var parentIndex = this.runningIndex;
            this.runningIndex = topIndex;
            topInfo.Run();
            this.runningIndex = parentIndex;
            // topInfo.IsRunning / this.runningException != null
            // true  / true  => ありえない（GetResult が呼ばれなかったということなので、未実行のタスクに例外を処理させようとしたことになる）
            // true  / false => 継続（利用者に例外 or キャンセルを握りつぶされた可能性はある）
            // false / true  => 例外 or キャンセルが発生したが全て実行し終えた
            // false / false => 全て実行し終えた（利用者に例外 or キャンセルを握りつぶされた可能性はある）
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // IsNotCompleted からしか呼ばれないので
        void UnsafeStack(int prevRootIndex, int nextTopIndex)
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

            // band の繋ぎ変え：情報取得
            ref var prevTopInfo = ref pool.UnsafeGet(prevTopIndex);
            ref var nextTopInfo = ref pool.UnsafeGet(nextTopIndex);
            Dev.Assert(prevTopInfo.HasOffset, string.Format(Messages.Exceptions.DoubleAwait, pool.UnsafeGet(prevRootIndex).GetMethodName(), nextTopInfo.GetMethodName()));
            Dev.Assert(nextTopInfo.HasOffset, string.Format(Messages.Exceptions.AwaitingWhileAwaited, pool.UnsafeGet(prevRootIndex).GetMethodName(), nextTopInfo.GetMethodName()));
            var prevOffset = prevTopInfo.Offset;
            var nextOffset = nextTopInfo.Offset;

            // band の繋ぎ変え：実行
            SetIndexOnBand(prevOffset, -1);
            SetIndexOnBand(nextOffset, prevTopIndex);
            prevTopInfo.Offset = nextOffset;
            nextTopInfo.Offset = -1;

            // 生存チェック情報を先頭へ反映
            prevTopInfo.IsPinned = nextTopInfo.IsPinned;
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // UnsafeFreeCore を呼ぶので
        void UnsafeFreeCore(int index)
        {
            UnsafeUnstack(index);
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            pool.UnsafeGet(index).Free();
            pool.UnsafeGet2(index).Free();
            pool.UnsafeFree(index);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // UnsafeFreeCore からしか呼ばれないので
        void UnsafeUnstack(int index)
        {
            // 必要な情報取得
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            ref var info = ref pool.UnsafeGet(index);
            ref var info2 = ref pool.UnsafeGet2(index);
            var offset = info.Offset;

            // 単体なら band から外すだけ
            if (info2.Next == index)
            {
                SetIndexOnBand(offset, -1);
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
            SetIndexOnBand(offset, nextIndex);
            ref var nextInfo = ref pool.UnsafeGet(nextIndex);
            nextInfo.Offset = offset;

            // 次へ生存チェック情報を渡す
            nextInfo.IsPinned = info.IsPinned;
        }

        public bool Boot(Story.Task task)
        {
            if (!task.IsValid) { return false; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);
            ref var rootInfo2 = ref pool.UnsafeGet2(rootIndex);

            var topIndex = rootInfo2.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.AlreadyBooted);

            // 削除要求されたタスクが起動しようとした
            if (GetRunningInfo().WillCancel) { return false; } // 終了を返す

            // 元の位置を退避してマスターがいなければ設定
            var offset = topInfo.Offset;
            TryKeep(ref rootInfo);

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
                SwitchBand(offset, LastAwaitBandNo);

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

        // [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            // 削除要求されたタスクが起動しようとした
            if (GetRunningInfo().WillCancel) { return false; } // 終了を返す

            // 呼び出し元を設定（結局復元してる...）
            var offset = topInfo.Offset;
            this.manualBand[offset].Caller = IsRunningValid ? new Story.Task(pool.UnsafeGetId(this.runningIndex)) : default;

            // 実行
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 手動タスクは switch しない（他タスクから呼び出されているときはどの道そこで await Yield されているし）。
                // SwitchBand(offset, LastAwaitBandNo);

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

        [MethodImpl(MethodImplOptions.NoInlining)] // UnsafeLink を呼ぶので
        public bool IsNotCompleted(Story.Task task)
        {
            Dev.Assert(IsRunningValid);

            if (!task.IsValid) { return false; }

            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            var rootIndex = task.Id.Index;
            ref var rootInfo = ref pool.UnsafeGet(rootIndex);
            ref var rootInfo2 = ref pool.UnsafeGet2(rootIndex);

            var topIndex = rootInfo2.Next;
            ref var topInfo = ref pool.UnsafeGet(topIndex);

            Dev.ValidateManualTask(ref rootInfo, ref topInfo, Messages.Exceptions.CannotAwaitAutoTask);

            // 削除要求されたタスクが起動しようとした
            if (GetRunningInfo().WillCancel) { return false; } // 終了を返す


            // マスターがいなければ設定
            TryKeep(ref rootInfo);

            // 実行
            if (UnsafeInvokeChain(topIndex))
                // ここで
                // 「(自身以外も含めて)info 配列等のアドレスが変わってる」
                // 可能性がある
            {
                // また、topIndex が指すタスクは
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // ここでは switch しない（呼び出し元で switch させる）
                // SwitchBand(offset, LastAwaitBandNo);

                // 前へ積む
                UnsafeStack(rootIndex, this.runningIndex);

                return true;
            }
            else
            {
                // また、解放されてなければ topIndex が指すタスクは
                // 「孤立して自動タスク化する」
                // 加えて
                // 「await Story.Task して top ではなくなっている」
                // 可能性がある

                // 握りつぶしちゃダメ！呼び出し元へ伝える
                // CaptureResult();
                // CaptureException();

                return false;
            }
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

        public void Free(Story.Task task)
        {
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            if (!pool.IsValid(task.Id)) { return; }

            // 先頭までキャンセル指示
            var index = task.Id.Index;
            ref var info = ref pool.UnsafeGet(index);
Dev.LoopBreak.Init();
            while (true)
            {
Dev.LoopBreak.Check(task.ToString());
                info.WillCancel = true;
                if (info.HasOffset) { break; }
                index = pool.UnsafeGet2(index).Prev;
                info = ref pool.UnsafeGet(index);
            }

            // キャンセル実行（即処理するため。実行中タスクならスキップされる）
            UnsafeInvokeChain(index);

            // 結果とキャンセルは渡さない
            CaptureResult();
            CaptureException();
        }
    }
}
