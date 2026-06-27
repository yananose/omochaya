namespace OmochayaTests
{
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Omochaya;
    // using Assert = UnityEngine.Assertions.Assert;

    public class StoryTests
    {
        // fields

        GameObject runnerObj;
        Utils.StoryTestRunner runner;
        GameObject ownerObj;
        Story.TaskBehaviour owner;

        // methods

        [SetUp]
        public void Setup()
        {
            this.runnerObj = new GameObject("StoryTestRunner");
            this.runner = this.runnerObj.AddComponent<Utils.StoryTestRunner>();
            this.ownerObj = new GameObject("StoryTestOwner");
            this.owner = this.ownerObj.AddComponent<Story.TaskBehaviour>();
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

        // 〜〜〜〜〜〜 ここからテスト 〜〜〜〜〜〜 

        [UnityTest]
        public IEnumerator LifeCycle_コルーチンとの比較()
        {
            // 記録帳
            var coroutineNote = new List<int>();
            var storyNote = new List<int>();

            Debug.Log("〜 各タスク生成 〜");
            var coroutineTask = CoroutineMain(coroutineNote); // コルーチンの場合
            var storyTask = StoryMain(storyNote); // Story の場合

            // 【Story専用】フレームをまたいだ時にオーナーがいないと解放されてしまうのでKeepしておく（直接Bootするなら不要）。
            storyTask.Keep(this.owner);

            Debug.Log("〜 0.1秒待つ 〜");
            yield return new WaitForSeconds(0.1f);

            Debug.Log("〜 各タスク起動 〜");
            this.owner.StartCoroutine(coroutineTask); // コルーチンの場合
            storyTask.Start(this.owner); // Story の場合

            // 各タスクが最後のループに到達するくらいまで待つ
            yield return new WaitForSeconds(0.5f);

            Debug.Log("〜 各タスク終了 〜");
            this.owner.StopCoroutine(coroutineTask); // コルーチンの場合
            storyTask.Stop(); // Story の場合

            Debug.Log("〜 0.1秒待つ 〜");
            yield return new WaitForSeconds(0.1f);

            // 結果
            Utils.Result(this.runner);
            Utils.Result(coroutineNote, storyNote);

            // 〜〜 ここからタスク定義 〜〜

            // メインタスク（コルーチン）
            IEnumerator CoroutineMain(List<int> note)
            {
                Utils.Take(note); Debug.Log("開始した（コルーチン：メイン）");
                yield return null;

                Utils.Take(note); Debug.Log("サブタスクの呼び出し（コルーチン：メイン）");
                yield return CoroutineSub(note).GetEnumerator();

                Utils.Take(note); Debug.Log("サブタスクを手動で回す（コルーチン：メイン）");
                foreach (var _ in CoroutineSub(note))
                {
                    yield return null;
                    Utils.Take(note);
                }

                Utils.Take(note); Debug.Log("最後のループに到達（コルーチン：メイン）");
                while (true)
                {
                    yield return null;
                    Utils.Take(note);
                }
            }

            // メインタスク（Story）
            async Story.Task StoryMain(List<int> note)
            {
                Utils.Take(note); Debug.Log("開始した（Story：メイン）");
                await Story.Yield;

                Utils.Take(note); Debug.Log("サブタスクの呼び出し（Story：メイン）");
                await StorySub(note);

                Utils.Take(note); Debug.Log("サブタスクを手動で回す（Story：メイン）");
                foreach (var _ in StorySub(note))
                {
                    await Story.Yield;
                    Utils.Take(note);
                }

                Utils.Take(note); Debug.Log("最後のループに到達（Story：メイン）");
                while (true)
                {
                    await Story.Yield;
                    Utils.Take(note);
                }
            }

            // サブタスク（コルーチン）
            IEnumerable CoroutineSub(List<int> note)
            {
                Utils.Take(note); Debug.Log("開始（コルーチン：サブ）");
                yield return null;
                Utils.Take(note); Debug.Log("システムから戻る（コルーチン：サブ）");
                yield return null;
                Utils.Take(note); Debug.Log("終了（コルーチン：サブ）");
            }

            // サブタスク（Story）
            async Story.Task StorySub(List<int> note)
            {
                Utils.Take(note); Debug.Log("開始（Story：サブ）");
                await Story.Yield;
                Utils.Take(note); Debug.Log("システムから戻る（Story：サブ）");
                await Story.Yield;
                Utils.Take(note); Debug.Log("終了（Story：サブ）");
            }
        }

        [UnityTest]
        public IEnumerator UpdateTiming_コルーチンとの比較()
        {
            // 記録帳
            var coroutineNote = new List<double>();
            var storyNote = new List<double>();

            Debug.Log("〜 各タスク生成/起動 〜");
            this.owner.StartCoroutine(CoroutineMain(coroutineNote)); // コルーチンの場合
            StoryMain(storyNote).Start(this.owner); // Story の場合

            // 各タスクが最後のループに到達するくらいまで待つ
            yield return new WaitForSeconds(0.5f);

            Debug.Log("〜 タスクを終了させる 〜");
            // コルーチンとStoryのどちらも、紐づいたオブジェクトが削除されると終了する
            if (this.ownerObj != null)
            {
                Object.Destroy(this.ownerObj);
                this.ownerObj = null;
                this.owner = null;
            }

            Debug.Log("〜 0.1秒待つ 〜");
            yield return new WaitForSeconds(0.1f);

            // 結果
            Utils.Result(this.runner);
            Utils.Result(coroutineNote, storyNote);

            // 〜〜 ここからタスク定義 〜〜

            // メインタスク（コルーチン）
            IEnumerator CoroutineMain(List<double> note)
            {
                Utils.Take(note); Debug.Log("開始した（コルーチン）");
                yield return null;

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(0)（コルーチン）");
                yield return new WaitForEndOfFrame();

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(1)（コルーチン）");
                yield return new WaitForEndOfFrame();

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(2)（コルーチン）");
                yield return new WaitForEndOfFrame();

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(0)（コルーチン）");
                yield return new WaitForFixedUpdate();

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(1)（コルーチン）");
                yield return new WaitForFixedUpdate();

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(2)（コルーチン）");
                yield return new WaitForFixedUpdate();

                Utils.Take(note); Debug.Log("最後のループに到達（コルーチン）");
                while (true)
                {
                    yield return null;
                    Utils.Take(note);
                }
            }

            // メインタスク（Story）
            async Story.Task StoryMain(List<double> note)
            {
                Utils.Take(note); Debug.Log("開始した（Story）");
                await Story.Yield;

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(0)（Story）");
                await Story.YieldLate;

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(1)（Story）");
                await Story.YieldLate;

                Utils.Take(note); Debug.Log("フレーム終わりまで待つ(2)（Story）");
                await Story.YieldLate;

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(0)（Story）");
                await Story.YieldFixed;

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(1)（Story）");
                await Story.YieldFixed;

                Utils.Take(note); Debug.Log("FixedUpdate まで待つ(2)（Story）");
                await Story.YieldFixed;

                Utils.Take(note); Debug.Log("最後のループに到達（Story）");
                while (true)
                {
                    await Story.Yield;
                    Utils.Take(note);
                }
            }
        }
    }
}
