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
// ↑コメントアウトを外すと簡易的にエディタ上でもある程度の製品版のロジックで動作させることができます

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
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG

    /// <summary>Don't touch! Only for system.</summary>
    internal interface IPoolMonitorForDebug
    {
        /// <summary>Don't touch! Only for system.</summary>
        string PoolName { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int ActiveCount { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int WorstCount { get; set; }

        /// <summary>Don't touch! Only for system.</summary>
        int FreeCount { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int TotalBytes { get; }

        /// <summary>Don't touch! Only for system.</summary>
        internal static readonly List<IPoolMonitorForDebug> Monitors = new();

        /// <summary>Don't touch! Only for system.</summary>
        internal static void Register(IPoolMonitorForDebug monitor) => Monitors.Add(monitor);
    }

    /// <summary>Don't touch! Only for system.</summary>
    internal static class DevForEditor
    {
        /// <summary>Don't touch! Only for system.</summary>
        internal static string FormatMemorySize(int bytes) => Dev.FormatMemorySize(bytes);

        /// <summary>Don't touch! Only for system.</summary>
        internal static class TaskMonitorAPI
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchAutoCount(ref int count) => count = TaskManager.Shared.AutoTopCount;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchManualCount(ref int count) => count = TaskManager.Shared.ManualTopCount;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchLateCount(ref int count) => count = TaskManager.Shared.LateTopCount;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchFixedCount(ref int count) => count = TaskManager.Shared.FixedTopCount;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void ExtractOwner(ref Component owner, Story.Task task) => owner = task.Info().Owner;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void GetOrder(ref long offset, Story.Task task) => offset = task.Info2().SortKeyForDebug;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void GetTaskList(List<Story.Task> outTasks)
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
                } while (!info.IsTop);
            }
        }
    }

#endif

