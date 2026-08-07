# Omochaya Story

「Coroutineは好きだ。でもGCは嫌いだ。」

**Omochaya Story** は、そんなUnityエンジニアのためのライブラリです。
Unityの標準コルーチン（`IEnumerator`）を完全に置き換えるために設計された、**ゲームロジック・シーケンス制御特化型**のゼロアロケーション非同期タスクエンジンです。

---

## 📖 開発の背景とコンセプト (Why Story?)

Coroutineはゲームシーケンスを自然に記述できる非常に優れた仕組みですが、実行のたびにヒープにゴミ（GCアロケーション）をまき散らす問題がありました。
一方、モダンな非同期処理のデファクトスタンダードである **UniTask** は非常に強力ですが、「手動で `MoveNext()` をゴリゴリ回す」「タスクを変数として保持して細かく進行をコントロールする」といった、コルーチン特有の泥臭いシーケンス制御には少しハードルがありました。

Storyは、**「Coroutineの実行モデルを維持したまま、ステートマシンを『構造体の配列』として扱い、アロケーションを完全に排除する」** というアプローチで開発されました。
目的は「`async/await` を使うこと」ではなく、**「Coroutineをより軽量・高速・安全に実行すること」** です。

---

## 🆚 UniTaskやCoroutineとの違い

Storyは、汎用非同期処理ライブラリである **UniTask の代替ではありません。** あくまで **Coroutine(Unity)の代替** に特化しています。

| 項目 | Omochaya Story | UniTask | Coroutine(Unity) |
|------|:---:|:---:|:---:|
| Coroutine(Unity)の置換 | **◎ 最適** | ○ | - |
| 手動の進行制御 (`MoveNext`等) | **◎ 最適** | △ | ○ |
| ネットワーク通信 / ファイルI/O | **△ (要ポーリング)** | ◎ | △ (要ポーリング) |
| マルチスレッド | **×** | ◎ | × |
| ゼロアロケーションへの特化 | **◎** | ○ | × |
| 安全な後始末 | **◎** | ○ | × |

**⭕ 向いているケース**
RPGイベント / 会話システム / ターン制バトル / カットシーン / UIシーケンス など、**メインスレッドで完結するフレームベースのゲームロジック**において最高のパフォーマンスを発揮します。

---

## ✨ 特徴 (Features)

### 1. 究極のゼロアロケーション & 高速実行
独自のステートマシンプールと世代管理付きIDにより、実行中のGCアロケートを排除（※キャンセル実行時とデバッグ時を除く）。Hot/Coolデータ分割によりCPUキャッシュヒット率を高め、高速な反復処理を実現しています。

### 2. コルーチンの機能を超えてできること
* **戻り値が受け取れる:** `async/await` ベースなので、サブタスクからの戻り値を自然に受け取れます。
* **脱・トークンバケツリレー:** `CancellationToken` はあえて採用していません。`StopCoroutine` と同じメンタルモデルで、タスクハンドルや紐づけたオーナーの破棄によって直感的にキャンセルを制御します。
* **安全な `finally` と キャンセルモード:** オブジェクト破棄時などによるキャンセルが発生しても、キャンセル例外により `finally` ブロックが確実に実行されるので安全に後始末が可能です。また **キャンセルモード** により `finally` ブロックのないタスクではキャンセル例外を発生させない `Drop` 、キャンセル例外の代わりに `return` で `finally` ブロックを処理する `DontThrow` などをタスク単位で制御することが可能です。後始末の不要なタスクが多い場合はデフォルト動作を `Drop` にし、必要なタスクのみキャンセル例外を使用する `Safe` にすることも可能です。

### 3. 直感的なコンテキストスイッチ
`await Story.YieldFixed` や `await Story.YieldLate` を呼ぶだけで、実行タイミング（Update層）をシームレスに移動可能。現在の実行タイミングを維持して待機する `Story.YieldSame` も搭載しています。

### 4. ゲーム特化のコンビネータ (`With` / `Until`)
* **`With` (並行実行):** 実行中のいずれかのタスクがキャンセルされても、残りのタスクを道連れにしません。
* **`Until` (競争実行):** いずれかのタスクが完了（勝負がついた）瞬間、敗者のタスクは自動的に安全なキャンセル処理へ移行します。

### 5. デバッグツール標準搭載
リアルタイムにタスクの実行状態を可視化する「Task Monitor」、プールの使用状況・メモリを追跡する「Pool Monitor」の2つの専用EditorWindowを完備しています。Task Monitorではコールスタック表示、コードジャンプ機能などもサポートしており、スムーズなデバッグが可能です。

