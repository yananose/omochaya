// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskBand.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Implements the execution band structures and dense pointer arrays that group, 
//   prioritize, and systematically compact active task nodes during the framework update.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System.Runtime.CompilerServices;

    // interfaces

    interface ITaskTop
    {
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        bool CheckInvalid();
    }

    // inner classes

    struct TaskTop : ITaskTop
    {
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CheckInvalid() => Index < 0;
    }

    struct ManualTaskTop : ITaskTop
    {
        // fields

        internal Story.Task Caller;

        // for itop

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool CheckInvalid()
        {
            var topIndex = Index;
            if (topIndex < 0) { return true; }

            if (Caller.IsEmpty)
            {
                var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
                if (!pool.UnsafeGet(topIndex).ShouldCancel) { return false; } // 完全に未使用ならここで ShouldCancel をチェック。未使用なのでチェインにはなってない。
            }
            else if (Caller.IsValid) { return false; } // 一度でも MoveNext されていれば ShouldCancel は次の MoveNext でチェックする。仮に放置されても呼び出したタスクが消えれば消える

            // 消えるべきものが消えるだけ。必要経費
            TaskManager.Shared.UnsafeCancelManualChain(topIndex);

            return true;
        }
    }

    struct TaskBand<T>
        where T : struct, ITaskTop
    {
        // inner classes

        // fields
        T[] tops;
        int count;
        int type;

        // overrides

        internal ref T this[int index] => ref this.tops[index];

        // properties

        internal readonly bool IsValid => this.tops != null;

        internal readonly int Count => this.count;

        internal readonly int Type => this.type;

        // constructors

        internal TaskBand(int type)
        {
            this.tops = null;
            this.count = 0;
            this.type = type;
        }

        // methods

        internal void Expand(int count) => Story.Pool.Expand(ref this.tops, count);

        internal int Add(int index)
        {
            var count = this.count;
            if (this.tops == null)
            {
                Story.Pool.Expand(
                    ref this.tops,
                    Story.Pool.GetNeedCountAtCreate(Unsafe.SizeOf<T>()));
            }
            else if (0 < count && this.tops[count - 1].Index == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
            else if (count == this.tops.Length)
            {
                Story.Pool.Expand(
                    ref this.tops,
                    Story.Pool.GetNeedCountAtExpand(count, Unsafe.SizeOf<T>()));
            }
            this.tops[count] = new T { Index = index };
            this.count = count + 1;
            return count | this.type;
        }

        internal ref T Get(int offset) => ref this.tops[offset & ~TaskManager.BAND_TYPE_MASK];

        internal void Compact()
        {
            // 最初の隙間
            var rawOffset = 0;
            var end = Count;
            while (rawOffset < end)
            {
                if (this.tops[rawOffset].CheckInvalid()) { break; }
                else { rawOffset++; }
            }

            // 詰める
            var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
            for (var i=rawOffset+1; i<end; ++i)
            {
                var top = this.tops[i];
                var index = top.Index;
                if (top.CheckInvalid()) { continue; }
                pool.UnsafeGet(index).Offset = rawOffset | Type;
                this.tops[rawOffset++] = top;
            }
            this.count = rawOffset;
        }
    }
}