// --------------------------------------------------------------------------------------------------------------------
// 開発向け機能

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG && !SIMPLE_CHECK

    internal class Dev : UnityEngine.Debug
    {
        /// <summary>Forcibly stores a temporary integer value used for pool tracking diagnostics.</summary>
        internal static void SetInt(int prm) => storedInt = prm;

        /// <summary>Retrieves the temporarily stored integer value used for pool tracking diagnostics.</summary>
        internal static int GetInt() => storedInt;

        /// <summary>Registers a diagnostic pool monitor instance to the global debug registry.</summary>
        internal static void PoolMonitorRegister(IPoolMonitorForDebug monitor)
            => IPoolMonitorForDebug.Register(monitor);

        /// <summary>Validates whether the awaited task type is supported natively inside the story task loop.</summary>
        internal static void ValidateAwaiter<T>()
        {
            if (!AwaiterValidator<T>.IsValid) { throw new NotSupportedException(string.Format(Messages.Exceptions.NotSupportedAwait, Type<T>.Name)); }
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class LoopBreak
        {
            static int count;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void Init() => count = 0;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void Check(string str = "")
            {
                if (255 < ++count) { throw new InvalidOperationException(string.Format(Messages.Exceptions.InfiniteLoop, str, TaskManager.Shared.GetRunningInfo().GetMethodName())); }
            }
        }

        /// <summary>Extracts the formatted bracketed name prefix of the specified state machine pool monitor.</summary>
        internal static string ToString(StateMachine.IStateMachinePool pool)
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
        internal static string ToString(Story.Task self)
        {
            ref var info = ref self.Info();
            ref var info2 = ref self.Info2();

            var stateStr = info2.StateForDebug;
            var methodName = info.IsValid ? info.GetMethodName() : string.Format(Messages.DebugInfo.InvalidTask, self.Id.Index, self.Id.Age);
            var ownerName = GetOwnerName(ref info);

            var offset = info.IsTop ? (info.Offset & ~TaskManager.BAND_TYPE_MASK).ToString() : "w";
            var prevIndex = info.IsTop ? -1 : info2.Prev;
            var nextIndex = info2.Next;
            if (0 <= nextIndex &&
                Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(nextIndex).IsTop) { nextIndex = -1; }

            return $"{stateStr} ({offset}) | [{ToDebugString(self.Id.Index)}/{self.Id.Age}] [{ToDebugString(prevIndex)}:{ToDebugString(nextIndex)}] | {methodName} @ {ownerName}";
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Type<T> { internal static string Name = GetTypeName(typeof(T)); }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Pool<T> { internal static string Name = "[Pool] " + Type<T>.Name; }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Pool<HOT, COOL> { internal static string Name = $"[Pool2] {Type<HOT>.Name} / {Type<COOL>.Name}"; }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class HiddenPool<T> { internal static string Name = "[Hidden] " + Type<T>.Name; }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class StateMachinePool<S> { internal static string Name = "[StateMachine] " + Type<S>.Name; }

        /// <summary>Formats a raw byte count into a human-readable string representation with appropriate binary units.</summary>
        internal static string FormatMemorySize(int bytes)
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

        /// <summary>Don't touch! Only for system.</summary>
        internal static void ValidateManualTask(ref TaskInfo rootInfo, ref TaskInfo topInfo, string message)
        {
            Assert(topInfo.IsTop, string.Format(Messages.Exceptions.AlreadyAwaited, rootInfo.GetMethodName()));
            Assert(TaskManager.Shared.IsManualBand(topInfo.Offset), string.Format(message, topInfo.GetMethodName()));
        }

        // for debug only

        static int storedInt;
        static class AwaiterValidator<T>
        {
            internal static readonly bool IsValid;
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
        static string GetOwnerName(ref TaskInfo info)
        {
            if (info.IsPinned) { return Messages.DebugInfo.StatePinned; }
            if (info.Owner is null) { return Messages.DebugInfo.OwnerNull; }
            if (info.ShouldCancel) { return Messages.DebugInfo.StateDead; }
            return info.Owner.name;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static string ToDebugString(int num) => num < 0 ? "_" : num.ToString();
    }

#else

    /// <summary>Don't touch! Only for system.</summary>
    internal class Dev : Debug
    {
        [Conditional("DUMMY")] internal static void SetInt(int prm) {}
        internal static int GetInt() => 0;
        [Conditional("DUMMY")] internal static void PoolMonitorRegister(object monitor) {}
        [Conditional("DUMMY")] internal static void ValidateAwaiter<T>() {}

        internal static class LoopBreak
        {
            [Conditional("DUMMY")] internal static void Init() {}
            [Conditional("DUMMY")] internal static void Check(string str) {}
        }

        internal static string ToString(StateMachine.IStateMachinePool pool) => string.Empty;
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        internal static string ToString(Story.Task self) => string.Empty;
        internal static class Type<T> { internal static string Name = string.Empty; }
        internal static class Pool<T> { internal static string Name = string.Empty; }
        internal static class Pool<HOT, COOL> { internal static string Name = string.Empty; }
        internal static class HiddenPool<T> { internal static string Name = string.Empty; }
        internal static class StateMachinePool<S> { internal static string Name = string.Empty; }
#endif
        internal static string FormatMemorySize(int bytes) => string.Empty;

        [Conditional("DUMMY")] internal static void ValidateManualTask(ref TaskInfo rootInfo, ref TaskInfo topInfo, string message) {}

        internal static class TaskMonitorAPI
        {
            [Conditional("DUMMY")] internal static void FetchAutoCount(ref int count) {}
            [Conditional("DUMMY")] internal static void FetchManualCount(ref int count) {}
            [Conditional("DUMMY")] internal static void FetchLateCount(ref int count) {}
            [Conditional("DUMMY")] internal static void FetchFixedCount(ref int count) {}
            [Conditional("DUMMY")] internal static void ExtractOwner(ref Component owner, Story.Task task) {}
            [Conditional("DUMMY")] internal static void GetOrder(ref long offset, Story.Task task) {}
            [Conditional("DUMMY")] internal static void GetTaskList(List<Story.Task> outTasks) {}
        }
    }

    internal class Debug
    {

#if false   // 製品版でパフォーマンスを重視したいとき

        [Conditional("DUMMY")] internal static void Assert(bool condition, string message) {}
        [Conditional("DUMMY")] internal static void Assert(bool condition) {}
        [Conditional("DUMMY")] internal static void LogException(System.Exception exception) {}
        [Conditional("DUMMY")] internal static void LogError(object message) {}

#else       // 製品版でリスクヘッジしたいとき

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Assert(bool condition, string message) { if (!condition) { throw new Exception(message); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Assert(bool condition) { if (!condition) { throw new Exception(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogException(System.Exception exception) { throw exception; }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogError(string message) { throw new Exception(message); }

#endif

        [Conditional("DUMMY")] internal static void Log(object message) {}
        [Conditional("DUMMY")] internal static void LogWarning(object message) {}
    }

#endif

}
