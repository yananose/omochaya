// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskBand.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    // interfaces

    /// <summary>Don't touch! Only for system.</summary>
    internal interface ITaskTop
    {
        /// <summary>Don't touch! Only for system.</summary>
        int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        bool CheckInvalid();
    }

    // inner classes

    /// <summary>Don't touch! Only for system.</summary>
    internal struct TaskTop : ITaskTop
    {
        /// <summary>Don't touch! Only for system.</summary>
        public int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        public bool CheckInvalid() => Index < 0;
    }

    /// <summary>Don't touch! Only for system.</summary>
    internal struct ManualTaskTop : ITaskTop
    {
        // fields

        /// <summary>Don't touch! Only for system.</summary>
        internal Story.Task Caller;

        // for itop

        /// <summary>Don't touch! Only for system.</summary>
        public int Index { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
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

    /// <summary>Don't touch! Only for system.</summary>
    internal struct TaskBand<T>
        where T : struct, ITaskTop
    {
        // inner classes

        // fields
        T[] tops;
        int count;
        int type;

        // overrides

        /// <summary>Don't touch! Only for system.</summary>
        internal ref T this[int index] => ref this.tops[index];

        // properties

        /// <summary>Don't touch! Only for system.</summary>
        internal readonly bool HasValues => this.tops != null;

        /// <summary>Don't touch! Only for system.</summary>
        internal readonly int Count => this.count;

        /// <summary>Don't touch! Only for system.</summary>
        internal readonly int Type => this.type;

        // constructors

        /// <summary>Don't touch! Only for system.</summary>
        internal TaskBand(int type)
        {
            this.tops = null;
            this.count = 0;
            this.type = type;
        }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        internal void Expand(int count) => Story.Pool.Expand(ref this.tops, count);

        /// <summary>Don't touch! Only for system.</summary>
        internal int Add(int index)
        {
            var count = this.count;
            if (this.tops == null) { Story.Pool.Create(ref this.tops); }
            else if (0 < count && this.tops[count - 1].Index == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
            else if (count == this.tops.Length) { Story.Pool.Expand(ref this.tops); }
            this.tops[count] = new T { Index = index };
            this.count = count + 1;
            return count | this.type;
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal ref T Get(int offset) => ref this.tops[offset & ~TaskManager.BAND_TYPE_MASK];

        /// <summary>Don't touch! Only for system.</summary>
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
