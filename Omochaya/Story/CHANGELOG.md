# Changelog

All notable changes to this project will be documented in this file.

The format is based on [Keep a Changelog](https://keepachangelog.com/en/1.0.0/),
and this project adheres to [Semantic Versioning](https://semver.org/spec/v2.0.0.html).

## [1.3.0] - 2026-08-08

### Added
- **Tween機能:**
  - 汎用的に利用できる `Story.Tween()` を追加（再生間隔、再生速度、イージング関数、開始・終了位置の指定等に対応）。
  - イージング関数を指定する `Story.Ease` を追加（基本22種、引数付き4種、加工3+21種）。
  - `Transform` や各種 Unity Component の主要プロパティを制御する `Tween` 拡張メソッドを追加。
- **デバッグ機能の強化:**
  - 例外へ変換される `RuntimeAssert` を追加。
- **IDE表示の最適化:**
  - 内部構造体やメソッドの入力候補非表示化のための `EditorBrowsableState.Never` 属性を追加。

### Changed
- **デバッグ処理の変更:**
  - 非エディタ実行時、通常の `Assert` を例外に変換せず、コードから取り除かれるように変更（代わりに `RuntimeAssert` を使用）。
- **Scripting Define Symbols の更新:**
  - `FOR_DEBUG` を `STORY_DEBUG` に変更。
  - `STORY_FAST` を `STORY_FULL_TUNE` に変更。
- **ドキュメント・動作要件の更新:**
  - README に Tween の解説を追加し、特徴・シンボル情報を更新。
  - C# 言語バージョンの誤りを修正（C# 8.0 → C# 9.0）。
  - サポート Unity バージョンを `2021.3` から `2022.3` に更新。

### Fixed
- 無効なタスクに対して `Keep()` や `At()` を呼び出した際、例外を投げずに無視するよう修正（Warmup 時を除きエラーログを出力）。

## [1.2.0] - 2026-07-26

### Added
- `await` 時やタスク実行中にオーナーコンポーネントを動的に指定（または再指定）できる `At()` メソッド（`Story.At(owner)`、`Task.At(owner)`、`Task<R>.At(owner)`）を追加。

### Changed
- プール管理のコアシステムをリファクタリング。安全な参照管理（`PoolMemory` 等の整備）や階層構造の見直しにより、メモリ管理の保守性向上と処理コストの削減（パフォーマンス改善）を実施。

## [1.1.1] - 2026-07-13

### Fixed
- Unity Package Managerでのバージョン認識の不整合を防ぐため、`package.json` のバージョン表記を修正。
- ※バージョン `1.1.0` は `package.json` の設定不備のためリリースを取り下げました。

## [1.1.0] - 2026-07-13 [YANKED]

### Added
- タスクがキャンセルされた時の挙動（`Safe` / `DontThrow` / `Drop`）を制御できるキャンセルモード機能 (`Story.CancelMode`, `Story.DefaultCancelMode`, `Story.TaskCancelMode`) を追加。
- タスクのプールを無駄なく事前確保する `Story.WarmupMode()` を追加。
- 並行実行ユーティリティ `Task.With()` に、引数5〜8をサポートするオーバーロードを追加。
- エディタ拡張「Story Task Monitor」をカラム表示化し、コードジャンプ・コピー機能付きのコールスタック表示機能を追加。

### Changed
- ステートマシンプールのコアを汎用プールと共通化し、コードブロート削減の観点からプールの `Alloc` および `Expand` を中心に処理を整理。
- 組み込みの各種タスク関数において、キャンセル時に不必要な例外が発生しないよう内部処理を改善。
- エディタ拡張「Story Pool Monitor」の `Worst` 値の計測精度を向上させ、表示する型名フォーマットを見やすく調整。

### Deprecated
- プール処理の共通化とコードブロート対策に伴い、一部のメソッド（`Story.Pool.Create<T>`, `Story.Pool.Expand<T>`, `Story.Pool.CreateBasedOnItemSize<T>`, `Story.Pool.ExpandBasedOnItemSize<T>`, `PoolBase<T>.Alloc` など）を廃止（`[Obsolete]`）に変更。※移行先はIDEのメッセージにて案内されます。

### Fixed
- ジェネリックなタスクにおいて `[Story.Capacity]` 属性による確保サイズ指定が無効になっていた不具合を修正。
- `STORY_NO_DEBUG` ディファインを有効にした際に発生する不具合を修正。

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