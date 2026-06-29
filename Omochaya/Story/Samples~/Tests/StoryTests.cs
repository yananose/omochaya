namespace OmochayaTests
{
    using System.Collections;
    using System.Collections.Generic;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Omochaya;

    public class StoryTests
    {
        // static
        static StoryTestRunner runner;

        // inner classes
        internal class StoryTestRunner : MonoBehaviour
        {
            // methods
            void Awake()
            {
                Story.Custom(3, 1024);
                Story.WaitTime(0f).Warmup(); // プールの事前確保
            }

            void Update() 
            { 
                using (Utils.Check())
                {
                    Story.Update();
                }
            }

            void LateUpdate()
            {
                using (Utils.Check())
                {
                    Story.LateUpdate();
                }
            }

            void FixedUpdate()
            {
                using (Utils.Check())
                {
                    Story.FixedUpdate();
                }
            }
        }

        // fields

        GameObject ownerObj;
        Story.TaskBehaviour owner;

        // methods

        [SetUp]
        public void Setup()
        {
            if (runner == null)
            {
                StoryTests.runner = new GameObject("StoryTestRunner").AddComponent<StoryTestRunner>();
            }
            this.ownerObj = new GameObject("StoryTestOwner");
            this.owner = this.ownerObj.AddComponent<Story.TaskBehaviour>();
        }

        [TearDown]
        public void Teardown()
        {
            if (this.ownerObj != null)
            {
                Object.DestroyImmediate(this.ownerObj);
                this.ownerObj = null;
                this.owner = null;
            }
        }

        // ------------------------------------------------------------------------
        // コルーチンとの比較
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_コルーチンとの比較()
        {
            // 【Story専用】各タスクを Warmup することでプールを事前確保できます（任意）
            StoryMain(null).Warmup();
            StorySub(null).Warmup(8); // サイズを指定することもできます（拡張のみ可能）

            // 記録帳
            var coroutineNote = new List<int>(1024);
            var storyNote = new List<int>(1024);

            Omochaya.HiddenStory.Dev.Log("〜 各タスク生成 〜");
            var coroutineTask = CoroutineMain(coroutineNote); // コルーチンの場合
            var storyTask = StoryMain(storyNote); // Story の場合

            // 【Story専用】フレームをまたいだ時にオーナーがいないと解放されてしまうのでKeepしておく（直接Bootするなら不要）。
            storyTask.Keep(this.owner);

            Omochaya.HiddenStory.Dev.Log("〜 0.1秒待つ 〜");
            yield return new WaitForSeconds(0.1f);

            Omochaya.HiddenStory.Dev.Log("〜 各タスク起動 〜");
            this.owner.StartCoroutine(coroutineTask); // コルーチンの場合
            storyTask.Start(this.owner); // Story の場合

            // 各タスクが最後のループに到達するくらいまで待つ
            yield return new WaitForSeconds(0.5f);

            Omochaya.HiddenStory.Dev.Log("〜 各タスク終了 〜");
            this.owner.StopCoroutine(coroutineTask); // コルーチンの場合
            storyTask.Stop(); // Story の場合

            Omochaya.HiddenStory.Dev.Log("〜 0.1秒待つ 〜");
            yield return new WaitForSeconds(0.1f);

            // 結果
            Utils.Result(coroutineNote, storyNote);
            Utils.LogGCAlloc();

            // 〜〜 ここからタスク定義 〜〜

            // メインタスク（コルーチン）
            IEnumerator CoroutineMain(List<int> note)
            {
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("開始した（コルーチン：メイン）");
                yield return null;

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("サブタスクの呼び出し（コルーチン：メイン）");
                yield return CoroutineSub(note).GetEnumerator();

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("サブタスクを手動で回す（コルーチン：メイン）");
                foreach (var _ in CoroutineSub(note))
                {
                    yield return null;
                    Utils.Take(note);
                }

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("最後のループに到達（コルーチン：メイン）");
                while (true)
                {
                    yield return null;
                    Utils.Take(note);
                }
            }

            // メインタスク（Story）
            async Story.Task StoryMain(List<int> note)
            {
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("開始した（Story：メイン）");
                await Story.Yield;

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("サブタスクの呼び出し（Story：メイン）");
                await StorySub(note);

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("サブタスクを手動で回す（Story：メイン）");
                foreach (var _ in StorySub(note))
                {
                    await Story.Yield;
                    Utils.Take(note);
                }

                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("最後のループに到達（Story：メイン）");
                while (true)
                {
                    await Story.Yield;
                    Utils.Take(note);
                }
            }

            // サブタスク（コルーチン）
            IEnumerable CoroutineSub(List<int> note)
            {
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("開始（コルーチン：サブ）");
                yield return null;
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("システムから戻る（コルーチン：サブ）");
                yield return null;
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("終了（コルーチン：サブ）");
            }

            // サブタスク（Story）
            async Story.Task StorySub(List<int> note)
            {
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("開始（Story：サブ）");
                await Story.Yield;
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("システムから戻る（Story：サブ）");
                await Story.Yield;
                Utils.Take(note); Omochaya.HiddenStory.Dev.Log("終了（Story：サブ）");
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

            Assert.IsFalse(task.IsValid, "実行が完了したタスクはプールに返却され、自動的に無効になるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
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

            Utils.LogGCAlloc();

            [Story.Capacity(8)] // これで最初に使用する時に確保するプールのサイズを指定できます
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

            Utils.LogGCAlloc();

            [Story.Capacity(1024)]
            async Story.Task EmptyTask() { await Story.Void; }
        }

        // ------------------------------------------------------------------------
        // ライフサイクルとキャンセル処理のテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_Stopを呼ぶとキャンセルされfinallyブロックが実行されること()
        {
            var hasReachedFinally = false;
            var hasExecutedAfterYield = false;

            var task = CancelTestTask();
            task.Start(this.owner);

            // 外部から強制キャンセル
            task.Stop();

            // タスクが動作していないことを確認するために1フレーム進める
            yield return null;

            Assert.IsFalse(hasExecutedAfterYield, "キャンセルされたため、Yield以降の通常処理は実行されないべき");
            Assert.IsTrue(hasReachedFinally, "キャンセルされた場合でも、finallyブロックは確実に実行されるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
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
            var hasReachedFinally = false;

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

            Assert.IsTrue(hasReachedFinally, "オーナーが破棄された場合、システムはそれを検知してfinallyブロックを実行させるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
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
            var parentFinally = false;
            var childFinally = false;

            var parentTask = ParentTask();
            parentTask.Start(this.owner);

            yield return null;

            // 子タスクをawaitしている最中の親をキャンセル
            parentTask.Stop();

            yield return null;

            Assert.IsTrue(childFinally, "親がキャンセルされた際、実行中の子タスクにもキャンセルが伝播してfinallyが実行されるべき");
            Assert.IsTrue(parentFinally, "子タスクのキャンセル処理（finally）が終わった後、親タスクのfinallyも実行されるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
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

            [Story.Capacity(8)]
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
            var taskBCompleted = false;

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

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task TaskA()
            {
                // 終わらないタスク
                while (true) { await Story.Yield; }
            }

            [Story.Capacity(8)]
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
            var loserFinallyExecuted = false;

            var winner = WinnerTask();
            var loser = LoserTask();

            // Untilで競争実行
            var untilTask = winner.Until(loser);
            untilTask.Start(this.owner);

            // 勝者タスクが完了し、システムが敗者タスクをクリーンアップするまで待つ
            while (winner.IsValid) { yield return null; }
            yield return null;

            Assert.IsTrue(loserFinallyExecuted, "Untilで勝者が決まった瞬間、敗者タスクは自動的にStopが呼ばれfinallyが実行されるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task WinnerTask()
            {
                // 早く終わるタスク（勝者）
                await Story.WaitTime(0.05f);
            }

            [Story.Capacity(8)]
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
            // テスト実施
            var testTask = ExecuteTest();
            testTask.Start(this.owner);

            // テスト完了待ち
            while (testTask.IsValid) { yield return null; }

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            static async Story.Task ExecuteTest()
            {
                var result = await Story.Until(
                    DelayedResultTask(0.5f, "Loser"), // WaitTime はフレーム単位で時間を見るため処理落ちで同着にならないよう delay は大きめに。
                    DelayedResultTask(0.05f, "Winner"));
                Assert.AreEqual("Winner", result, "先に完了したタスクの結果が返却されるべき");
            }

            [Story.Capacity(8)]
            static async Story.Task<string> DelayedResultTask(float delay, string result)
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
            var executionStep = 0;
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

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
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
            var startFrame = Time.frameCount;
            var endFrame = 0;

            var task = FrameTask();
            task.Start(this.owner);

            // タスクが終了するまで（タイムアウト付きで）待機
            while (endFrame == 0)
            {
                yield return null;
                if (Time.frameCount - startFrame > 10) break; // 無限ループ防止
            }

            // Unityのコルーチン呼び出しタイミングの都合上、厳密な一致または+1フレームの許容を考慮
            var waitedFrames = endFrame - startFrame;
            Assert.IsTrue(waitedFrames == 5 || waitedFrames == 6, $"WaitFrame(5)は指定フレーム数経過後に再開されるべき（実測: {waitedFrames}フレーム）");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task FrameTask()
            {
                await Story.WaitFrame(5);
                endFrame = Time.frameCount;
            }
        }

        [UnityTest]
        public IEnumerator Task_WaitTimeが指定時間だけ待機すること()
        {
            var targetWaitTime = 0.1f;
            var startTime = Time.time;
            var endTime = 0f;

            var task = TimeTask();
            task.Start(this.owner);

            // タスクが終了するまで（タイムアウト付きで）待機
            while (endTime == 0f)
            {
                yield return null;
                if (Time.time - startTime > 1.0f) break; // 無限ループ防止
            }

            var duration = endTime - startTime;
            // フレームレート（Time.deltaTime）による誤差を考慮してアサート
            Assert.IsTrue(duration >= targetWaitTime && duration < targetWaitTime + 0.05f, 
                $"WaitTime({targetWaitTime}f)は指定時間経過後に再開されるべき（実測: {duration}秒）");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task TimeTask()
            {
                await Story.WaitTime(targetWaitTime);
                endTime = Time.time;
            }
        }

        // ------------------------------------------------------------------------
        // 一時停止（Pause）のテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_オーナーが非アクティブになるとタスクが一時停止し_アクティブで再開されること()
        {
            var step = 0;
            var task = PauseTestTask();
            task.Start(this.owner);

            Assert.AreEqual(1, step, "最初のYieldまでは進むべき");
            yield return null;
            Assert.AreEqual(2, step, "次のYieldまで進むべき");

            // オーナーを無効化（一時停止）
            this.owner.enabled = false;
            
            // 数フレーム待機しても進まないことを確認
            for (var i = 0; i < 5; i++) { yield return null; }
            Assert.AreEqual(2, step, "オーナーが非アクティブの間はタスクが進行しないべき");

            // オーナーを有効化（再開）
            this.owner.enabled = true;
            yield return null;
            Assert.AreEqual(3, step, "オーナーがアクティブに戻るとタスクが再開されるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task PauseTestTask()
            {
                step = 1;
                await Story.Yield;
                step = 2;
                await Story.Yield;
                step = 3;
            }
        }

        // ------------------------------------------------------------------------
        // キャンセル時の戻り値無効検知テスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_子タスクがキャンセルされた際_IsResultInvalidで検知できること()
        {
            var isResultInvalid = false;
            var receivedResult = "Not Set";

            var child = ChildTaskWithResult();
            var parent = ParentTask(child);
            parent.Start(this.owner);

            yield return null;

            // 結果を返す子タスクをキャンセル
            child.Stop();

            yield return null;

            Assert.IsTrue(isResultInvalid, "結果の取得に失敗しているべき");
            Assert.AreEqual(receivedResult, default(string), "失敗した時の結果はデフォルト値であるべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task ParentTask(Story.Task<string> child)
            {
                receivedResult = await child;
                isResultInvalid = Story.IsResultInvalid();
            }

            [Story.Capacity(8)]
            async Story.Task<string> ChildTaskWithResult()
            {
                await Story.WaitTime(10f);
                return "Success";
            }
        }

        [UnityTest]
        public IEnumerator Task_Timeoutコンビネータが指定時間でタスクをキャンセルすること()
        {
            var hasReachedFinally = false;

            var task = InfiniteTask().Timeout(0.1f);
            task.Start(this.owner);

            // Timeout時間より少し長く待つ
            yield return new WaitForSeconds(0.5f);

            Assert.IsTrue(hasReachedFinally, "Timeoutによって元のタスクがキャンセルされ、finallyが呼ばれているべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task InfiniteTask()
            {
                try
                {
                    while (true) { await Story.Yield; }
                }
                finally
                {
                    hasReachedFinally = true;
                }
            }
        }

        // ------------------------------------------------------------------------
        // Void と Extra のテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_Voidをawaitしてもフレームを消費しないこと()
        {
            var startFrame = Time.frameCount;
            var endFrame = 0;

            var task = VoidTask();
            task.Start(this.owner);

            // 1フレーム進める
            yield return null;

            Assert.AreEqual(startFrame, endFrame, "Story.Voidのawaitは同フレーム内で即座に完了するべき");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task VoidTask()
            {
                await Story.Void;
                endFrame = Time.frameCount;
            }
        }

        [Test]
        public void Task_Extra領域にデータを保存し取得できること()
        {
            var task = ExtraTestTask();
            // StartしていなくてもTaskハンドルが有効であればExtraは使える
            task.SetExtra(new TestExtraData { Value = 42 });

            var extra = task.GetExtra<TestExtraData>();
            Assert.AreEqual(42, extra.Value, "SetExtraで設定した値がGetExtraで正確に取得できるべき");

            task.Stop();

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task ExtraTestTask() { await Story.Void; }
        }

        struct TestExtraData
        {
            public int Value;
        }


        // ------------------------------------------------------------------------
        // 例外伝播とキャンセル（例外握りつぶし）の挙動テスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Task_子タスクで発生した例外を親タスクのtry_catchで正常に捕捉できること()
        {
            var hasCaughtException = false;
            var testException = new System.Exception("ExpectedTestException");

            ThrowChildTask().Warmup(); // testException を参照するのでこの位置

            var task = ParentTask();
            task.Start(this.owner);

            yield return null;

            Assert.IsTrue(hasCaughtException, "子タスクでスローされた例外が、親タスクのcatchブロックで正しく捕捉されるべき");

            // SIMPLE_CHECK 時に失敗することがある...ナゼ？
            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task ParentTask()
            {
                try
                {
                    await ThrowChildTask();
                }
                catch (System.Exception e)
                {
                    hasCaughtException = true;
                    Assert.AreEqual("ExpectedTestException", e.Message);
                }
            }

            [Story.Capacity(8)]
            async Story.Task ThrowChildTask()
            {
                await Story.Yield;
                throw testException;
            }
        }

        [UnityTest]
        public IEnumerator Task_警告_すべてのExceptionをcatchするとキャンセル処理も握りつぶしてしまうことの確認()
        {
            var hasCaughtException = false;
            var hasReachedNextLine = false;

            // 良い例
            var task = GoodCancelTask();
            task.Start(this.owner);

            yield return null;

            // 外部からキャンセルを要求
            task.Stop();

            Assert.IsTrue(hasCaughtException, "キャンセルすると例外が発生しているはず");
            Assert.IsFalse(hasReachedNextLine, "適切にキャンセルされていれば到達していないはず");

            // 悪い例
            task = BadCancelTask();
            task.Start(this.owner);

            yield return null;

            // 外部からキャンセルを要求
            task.Stop();

            Assert.IsTrue(hasCaughtException, "キャンセルすると例外が発生しているはず");
            Assert.IsTrue(hasReachedNextLine, "キャンセルを握り潰すと到達しているはず");

            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task GoodCancelTask()
            {
                hasCaughtException = false;
                hasReachedNextLine = false;
                try
                {
                    await Story.WaitTime(1f);
                }
                catch (System.Exception e)
                {
                    hasCaughtException = true; // 例外発生
                    // キャンセルだったら上に通す
                    Story.ThrowIfCancel(e);
                }

                hasReachedNextLine = true; // 握りつぶすとここまで到達してしまう
            }

            [Story.Capacity(8)]
            async Story.Task BadCancelTask()
            {
                hasCaughtException = false;
                hasReachedNextLine = false;
                try
                {
                    await Story.WaitTime(1f);
                }
                catch (System.Exception)
                {
                    hasCaughtException = true; // 例外発生
                    // 握りつぶす
                    // // キャンセルだったら上に通す
                    // Story.ThrowIfCancel(e);
                }

                hasReachedNextLine = true; // 握りつぶすとここまで到達してしまう
            }
        }

        [UnityTest]
        public IEnumerator Task_誰もawaitしていないタスクの例外が発生しても_マネージャーの更新ループがクラッシュしないこと()
        {
            // Unityコンソールに例外が出力されること自体は許容（期待）する
            UnityEngine.TestTools.LogAssert.Expect(LogType.Exception, new System.Text.RegularExpressions.Regex(".*UnhandledException.*"));

            var isSurvivorExecuted = false;
            var unhandledException = new System.Exception("UnhandledException");

            var throwTask = ThrowTask();
            var survivorTask = SurvivorTask();

            // 意図的に同じフレームで実行されるように並べてStart
            throwTask.Start(this.owner);
            survivorTask.Start(this.owner);

            // 1フレーム進めて、TaskManagerに両方のタスクを処理させる
            yield return null;
            yield return null;

            // もし内部で「throw;」されてループがクラッシュしていると、
            // 後続の survivorTask が処理されず、ここは false のままになりテストが失敗する
            Assert.IsTrue(isSurvivorExecuted, "例外が発生したタスクがあってもマネージャーのループがクラッシュせず、他のタスクが正常に処理されるべき");

            UnityEngine.Debug.Log("ここまできてますか？");

            // SIMPLE_CHECK 時はここで必ず失敗する（LogException()が呼ばれるためアロケートする）
            Utils.LogGCAlloc();

            [Story.Capacity(8)]
            async Story.Task ThrowTask()
            {
                await Story.Yield;
                throw unhandledException;
            }

            [Story.Capacity(8)]
            async Story.Task SurvivorTask()
            {
                await Story.Yield;
                await Story.Yield;
                isSurvivorExecuted = true;
            }
        }

    }
}
