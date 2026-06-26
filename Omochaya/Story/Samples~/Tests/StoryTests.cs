namespace OmochayaTests
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using UnityEngine.Profiling;
    using Omochaya;
    using Assert = UnityEngine.Assertions.Assert;

    public class StoryTests
    {
        // inner classes
        class StoryTestRunner : MonoBehaviour
        {
            // fields
            Recorder gcRecorder;
            int lastSampleCount;
            bool isRecording;

            // properties
            public int TotalAllocatedFrames { get; private set; }

            // methods
            void Awake() => this.gcRecorder = Recorder.Get("GC.Alloc");

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

        // fields
        GameObject runnerObj;
        StoryTestRunner runner;
        GameObject ownerObj;
        Story.TaskBehaviour owner;

        // methods

        [SetUp]
        public void Setup()
        {
            this.runnerObj = new GameObject("StoryTestRunner");
            this.runner = this.runnerObj.AddComponent<StoryTestRunner>();
            this.ownerObj = new GameObject("StoryTestOwner");
            this.owner = this.runnerObj.AddComponent<Story.TaskBehaviour>();
        }

        [TearDown]
        public void Teardown()
        {
            if (this.runnerObj != null)
            {
                Object.DestroyImmediate(this.runnerObj);
                this.runnerObj = null;
                this.runner = null;
            }
            if (this.ownerObj != null)
            {
                Object.DestroyImmediate(this.ownerObj);
                this.ownerObj = null;
                this.owner = null;
            }
        }

        void Terminate()
        {
#if STORY_FAST
            if (this.runner != null)
            {
                Assert.IsTrue(this.runner.TotalAllocatedFrames == 0, "アロケーションが発生していないこと");
            }
#endif
        }

        // 〜〜〜〜〜〜 ここからテスト 〜〜〜〜〜〜 


        [UnityTest]
        public IEnumerator WaitTime_指定時間後にタスクが完了すること()
        {
            bool isCompleted = false;

            // テスト用のタスク定義
            async Story.Task TestTask()
            {
                await Story.WaitTime(0.1f);
                isCompleted = true; // クロージャはあかん
            }

            // 2. 実行 (Act)
            TestTask().Boot(this.owner);

            // 0.05秒待つ（まだ完了していないはず）
            yield return new WaitForSeconds(0.05f);
            Assert.IsFalse(isCompleted, "指定時間前には完了していないこと");

            // さらに0.1秒待つ（完了しているはず）
            yield return new WaitForSeconds(0.1f);

            // 3. 検証 (Assert)
            Assert.IsTrue(isCompleted, "指定時間後にタスクが完了していること");

            // 後片付け
            Terminate();
        }
    }
}
