// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryDebug.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Defines internal diagnostic tools, validation utilities, and custom assertion systems
//   to monitor and debug the execution runtime of the Omochaya Story framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// #define SIMPLE_CHECK
// ↑コメントアウトを外すとエディタ上でもある程度は製品版のロジックで動作させることができます

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Runtime.CompilerServices;
    using System.Text;
    using UnityEngine;
    using UnityEngine.LowLevel;

// --------------------------------------------------------------------------------------------------------------------
// エディタ向け機能
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST

    public interface IPoolMonitorForDebug
    {
        /// <summary>Gets the unique text identifier formatted for this state machine pool instance.</summary>
        string PoolName { get; }

        /// <summary>Gets the total number of actively allocated elements inside the state machine core.</summary>
        int ActiveCount { get; }

        /// <summary>Gets or sets the historical maximum peak of concurrently active elements observed during runtime.</summary>
        int WorstCount { get; set; }

        /// <summary>Gets the remaining unallocated element capacity within the state machine pool.</summary>
        int FreeCount { get; }

        /// <summary>Gets the total physical memory footprint of this state machine pool in bytes.</summary>
        int TotalBytes { get; }

        public static readonly List<IPoolMonitorForDebug> Monitors = new();
        public static void Register(IPoolMonitorForDebug monitor) => Monitors.Add(monitor);
    }

    public static class DevForEditor
    {
        public static string FormatMemorySize(int bytes) => Dev.FormatMemorySize(bytes);
        public static class TaskMonitorAPI
        {
            /// <summary>Fetches the current offset count of automated tasks from the shared task manager engine.</summary>
            public static void FetchAutoCount(ref int count) => count = TaskManager.Shared.AutoTopCount;

            /// <summary>Fetches the active offset count of manually driven tasks within the active execution window.</summary>
            public static void FetchManualCount(ref int count) => count = TaskManager.Shared.ManualTopCount;

            /// <summary></summary>
            public static void FetchLateCount(ref int count) => count = TaskManager.Shared.LateTopCount;

            /// <summary></summary>
            public static void FetchFixedCount(ref int count) => count = TaskManager.Shared.FixedTopCount;

            /// <summary>Extracts the binding lifecycle master component from the target task info block.</summary>
            public static void ExtractMaster(ref Component master, Story.Task task) => master = task.Info().Master;

            /// <summary>Retrieves the unique sorting layout key assigned to the specified debug task handle.</summary>
            public static void GetOrder(ref long offset, Story.Task task) => offset = task.Info2().SortKeyForDebug;

            /// <summary>Traverses the entire linear execution tree of the task manager to compile a flat snapshot list of active tasks.</summary>
            public static void GetTaskList(List<Story.Task> outTasks)
            {
                var self = TaskManager.Shared;
                outTasks.Clear();
                for (var i=0; i<self.ManualTopCount; ++i)
                {
                    var index = self.ManualIndexForDebug(i);
                    TaskListAdd(outTasks, index, "Manual");
                }
                for (var i=0; i<self.AutoTopCount; ++i)
                {
                    var index = self.AutoIndexForDebug(i);
                    TaskListAdd(outTasks, index, "Auto");
                }
                for (var i=0; i<self.LateTopCount; ++i)
                {
                    var index = self.LateIndexForDebug(i);
                    TaskListAdd(outTasks, index, "Late");
                }
                for (var i=0; i<self.FixedTopCount; ++i)
                {
                    var index = self.FixedIndexForDebug(i);
                    TaskListAdd(outTasks, index, "Fixed");
                }
            }
            static void TaskListAdd(List<Story.Task> outTasks, int index, string state)
            {
                if (index < 0) { return; }
                var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
                ref var info = ref pool.UnsafeGet(index);
                var sortKey = ((long)info.Offset + 1) << 32;
Dev.LoopBreak.Init();
                do
                {
Dev.LoopBreak.Check(index.ToString());
                    ref var info2 = ref pool.UnsafeGet2(index);
                    info2.SortKeyForDebug = sortKey--;
                    info2.StateForDebug = state;
                    outTasks.Add(Story.Task.UnsafeCreate(index));
                    index = info2.Next;
                    info = ref pool.UnsafeGet(index);
                } while (!info.HasOffset);
            }
        }
    }

#endif

