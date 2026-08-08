namespace OmochayaTests
{
    using System.Collections.Generic;
    using UnityEngine;
    using NUnit.Framework;
    using System;
    using UnityEngine.TestTools;
    using System.Diagnostics;
    using Omochaya;

    // ------------------------------------------------------------------------
    // テスト用のランナー（共有）
    // ------------------------------------------------------------------------
    internal class StoryTestRunner : MonoBehaviour
    {
        static StoryTestRunner instance;

        internal static void Require()
        {
            if (instance == null)
            {
                instance = new GameObject("StoryTestRunner").AddComponent<StoryTestRunner>();
            }
        }

        internal static void Dispose()
        {
            if (instance != null)
            {
                UnityEngine.Object.DestroyImmediate(instance.gameObject);
                instance = null;
            }
        }

        void Awake()
        {
            // 【任意】キャンセルモードの指定
            Story.DefaultCancelMode = Story.CancelMode.Safe;

            // 【任意】一度に実行するおおよそのタスク数の指定（実際に使用するタスク数よりも多めに指定してください）
            Story.Warmup(1024);

            // 【任意】各タスクのプールの事前確保
            using (Story.WarmupMode())
            {
                // 使用するタスクのプールを事前確保する
                // ※Capacity属性が設定されていればそのサイズ、引数で上書きも可能
                Story.WaitTime(0f).Warmup();
            }
        }

        void Update() 
        { 
            using (Utils.Check()) { Story.Update(); }
        }

        void LateUpdate()
        {
            using (Utils.Check()) { Story.LateUpdate(); }
        }

        void FixedUpdate()
        {
            using (Utils.Check()) { Story.FixedUpdate(); }
        }
    }

    // ------------------------------------------------------------------------
    // ユーティリティ
    // ------------------------------------------------------------------------
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
            Utils.AssertIsTrue(count == 0, string.Format("[アロケーションが発生していないこと] {0}", count));
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

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        internal static void ExpectError(string message) =>  LogAssert.Expect(LogType.Error, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        internal static void ExpectAssert(string message) =>  LogAssert.Expect(LogType.Assert, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        internal static void ExpectRuntimeAsseert(string message) =>  LogAssert.Expect(LogType.Assert, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        internal static void ExpectException(string message) =>  LogAssert.Expect(LogType.Exception, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        [Conditional("DUMMY")] static void AssertIsTrue(bool condition, string message) {}
#else
        [Conditional("DUMMY")] internal static void ExpectError(string message) {}
        [Conditional("DUMMY")] internal static void ExpectAssert(string message) {}
#if STORY_FULL_TUNE || STORY_NO_DEBUG
        [Conditional("DUMMY")] internal static void ExpectRuntimeAsseert(string message) {}
        [Conditional("DUMMY")] internal static void ExpectException(string message) {}
        static void AssertIsTrue(bool condition, string message) => NUnit.Framework.Assert.IsTrue(condition, message);
#else
        internal static void ExpectRuntimeAsseert(string message) =>  LogAssert.Expect(LogType.Exception, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        internal static void ExpectException(string message) =>  LogAssert.Expect(LogType.Exception, new System.Text.RegularExpressions.Regex($".*{message}.*"));
        [Conditional("DUMMY")] static void AssertIsTrue(bool condition, string message) {}
#endif
#endif
    }
}
