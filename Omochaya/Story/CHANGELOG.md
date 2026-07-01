# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.0.0] - 2026-07-01

### Added
- **Initial Release**
- **Core Engine:**
  - 独自の構造体ステートマシンプールと世代管理IDによるゼロアロケーション非同期タスクエンジン (`Story.Task`, `Story.Task<T>`) を追加。
  - コルーチンライクな手動の進行制御（`MoveNext()` や `foreach` での実行）をサポート。
  - `Story.HasValidResult()` を用いた、タスクからの戻り値の受け取り機能を追加。
  - タスクのキャンセル（`Stop()`）やオーナー破棄時に、`try-finally` ブロックを確実に実行させる安全なクリーンアップ機能を追加。
  - キャンセル例外を安全に処理するための `Story.IsCanceledException(e)` を追加。
- **Context Switches (Yields):**
  - 実行タイミングをシームレスに移動・待機するための各種Yieldを追加 (`Story.Yield`, `Story.YieldLate`, `Story.YieldFixed`, `Story.YieldSame`)。
  - 指定時間・フレーム待機のための非同期メソッドを追加 (`Story.WaitTime()`, `Story.WaitTimeUnscaled()`, `Story.WaitFrame()`)。
- **Combinators:**
  - 並行実行で他タスクを道連れにしない `Task.With()` を追加。
  - 競争実行で敗者を安全に自動キャンセルする `Task.Until()` を追加。
  - タスクのタイムアウト制御を行う `Task.Timeout()`, `Task.TimeoutUnscaled()` を追加。
  - タスクの直列実行を簡潔に記述する `Task.Then()` を追加。
- **Performance Optimization:**
  - 偽装nullチェックのオーバーヘッドを削減し、一時停止状態（Pause）に対応する `Story.TaskBehaviour` および `Story.ITaskOwner` を追加。
  - アロケーションなしでタスクに独自のデータを紐づけられる `Extra` 領域機能を追加。
  - アプリ起動時のプール事前拡張・初期化を行う `Story.Warmup()` と `Story.Custom()` を追加。
  - 事前確保のアロケーションをオプトアウトする `STORY_NO_PRE_CAPACITY` ディファインを追加。
- **Debugging Tools:**
  - 現在実行中のタスク状態を可視化するエディタ拡張「Story Task Monitor」を追加。
  - 各種プールの使用状況とメモリを追跡するエディタ拡張「Story Pool Monitor」を追加。
- **Samples:**
  - UPMからインポート可能な、動作検証および厳格なゼロアロケーションテストコード一式 (`Framework Validation & Allocation Tests`) を追加。