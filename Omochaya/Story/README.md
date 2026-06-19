# Omochaya Story

Omochaya Story は、Unityの標準コルーチン（`IEnumerator`）を完全に置き換えるために設計された、**ゲームロジック・シーケンス制御特化型**のゼロアロケーション非同期タスクエンジンです。

本フレームワークはメインスレッド限定のため、汎用的な非同期処理（ファイルI/Oやネットワーク通信など）には向きません。その代わり既存の非同期処理より**シンプルにコルーチンを置き換えつつ、ゼロアロケーションを実現**できます。

## ✨ 特徴 (Features)

* **柔軟な実行制御**: 遅延起動（Lazy Evaluation）、手動での `MoveNext()`、他タスクからの `await` にシームレスに対応。
* **直感的なコンテキストスイッチ**: `await Story.YieldFixed` や `await Story.YieldLate` を呼ぶだけで、標準コルーチンと同様に実行タイミング（Update層）をシームレスに移動可能。
* **安全なライフサイクル管理**: `MonoBehaviour` などのコンポーネントをマスターとして紐付け、オブジェクト破棄時の安全なタスクキャンセルを保証。
* **ゼロアロケーション & 高速実行**: 独自のステートマシンプールと世代管理付きIDにより、実行中のGCアロケートを完全に排除。
* **キャッシュ効率の最大化 (DOD)**: Hot/Coolデータ分割により、CPUキャッシュヒット率を高めた高速な反復処理。
* **便利なデバッグツール**: エディタ拡張として、リアルタイムにタスクの実行順や待機状態を可視化する「Task Monitor」、メモリ使用量を追跡する「Pool Monitor」を標準搭載。

## ⚙️ 動作要件 (Requirements)

* **Tested on:** Unity 6.3
* **Language:** C# 8.0 以上

> **Note:**
> 本フレームワークは C# 8.0 の機能や `System.Runtime.CompilerServices.Unsafe` などを利用しているため、理論上は **Unity 2021.3 LTS 以降** であれば動作するはずです。
> 作者の環境（Unity 6.3）でのみ動作確認を行っているため、もし古いバージョンで正常に動作した、あるいはエラーが出たという方がいらっしゃいましたら、ぜひ Issue や PR でご報告いただけると大変助かります！

## 📦 インストール方法 (Installation)

本システムは、フォルダを直接配置して導入する形式となっています。

1. 本リポジトリの Omochaya/Story フォルダを、ご自身のUnityプロジェクトの Assets フォルダ配下にそのままコピーしてください。

> **Note: アセンブリ定義 (.asmdef) と UPM 対応について**
> 現在は初期公開フェーズのため、Unity Package Manager (UPM) 経由での配信や .asmdef の同梱は行っていません。将来的なアップデートでパッケージ構成が見直される可能性があります。

## 🚀 基本的な使い方 (Getting Started)

### 1. マネージャーの更新設定
タスクを処理するため、プロジェクトの任意の場所（シングルトンやメインループを管理するクラスなど）で、各実行タイミングの Update を呼び出してください。

```csharp
    void Update() { Story.Update(); }
    void LateUpdate() { Story.LateUpdate(); }
    void FixedUpdate() { Story.FixedUpdate(); }
```

### 2. タスクの定義とコンテキストの移動
`Story.Task` を戻り値とする `async` メソッドを定義し、標準の `MonoBehaviour` に紐づけて `Boot` します。
```csharp
using UnityEngine;
using Omochaya;

// 普段お使いの MonoBehaviour でそのまま動作します
public class Actor : MonoBehaviour 
{
    void Start()
    {
        // 自身(this)をライフサイクルのマスターとして起動
        // （GameObjectが破棄された場合は自動でキャンセル・解放されます）
        ActionSequence().Boot(this);
    }

    async Story.Task ActionSequence()
    {
        // 1. 【Update層】入力を待機
        while (!Input.GetKeyDown(KeyCode.Space)) { await Story.Yield; }

        // 2. 【FixedUpdate層】物理演算による移動
        for (int i = 0; i < 10; i++)
        {
            await Story.YieldFixed; // 次の物理ステップまで待機
            GetComponent<Rigidbody>().MovePosition(...);
        }

        // 3. 【LateUpdate層】カメラ追従などの計算後処理
        await Story.YieldLate; // 次の LateUpdate 層まで待機
        UpdateCustomIK();

        // 4. 【Update層】待機
        await Story.Yield; // 次の Update 層まで待機
        await Story.WaitTime(1.0f);

        // 5. 【Update層】後処理をサブタスクで行う
        await ProActionSequence();

        // 7. 【Update層】終了
        Debug.Log("Bootで指定したコンポーネントが破棄されても処理中のタスクは await まで実行されます");
        await Story.Yield;
        Debug.Log("await の後は実行されません（このログは出力されません）");
    }

    async Story.Task ProActionSequence()
    {
        // 6. 【Update層】待ってから破棄
        await Story.WaitTimeUnscaled(1.0f);
        Destory(this.gameObject);
    }
}
```

### 3. デバッグツールの起動
エディタのメニューバーの `Window` > `Omochaya` から起動してください。

## ⚡ パフォーマンスチューニングと拡張 (ITaskMaster)

Omochaya Story は標準の `MonoBehaviour` 及び任意の `Component` で問題なく動作しますが、毎フレーム発生する Unityオブジェクトの偽装nullチェック（C++側へのアクセス）は、数千のタスクを回す際には微小なオーバーヘッドになります。

極限のパフォーマンスを求める場合や、タスクの「一時停止（ポーズ）」を実装したい場合は、Bootに指定するコンポーネントに `Story.ITaskMaster` インターフェースを実装してください。あるいは用意されている `Story.TaskBehaviour` を継承するとコンポーネントの非アクティブ時に一時停止させつつ偽装nullチェックを回避できます。
```csharp
// TaskBehaviour を継承（または ITaskMaster を実装）することで、
// 偽装nullチェックがスキップされ、タスク管理がさらに高速化されます。
public class HeavyActor : Story.TaskBehaviour
{
    void Start()
    {
        HeavySequence().Boot(this);
    }
    
    async Story.Task HeavySequence()
    {
        // this が非アクティブの間は、このタスクの進行は停止します
        while (true)
        {
            // ... アクティブ時のみ実行したい処理 ...
            await Story.Yield;
        }
    }
}
```

## ⚠️ 制限事項とトレードオフ (Limitations & Trade-offs)

本システムは、特化型の高速なコルーチン代替エンジンとして設計されているため、以下の設計上のトレードオフを抱えています。ご利用の際はご留意ください。

* **外部タスクの await 禁止**
  System.Threading.Tasks.Task や UniTask などの外部の非同期タスクを、本タスク内で直接 `await` することはできません。
* **複数からの同時 await の禁止**
  1つのタスクを、別の複数のタスクから同時に `await` することはできません（C#標準の ValueTask と同様の制約です）。
* **プールサイズの事前確保とキャッシュ効率のジレンマ**
  実行中の動的なメモリアロケーション（GC発生）を完全に防ぐため、内部のステートマシン配列等はあらかじめ大きくメモリを確保します。そのため、タスクの同時実行数が極端に少ない場面ではメモリ空間に「空き（疎）」ができ、設計意図に反してCPUキャッシュ効率が低下する可能性があります。用途に合わせてプールの初期サイズ（`Story.SetInitialTaskCount`等）をチューニングしてご利用ください。

## 📄 ライセンス (License)

This project is licensed under the MIT License - see the LICENSE file for details.
