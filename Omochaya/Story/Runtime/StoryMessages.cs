// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryMessages.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
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
    /// <summary>Don't touch! Only for system.</summary>
    internal static class Messages
    {
#if true
        // ------------------------------------------------------------------------
        // エラー・アサート・例外メッセージ (Exceptions & Assertions)
        // ------------------------------------------------------------------------
        internal static class Exceptions
        {
            internal const string NotSupportedBandUpdate = "Manual と Auto は Story.Update で処理されます。 BandUpdate は対応していません：{0}";
            internal const string NotSupportedAwait = "Story.Task 内で外部のタスク({0})を await することはサポートされていません。";
            internal const string InfiniteLoop = "無限ループ:{0}(RunningTask.Name:{1})";
            internal const string AlreadyAwaited = "{0}:他のタスクに await されている";
            internal const string InvalidExtraOperation = "不正なタスクの Extra を操作しようとした";
            internal const string OwnerCannotBeNull = "owner を null にすることはできません。タスク外で Start するときは owner を指定してください";
            internal const string InvalidTaskOnBoot = "無効なタスクです。タスクを作成してから Start するまでにフレームをまたぐ場合は Keep してください。";
            internal const string AlreadyBooted = "{0}:既に起動されているので Start できない";
            internal const string CannotMoveNextAutoTask = "{0}:自動タスクなので MoveNext できない";
            internal const string CannotAwaitAutoTask = "{0}:自動タスクは await できません。 while (task.IsValid) {{ await Story.Yield; }} してください";
            internal const string DoubleAwait = "await されているのに await された：[{0}] - [{1}]";
            internal const string AwaitingWhileAwaited = "await しているのに await した：[{0}] - [{1}]";
            internal const string LockCapacityExceeded = "EnterLock で宣言した数を超えた割り当てが発生";
            internal const string TaskCanceled = "タスクがキャンセルされました。";
        }

        // ------------------------------------------------------------------------
        // 警告ログ (Warnings)
        // ------------------------------------------------------------------------
        internal static class Warnings
        {
            internal const string CancelAbortedFinishedTask = "タスクが終点まで実行されたので、予定していたキャンセルの実行は中止します：{0}";
            internal const string CancelPendingWhileRunning = "キャンセルを指示されましたがタスクが動作中なので、終わり次第キャンセルを実行します：{0}";
            internal const string CancelFailedTaskRunning = "キャンセルしたのに終了しませんでした。続きは自動タスクで処理します。以降はownerを無視するので責任を持って終了させてください：{0}";
            internal const string WarmupOnly = "Warmupはプールの拡張のみ可能です({0} -> {1})";
            internal const string ArrayExpanded = "配列が拡張されました: {0} -> {1}({2}) [{3}]";
            internal const string ArrayExpanded_StateMachine = "配列が拡張されました: {0} -> {1}({2}) [{3}] (StateMachine)";
            internal const string ArrayExpanded_BasedOn = "配列が拡張されました: {0} -> {1}({2}) [{3}] (by based on)";
        }

        // ------------------------------------------------------------------------
        // エディタUI・メニュー (Editor UI)
        // ------------------------------------------------------------------------
        internal static class EditorUI
        {
            // Task Monitor 関連
            internal const string TaskMonitor_PlayModeOnly = "Playモードに入ると、実行中のタスクのリアルタイム状況が表示されます。";
            internal const string TaskMonitor_MenuPingOwner = "オーナーを検索";
            internal const string TaskMonitor_MenuForceFree = "強制終了";
            internal const string TaskMonitor_Refresh = "Refresh";
            internal const string TaskMonitor_AutoRefresh = "Auto Refresh";
            internal const string TaskMonitor_StatAuto = "Auto: {0}";
            internal const string TaskMonitor_StatManual = "Manual: {0}";
            internal const string TaskMonitor_StatLate = "Late: {0}";
            internal const string TaskMonitor_StatFixed = "Fixed: {0}";
            internal const string TaskMonitor_StatWait = "Wait: {0}";
            internal const string TaskMonitor_StatTotal = "Total: {0}";

            // Pool Monitor 関連
            internal const string PoolMonitor_PlayModeOnly = "Playモードに入ると、生成されたプールのリアルタイム状況が表示されます。";
            internal const string PoolMonitor_ColActive = "Active";
            internal const string PoolMonitor_ColWorst = "Worst";
            internal const string PoolMonitor_ColFree = "Free";
            internal const string PoolMonitor_ColMemory = "Total Memory";
            internal const string PoolMonitor_ColName = "Pool Name";
            internal const string PoolMonitor_StatCount = "Pool Count: {0}";
            internal const string PoolMonitor_StatUsage = "Memory Usage: {0:P}";
            internal const string PoolMonitor_StatMemory = "Total Memory: {0}";
        }

        // ------------------------------------------------------------------------
        // デバッグテキスト・ToStringフォーマット (Debug Info)
        // ------------------------------------------------------------------------
        internal static class DebugInfo
        {
            internal const string InvalidTask = "[INVALID Task ({0}:{1})]";
            internal const string StatePinned = "[Pinned]";
            internal const string StateDead = "[Dead]";
            internal const string OwnerNull = "null";
            internal const string TypeUnknown = "Unknown";
        }
#else
        // ------------------------------------------------------------------------
        // Exceptions & Assertions
        // ------------------------------------------------------------------------
        internal static class Exceptions
        {
            internal const string NotSupportedBandUpdate = "：{0}";
            internal const string NotSupportedAwait = "Awaiting external tasks ({0}) inside Story.Task is not supported.";
            internal const string InfiniteLoop = "Infinite loop detected: {0} (RunningTask.Name: {1})";
            internal const string AlreadyAwaited = "{0}: Already being awaited by another task.";
            internal const string InvalidExtraOperation = "Attempted to manipulate Extra on an invalid task.";
            internal const string OwnerCannotBeNull = "Owner cannot be null. When starting outside a task, a owner must be specified.";
            internal const string InvalidTaskOnBoot = "Invalid task. If spanning frames between task creation and Start, please call Keep().";
            internal const string AlreadyBooted = "{0}: Cannot Start because it is already running.";
            internal const string CannotMoveNextAutoTask = "{0}: Cannot call MoveNext on an auto-task.";
            internal const string CannotAwaitAutoTask = "{0}: Auto-tasks cannot be awaited. Please use 'while (task.IsValid) {{ await Story.Yield; }}'.";
            internal const string DoubleAwait = "Awaited while already being awaited: [{0}] - [{1}]";
            internal const string AwaitingWhileAwaited = "Attempted to await while currently awaiting: [{0}] - [{1}]";
            internal const string LockCapacityExceeded = "Allocation exceeded the reserve count declared in EnterLock.";
            internal const string TaskCanceled = "Task was canceled.";
        }

        // ------------------------------------------------------------------------
        // Warnings
        // ------------------------------------------------------------------------
        internal static class Warnings
        {
            internal const string CancelAbortedFinishedTask = "Task has executed to the end, aborting the scheduled cancellation: {0}";
            internal const string CancelPendingWhileRunning = "Cancellation requested while the task is running; it will be canceled upon completion: {0}";
            internal const string CancelFailedTaskRunning = "Task did not finish despite being canceled. The remainder will be processed as an auto-task. The owner will be ignored from now on, so please ensure it is terminated properly: {0}";
            internal const string WarmupOnly = "Warmup can only expand the pool. ({0} -> {1})";
            internal const string ArrayExpanded = "Array is expanded: {0} -> {1}({2}) [{3}]";
            internal const string ArrayExpanded_StateMachine = "Array is expanded: {0} -> {1}({2}) [{3}] (StateMachine)";
            internal const string ArrayExpanded_BasedOn = "Array is expanded: {0} -> {1}({2}) [{3}] (by based on)";
        }

        // ------------------------------------------------------------------------
        // Editor UI
        // ------------------------------------------------------------------------
        internal static class EditorUI
        {
            // Task Monitor
            internal const string TaskMonitor_PlayModeOnly = "Tasks are only displayed during Play Mode.";
            internal const string TaskMonitor_MenuPingOwner = "Ping Owner";
            internal const string TaskMonitor_MenuForceFree = "Force Free Task";
            internal const string TaskMonitor_Refresh = "Refresh";
            internal const string TaskMonitor_AutoRefresh = "Auto Refresh";
            internal const string TaskMonitor_StatAuto = "Auto: {0}";
            internal const string TaskMonitor_StatManual = "Manual: {0}";
            internal const string TaskMonitor_StatLate = "Late: {0}";
            internal const string TaskMonitor_StatFixed = "Fixed: {0}";
            internal const string TaskMonitor_StatWait = "Wait: {0}";
            internal const string TaskMonitor_StatTotal = "Total: {0}";

            // Pool Monitor
            internal const string PoolMonitor_PlayModeOnly = "Real-time pool status will be displayed when entering Play Mode.";
            internal const string PoolMonitor_ColActive = "Active";
            internal const string PoolMonitor_ColWorst = "Worst";
            internal const string PoolMonitor_ColFree = "Free";
            internal const string PoolMonitor_ColMemory = "Total Memory";
            internal const string PoolMonitor_ColName = "Pool Name";
            internal const string PoolMonitor_StatCount = "Pool Count: {0}";
            internal const string PoolMonitor_StatUsage = "Memory Usage: {0:P}";
            internal const string PoolMonitor_StatMemory = "Total Memory: {0}";
        }

        // ------------------------------------------------------------------------
        // Debug Info
        // ------------------------------------------------------------------------
        internal static class DebugInfo
        {
            internal const string InvalidTask = "[INVALID Task ({0}:{1})]";
            internal const string StatePinned = "[Pinned]";
            internal const string StateDead = "[Dead]";
            internal const string OwnerNull = "null";
            internal const string TypeUnknown = "Unknown";
        }
#endif
    }
}