---

## ⚙️ 動作要件 (Requirements)

* **Tested on:** Unity 6.3
* **Language:** C# 9.0 以上

> **Note:**
> 本フレームワークは C# 9.0 の機能や `System.Runtime.CompilerServices.Unsafe` などを利用しているため、理論上は **Unity 2022.3 LTS 以降** であれば動作するはずです。

---

## 📦 インストール方法 (Installation)

Unity Package Manager (UPM) を使用してインストールします。

1. Unityエディタのメニューから `Window` > `Package Manager` を開きます。
2. 左上の `+` ボタンをクリックし、`Add package from git URL...` を選択します。
3. 以下のURLを入力して `Add` をクリックします。

```text
https://github.com/yananose/omochaya.git?path=/Omochaya/Story#story/1.3.0
```

> **💡 バージョン指定について**
> 上記のURLはバージョン `1.3.0` で固定されています。最新版や別のバージョンを利用したい場合は、URL末尾の `#story/1.3.0` の部分を任意のバージョンタグ（例: `#story/1.0.0`）に変更するか、`#` 以降を削除して `main` ブランチの最新を取得してください。

> **💡 テストコードの導入**
> インストール後、Package Managerの Story のページから `Samples` にある `Framework Validation & Allocation Tests` をプロジェクトにインポートできます。生きたリファレンスとしてご活用ください。

---

## 🚀 基本的な使い方 (Getting Started)

### 1. マネージャーの更新設定
タスクを処理するため、プロジェクトのメインループなどで、各実行タイミングの Update を呼び出してください。
また、実行中のアロケーションを回避したい場合は、`Story.WarmupMode()` を利用してタスクのプールを事前確保してください。

```csharp
using Omochaya;

public class StoryManager : MonoBehaviour
{
    void Awake() 
    {
        // 【任意】キャンセルモードの指定
        Story.DefaultCancelMode = Story.CancelMode.Drop;

        // 【任意】一度に実行するおおよそのタスク数の指定（実際に使用するタスク数よりも多めに指定してください）
        Story.Warmup(32);

        // 【任意】各タスクのプールの事前確保
        using (Story.WarmupMode())
        {
            // 使用するタスクのプールを事前確保する
            // ※Capacity属性が設定されていればそのサイズ、引数で上書きも可能
            ActionSequence().Warmup(1024);
        }
    }
    void Update() { Story.Update(); }
    void LateUpdate() { Story.LateUpdate(); }
    void FixedUpdate() { Story.FixedUpdate(); }
}
```

### 2. タスクの定義と実行
`Story.Task` を戻り値とする `async` メソッドを定義し、標準の `MonoBehaviour` に紐づけて `Start` します。

```csharp
using UnityEngine;
using Omochaya;

public class Actor : MonoBehaviour 
{
    void Start()
    {
        // 自身(this)をライフサイクルのオーナーとしてタスクを起動
        // （GameObjectが破棄された場合は、自動的にタスクもキャンセル・解放されます）
        ActionSequence().Start(this);
    }

    async Story.Task ActionSequence()
    {
        // 1. 条件を満たすまで毎フレーム待機
        while (!Input.GetKeyDown(KeyCode.Space)) { await Story.Yield; }

        // 2. 指定した時間（秒）だけ待機
        await Story.WaitTime(1.0f);

        // 3. 別のサブタスクを呼び出して完了を待つ
        var result = await SubSequence();
        if (Story.HasValidResult())
        {
            Debug.Log($"サブタスク完了: {result}");
        }
    }

    // 初回実行時に確保されるこのタスク専用プールのサイズを指定（オプション）
    [Story.Capacity(128)]
    async Story.Task<int> SubSequence()
    {
        await Story.Yield;
        return 100;
    }
}
```

---

## ⚡ パフォーマンスチューニング (ITaskOwner)

標準の `MonoBehaviour` でも動作しますが、数千のタスクを回す際のUnityオブジェクト偽装nullチェックの微小なオーバーヘッドを削りたい場合や、タスクの一時停止をしたい場合は、`Story.TaskBehaviour` を継承（または `ITaskOwner` を実装）してください。

```csharp
public class HeavyActor : Story.TaskBehaviour
{
    void Start() { HeavySequence().Start(this); }
    
    async Story.Task HeavySequence()
    {
        while (true)
        {
            // this.gameObject が非アクティブの間、進行は自動的に一時停止します
            await Story.Yield;
        }
    }
}
```

