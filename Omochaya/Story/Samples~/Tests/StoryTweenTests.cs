namespace OmochayaTests
{
    using System.Collections;
    using NUnit.Framework;
    using UnityEngine;
    using UnityEngine.TestTools;
    using Omochaya;

    public class StoryTweenTests
    {
        GameObject testObj;
        Transform testTransform;

        [SetUp]
        public void Setup()
        {
            StoryTestRunner.Require();

            this.testObj = new GameObject("TweenTestObject");
            this.testTransform = this.testObj.transform;
        }

        [TearDown]
        public void Teardown()
        {
            if (this.testObj != null)
            {
                Object.DestroyImmediate(this.testObj);
                this.testObj = null;
                this.testTransform = null;
            }
        }

        // ------------------------------------------------------------------------
        // Tweenの挙動テスト
        // ------------------------------------------------------------------------

        [Test]
        public void Ease_CurveImplが正確に評価されアロケーションが発生しないこと()
        {
            // テスト用の直線カーブ（時間 0〜1、値 0〜1）を生成
            var linearCurve = AnimationCurve.Linear(0f, 0f, 1f, 1f);
            var ease = linearCurve.ToEase();

            // 1. アロケーションと評価のテスト
            using (Utils.Check())
            {
                // 0.0, 0.5, 1.0 の評価が正確か
                Assert.AreEqual(0.0f, ease.Calc(0.0f), "進行度0での評価が間違っています");
                Assert.AreEqual(0.5f, ease.Calc(0.5f), "進行度0.5での評価が間違っています");
                Assert.AreEqual(1.0f, ease.Calc(1.0f), "進行度1での評価が間違っています");
                
                // （任意）オーバーシュートのテストが必要な場合は、1を超えるカーブで検証
            }

            // 2. Nullフォールバックのテスト
            var nullEase = Story.Ease.Curve(null);
            Assert.DoesNotThrow(() => nullEase.Calc(0.5f), "Nullのカーブが渡された場合に例外が発生してはいけません");
            Assert.AreEqual(0.5f, nullEase.Calc(0.5f), "Null時は進行度がそのまま返却されるべきです");
        }

        // ------------------------------------------------------------------------
        // エッジケース・ライフサイクルのテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Tween_オーナーのGameObjectが破壊されるとTweenが安全にキャンセルされること()
        {
            // Mover.TryKeep によって testTransform がオーナーとして自動設定される
            var task = this.testTransform.TweenLocalPosition().To(Vector3.one * 100f).Interval(10f);
            task.Start();

            // 1フレーム進めてTweenを開始させる
            yield return null;

            // 実行中にオーナーを破棄
            Object.Destroy(this.testObj);
            this.testObj = null;

            // オーナーの破棄（null化）をシステムが検知してキャンセル処理を行うまで待つ
            yield return null;

            Assert.IsFalse(task.IsValid, "オーナーとなるオブジェクトが破壊された場合、タスクは安全にキャンセルされ無効になるべき");

            // STORY_NO_DEBUG 時に失敗することがある（owner の削除を検知してキャンセルを発生させる時に throw するため）
            // Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_Intervalが0の場合は即座に完了し最終値が適用されること()
        {
            var from = Vector3.zero;
            var to = Vector3.one;
            this.testTransform.localPosition = from;

            // 期間0秒で指定
            var task = this.testTransform.TweenLocalPosition().To(to).Interval(0f);
            task.Start();

            Assert.IsFalse(task.IsValid, "Intervalが0の場合は即座に完了しているべき");
            Assert.AreEqual(to, this.testTransform.localPosition, "即座に完了しても、最終値は正しく適用されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_Speedが0の場合は移動せずに即座に完了すること()
        {
            var from = Vector3.zero;
            var to = Vector3.one;
            this.testTransform.localPosition = from;

            // 速度0で指定 (StoryTween.cs の仕様上、speed=0 は始まらない＝初期値のまま即了)
            var task = this.testTransform.TweenLocalPosition().To(to).Speed(0f);
            task.Start();

            Assert.IsFalse(task.IsValid, "Speedが0の場合は即座に完了しているべき");
            Assert.AreEqual(from, this.testTransform.localPosition, "Speedが0の場合、移動は一切発生しないべき（初期値のまま）");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_By指定による相対移動が現在の値に正しく加算されること()
        {
            // By で相対移動 (x に +10f)
            var task = this.testTransform.TweenLocalPosition().By(new Vector3(10f, 0f, 0f)).Interval(0.1f);

            // 相対位置は遅延評価される
            this.testTransform.localPosition = new Vector3(1f, 2f, 3f);

            task.Start();

            while (task.IsValid) { yield return null; }

            Assert.AreEqual(new Vector3(11f, 2f, 3f), this.testTransform.localPosition, "By()による指定は、開始時点の現在値に正確に加算されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        // ------------------------------------------------------------------------
        // 特殊なTweenのテスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Tween_UnscaledIntervalがTimeScaleの影響を受けずに進行すること()
        {
            var originalTimeScale = Time.timeScale;
            Time.timeScale = 0f; // タイムスケールをゼロにする

            this.testTransform.localPosition = Vector3.zero;
            
            // Unscaled で実行
            var task = this.testTransform.TweenLocalPosition().To(Vector3.one).UnscaledInterval(0.1f);
            task.Start();

            var startTime = Time.realtimeSinceStartup;
            while (task.IsValid)
            {
                yield return null;
                if (Time.realtimeSinceStartup - startTime > 1.0f) { break; } // フェイルセーフ
            }

            Time.timeScale = originalTimeScale; // 復元

            Assert.IsFalse(task.IsValid, "Time.timeScale = 0 の環境下でも、UnscaledTweenは進行し完了するべき");
            Assert.AreEqual(Vector3.one, this.testTransform.localPosition, "UnscaledTweenによる最終値が正しく適用されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_デリゲートによる独自更新処理がアロケーションなしで実行されること()
        {
            var from = 1f;
            var to = 3f;
            this.testTransform.localPosition = Vector3.zero;

            // Updaterデリゲートを利用したTween
            var task = Story.Tween(from, to, 100f, this.testTransform, static (self, now) => self.localPosition = new(now, 0f, 0f));
            task.Start(this.testTransform);
            Assert.AreEqual(from, this.testTransform.localPosition.x, "指定した値から始まっているべき");

            while (task.IsValid) { yield return null; }

            Assert.AreEqual(to, this.testTransform.localPosition.x, "指定した値まで処理されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        // ------------------------------------------------------------------------
        // 追加のTween挙動テスト
        // ------------------------------------------------------------------------

        [UnityTest]
        public IEnumerator Tween_一部の成分のみを指定した補間が正しく機能し_他の成分に影響を与えないこと()
        {
            var initialPosition = new Vector3(1f, 2f, 3f);
            this.testTransform.localPosition = initialPosition;
            
            // X成分のみを 10f に変更するTween
            var task = this.testTransform.TweenLocalPosition().To(x: 10f).Interval(0.1f);
            var task2 = this.testTransform.TweenLocalPosition().To(y: 20f).Interval(0.1f);
            task.Start();
            task2.Start();

            while (task.IsValid) { yield return null; }

            var pos = this.testTransform.localPosition;
            Assert.AreEqual(10f, pos.x, "指定したX成分が更新されているべき");
            Assert.AreEqual(20f, pos.y, "指定したY成分が更新されているべき");
            Assert.AreEqual(3f, pos.z, "指定していないZ成分は初期値のまま維持されるべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_Stopにより手動でキャンセルした場合_その時点の値で更新が停止すること()
        {
            this.testTransform.localPosition = Vector3.zero;
            
            // 1秒かけてX軸に100移動するTween
            var task = this.testTransform.TweenLocalPosition().To(x: 100f).Interval(1f);
            task.Start();

            // 数フレームだけ進める
            yield return null;
            yield return null;
            yield return null;

            // 手動でキャンセル
            task.Stop();

            // キャンセル後、さらに数フレーム進める
            yield return null;
            yield return null;

            var pos = this.testTransform.localPosition;
            Assert.IsFalse(task.IsValid, "Stop後はタスクが無効になっているべき");
            Assert.IsTrue(pos.x > 0f && pos.x < 100f, $"Tweenは途中で停止しているべき（現在のX座標: {pos.x}）");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_awaitにより完了まで待機でき_後続処理が正しく実行されること()
        {
            var isCompleted = false;

            using (Story.WarmupMode())
            {
                this.testTransform.TweenLocalPosition().To(Vector3.one).Interval(0.1f).Warmup();
            }
            
            // ローカル関数としてテスト用タスクを定義
            [Story.Capacity(8)]
            async Story.Task AwaitTestTask()
            {
                // 0.1秒のTweenをawait
                await this.testTransform.TweenLocalPosition().To(Vector3.one).Interval(0.1f);
                isCompleted = true;
            }

            var task = AwaitTestTask();
            task.Start(this.testObj.AddComponent<Story.TaskBehaviour>());

            while (task.IsValid) { yield return null; }

            Assert.IsTrue(isCompleted, "awaitでの待機が完了し、後続のフラグ変更処理が実行されているべき");
            Assert.AreEqual(Vector3.one, this.testTransform.localPosition, "Tweenの最終値が適用されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [Test]
        public void Ease_合成や加工を行う拡張メソッドが例外なく動作し_境界値が正確であること()
        {
            using (Utils.Check())
            {
                // 1. Stitchのテスト（SineAccとSineDecの結合）
                var joinEase = Story.Ease.SineDec.Stitch(Story.Ease.SineAcc);
                Assert.AreEqual(0f, joinEase.Calc(0f), "前半（SineAcc）の開始時は0fであるべき");
                Assert.IsTrue(Mathf.Approximately(0.5f, joinEase.Calc(0.5f)), "中間地点（0.5f）では適切に接続されているべき");
                Assert.AreEqual(1f, joinEase.Calc(1f), "後半（SineDec）の終了時は1fであるべき");

                // 2. Fixのテスト（正規化）
                var fixEase = Story.Ease.SineAcc.FromTo(10f, 20f).Fix();
                Assert.AreEqual(0f, fixEase.Calc(0f), "Fix加工後は開始値が0fに正規化されているべき");
                Assert.AreEqual(1f, fixEase.Calc(1f), "Fix加工後は終了値が1fに正規化されているべき");

                // 3. Reverseのテスト
                var reverseEase = Story.Ease.QuadAcc.Reverse();
                Assert.AreEqual(1f, reverseEase.Calc(0f), "Reverseの開始は1f");
                Assert.AreEqual(0f, reverseEase.Calc(1f), "Reverseの終了は0f");
            }
        }

        [UnityTest]
        public IEnumerator Tween_QuaternionのBy指定による相対回転が正しく計算されること()
        {
            // 初期角度をY軸45度に設定
            this.testTransform.localRotation = Quaternion.Euler(0f, 45f, 0f);
            
            // Y軸にさらに90度相対回転させる (45 + 90 = 135度になるはず)
            var deltaRotation = Quaternion.Euler(0f, 90f, 0f);
            var task = this.testTransform.TweenLocalRotation().By(deltaRotation).Interval(0.1f);
            task.Start();

            while (task.IsValid) { yield return null; }

            // 浮動小数点誤差を許容して角度を比較
            var finalEulerY = this.testTransform.localEulerAngles.y;
            Assert.IsTrue(Mathf.Abs(Mathf.DeltaAngle(135f, finalEulerY)) < 0.1f, 
                $"Quaternionの相対回転(By)が正しく乗算されているべき（実測: {finalEulerY}）");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_Colorの一部の成分のみを指定した補間が正しく機能すること()
        {
            // テスト用のSpriteRendererを追加して初期色を設定
            var spriteRenderer = this.testObj.AddComponent<SpriteRenderer>();
            spriteRenderer.color = new Color(1f, 0f, 0f, 1f); // 赤色、不透明
            
            // アルファ値（a）のみを 0.5f に変更するTween
            var task = spriteRenderer.TweenColor().To(a: 0.5f).Interval(0.1f);
            task.Start();

            while (task.IsValid) { yield return null; }

            var color = spriteRenderer.color;
            Assert.AreEqual(0.5f, color.a, "指定したAlpha成分が更新されているべき");
            Assert.AreEqual(1f, color.r, "指定していないRed成分は初期値のまま維持されるべき");
            Assert.AreEqual(0f, color.g, "指定していないGreen成分は初期値のまま維持されるべき");
            Assert.AreEqual(0f, color.b, "指定していないBlue成分は初期値のまま維持されるべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_Speed指定による移動が距離に基づいた正確な時間で完了すること()
        {
            // 距離100の移動をセット
            this.testTransform.localPosition = Vector3.zero;
            var targetPos = new Vector3(100f, 0f, 0f);
            
            // 速度200で移動（100 / 200 = 0.5秒で完了するはず）
            var speed = 200f;
            var expectedDuration = 0.5f;
            
            var startTime = Time.time;
            var task = this.testTransform.TweenLocalPosition().To(targetPos).Speed(speed);
            task.Start();

            while (task.IsValid)
            {
                yield return null;
                // 無限ループ防止フェイルセーフ
                if (Time.time - startTime > 2.0f) { break; }
            }

            var actualDuration = Time.time - startTime;

            // 最終位置の確認
            Assert.AreEqual(targetPos, this.testTransform.localPosition, "Speed指定のTweenが目標位置に到達しているべき");

            // 所要時間の確認（フレームレートのブレを考慮し、±0.05秒の誤差を許容）
            Assert.IsTrue(actualDuration >= expectedDuration && actualDuration < expectedDuration + 0.05f, 
                $"Speed指定による所要時間({expectedDuration}秒)が正確に計算されているべき（実測: {actualDuration}秒）");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }

        [UnityTest]
        public IEnumerator Tween_開始時間を未来に設定した遅延起動_Delay_が正しく機能すること()
        {
            this.testTransform.localPosition = Vector3.zero;
            
            // 現在時刻から0.3秒後を開始時間として設定
            var delaySeconds = 0.3f;
            var start = Story.GetStart() + delaySeconds;
            
            // 0.3秒後に開始し、0.1秒かけて移動するTween
            var task = this.testTransform.TweenLocalPosition().To(Vector3.one).Interval(0.1f, ref start);
            task.Start();

            // 0.15秒待機 (まだ開始時間である0.3秒に到達していない)
            var waitStartTime = Time.time;
            while (Time.time - waitStartTime < 0.15f) { yield return null; }

            Assert.AreEqual(Vector3.zero, this.testTransform.localPosition, "指定した開始時間に達するまでは、Tweenによる値の更新は行われないべき");
            Assert.IsTrue(task.IsValid, "遅延待機中もタスクは有効であるべき");

            // さらに待機して完了させる
            while (task.IsValid)
            {
                yield return null;
                if (Time.time - waitStartTime > 1.0f) { break; } // フェイルセーフ
            }

            Assert.AreEqual(Vector3.one, this.testTransform.localPosition, "開始時間を過ぎた後にTweenが実行され、最終値が適用されているべき");

            Utils.LogGCAlloc();

            // compaction を処理させて次のテストへ影響させない
            yield return null;
        }
    }
}
