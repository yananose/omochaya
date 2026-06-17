# Omochaya Story

Omochaya Story は、Unityの標準コルーチン(IEnumerator)を完全に置き換えるために設計された、高パフォーマンスな非同期タスク（async/await）フレームワークです。

毎フレームの自動実行ループ、独自に実装されたステートマシンプール、そしてキャッシュ効率を極限まで高めるHot/Coolデータ分割(Data-Oriented Design)により、ゼロアロケーションかつ高速なタスク処理を実現します。

## ✨ 特徴 (Features)

* **ゼロアロケーション & 高速実行**: 独自のステートマシンプールと世代管理付きIDにより、GCメモリ確保を排除。
* **キャッシュ効率の最適化 (DOD)**: Hot/Cool分割されたメモリープールにより、CPUキャッシュヒット率を向上。
* **柔軟な実行制御**: 遅延起動（Lazy Evaluation）、手動での `MoveNext()`、他タスクからの `await` にシームレスに対応。
* **安全なライフサイクル管理**: `MonoBehaviour` などのコンポーネントをマスターとして紐付け、オブジェクト破棄時の安全なタスクキャンセルを保証。
* **便利なデバッグツール**: エディタ拡張として、リアルタイムにタスクの実行順や待機状態を可視化する「Task Monitor」、メモリ使用量を追跡する「Pool Monitor」を標準搭載。

## ⚙️ 動作要件 (Requirements)

* **Tested on (動作確認済み環境):** Unity 6.3
* **Language:** C# 8.0 以上

> **Note:**
> 本フレームワークは C# 8.0 の機能や `System.Runtime.CompilerServices.Unsafe` などを利用しているため、理論上は **Unity 2021.3 LTS 以降** であれば動作するはずです。
> 作者の環境（Unity 6.3）でのみ動作確認を行っているため、もし古いバージョンで正常に動作した、あるいはエラーが出たという方がいらっしゃいましたら、ぜひ Issue や PR でご報告いただけると大変助かります！

## 📦 インストール方法 (Installation)

本システムはフォルダを直接配置して導入する形式となっています。

1. 本リポジトリの `Assets/Omochaya` フォルダ（`StoryTask.cs`、`StoryPool.cs`、`StoryMessages.cs` などのコアファイル、およびエディタ拡張スクリプトが含まれるフォルダ）を、ご自身のUnityプロジェクトの `Assets` フォルダ配下にそのままコピーしてください。

## 🚀 基本的な使い方 (Getting Started)

`Story.Task` を戻り値とする `async` メソッドを定義し、`Boot()` で起動します。

```csharp
using UnityEngine;
using Omochaya;

public class SampleActor : MonoBehaviour
{
    void Start()
    {
        // 自身(this)をライフサイクルのマスターとしてタスクを起動
        MyRoutineAsync().Boot(this);
    }

    async Story.Task MyRoutineAsync()
    {
        Debug.Log("処理開始");

        // 1フレーム待機
        await Story.Yield;

        Debug.Log("1フレーム経過");

        // 他のタスクをawaitすることも可能
        await SubRoutineAsync();

        Debug.Log("処理完了");
    }

    async Story.Task SubRoutineAsync()
    {
        // ... 何らかの処理 ...
        await Story.Yield;
    }
}
```

## ⚠️ 使用上の注意点

* **外部タスクの await 禁止**: `Story.Task` 内で標準の `Task` や `UniTask` など、外部の非同期タスクを直接 `await` することはサポートされていません。
* **マネージャーの更新呼び出し**: 必ず `Story.Update()` を毎フレーム呼び出す機構（既存のマネージャーコンポーネントやPlayerLoopへのインジェクションなど。MonoBehaviour の Update で呼び出すだけでもOK）を用意してください。

## ⚠️ 制限事項とトレードオフ (Limitations & Trade-offs)

本システムは極限のパフォーマンスとゼロアロケーションを追求した結果、以下の設計上のトレードオフを抱えています。ご利用の際はご留意ください。

* **複数からの同時 `await` の禁止 (Single Await)**
  C#標準の `ValueTask` と同様に、1つのタスクを別の複数のタスクから同時に `await` することはできません。
* **メモリの事前確保とキャッシュ効率のジレンマ**
  実行中の動的なメモリアロケーション（GC発生）を完全に防ぐため、内部のステートマシン配列等はあらかじめ大きくメモリを確保します。そのため、タスクの同時実行数が極端に少ない場面ではメモリ空間に「空き（疎）」ができ、設計意図に反してCPUキャッシュ効率が低下する可能性があります。用途に合わせてプールの初期サイズ（`Story.SetInitialTaskCount`等）をチューニングしてご利用ください。

## 📄 ライセンス (License)

This project is licensed under the MIT License - see the LICENSE file for details.