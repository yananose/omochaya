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

        // ------------------------------------------------------------------------
        // ライフサイクルのコルーチンとの比較
        // ------------------------------------------------------------------------

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

        // ------------------------------------------------------------------------
        // プールとID（世代管理）の整合性テスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_完了後にIDが無効になること()
        {
            var task = SimpleTask();
            Assert.IsTrue(task.IsValid, "生成直後はタスクが有効であるべき");

            task.Start(this.owner);

            // タスクが終わるまで十分に待機する
            yield return null; 
            yield return null;

            Assert.IsFalse(task.IsValid, "実行が完了したタスクはプールに返却され、自動的に無効になるべき");

            async Story.Task SimpleTask()
            {
                await Story.Yield;
            }
        }

        [Test]
        public void Task_解放済みタスクに対する操作のフェイルセーフが仕様通り機能すること()
        {
            var task = EmptyTask();
            task.Stop(); // 手動で即座に解放

            Assert.IsFalse(task.IsValid, "Stopを呼んだ後は無効になるべき");

            // 終了後も許容される操作（冪等性・イテレータ仕様に基づく保護）
            Assert.DoesNotThrow(() => task.Stop(), "解放済みタスクの二重Stopは安全に無視されるべき");
            Assert.DoesNotThrow(() => task.MoveNext(), "解放済みタスクのMoveNextは例外を出さず安全に無視されるべき");
            Assert.IsFalse(task.MoveNext(), "解放済みタスクのMoveNextは状態を進めずfalseを返すこと");

            // 終了後は禁止される操作（Use-After-Free防止のためのAssert）
            // ※ StoryDebug.cs の仕様により、不正操作時は System.Exception がスローされる
            Assert.Throws<System.Exception>(() => task.Start(this.owner), "解放済みタスクのStartは例外(Assert)を出して弾かれるべき");
            Assert.Throws<System.Exception>(() => task.Keep(this.owner), "解放済みタスクのKeepは例外(Assert)を出して弾かれるべき");
            
            async Story.Task EmptyTask() { await Story.Void; }
        }


        [Test]
        public void Task_大量生成と解放で世代管理_Age_が正しく機能すること()
        {
            var oldTasks = new List<Story.Task>();
            
            // プールの初期容量を超える程度に大量生成する
            for (int i = 0; i < 2000; i++)
            {
                oldTasks.Add(EmptyTask());
            }

            // 全て手動で解放する（プールにスロットが返却される）
            foreach (var t in oldTasks)
            {
                t.Stop();
            }

            foreach (var t in oldTasks)
            {
                Assert.IsFalse(t.IsValid, "解放されたタスクは直ちに無効になるべき");
            }

            // 再度生成し、返却されたプールのインデックスを再利用させる
            var newTasks = new List<Story.Task>();
            for (int i = 0; i < 2000; i++)
            {
                newTasks.Add(EmptyTask());
            }

            // ここが重要：インデックスが再利用されても、Ageがインクリメントされているため古いIDは無効判定になること
            foreach (var t in oldTasks)
            {
                Assert.IsFalse(t.IsValid, "同じインデックスが再利用されても、世代(Age)が古いためIsValidはfalseを返すこと");
            }

            // 後始末
            foreach (var t in newTasks)
            {
                t.Stop();
            }

            async Story.Task EmptyTask() { await Story.Void; }
        }

        // ------------------------------------------------------------------------
        // ライフサイクルとキャンセル処理のテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_Stopを呼ぶとキャンセルされfinallyブロックが実行されること()
        {
            bool hasReachedFinally = false;
            bool hasExecutedAfterYield = false;

            var task = CancelTestTask();
            task.Start(this.owner);

            // タスクが最初のYieldで待機するまで1フレーム進める
            yield return null;

            // 外部から強制キャンセル
            task.Stop();

            // キャンセル処理をシステムに反映させるため1フレーム進める
            yield return null;

            Assert.IsFalse(hasExecutedAfterYield, "キャンセルされたため、Yield以降の通常処理は実行されないべき");
            Assert.IsTrue(hasReachedFinally, "キャンセルされた場合でも、finallyブロックは確実に実行されるべき");

            async Story.Task CancelTestTask()
            {
                try
                {
                    await Story.Yield;
                    hasExecutedAfterYield = true; // ここには到達しないはず
                }
                finally
                {
                    hasReachedFinally = true;
                }
            }
        }

        [UnityTest]
        public IEnumerator Task_オーナーが破棄されると自動的にキャンセルされfinallyブロックが実行されること()
        {
            bool hasReachedFinally = false;

            // テスト用の一時的なオーナーを作成
            var tempObj = new GameObject("TempOwner");
            var tempOwner = tempObj.AddComponent<Story.TaskBehaviour>();

            var task = OwnerDestroyTestTask();
            task.Start(tempOwner);

            yield return null;

            // オーナーを破棄（シーン遷移等での破棄をシミュレート）
            Object.Destroy(tempObj);

            // システムがオーナーの死を検知し、タスクをキャンセルしてfinallyを実行するまで待機
            yield return null;
            yield return null;

            Assert.IsTrue(hasReachedFinally, "オーナーが破棄された場合、システムはそれを検知してfinallyブロックを実行させるべき");

            async Story.Task OwnerDestroyTestTask()
            {
                try
                {
                    while (true)
                    {
                        await Story.Yield;
                    }
                }
                finally
                {
                    hasReachedFinally = true;
                }
            }
        }

        [UnityTest]
        public IEnumerator Task_子タスクをawait中にキャンセルされた場合_親子両方のfinallyが実行されること()
        {
            bool parentFinally = false;
            bool childFinally = false;

            var parentTask = ParentTask();
            parentTask.Start(this.owner);

            yield return null;

            // 子タスクをawaitしている最中の親をキャンセル
            parentTask.Stop();

            yield return null;

            Assert.IsTrue(childFinally, "親がキャンセルされた際、実行中の子タスクにもキャンセルが伝播してfinallyが実行されるべき");
            Assert.IsTrue(parentFinally, "子タスクのキャンセル処理（finally）が終わった後、親タスクのfinallyも実行されるべき");

            async Story.Task ParentTask()
            {
                try
                {
                    await ChildTask();
                }
                finally
                {
                    parentFinally = true;
                }
            }

            async Story.Task ChildTask()
            {
                try
                {
                    while (true) { await Story.Yield; }
                }
                finally
                {
                    childFinally = true;
                }
            }
        }

        // ------------------------------------------------------------------------
        // 特殊なコンビネータ（With / Until）の仕様テスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_Withコンビネータで一部がキャンセルされても他のタスクは道連れにされないこと()
        {
            bool taskBCompleted = false;

            var taskA = TaskA();
            var taskB = TaskB();
            
            // Withで並行実行
            var withTask = taskA.With(taskB);
            withTask.Start(this.owner);

            yield return null;

            // TaskAだけを外部から強制キャンセルする
            taskA.Stop();

            yield return null;
            
            // TaskBが自然に終わると思われる時間まで待機
            yield return new WaitForSeconds(0.1f);

            Assert.IsTrue(taskBCompleted, "Withで束ねられたTaskAがキャンセルされても、TaskBは道連れにされず最後まで実行されるべき");

            async Story.Task TaskA()
            {
                // 終わらないタスク
                while (true) { await Story.Yield; }
            }

            async Story.Task TaskB()
            {
                // すぐに終わるタスク
                await Story.WaitTime(0.05f);
                taskBCompleted = true;
            }
        }

        [UnityTest]
        public IEnumerator Task_Untilコンビネータで1つが完了すると敗者のタスクは自動的にキャンセルされること()
        {
            bool loserFinallyExecuted = false;

            var winner = WinnerTask();
            var loser = LoserTask();

            // Untilで競争実行
            var untilTask = winner.Until(loser);
            untilTask.Start(this.owner);

            // 勝者タスクが完了し、システムが敗者タスクをクリーンアップするまで待つ
            yield return new WaitForSeconds(0.1f);
            yield return null;

            Assert.IsTrue(loserFinallyExecuted, "Untilで勝者が決まった瞬間、敗者タスクは自動的にStopが呼ばれfinallyが実行されるべき");

            async Story.Task WinnerTask()
            {
                // 早く終わるタスク（勝者）
                await Story.WaitTime(0.05f);
            }

            async Story.Task LoserTask()
            {
                // 終わらないタスク（敗者）
                try
                {
                    while (true) { await Story.Yield; }
                }
                finally
                {
                    loserFinallyExecuted = true;
                }
            }
        }
        
        [UnityTest]
        public IEnumerator Task_Untilの結果受け取りが正しく行われること()
        {
            var taskA = DelayedResultTask(0.1f, "Loser");
            var taskB = DelayedResultTask(0.05f, "Winner"); // こちらが先に終わる

            // 実行と結果取得
            var resultTask = ExecuteAndGetResult();
            resultTask.Start(this.owner);
            
            yield return new WaitForSeconds(0.15f);

            async Story.Task ExecuteAndGetResult()
            {
                var result = await taskA.Until(taskB);
                Assert.AreEqual("Winner", result, "先に完了したタスクの結果が返却されるべき");
            }

            async Story.Task<string> DelayedResultTask(float delay, string result)
            {
                await Story.WaitTime(delay);
                return result;
            }
        }

        // ------------------------------------------------------------------------
        // 実行タイミング（バンド）と同期のテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_指定した実行バンド_Late_Fixed_で正確に再開されること()
        {
            int executionStep = 0;
            var task = BandTestTask();
            
            // Startを呼んだ直後に最初のawait(YieldLate)まで同期的に進む
            task.Start(this.owner);
            Assert.AreEqual(1, executionStep, "Start直後に最初のawaitまで同期的に実行されるべき");

            // YieldLateを待機しているので、このフレームの LateUpdate でタスクが進行する。
            // WaitForEndOfFrame は LateUpdate の直後に実行されるため、これで LateUpdate 通過後を捕捉する。
            yield return new WaitForEndOfFrame();
            Assert.AreEqual(2, executionStep, "LateUpdateが経過したので、YieldFixedの待機まで進んでいるべき");

            // 次は YieldFixed を待機している。
            // WaitForFixedUpdate は FixedUpdate 実行直後に再開されるため、これを利用する。
            yield return new WaitForFixedUpdate();
            Assert.AreEqual(3, executionStep, "FixedUpdateが経過したので、最後まで完了しているべき");

            async Story.Task BandTestTask()
            {
                executionStep = 1;
                await Story.YieldLate;
                
                executionStep = 2;
                await Story.YieldFixed;
                
                executionStep = 3;
            }
        }


        [UnityTest]
        public IEnumerator Task_WaitFrameが指定フレーム数だけ待機すること()
        {
            int startFrame = Time.frameCount;
            int endFrame = 0;

            var task = FrameTask();
            task.Start(this.owner);

            // タスクが終了するまで（タイムアウト付きで）待機
            while (endFrame == 0)
            {
                yield return null;
                if (Time.frameCount - startFrame > 10) break; // 無限ループ防止
            }

            // Unityのコルーチン呼び出しタイミングの都合上、厳密な一致または+1フレームの許容を考慮
            int waitedFrames = endFrame - startFrame;
            Assert.IsTrue(waitedFrames == 5 || waitedFrames == 6, $"WaitFrame(5)は指定フレーム数経過後に再開されるべき（実測: {waitedFrames}フレーム）");

            async Story.Task FrameTask()
            {
                await Story.WaitFrame(5);
                endFrame = Time.frameCount;
            }
        }

        [UnityTest]
        public IEnumerator Task_WaitTimeが指定時間だけ待機すること()
        {
            float targetWaitTime = 0.1f;
            float startTime = Time.time;
            float endTime = 0f;

            var task = TimeTask();
            task.Start(this.owner);

            // タスクが終了するまで（タイムアウト付きで）待機
            while (endTime == 0f)
            {
                yield return null;
                if (Time.time - startTime > 1.0f) break; // 無限ループ防止
            }

            float duration = endTime - startTime;
            // フレームレート（Time.deltaTime）による誤差を考慮してアサート
            Assert.IsTrue(duration >= targetWaitTime && duration < targetWaitTime + 0.05f, 
                $"WaitTime({targetWaitTime}f)は指定時間経過後に再開されるべき（実測: {duration}秒）");

            async Story.Task TimeTask()
            {
                await Story.WaitTime(targetWaitTime);
                endTime = Time.time;
            }
        }
    }
}