---

## Scripting Define Symbols

* `STORY_NO_DEBUG` ：エディタ上でもエディタ実行でない場合と同等の処理になります。モニタ類には情報が表示されなくなりますが、テストは行なえますのでアロケーションが発生しないことをテストで確認できるようになります。なお、エディタ実行でない場合は `STORY_NO_DEBUG`を設定していなくても最適化されます。
* `STORY_DEBUG` ：エディタ実行でない場合でもエディタ実行と同等の処理になります。実機でデバッグログ等を確認したい時に使用してください。 **製品版では必ず無効にしてください。**
* `STORY_NO_PRE_CAPACITY` ：事前確保を行わずに初回実行時にプールが確保される場合に `[Story.Capacity()]` を無視してデフォルトサイズのプールを確保します。実行中のリフレクションを回避したい時に使用してください。
* `STORY_FULL_TUNE` ：エディタ実行でない場合に `Story` が行うエラーや例外の出力を無効化します。無駄を完全に排除しますが `crashlytics` 等で `Story` が行うエラーや例外が検知できなくなります。
* `STORY_NO_TIME_CACHE` ： `Tween` で使用する `Time.deltaTime` 等のキャッシュを無効化します。キャッシュは `PlayerLoop` を利用し `EarlyUpdate` の最後で行っており、それが都合が悪い場合に使用してください。トレードオフで実行速度が低下します。
* `STORY_EASE_COMPACT` ： `Tween` で使用する `Ease` を共通化しコンパイル時間とバイナリサイズを節約します。トレードオフで実行速度が低下します。
* `STORY_MOVER_FAST` ： `Tween` で使用するタスクを細分化し実行速度を向上させます。トレードオフでコンパイル時間とバイナリサイズが増加します。

---

## Tween Animations

Omochaya Storyは、タスクシステムに統合されたゼロアロケーションのTween（アニメーション）機能を提供しています。
対象のコンポーネントに対する拡張メソッドとして実装されているため、直感的なメソッドチェーンで記述できます。

### 基本的な使い方 (To / By, Interval / Speed)

Tweenの目標値は `To()`（絶対値）または `By()`（相対値）で指定し、アニメーションの進行は `Interval()`（時間・秒）または `Speed()`（速度）でタスク化します。

```csharp
using UnityEngine;
using Omochaya;

public class TweenSample : MonoBehaviour
{
    [SerializeField] RectTransform rectTransform;
    [SerializeField] CanvasGroup canvasGroup;
    public float MyValue;

    public async Story.Task PlayAnimations()
    {
        // 時間（秒）を指定して絶対座標へ移動
        await this.rectTransform.TweenLocalPosition().To(new Vector3(100f, 50f, 0f)).Interval(1.0f);

        // 速度を指定して現在の値から相対的に移動
        await this.rectTransform.TweenAnchoredPosition().By(new Vector3(0f, 50f, 0f)).Speed(200f);

        // アルファ値を1.0へ時間指定でフェード
        await this.canvasGroup.TweenAlpha().To(1.0f).Interval(0.5f);

        // 任意のパラメータを変化（MyValue を 0 から 100 へ 50/sec の速度で変化させる）
        await Story.Tween(0f, 100f, 50f, this, static (self, value) => self.MyValue = value);
    }
}
```

### イージング (Easing) の適用

アニメーションのカーブは `Story.EaseFast` などを引数に渡すことで簡単に適用できます。

```csharp
// 減速（Deceleration）しながら移動
await this.rectTransform.TweenLocalScale()
    .To(Vector3.one)
    .Interval(1.0f, Story.Ease.QuadDec);

// AnimationCurve を直接使用することも可能です
[SerializeField] AnimationCurve myCurve;
// ...
await this.rectTransform.TweenLocalPosition()
    .To(Vector3.one)
    .Interval(1.0f, Story.Ease.Curve(myCurve));
```

用意している主要なイージングは以下の通りです。

* `SineAcc`,`QuadAcc`,`CubicAcc`,`QuartAcc`,`SqrtAcc`,`ExpoAcc`,`CircAcc`,`BackAcc`,`ElasticAcc`,`BounceAcc`
* `SineDec`,`QuadDec`,`CubicDec`,`QuartDec`,`SqrtDec`,`ExpoDec`,`CircDec`,`BackDec`,`ElasticDec`,`BounceDec`
* `None` (デフォルト。線形),`Reverse()`,`Pow(e)`,`Curve(curve)`,`FromTo(a,b)`
* `ease.Stitch(ease)`: ２つのイージング組み合わせて InOut/OutIn を作成（逆方向は繋がらない）。

