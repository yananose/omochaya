# Omochaya Story

Coroutineは好きだ。

でもGCは嫌いだ。

**Omochaya Story** はそんな人のためのライブラリです。

Unityの標準コルーチン（`IEnumerator`）を完全に置き換えるために設計された、**ゲームロジック・シーケンス制御特化型**のゼロアロケーション非同期タスクエンジンです。

---

## 📖 Storyとは

Storyは Unity の Coroutine を軽量化・モダン化するためのライブラリです。

```csharp
// 従来のCoroutine
IEnumerator Sequence()
{
    yield return new WaitForSeconds(1);
    yield return FadeIn();
    yield return Battle();
}
```

Coroutineはゲームシーケンスを自然に記述できる非常に優れた仕組みですが、`IEnumerator` を利用する以上、ステートマシンはヒープ上に生成されGCアロケーションが発生してしまいます。

Storyは、**「Coroutineの実行モデルを維持したまま、アロケーションを完全に排除できないか」** という発想から生まれました。

```csharp
// Storyを使用した記述
async Story.Task Sequence()
{
    await Story.WaitTime(1f);
    await FadeIn();
    await Battle();
}
```

Storyの目的は「`async/await` を使うこと」ではなく、**「Coroutineをより軽量・高速に実行すること」** です。結果として `async/await` の構文を採用していますが、内部的にはCoroutineと同じようにステートマシンを段階的に進行させる実行モデルを継承しています。

---

## 🆚 UniTaskとの違い

Storyは、汎用非同期処理ライブラリである **UniTask の代替ではありません。**
Storyは「Coroutineを置き換えること」だけに特化しています。

| 項目 | Story | UniTask |
|------|:---:|:---:|
| Coroutineの置換 | ◎ | ○ |
| シーケンス記述 | ◎ | ○ |
| ネットワーク通信 (HTTP等) | × | ◎ |
| マルチスレッド / ファイルI/O | × | ◎ |
| ゼロアロケーションへの特化 | ◎ | ○ |
| Coroutine実行モデルの継承 | ◎ | △ |

### ⭕ Storyが向いているケース
RPGイベント / 会話システム / 演出制御 / ターン制バトル / カットシーン / UIシーケンス など、メインスレッドで完結するゲームロジック。

### ❌ Storyが向いていないケース
HTTP通信 / ファイルI/O / マルチスレッド処理 など、スレッドを跨ぐ汎用的な非同期処理。（これらをStoryで実現するにはコルーチンと同様にポーリングが必要です）

---

## ✨ 特徴 (Features)

* **ゼロアロケーション & 高速実行**: 独自のステートマシンプールと世代管理付きIDにより、実行中のGCアロケートを完全に排除。
* **キャッシュ効率の最大化 (DOD)**: Hot/Coolデータ分割により、CPUキャッシュヒット率を高めた高速な反復処理。
* **直感的なコンテキストスイッチ**: `await Story.YieldFixed` や `await Story.YieldLate` を呼ぶだけで、標準コルーチンと同様に実行タイミング（Update層）をシームレスに移動可能。
* **柔軟なコンビネータ**: 複数タスクの並行待機（`With`）や競争待機（`Until`）において、ゲーム制御に最適化された独自のキャンセル挙動（道連れキャンセルの制御）を実装。
* **安全なライフサイクル管理**: `MonoBehaviour` などのコンポーネントをオーナーとして紐付け、オブジェクト破棄時の安全なタスクキャンセルと `finally` の実行を保証。
* **デバッグツール標準搭載**: リアルタイムにタスクの実行状態を可視化する「Task Monitor」、メモリ使用量を追跡する「Pool Monitor」を完備。

---

## ⚙️ 動作要件 (Requirements)

* **Tested on:** Unity 6.3
* **Language:** C# 8.0 以上

> **Note:**
> 本フレームワークは C# 8.0 の機能や `System.Runtime.CompilerServices.Unsafe` などを利用しているため、理論上は **Unity 2021.3 LTS 以降** であれば動作するはずです。
> 作者の環境（Unity 6.3）でのみ動作確認を行っているため、もし古いバージョンで正常に動作した、あるいはエラーが出たという方がいらっしゃいましたら、ぜひ Issue や PR でご報告ください。

---

## 📦 インストール方法 (Installation)

Unity Package Manager (UPM) を使用してインストールします。

