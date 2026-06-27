namespace OmochayaTests
{
    using System.Collections.Generic;
    using UnityEngine;
    using UnityEngine.Profiling;
    using Omochaya;
    using NUnit.Framework;

    // using Assert = UnityEngine.Assertions.Assert;

    static class Utils
    {
        // inner classes
        internal class StoryTestRunner : MonoBehaviour
        {
            // fields
            Recorder gcRecorder;
            int lastSampleCount;
            bool isRecording;

            // properties
            public int TotalAllocatedFrames { get; private set; }

            // methods
            void Awake()
            {
                Story.Custom(3, 2048);
                this.gcRecorder = Recorder.Get("GC.Alloc");
            }

            void Update() 
            { 
                StartRecord();
                Story.Update(); 
                EndRecord();
            }

            void LateUpdate()
            {
                StartRecord();
                Story.LateUpdate();
                EndRecord();
            }

            void FixedUpdate()
            {
                StartRecord();
                Story.FixedUpdate();
                EndRecord();
            }

            void StartRecord()
            {
                this.isRecording = this.gcRecorder != null && this.gcRecorder.isValid;
                if (this.isRecording)
                {
                    this.gcRecorder.enabled = true;
                    this.lastSampleCount = this.gcRecorder.sampleBlockCount;
                }
            }

            void EndRecord()
            {
                if (this.isRecording)
                {
                    this.gcRecorder.enabled = false;
                    if (this.lastSampleCount != this.gcRecorder.sampleBlockCount)
                    {
                        TotalAllocatedFrames++;
                    }
                }
            }
        }

        internal static void Result(StoryTestRunner runner)
        {
#if STORY_FAST
            if (runner != null)
            {
                Assert.IsTrue(runner.TotalAllocatedFrames == 0, $"[アロケーションが発生していないこと] {runner.TotalAllocatedFrames}");
            }
#endif
        }

        internal static void Take(List<int> note) =>  note.Add(Time.frameCount);
        internal static void Result(List<int> noteA, List<int> noteB)
        {
            Assert.IsTrue(noteA.Count == noteB.Count, $"[記録数が同じこと] {noteA.Count} == {noteB.Count}");
            for (var i=0; i<noteA.Count; ++i)
            {
                Assert.IsTrue(noteA[i] == noteB[i], $"[タイミングが同じこと] {i} ({noteA[i]}, {noteB[i]})");
            }
        }

        internal static void Take(List<double> note) =>  note.Add(Time.realtimeSinceStartupAsDouble);
        internal static void Result(List<double> noteA, List<double> noteB)
        {
            Assert.IsTrue(noteA.Count == noteB.Count, $"[記録数が同じこと] {noteA.Count} == {noteB.Count}");
            for (var i=0; i<noteA.Count; ++i)
            {
                var delta = noteA[i] - noteB[i];
                Assert.IsTrue(Mathf.Approximately((float)delta, 0.001f), $"[タイミングが同じこと] {i} ({delta})");
            }
        }
    }
}
