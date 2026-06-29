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
            internal const string AlreadyBooted = "{0}:既に起動されているので Start できない";
            internal const string CannotMoveNextAutoTask = "{0}:自動タスクなので MoveNext できない";
            internal const string CannotAwaitAutoTask = "{0}:自動タスクは await できません。 while (task.IsValid) {{ await Story.Yield; }} してください";
            internal const string DoubleAwait = "await されているのに await された：[{0}] - [{1}]";
            internal const string AwaitingWhileAwaited = "await しているのに await した：[{0}] - [{1}]";
            internal const string TaskCanceled = "タスクがキャンセルされました。";
            internal const string CannotOperateInvalidTask = "無効な（あるいは終了した）タスクは操作できません";
            internal const string CannotOperateInvalidTaskFormat = "無効な（あるいは終了した）タスクは操作できません：{0}";
            internal const string InvalidBandCount = "bandCountは1〜7を指定してください。標準は3です：{0}";
        }

        // ------------------------------------------------------------------------
        // 警告ログ (Warnings)
        // ------------------------------------------------------------------------
        internal static class Warnings
        {
            internal const string WarmupOnly = "Warmupはプールの拡張のみ可能です({0} -> {1})";
            internal const string ArrayExpanded = "配列が拡張されました: {0} -> {1}({2}) [{3}]";
            internal const string ArrayExpanded_StateMachine = "配列が拡張されました: {0} -> {1}({2}) [{3}] (StateMachine)";
            internal const string ArrayExpanded_BasedOn = "配列が拡張されました: {0} -> {1}({2}) [{3}] (by based on)";
            internal const string CannotCustomizeAfterStart = "使用を開始した後はカスタムできません";
            internal const string UnhandledResult = "結果が受け取られませんでした：{0}";
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
            internal const string AlreadyBooted = "{0}: Cannot Start because it is already running.";
            internal const string CannotMoveNextAutoTask = "{0}: Cannot call MoveNext on an auto-task.";
            internal const string CannotAwaitAutoTask = "{0}: Auto-tasks cannot be awaited. Please use 'while (task.IsValid) {{ await Story.Yield; }}'.";
            internal const string DoubleAwait = "Awaited while already being awaited: [{0}] - [{1}]";
            internal const string AwaitingWhileAwaited = "Attempted to await while currently awaiting: [{0}] - [{1}]";
            internal const string TaskCanceled = "Task was canceled.";
            internal const string CannotOperateInvalidTask = "Cannot operate on an invalid or already finished task.";
            internal const string CannotOperateInvalidTaskFormat = "Cannot operate on an invalid or already finished task: {0}";
            internal const string InvalidBandCount = "bandCount must be between 1 and 7. The default is 3: {0}";
        }

        // ------------------------------------------------------------------------
        // Warnings
        // ------------------------------------------------------------------------
        internal static class Warnings
        {
            internal const string WarmupOnly = "Warmup can only expand the pool. ({0} -> {1})";
            internal const string ArrayExpanded = "Array is expanded: {0} -> {1}({2}) [{3}]";
            internal const string ArrayExpanded_StateMachine = "Array is expanded: {0} -> {1}({2}) [{3}] (StateMachine)";
            internal const string ArrayExpanded_BasedOn = "Array is expanded: {0} -> {1}({2}) [{3}] (by based on)";
            internal const string CannotCustomizeAfterStart = "Cannot customize after execution has started.";
            internal const string UnhandledResult = "Result was not received (unhandled): {0}";
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
