// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryMessages.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
// </copyright>
// <summary>
//   Centralizes all system messages, warnings, and UI text to facilitate future localization.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    public static class Messages
    {
#if true
        // ------------------------------------------------------------------------
        // エラー・アサート・例外メッセージ (Exceptions & Assertions)
        // ------------------------------------------------------------------------
        public static class Exceptions
        {
            public const string NotSupportedAwait = "Story.Task 内で外部のタスク({0})を await することはサポートされていません。";
            public const string InfiniteLoop = "無限ループ:{0}(RunningTask.Name:{1})";
            public const string AlreadyAwaited = "{0}:他のタスクに await されている";
            public const string InvalidExtraOperation = "不正なタスクの Extra を操作しようとした";
            public const string MasterCannotBeNull = "master を null にすることはできません。タスク外で Boot するときは master を指定してください";
            public const string InvalidTaskOnBoot = "無効なタスクです。タスクを作成してから Boot するまでにフレームをまたぐ場合は Keep してください。";
            public const string AlreadyBooted = "{0}:既に起動されているので Boot できない";
            public const string CannotMoveNextAutoTask = "{0}:自動タスクなので MoveNext できない";
            public const string CannotAwaitAutoTask = "{0}:自動タスクは await できません。 while (task.IsValid) {{ await Story.Yield; }} してください";
            public const string DoubleAwait = "await されているのに await された：[{0}] - [{1}]";
            public const string AwaitingWhileAwaited = "await しているのに await した：[{0}] - [{1}]";
            public const string LockCapacityExceeded = "EnterLock で宣言した数を超えた割り当てが発生";
            public const string TaskCanceled = "タスクがキャンセルされました。";
            public const string LockCountNegative = "ロックカウントがマイナスになりました！";
        }

        // ------------------------------------------------------------------------
        // 警告ログ (Warnings)
        // ------------------------------------------------------------------------
        public static class Warnings
        {
            public const string ResultOverwritten = "格納していた結果を受け取る前に次の結果が格納された：{0}";
            public const string ResultNotFound = "結果を取得できなかった：{0}";
            public const string RecursiveInvokeIgnored = "再帰実行しようとしたので無視します：{0}";
            public const string CancelPending = "キャンセルされようとしていたのでこの実行が終わり次第、キャンセルを実行します";
            public const string CancelExecutedAfterRun = "実行が終わったので、キャンセルを実行します：{0}";
            public const string CancelAbortedFinishedTask = "タスクが終点まで実行されたので、予定していたキャンセルの実行は中止します：{0}";
            public const string CancelPendingWhileRunning = "キャンセルを指示されましたがタスクが動作中なので、終わり次第キャンセルを実行します：{0}";
            public const string CancelFailedTaskRunning = "キャンセルしたのに終了しませんでした。続きは自動タスクで処理します。以降はmasterを無視するので責任を持って終了させてください：{0}";
            public const string ExpandOnly = "Expandはプールの拡張のみ可能です({0} -> {1})";
            public const string ArrayExpanded = "配列が拡張されました: {0} -> {1}({2}) [{3}]";
            public const string ArrayExpanded_StateMachine = "配列が拡張されました: {0} -> {1}({2}) [{3}] (StateMachine)";
            public const string ArrayExpanded_BasedOn = "配列が拡張されました: {0} -> {1}({2}) [{3}] (by based on)";
        }

        // ------------------------------------------------------------------------
        // エディタUI・メニュー (Editor UI)
        // ------------------------------------------------------------------------
        public static class EditorUI
        {
            // Task Monitor 関連
            public const string TaskMonitor_PlayModeOnly = "プレイモード中のみタスクを表示します。";
            public const string TaskMonitor_MenuPingMaster = "マスターを検索";
            public const string TaskMonitor_MenuForceFree = "強制終了";
            public const string TaskMonitor_Refresh = "Refresh";
            public const string TaskMonitor_AutoRefresh = "Auto Refresh";
            public const string TaskMonitor_StatAuto = "Auto: {0}";
            public const string TaskMonitor_StatManual = "Manual: {0}";
            public const string TaskMonitor_StatLate = "Late: {0}";
            public const string TaskMonitor_StatFixed = "Fixed: {0}";
            public const string TaskMonitor_StatWait = "Wait: {0}";
            public const string TaskMonitor_StatTotal = "Total: {0}";

            // Pool Monitor 関連
            public const string PoolMonitor_PlayModeOnly = "Playモードに入ると、生成されたプールのリアルタイム状況が表示されます。";
            public const string PoolMonitor_ColActive = "Active";
            public const string PoolMonitor_ColWorst = "Worst";
            public const string PoolMonitor_ColFree = "Free";
            public const string PoolMonitor_ColMemory = "Total Memory";
            public const string PoolMonitor_ColName = "Pool Name";
            public const string PoolMonitor_StatCount = "Pool Count: {0}";
            public const string PoolMonitor_StatUsage = "Memory Usage: {0:P}";
            public const string PoolMonitor_StatMemory = "Total Memory: {0}";
        }

        // ------------------------------------------------------------------------
        // デバッグテキスト・ToStringフォーマット (Debug Info)
        // ------------------------------------------------------------------------
        public static class DebugInfo
        {
            public const string InvalidTask = "[INVALID Task ({0}:{1})]";
            public const string StatePinned = "[Pinned]";
            public const string StateDead = "[Dead]";
            public const string MasterNull = "null";
            public const string TypeUnknown = "Unknown";
        }
#else
        // ------------------------------------------------------------------------
        // Exceptions & Assertions
        // ------------------------------------------------------------------------
        public static class Exceptions
        {
            public const string NotSupportedAwait = "Awaiting external tasks ({0}) inside Story.Task is not supported.";
            public const string InfiniteLoop = "Infinite loop detected: {0} (RunningTask.Name: {1})";
            public const string AlreadyAwaited = "{0}: Already being awaited by another task.";
            public const string InvalidExtraOperation = "Attempted to manipulate Extra on an invalid task.";
            public const string MasterCannotBeNull = "Master cannot be null. When booting outside a task, a master must be specified.";
            public const string InvalidTaskOnBoot = "Invalid task. If spanning frames between task creation and Boot, please call Keep().";
            public const string AlreadyBooted = "{0}: Cannot Boot because it is already running.";
            public const string CannotMoveNextAutoTask = "{0}: Cannot call MoveNext on an auto-task.";
            public const string CannotAwaitAutoTask = "{0}: Auto-tasks cannot be awaited. Please use 'while (task.IsValid) {{ await Story.Yield; }}'.";
            public const string DoubleAwait = "Awaited while already being awaited: [{0}] - [{1}]";
            public const string AwaitingWhileAwaited = "Attempted to await while currently awaiting: [{0}] - [{1}]";
            public const string LockCapacityExceeded = "Allocation exceeded the reserve count declared in EnterLock.";
            public const string TaskCanceled = "Task was canceled.";
            public const string LockCountNegative = "Lock count went negative!";
        }

        // ------------------------------------------------------------------------
        // Warnings
        // ------------------------------------------------------------------------
        public static class Warnings
        {
            public const string ResultOverwritten = "The next result was stored before the previous result was retrieved: {0}";
            public const string ResultNotFound = "Failed to retrieve the result: {0}";
            public const string RecursiveInvokeIgnored = "Ignored attempt to invoke recursively: {0}";
            public const string CancelPending = "A cancellation was pending, so it will be executed as soon as this run completes.";
            public const string CancelExecutedAfterRun = "Execution has completed, executing cancellation: {0}";
            public const string CancelAbortedFinishedTask = "Task has executed to the end, aborting the scheduled cancellation: {0}";
            public const string CancelPendingWhileRunning = "Cancellation requested while the task is running; it will be canceled upon completion: {0}";
            public const string CancelFailedTaskRunning = "Task did not finish despite being canceled. The remainder will be processed as an auto-task. The master will be ignored from now on, so please ensure it is terminated properly: {0}";
            public const string ExpandOnly = "Expand can only expand the pool. ({0} -> {1})";
            public const string ArrayExpanded = "Array is expanded: {0} -> {1}({2}) [{3}]";
            public const string ArrayExpanded_StateMachine = "Array is expanded: {0} -> {1}({2}) [{3}] (StateMachine)";
            public const string ArrayExpanded_BasedOn = "Array is expanded: {0} -> {1}({2}) [{3}] (by based on)";
        }

        // ------------------------------------------------------------------------
        // Editor UI
        // ------------------------------------------------------------------------
        public static class EditorUI
        {
            // Task Monitor
            public const string TaskMonitor_PlayModeOnly = "Tasks are only displayed during Play Mode.";
            public const string TaskMonitor_MenuPingMaster = "Ping Master";
            public const string TaskMonitor_MenuForceFree = "Force Free Task";
            public const string TaskMonitor_Refresh = "Refresh";
            public const string TaskMonitor_AutoRefresh = "Auto Refresh";
            public const string TaskMonitor_StatAuto = "Auto: {0}";
            public const string TaskMonitor_StatManual = "Manual: {0}";
            public const string TaskMonitor_StatLate = "Late: {0}";
            public const string TaskMonitor_StatFixed = "Fixed: {0}";
            public const string TaskMonitor_StatWait = "Wait: {0}";
            public const string TaskMonitor_StatTotal = "Total: {0}";

            // Pool Monitor
            public const string PoolMonitor_PlayModeOnly = "Real-time pool status will be displayed when entering Play Mode.";
            public const string PoolMonitor_ColActive = "Active";
            public const string PoolMonitor_ColWorst = "Worst";
            public const string PoolMonitor_ColFree = "Free";
            public const string PoolMonitor_ColMemory = "Total Memory";
            public const string PoolMonitor_ColName = "Pool Name";
            public const string PoolMonitor_StatCount = "Pool Count: {0}";
            public const string PoolMonitor_StatUsage = "Memory Usage: {0:P}";
            public const string PoolMonitor_StatMemory = "Total Memory: {0}";
        }

        // ------------------------------------------------------------------------
        // Debug Info
        // ------------------------------------------------------------------------
        public static class DebugInfo
        {
            public const string InvalidTask = "[INVALID Task ({0}:{1})]";
            public const string StatePinned = "[Pinned]";
            public const string StateDead = "[Dead]";
            public const string MasterNull = "null";
            public const string TypeUnknown = "Unknown";
        }
#endif
    }
}
