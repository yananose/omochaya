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

**⭕ 向いているケース**
RPGイベント / 会話システム / ターン制バトル / カットシーン / UIシーケンス など、**メインスレッドで完結するフレームベースのゲームロジック**において最高のパフォーマンスを発揮します。

---

## ✨ 特徴 (Features)

### 1. 究極のゼロアロケーション & 高速実行
独自のステートマシンプールと世代管理付きIDにより、実行中のGCアロケートを排除（※キャンセル実行時とデバッグ時を除く）。Hot/Coolデータ分割によりCPUキャッシュヒット率を高め、高速な反復処理を実現しています。

### 2. コルーチンの機能を超えてできること
* **戻り値が受け取れる:** `async/await` ベースなので、サブタスクからの戻り値を自然に受け取れます。
* **安全な `finally`:** オブジェクト破棄や外部からのキャンセル（Stop）が発生しても、`finally` ブロックが確実に実行され、安全に後始末（Cleanup）が可能です。
* **脱・トークンバケツリレー:** `CancellationToken` はあえて採用していません。`StopCoroutine` と同じメンタルモデルで、タスクハンドルや紐づけたオーナーの破棄によって直感的にキャンセルを制御します。

### 3. 直感的なコンテキストスイッチ
`await Story.YieldFixed` や `await Story.YieldLate` を呼ぶだけで、実行タイミング（Update層）をシームレスに移動可能。現在の実行タイミングを維持して待機する `Story.YieldSame` も搭載しています。

### 4. ゲーム特化のコンビネータ (`With` / `Until`)
* **`With` (並行実行):** 実行中のいずれかのタスクがキャンセルされても、残りのタスクを道連れにしません。
* **`Until` (競争実行):** いずれかのタスクが完了（勝負がついた）瞬間、敗者のタスクは自動的に安全なキャンセル処理へ移行します。

### 5. デバッグツール標準搭載
リアルタイムにタスクの実行状態を可視化する「Task Monitor」、プールの使用状況・メモリを追跡する「Pool Monitor」の2つの専用EditorWindowを完備しています。

---

## ⚙️ 動作要件 (Requirements)

* **Tested on:** Unity 6.3
* **Language:** C# 8.0 以上

> **Note:**
> 本フレームワークは C# 8.0 の機能や `System.Runtime.CompilerServices.Unsafe` などを利用しているため、理論上は **Unity 2021.3 LTS 以降** であれば動作するはずです。

---

## 📦 インストール方法 (Installation)

Unity Package Manager (UPM) を使用してインストールします。

1. Unityエディタのメニューから `Window` > `Package Manager` を開きます。
2. 左上の `+` ボタンをクリックし、`Add package from git URL...` を選択します。
3. 以下のURLを入力して `Add` をクリックします。

```text
https://github.com/yananose/omochaya.git?path=/Omochaya/Story
```

> **💡 テストコードの導入**
> インストール後、Package Managerの Story のページから `Samples` にある `Framework Validation & Allocation Tests` をプロジェクトにインポートできます。生きたリファレンスとしてご活用ください。

---

## 🚀 基本的な使い方 (Getting Started)

### 1. マネージャーの更新設定
タスクを処理するため、プロジェクトのメインループなどで、各実行タイミングの Update を呼び出してください。
また、同時に扱うタスク数が多い環境化でアロケーションが発生してしまう場合は、`Story.Warmup()` で使用するタスク数を宣言しておくことで回避できます。

```csharp
using Omochaya;

public class StoryManager : MonoBehaviour
{
    void Awake() { Story.Warmup(2048); }
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

> **💡 Tips: プール事前拡張の無効化**
> Scripting Define Symbols に `STORY_NO_PRE_CAPACITY` を定義することで、`[Story.Capacity]` 属性によるリフレクション走査のCPU負荷を完全に無効化（オプトアウト）できます。

---

## ⚠️ 制約事項 (Limitations)

`async` 構文を採用していますが、Story特有の以下の制約があります。

* **メインスレッド限定:** マルチスレッドには対応していません。
* **複数からの同時 await 禁止:** 1つのタスクを複数箇所から同時に await することはできません。
* **終了したタスクの await 禁止:** すでに完了したタスクを await して結果を取り出すことはできません。
* **外部非同期タスクとの混在不可:** Storyの非同期メソッド内で、標準の `Task` や `UniTask` などの外部非同期メソッドを await することはできません。逆も同様です。
* **メモリの疎化:** 実行中の動的アロケーションを防ぐため、プール拡張時にメモリを大きく確保します。同時実行数が少ない場合はCPUキャッシュ効率が低下する可能性があるため、`Story.Warmup()` で初期サイズを調整してください。

---

## 📄 ライセンス (License)

MIT License

> **注意:** 本ライブラリは個人開発のため、不具合対応や機能追加に時間がかかる場合、または更新が停止する場合があります。ご利用の際は自己責任でお願いいたします。