1. Unityエディタのメニューから `Window` > `Package Manager` を開きます。
2. 左上の `+` ボタンをクリックし、`Add package from git URL...` を選択します。
3. 以下のURLを入力して `Add` をクリックします。

```text
https://github.com/yananose/omochaya?path=Omochaya/Story
```

> **💡 テストコードの導入**
> インストール後、Package Managerの Story のページから `Samples` にある「フレームワーク動作検証・アロケーションテスト」をプロジェクトにインポートできます。動作確認や仕様のリファレンスとしてご活用ください。

---

## 🚀 基本的な使い方 (Getting Started)

### 1. マネージャーの更新設定
タスクを処理するため、プロジェクトの任意の場所（シングルトンやメインループを管理するクラスなど）で、各実行タイミングの Update を呼び出してください。
また、同時に扱うタスク数が多い環境化でアロケーションが発生してしまう場合は、Story.Custom(taskCount: ) で使用するタスク数を宣言しておくことで回避できます。

```csharp
using Omochaya;

public class StoryManager : MonoBehaviour
{
    void Awake() { Story.Custom(taskCount: 2048); }
    void Update() { Story.Update(); }
    void LateUpdate() { Story.LateUpdate(); }
    void FixedUpdate() { Story.FixedUpdate(); }
}
```

### 2. タスクの定義とコンテキストの移動
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
        Debug.Log("シーケンス開始");

        // 1. 条件を満たすまで毎フレーム待機（スペースキーの入力を待つ）
        while (!Input.GetKeyDown(KeyCode.Space)) 
        { 
            await Story.Yield; 
        }

        // 2. 指定した時間（秒）だけ待機
        Debug.Log("スペースキーが押されました。1秒待機します。");
        await Story.WaitTime(1.0f);

        // 3. 別のサブタスクを呼び出して完了を待つ
        await SubSequence();

        Debug.Log("全シーケンス完了");
    }

    async Story.Task SubSequence()
    {
        Debug.Log("サブタスク開始");
        await Story.Yield;
        Debug.Log("サブタスク終了");
    }
}
```

> **💡 より高度な使い方と仕様の確認**
> `FixedUpdate` / `LateUpdate` タイミングへの切り替え、`With` や `Until` といった特殊な並行・競争コンビネータの挙動、および厳密なキャンセルライフサイクルの実例については、本パッケージ同梱の **Samples** に含まれるテストコード群（`StoryTests.cs`）に非常にクリーンなサンプルが網羅されています。ぜひそちらを仕様リファレンスとしてご活用ください。

---

## ⚡ パフォーマンスチューニングと拡張 (ITaskOwner)

Omochaya Story は標準の `MonoBehaviour` で問題なく動作しますが、毎フレーム発生する Unityオブジェクトの偽装nullチェック（C++側へのアクセス）は、数千のタスクを回す際には微小なオーバーヘッドになります。

極限のパフォーマンスを求める場合や、タスクの「一時停止（ポーズ）」を実装したい場合は、用意されている `Story.TaskBehaviour` を継承してください。偽装nullチェックを回避しつつ、コンポーネントの非アクティブ時にタスクを一時停止させることが可能になります。

```csharp
public class HeavyActor : Story.TaskBehaviour
{
    void Start()
    {
        HeavySequence().Start(this);
    }
    
    async Story.Task HeavySequence()
    {
        while (true)
        {
            // this.gameObject が非アクティブの間は、このタスクの進行は自動的に停止します
            await Story.Yield;
        }
    }
}
```

---

## ⚠️ 制限事項とトレードオフ (Limitations & Trade-offs)

本システムは、特化型の高速なコルーチン代替エンジンとして設計されているため、以下の設計上のトレードオフを抱えています。

* **外部タスクの await 禁止**
  `System.Threading.Tasks.Task` や `UniTask` などの外部の非同期タスクを、本タスク内で直接 `await` することはできません。
* **複数からの同時 await の禁止**
  1つのタスクを、別の複数のタスクから同時に `await` することはできません（C#標準の `ValueTask` と同様の制約です）。
* **動的アロケーション回避によるメモリの疎化**
  実行中の動的なメモリアロケーション（GC発生）を防ぐため、内部のステートマシン配列等は拡張時に大きくメモリを確保します。タスクの同時実行数が極端に少ない場面ではメモリ空間に空きができ、CPUキャッシュ効率が低下する可能性があります。用途に合わせてプールの初期サイズ（`Story.Custom()`）をチューニングしてください。

---

## 📄 ライセンス (License)

MIT License