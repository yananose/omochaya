namespace OmochayaTests
{
    using System.Collections.Generic;
    using UnityEngine;
    using NUnit.Framework;
    using System;

    static class Utils
    {
        // inner classes
        public struct Checker : IDisposable
        {
            long lastValue;
            public Checker(int dummy)
            {
                this.lastValue = Utils.LastValue;
            }
            public void Dispose()
            {
                var delta = (int)(Utils.LastValue - this.lastValue);
                if (0 < delta) { Utils.GCAlloc += delta; }
                // if (0 < delta) { Utils.GCAlloc++; }
                // Utils.GCAlloc = Mathf.Max(Utils.GCAlloc, delta);
            }
        }

        internal static int GCAlloc { get; set; }
        internal static long LastValue => GC.GetTotalMemory(false);

        internal static Checker Check() => new Checker(0);

        internal static void LogGCAlloc()
        {
            var count = Utils.GCAlloc;
            Utils.GCAlloc = 0;
            Omochaya.HiddenStory.Dev.AssertIsTrue(count == 0, string.Format("[アロケーションが発生していないこと] {0}", count));
        }

        internal static void Take(List<int> note) =>  note.Add(Time.frameCount);
        internal static void Result(List<int> noteA, List<int> noteB)
        {
            Assert.IsTrue(noteA.Count == noteB.Count, string.Format("[記録数が同じこと] {0} == {1}", noteA.Count, noteB.Count));
            for (var i=0; i<noteA.Count; ++i)
            {
                Assert.IsTrue(noteA[i] == noteB[i], string.Format("[タイミングが同じこと] {0} ({1}, {2})", i, noteA[i], noteB[i]));
            }
        }

        internal static void Take(List<double> note) =>  note.Add(Time.realtimeSinceStartupAsDouble);
        internal static void Result(List<double> noteA, List<double> noteB)
        {
            Assert.IsTrue(noteA.Count == noteB.Count, string.Format("[記録数が同じこと] {0} == {1}", noteA.Count, noteB.Count));
            for (var i=0; i<noteA.Count; ++i)
            {
                var delta = noteA[i] - noteB[i];
                Assert.IsTrue(Mathf.Abs((float)delta) < 0.005f, string.Format("[タイミングが同じこと] {0} ({1})", i, delta));
            }
        }
    }
}
