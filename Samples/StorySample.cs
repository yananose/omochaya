using UnityEngine;
using Omochaya;

public class StorySample : MonoBehaviour
// public class StorySample : Story.TaskBehaviour // こちらだと Start に指定した場合にタスクが高速化されます。OnDestroy が必要な場合は代わりに OnDestroyed を override してください。
{
    const float Range = 500f;
    const float Speed = 250f;

    [SerializeField]
    RectTransform red;

    [SerializeField]
    RectTransform blue;

    [SerializeField]
    RectTransform white;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        // Android で移動がカクつく場合は、Project Settings から Optimize Frame Pacing にチェックを入れてください。

        // 使用するタスク数を宣言します。
        // 宣言した数を超えても動作しますが、管理配列がリサイズされてアロケートが発生します。
        // 気にしない場合は省略しても問題ありません。
        Story.Custom(taskCount: 10);

        // タスクを起動します。
        // ここではテスト用のルートタスクを起動しています。
        // タスク外で起動する場合は、誰に所属するか指定する必要があります。
        // ここでは this を指定しています。
        RootTask().Start(this);

        // テスト前に初期位置へ移動させておく
        void SetInitialPosition(RectTransform rt)
        {
            var position = rt.anchoredPosition;
            position.x = -Range * 0.5f;
            rt.anchoredPosition = position;
        }
        SetInitialPosition(this.red);
        SetInitialPosition(this.blue);
        SetInitialPosition(this.white);
    }

    // Update is called once per frame
    void Update()
    {
        // 全タクスを更新するので、毎フレーム１回だけ呼び出してください。
        // 例えばここで呼ぶ場合は他のコンポーネントでは呼ばないでください。
        // シングルトンの常駐するゲームオブジェクトがある場合は、その Update() でのみ呼び出すのが確実です。
        Story.Update();
    }
    void LateUpdate()
    {
        // 全タクスを更新するので、毎フレーム１回だけ呼び出してください。
        // 例えばここで呼ぶ場合は他のコンポーネントでは呼ばないでください。
        // シングルトンの常駐するゲームオブジェクトがある場合は、その LateUpdate() でのみ呼び出すのが確実です。
        Story.LateUpdate();
    }
    void FixedUpdate()
    {
        // 全タクスを更新するので、毎フレーム１回だけ呼び出してください。
        // 例えばここで呼ぶ場合は他のコンポーネントでは呼ばないでください。
        // シングルトンの常駐するゲームオブジェクトがある場合は、その FixedUpdate() でのみ呼び出すのが確実です。
        Story.FixedUpdate();
    }

    async Story.Task RootTask()
    {
        // アプリ起動直後は安定しないのでちょっと待つ
        var wait = Time.time + 0.5f;
        while (Time.time < wait) { await Story.Yield; }

        // タスク内でタスクを起動するときは、誰に所属するかは省略できます。
        // 省略した場合は起動したタスクと同じところに所属します。
        var whiteSubTask = SubTask(this.white);
        whiteSubTask.Start();

        // 別の所属先を指定することも可能です。
        SubTask(this.red).Start(this.red);
        SubTask(this.blue).Start(this.red);
        // BlueTask(this.blue).Start(this.red); // SubTask の代わりにこちらで finally ブロックのテストができます。赤が消えた後も青が左端まで移動します

        // 【タスク停止のテスト】待つ
        wait = Time.time + 1f;
        while (Time.time < wait) { await Story.Yield; }

        // 【タスク解放のテスト】白が停止します。
        whiteSubTask.Stop();

        // 【タスク停止のテスト】待つ
        wait = Time.time + 1f;
        while (Time.time < wait) { await Story.Yield; }

        while (true)
        {
            // await でのタスク実行
            await SubTaskRight(this.white);
            await SubTaskLeft(this.white);

            // MoveNext での手動タスク実行
            foreach (var _ in SubTaskRight(this.white)) { await Story.Yield; }
            foreach (var _ in SubTaskLeft(this.white)) { await Story.Yield; }

            // 【遅延起動のテスト】タスクを作成だけする
            var delayRight = SubTaskRight(this.white);
            var delayLeft = SubTaskLeft(this.white);

            // 【遅延起動のテスト】実行しないタスクが勝手に解放されないようにする
            delayRight.Keep();
            delayLeft.Keep();

            // 【遅延起動のテスト】待つ
            wait = Time.time + 1f;
            while (Time.time < wait) { await Story.Yield; }

            // 【遅延起動のテスト】作成していたタスクを遅延起動する
            await delayRight;
            await delayLeft;

            // 【所属先が消えたらタスクが停止するテスト】
            // 青 が所属している 赤 が消えると 青 の移動（タスク）が停止します。
            // 青のタスクはルートタスクの後で起動しているため、青のタスクがルートタスクよりも先に処理されます（後着優先）。
            // そのためルートタスクで操作（赤のタスクの削除）した結果が青のタスクに反映されるのは１フレーム後になります。
            // ルートタスクを先に処理したい場合は、ルートタスクは Start せずに Story.Update の前で手動実行してください。
            // なお、このサンプルでは左右移動にかかるフレーム数は一定ではないため、赤が右端に移動する前に消えることがあり、合わせて青も手前で止まることがあります。
            if (this.red != null)
            {
                GameObject.Destroy(this.red.gameObject);
                this.red = null;
            }
            else
            {
                // 【並列処理のテスト】With を使用して await するパターン
                var task0 = MoveTask(this.white, Range * 0.5f);
                var task1 = MoveTask(this.blue, -Range * 0.5f);
Debug.Log(task0);
Debug.Log(task1);
                await task0.With(task1);
Debug.Log(7);

                // （バグに見えるので青を右端まで移動させる）
                await MoveTask(this.blue, Range * 0.5f);

                // 【並列処理のテスト】手動タスク実行でゴリゴリに書くパターン
                foreach (var _ in MoveTask(this.white, -Range * 0.5f))
                {
                    var position = this.blue.anchoredPosition;
                    position.x = this.white.anchoredPosition.x;
                    this.blue.anchoredPosition = position;
                    await Story.Yield;
                }
                // 【並列処理のテスト】白の移動が終わるとすぐに foreach を抜けるので青に反映させる必要がある
                var position2 = this.blue.anchoredPosition;
                position2.x = this.white.anchoredPosition.x;
                this.blue.anchoredPosition = position2;
            }
         }
    }

    // サブタスク：往復移動させる
    async Story.Task SubTask(RectTransform rt)
    {
        var position = rt.anchoredPosition;
        while (true)
        {
            while (position.x < Range * 0.5f)
            {
                position.x += Speed * Time.deltaTime;
                rt.anchoredPosition = position;
                await Story.Yield;
            }
            while (-Range * 0.5f < position.x)
            {
                position.x += -Speed * Time.deltaTime;
                rt.anchoredPosition = position;
                await Story.Yield;
            }
        }
    }

    // 指定した位置へ移動させる
    async Story.Task MoveTask(RectTransform rt, float to)
    {
        var x = rt.anchoredPosition.x;
        var move = x < to ? Speed : -Speed;
        while (true)
        {
            x += move * Time.deltaTime; // 経過時間で計算しているため、完了までのフレーム数は必ずしも一定にはなりません。
            if ((to - x) * move <= 0f) // 移動先と移動量が逆転したら終了
            {
                var position = rt.anchoredPosition;
                position.x = to; // 確実に to で終わらせる
                rt.anchoredPosition = position;
                return;
            }
            else
            {
                var position = rt.anchoredPosition;
                position.x = x;
                rt.anchoredPosition = position;
                // await Story.Yield;
                if ((Time.frameCount & 16) == 0) { await Story.Yield; }
                else if ((Time.frameCount & 32) == 0) { await Story.YieldLate; }
                else { await Story.YieldFixed; }
            }
        }
    }

    // 【非同期タスクを返すメソッドのテスト】右端まで移動させる
    /* async */ Story.Task SubTaskRight(RectTransform rt)
    {
        return MoveTask(rt, Range * 0.5f);
    }

    // 【孫タスクのテスト】左端まで移動させる
    async Story.Task SubTaskLeft(RectTransform rt)
    {
        // 【孫タスクのテスト】手動実行で半分まで移動
        foreach (var _ in MoveTask(rt, 0f)) { await Story.Yield; }

        // 【孫タスクのテスト】await で最後まで移動
        await MoveTask(rt, -Range * 0.5f);
    }

    // 【finally ブロックのテスト】キャンセルされても左端まで移動する
    async Story.Task BlueTask(RectTransform rt)
    {
        try
        {
            while (true)
            {
                await MoveTask(rt, Range * 0.5f);
                await MoveTask(rt, -Range * 0.5f);
            }
        }
        finally
        {
            // キャンセルされても左端までは移動させる
            await MoveTask(rt, -Range * 0.5f);
        }
    }
}