### 操作対象の自由な指定

`To()` `By()` に引数名を指定することで指定した要素だけを変化させることができます。

```csharp
async Story.Task PlaySpecificAnimations()
{
    // x を加速移動させるアニメーション
    var task1 = this.rectTransform.TweenLocalPosition().To(x: 100f).Interval(100f, Story.QuadAcc);
    
    // y を減速移動させるアニメーション
    var task2 = this.rectTransform.TweenLocalPosition().To(y: 50f).Interval(100f, Story.Ease.QuadDec);

    // 全てのタスクの完了を待機（x は加速移動しつつ y は減速移動します。z は変化しないのでタスク外で操作可能です）
    await task1.With(task2);
}
```

### 複数Tweenのシームレスな連続再生

`ref double start` を引数に渡すことで、複数のTweenを滑らかに連結（チェイン）して再生できます。
Tweenタスクの生成時に所要時間が自動的に加算されるため、フレーム単位の実行遅延に影響されることなく、前のタスクの終了直後に次のタスクの開始時刻を正確に設定できます。

```csharp
async Story.Task PlaySequentialAnimations()
{
    this.rectTransform.localPosition = Vector3.zero;

    // 現在の基準時間を取得
    var start = Story.GetStart(); 

    // 1つ目のアニメーション。生成時に所要時間が計算され start に加算されます
    var task1 = this.rectTransform.TweenLocalPosition().To(x: 50f).Speed(100f, Story.Ease.SineAcc, ref start);
    
    // 2つ目のアニメーション。task1の終了時刻がtask2の開始時刻となるため、動きが切れ目無くつながります。
    var task2 = this.rectTransform.TweenLocalPosition().By(x: 50f).Speed(100f, Story.Ease.SineDec, ref start);

    // 全てのタスクの完了を待機
    await task1.With(task2);
}
```

### 主な対応コンポーネントとプロパティ

主要なUnityコンポーネント向けに、以下の拡張メソッドが標準で用意されています。

*   **Transform**: `TweenLocalPosition()`, `TweenPosition()`, `TweenLocalScale()`, `TweenLocalRotation()`, `TweenRotation()`, `TweenLocalEulerAngles()`, `TweenEulerAngles()`
*   **RectTransform**: `TweenAnchoredPosition()`, `TweenSizeDelta()`, `TweenPivot()`
*   **CanvasGroup / TMP_Text**: `TweenAlpha()`
*   **TMP_Text**: `TweenFontSize()`, `TweenMaxVisibleCharacters()`
*   **Image**: `TweenFillAmount()`
*   **Graphic / SpriteRenderer**: `TweenColor()`
*   **Slider**: `TweenValue()`
*   **Camera**: `TweenFieldOfView()`, `TweenOrthographicSize()`, `TweenBackgroundColor()`
*   **AudioSource**: `TweenVolume()`, `TweenPitch()`

---

## ⚠️ 制約事項 (Limitations)

`async` 構文を採用していますが、Story特有の以下の制約があります。

* **メインスレッド限定:** マルチスレッドには対応していません。
* **複数からの同時 await 禁止:** 1つのタスクを複数箇所から同時に await することはできません。
* **終了したタスクの await 禁止:** すでに完了したタスクを await して結果を取り出すことはできません。
* **外部非同期タスクとの混在不可:** Storyの非同期メソッド内で、標準の `Task` や `UniTask` などの外部非同期メソッドを await することはできません。逆も同様です。
* **メモリの疎化:** 実行中の動的アロケーションを防ぐため、プール拡張時にメモリを大きく確保します。同時実行数が少ない場合はCPUキャッシュ効率が低下する可能性があるため、高度な最適化を行う場合は `Story.WarmupMode()` 等で初期サイズを調整してください。
* **特殊なキャンセルモード利用時の注意:** `Story.TaskCancelMode` でキャンセル時の挙動を変更する場合、`DontThrow` 指定時はすべての `await` 直後に `if (Story.IsCanceled) { return; }` による手動キャンセル確認が必要です。また、`Drop` 指定時は `finally` ブロックが実行されずにタスクが消失するため、後始末が必要なタスクには絶対に使用しないでください。

---

## 📄 ライセンス (License)

MIT License

> **注意:** 本ライブラリは個人開発のため、不具合対応や機能追加に時間がかかる場合、または更新が停止する場合があります。ご利用の際は自己責任でお願いいたします。