// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskBand.cs" company="Omochaya">
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
    // タスクマネージャ（アップデータ）
    /// <summary>Don't touch! Only for system.</summary>
    partial class TaskManager
    {
        // interfaces
        interface ITop { bool CheckInvalid(); int Index { get; set; } }

        // inner classes
        struct Top : ITop
        {
            public bool CheckInvalid() => Index < 0;
            public int Index { get; set; }
        }

        struct ManualTop : ITop
        {
            // fields
            public Story.Task Caller;

            // for itop
            public bool CheckInvalid()
            {
                var topIndex = Index;
                if (topIndex < 0) { return true; }

                if (Caller.IsEmpty)
                {
                    var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
                    if (!pool.UnsafeGet(topIndex).IsOrphaned) { return false; } // 完全に未使用ならここで IsOrphaned をチェックして孤立していれば削除する。未使用なのでチェインにはなってない。
                }
                else if (Caller.IsValid) { return false; } // 一度でも MoveNext されていれば次の MoveNext もあるはずなので、IsOrphaned はそこでチェックする。仮に放置されても呼び出したタスクが消えれば消える

                // 消えるべきものが消えるだけ。必要経費
                TaskManager.Shared.UnsafeCancelManualChain(topIndex);

                return true;
            }
            public int Index { get; set; }
        }

        struct Band<T>
            where T : struct, ITop
        {
            // inner classes

            // fields
            T[] tops;
            int count;
            int type;

            // overrides
            public ref T this[int index] => ref this.tops[index];

            // properties
            public readonly int Count => this.count;
            public readonly int Type => this.type;

            // constructors
            public Band(int type)
            {
                this.tops = null;
                this.count = 0;
                this.type = type;
            }

            // methods
            public void Expand(int count) => Story.Pool.Expand(ref this.tops, count);
            public int Add(int index)
            {
                var count = this.count;
                if (this.tops == null) { Story.Pool.Create(ref this.tops); }
                else if (0 < count && this.tops[count - 1].Index == -1) { count--; } // 最後が空いてたら入れる（頻度次第だが後で詰め直すよりここで判定したほうがマシなはず）
                else if (count == this.tops.Length) { Story.Pool.Expand(ref this.tops); }
                this.tops[count] = new T { Index = index };
                this.count = count + 1;
                return count | this.type;
            }

            public ref T Get(int offset) => ref this.tops[offset & ~BAND_TYPE_MASK];
            public void Compact()
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

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public int ManualTopCount => this.manualBand.Count;
        public int AutoTopCount => this.bandArray[0].Count;
        public int LateTopCount => this.bandArray[1].Count;
        public int FixedTopCount => this.bandArray[2].Count;

        public int ManualIndexForDebug(int rawOffset) => this.manualBand[rawOffset].Index;
        public int AutoIndexForDebug(int rawOffset) => this.bandArray[0][rawOffset].Index;
        public int LateIndexForDebug(int rawOffset) => this.bandArray[1][rawOffset].Index;
        public int FixedIndexForDebug(int rawOffset) => this.bandArray[2][rawOffset].Index;
#endif

    }
}