// --------------------------------------------------------------------------------------------------------------------
// 開発向け機能

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST && !SIMPLE_CHECK

    class Dev : UnityEngine.Debug
    {
        /// <summary>Forcibly stores a temporary integer value used for pool tracking diagnostics.</summary>
        public static void SetInt(int prm) => storedInt = prm;

        /// <summary>Retrieves the temporarily stored integer value used for pool tracking diagnostics.</summary>
        public static int GetInt() => storedInt;

        /// <summary>Registers a diagnostic pool monitor instance to the global debug registry.</summary>
        public static void PoolMonitorRegister(IPoolMonitorForDebug monitor)
            => IPoolMonitorForDebug.Register(monitor);

        /// <summary>Validates whether the awaited task type is supported natively inside the story task loop.</summary>
        public static void ValidateAwaiter<T>()
        {
            if (!AwaiterValidator<T>.IsValid) { throw new NotSupportedException(string.Format(Messages.Exceptions.NotSupportedAwait, Type<T>.Name)); }
        }

        public static class LoopBreak
        {
            static int count;

            /// <summary>Initializes the infinite loop detection counter for the current frame branch.</summary>
            public static void Init() => count = 0;

            /// <summary>Increments and verifies the loop counter to abort and throw upon detecting an infinite execution loop.</summary>
            public static void Check(string str = "")
            {
                if (255 < ++count) { throw new InvalidOperationException(string.Format(Messages.Exceptions.InfiniteLoop, str, TaskManager.Shared.GetRunningInfo().GetMethodName())); }
            }
        }

        /// <summary>Extracts the formatted bracketed name prefix of the specified state machine pool monitor.</summary>
        public static string ToString(StateMachine.IStateMachinePool pool)
        {
            if (pool is IPoolMonitorForDebug monitor)
            {
                var name = monitor.PoolName;
                var end = name.IndexOf("]");
                if (0 < end) { return name.Substring(end+1); }
                return name;
            }
            return GetTypeName(null);
        }

        /// <summary>Formats the entire active or historical state configuration of a task handle into a comprehensive debug string.</summary>
        public static string ToString(Story.Task self)
        {
            ref var info = ref self.Info();
            ref var info2 = ref self.Info2();

            var stateStr = info2.StateForDebug;
            var methodName = info.IsValid ? info.GetMethodName() : string.Format(Messages.DebugInfo.InvalidTask, self.Id.Index, self.Id.Age);
            if (info.HasException) { methodName += " -EXCEPTION"; }
            var masterName = GetMasterName(ref info);

            var offset = info.HasOffset ? (info.Offset & ~TaskManager.BAND_TYPE_MASK).ToString() : "w";
            var prevIndex = info.HasOffset ? -1 : info2.Prev;
            var nextIndex = info2.Next;
            if (Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(nextIndex).HasOffset) { nextIndex = -1; }

            return $"{stateStr} ({offset}) | [{ToDebugString(self.Id.Index)}/{self.Id.Age}] [{ToDebugString(prevIndex)}:{ToDebugString(nextIndex)}] | {methodName} @ {masterName}";
        }
        public static class Type<T> { public static string Name = GetTypeName(typeof(T)); }
        public static class Pool<T> { public static string Name = "[Pool] " + Type<T>.Name; }
        public static class Pool<HOT, COOL> { public static string Name = $"[Pool2] {Type<HOT>.Name} / {Type<COOL>.Name}"; }
        public static class HiddenPool<T> { public static string Name = "[Hidden] " + Type<T>.Name; }
        public static class StateMachinePool<S> { public static string Name = "[StateMachine] " + Type<S>.Name; }

        /// <summary>Formats a raw byte count into a human-readable string representation with appropriate binary units.</summary>
        public static string FormatMemorySize(int bytes)
        {
            string[] units = { "B", "KB", "MB", "GB", "TB" };
            var i = 0;
            float ret = bytes;
            while (ret >= 1024 && i < units.Length - 1)
            {
                ret /= 1024f;
                i++;
            }
            return $"{ret:F2} {units[i]}";
        }

        /// <summary>Validates that a manually driven task is not currently being awaited by another active state machine.</summary>
        public static void ValidateManualTask(ref TaskInfo rootInfo, ref TaskInfo topInfo, string message)
        {
            Assert(topInfo.HasOffset, string.Format(Messages.Exceptions.AlreadyAwaited, rootInfo.GetMethodName()));
            Assert(TaskManager.Shared.IsManualBand(topInfo.Offset), string.Format(message, topInfo.GetMethodName()));
        }

        /// <summary>Captures the current Unity PlayerLoop architecture layout and dumps its hierarchy to the log output.</summary>
        public static void DumpPlayerLoop()
        {
            var rootLoop = PlayerLoop.GetCurrentPlayerLoop();
            var sb = new StringBuilder();
            DumpPlayerLoop(rootLoop, sb, 0);
            Dev.Log(sb.ToString());
        }

        // for debug only

        static int storedInt;
        static class AwaiterValidator<T>
        {
            public static readonly bool IsValid;
            static AwaiterValidator()
            {
                var type = typeof(T);
                IsValid = type == typeof(Awaiter) ||
                        type == typeof(YieldCore) ||
                        type == typeof(VoidCore) ||
                        (type.IsGenericType && type.GetGenericTypeDefinition() == typeof(Awaiter<>));
            }
        }
        static string GetTypeName(Type type)
        {
            if (type == null) return Messages.DebugInfo.TypeUnknown;
            var name = type.FullName;
            var end = name.IndexOf(">");
            if (0 < end) { return name.Substring(0, end+1); }
            end = name.IndexOf("`");
            if (0 < end) { return name.Substring(0, end); }
            return name;
        }
        static string GetMasterName(ref TaskInfo info)
        {
            if (info.IsPinned) { return Messages.DebugInfo.StatePinned; }
            if (info.Master is null) { return Messages.DebugInfo.MasterNull; }
            if (info.IsOrphaned) { return Messages.DebugInfo.StateDead; }
            return info.Master.name;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string ToDebugString(int num) => num < 0 ? "_" : num.ToString();
        static void DumpPlayerLoop(PlayerLoopSystem system, StringBuilder sb, int indent)
        {
            if (system.type != null)
            {
                sb.AppendLine($"{new string(' ', indent * 2)}- {system.type.Name}");
            }

            if (system.subSystemList != null)
            {
                foreach (var sub in system.subSystemList)
                {
                    DumpPlayerLoop(sub, sb, indent + 1);
                }
            }
        }
    }

#else

    class Dev : Debug
    {
        [Conditional("DUMMY")] public static void SetInt(int prm) {}
        public static int GetInt() => 0;
        [Conditional("DUMMY")] public static void PoolMonitorRegister(object monitor) {}
        [Conditional("DUMMY")] public static void ValidateAwaiter<T>() {}

        public static class LoopBreak
        {
            [Conditional("DUMMY")] public static void Init() {}
            [Conditional("DUMMY")] public static void Check(string str) {}
        }

        public static string ToString(StateMachine.IStateMachinePool pool) => string.Empty;
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public static string ToString(Story.Task self) => string.Empty;
        public static class Type<T> { public static string Name = string.Empty; }
        public static class Pool<T> { public static string Name = string.Empty; }
        public static class Pool<HOT, COOL> { public static string Name = string.Empty; }
        public static class HiddenPool<T> { public static string Name = string.Empty; }
        public static class StateMachinePool<S> { public static string Name = string.Empty; }
#endif
        public static string FormatMemorySize(int bytes) => string.Empty;

        [Conditional("DUMMY")] public static void ValidateManualTask(ref TaskInfo rootInfo, ref TaskInfo topInfo, string message) {}

        public static class TaskMonitorAPI
        {
            [Conditional("DUMMY")] public static void FetchAutoCount(ref int count) {}
            [Conditional("DUMMY")] public static void FetchManualCount(ref int count) {}
            [Conditional("DUMMY")] public static void FetchLateCount(ref int count) {}
            [Conditional("DUMMY")] public static void FetchFixedCount(ref int count) {}
            [Conditional("DUMMY")] public static void ExtractMaster(ref Component master, Story.Task task) {}
            [Conditional("DUMMY")] public static void GetOrder(ref long offset, Story.Task task) {}
            [Conditional("DUMMY")] public static void GetTaskList(List<Story.Task> outTasks) {}
        }

        [Conditional("DUMMY")] public static void DumpPlayerLoop() {}
    }

    class Debug
    {

#if false   // 製品版でパフォーマンスを重視したいとき

        [Conditional("DUMMY")] public static void Assert(bool condition, string message) {}
        [Conditional("DUMMY")] public static void Assert(bool condition) {}
        [Conditional("DUMMY")] public static void LogException(System.Exception exception) {}
        [Conditional("DUMMY")] public static void LogError(object message) {}

#else       // 製品版でリスクヘッジしたいとき

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition, string message) { if (!condition) { throw new Exception(message); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Assert(bool condition) { if (!condition) { throw new Exception(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogException(System.Exception exception) { throw exception; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LogError(string message) { throw new Exception(message); }

#endif

        [Conditional("DUMMY")] public static void Log(object message) {}
        [Conditional("DUMMY")] public static void LogWarning(object message) {}
    }

#endif

}
