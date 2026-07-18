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

// #define STORY_NO_DEBUG
// ↑ここのコメントアウトを外すと簡易的にエディタ上でも Story をある程度製品版のロジックで動作させることができます
// が、Story を完全に製品版ロジックで動作させるには Scripting Define Symbols に STORY_NO_DEBUG を追加してください

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

// --------------------------------------------------------------------------------------------------------------------
// エディタ向け機能
#if (FOR_DEBUG && !STORY_NO_DEBUG) || UNITY_EDITOR

    /// <summary>Don't touch! Only for system.</summary>
    internal interface IPoolMonitorForDebug
    {
        /// <summary>Don't touch! Only for system.</summary>
        string PoolName { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int ActiveCount { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int WorstCount { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int FreeCount { get; }

        /// <summary>Don't touch! Only for system.</summary>
        int TotalBytes { get; }

        /// <summary>Don't touch! Only for system.</summary>
        internal static readonly List<IPoolMonitorForDebug> Monitors = new();

        /// <summary>Don't touch! Only for system.</summary>
        internal static void Register(IPoolMonitorForDebug monitor) => Monitors.Add(monitor);
    }

#if STORY_NO_DEBUG

    /// <summary>Don't touch! Only for system.</summary>
    internal static class DevForEditor
    {
        /// <summary>Don't touch! Only for system.</summary>
        internal static string FormatMemorySize(int bytes) => Dev.FormatMemorySize(bytes);

        /// <summary>Don't touch! Only for system.</summary>
        internal static class TaskMonitorAPI
        {
            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void FetchAutoCount(ref int count) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void FetchManualCount(ref int count) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void FetchLateCount(ref int count) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void FetchFixedCount(ref int count) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void ExtractOwner(ref Component owner, Story.Task task) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void GetOrder(ref long offset, Story.Task task) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void ExtractCreationTrace(ref string trace, Story.Task task) {}

            /// <summary>Don't touch! Only for system.</summary>
            [Conditional("DUMMY")] internal static void GetTaskList(List<Story.Task> outTasks) {}
        }
    }

#else // (STORY_NO_DEBUG) == false

    /// <summary>Don't touch! Only for system.</summary>
    internal static class DevForEditor
    {
        /// <summary>Don't touch! Only for system.</summary>
        internal static string FormatMemorySize(int bytes) => Dev.FormatMemorySize(bytes);

        /// <summary>Don't touch! Only for system.</summary>
        internal static class TaskMonitorAPI
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchAutoCount(ref int count) => count = TaskManager.Shared.TopCountForDebug(0);

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchManualCount(ref int count) => count = TaskManager.Shared.TopCountForDebug();

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchLateCount(ref int count) => count = TaskManager.Shared.TopCountForDebug(1);

            /// <summary>Don't touch! Only for system.</summary>
            internal static void FetchFixedCount(ref int count) => count = TaskManager.Shared.TopCountForDebug(2);

            /// <summary>Don't touch! Only for system.</summary>
            internal static void ExtractOwner(ref Component owner, Story.Task task) => owner = task.Info().Owner;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void GetOrder(ref long offset, Story.Task task) => offset = task.Info2().SortKeyForDebug;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void ExtractCreationTrace(ref string trace, Story.Task task) => trace = task.Info2().CreationTraceForDebug;

            /// <summary>Don't touch! Only for system.</summary>
            internal static void GetTaskList(List<Story.Task> outTasks)
            {
                var self = TaskManager.Shared;
                outTasks.Clear();
                for (var i=0; i<self.TopCountForDebug(); ++i)
                {
                    var index = self.TopIndexForDebug(i);
                    TaskListAdd(outTasks, index);
                }
                var bandCount = self.BandCountForDebug();
                for (var j=0; j<bandCount; ++j)
                {
                    var topCount = self.TopCountForDebug(j);
                    for (var i=0; i<topCount; ++i)
                    {
                        var index = self.TopIndexForDebug(j, i);
                        TaskListAdd(outTasks, index);
                    }
                }
            }
            static void TaskListAdd(List<Story.Task> outTasks, int index)
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
                    outTasks.Add(Story.Task.UnsafeCreate(index));
                    index = info2.Next;
                    info = ref pool.UnsafeGet(index);
                } while (!info.IsTop);
            }
        }
    }

#endif // STORY_NO_DEBUG

#endif // (FOR_DEBUG && !STORY_NO_DEBUG) || UNITY_EDITOR

// --------------------------------------------------------------------------------------------------------------------
// 開発向け機能

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG

    internal class Dev : UnityEngine.Debug
    {
        /// <summary></summary>
        internal static bool IsEnableAssert = true;

        /// <summary>Enables recording of task creation stack traces for debugging.</summary>
        internal static bool EnableTaskTracking = false;

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
        internal static StringBuilder ToString(StringBuilder sb, StateMachine.StateMachinePool pool)
        {
            if (pool is IPoolMonitorForDebug monitor)
            {
                var name = monitor.PoolName;
                var start = name.IndexOf("]");
                if (0 < start)
                {
                    start++;
                    return sb.Append(name, start, name.Length - start);
                }
                return sb.Append(name);
            }
            return sb.Append(Messages.DebugInfo.TypeUnknown);
        }

        /// <summary>Formats the entire active or historical state configuration of a task handle into a comprehensive debug string.</summary>
        internal static string ToString(Story.Task self)
        {
            ref var info = ref self.Info();
            ref var info2 = ref self.Info2();

            var prevIndex = info.IsTop ? -1 : info2.Prev;
            var nextIndex = info2.Next;
            if (0 <= nextIndex &&
                Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(nextIndex).IsTop) { nextIndex = -1; }

            var sb = new StringBuilder(256);
            GetBandAndOffset(sb, ref info, ref info2);
            sb.Append(" | ");
            ToDebugString(sb, self.Id.Index);
            sb.Append(" [");
            sb.Append(self.Id.Age);
            sb.Append("] | ");
            ToDebugString(sb, prevIndex);
            sb.Append(" : ");
            ToDebugString(sb, nextIndex);
            sb.Append(" | ");
            if (info.IsValid) { info.GetMethodName(sb); }
            else { sb.AppendFormat(Messages.DebugInfo.InvalidTask, self.Id.Index, self.Id.Age); }
            sb.Append(" | @");
            sb.Append(GetOwnerName(ref info));
            return sb.ToString();
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Type<T> { internal static string Name = GetTypeName(new StringBuilder(256), typeof(T)).ToString(); }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Pool<T> { internal static string Name = string.Format("[Pool] {0}", Type<T>.Name); }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class Pool<HOT, COOL> { internal static string Name = string.Format("[Pool2] {0} / {1}", Type<HOT>.Name, Type<COOL>.Name); }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class HiddenPool<T> { internal static string Name = string.Format("[Hidden] {0}", Type<T>.Name); }

        /// <summary>Don't touch! Only for system.</summary>
        internal static class StateMachinePool<S> { internal static string Name = string.Format("[StateMachine] {0}", Type<S>.Name); }

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

        /// <summary>Don't touch! Only for system.</summary>
        [Conditional("DUMMY")] internal static void AssertIsTrue(bool condition, string message) {}

        // for debug only

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
        static StringBuilder GetTypeName(StringBuilder sb, Type type)
        {
            if (sb == null) { return null; }

            if (type == null) { return sb.Append(Messages.DebugInfo.TypeUnknown); }

            var name = type.Name;
            var declaringType = type.DeclaringType;

            // 親クラス名（DeclaringType）があれば先に書き込む
            if (declaringType != null) { sb.Append(declaringType.Name).Append('.'); }

            // ジェネリックの型引数カウント (`1 など) を除去した有効な長さを計算
            var len = name.Length;
            var backtickIndex = name.IndexOf('`');
            if (0 < backtickIndex) { len = backtickIndex; }

            // コンパイラ生成クラス（ステートマシン）のパース
            if (name.StartsWith("<"))
            {
                // ローカル関数の場合: <<Caller>g__LocalFunction|0_0>d
                var gIndex = name.IndexOf(">g__");
                if (0 < gIndex && gIndex < len)
                {
                    var pipeIndex = name.IndexOf('|', gIndex);
                    if (0 < pipeIndex && pipeIndex < len)
                    {
                        var start = gIndex + 4;
                        var count = pipeIndex - start;
                        return sb.Append(name, start, count);
                    }
                }
                
                // 通常の非同期メソッドの場合: <MethodName>d__0
                var dIndex = name.IndexOf(">d__");
                if (0 < dIndex && dIndex < len)
                {
                    var count = dIndex - 1;
                    return sb.Append(name, 1, count);
                }
            }

            // 通常の型名、またはパース条件から外れた場合はそのまま追加
            return sb.Append(name, 0, len);
        }
        static string GetOwnerName(ref TaskInfo info)
        {
            if (info.IsPinned) { return Messages.DebugInfo.StatePinned; }
            if (info.Owner is null) { return Messages.DebugInfo.OwnerNull; }
            if (info.ShouldCancel) { return Messages.DebugInfo.StateDead; }
            return info.Owner.name;
        }
        static readonly string[] bandLabels = {"Manual | ", "Auto | ", "Late | ", "Fixed | ", "(3) | ", "(4) | ", "(5) | ", "(6) | "};
        static StringBuilder GetBandAndOffset(StringBuilder sb, ref TaskInfo info, ref TaskInfo2 info2)
        {
            var offset = info.Offset;
            if (offset < 0)
            {
                var pool = Story.Pool<TaskInfo, TaskInfo2>.Shared;
Dev.LoopBreak.Init();
                while (offset < 0)
                {
Dev.LoopBreak.Check("bad");
                    var index = info2.Prev;
                    if (index < 0)
                    {
                        sb.Append("_ | (_)");
                        return sb;
                    }
                    info2 = ref pool.UnsafeGet2(index);
                    offset = pool.UnsafeGet(index).Offset;
                }
            }
            var band = (offset & TaskManager.BAND_TYPE_MASK) >> TaskManager.BAND_TYPE_SHIFT;
            if (band < 0) { sb.Append("_ | "); }
            else { sb.Append(bandLabels[band]); }
            if (!info.IsTop) { sb.Append("("); }
            sb.Append(offset & ~TaskManager.BAND_TYPE_MASK);
            if (!info.IsTop) { sb.Append(")"); }
            return sb;
        }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static StringBuilder ToDebugString(StringBuilder sb, int num) => num < 0 ? sb.Append("_") : sb.Append(num);
    }

#else // (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG == false

    /// <summary>Don't touch! Only for system.</summary>
    internal class Dev : Debug
    {
        internal static bool IsEnableAssert = false;

        internal static bool EnableTaskTracking { get => false; set {} }
        [Conditional("DUMMY")] internal static void PoolMonitorRegister(object monitor) {}
        [Conditional("DUMMY")] internal static void ValidateAwaiter<T>() {}

        internal static class LoopBreak
        {
            [Conditional("DUMMY")] internal static void Init() {}
            [Conditional("DUMMY")] internal static void Check(string str) {}
        }

        internal static StringBuilder ToString(StringBuilder sb, StateMachine.IStateMachinePool pool) => null;

#if FOR_DEBUG || UNITY_EDITOR

        internal static string ToString(Story.Task self) => string.Empty;
        internal static class Type<T> { internal static string Name = string.Empty; }
        internal static class Pool<T> { internal static string Name = string.Empty; }
        internal static class Pool<HOT, COOL> { internal static string Name = string.Empty; }
        internal static class HiddenPool<T> { internal static string Name = string.Empty; }
        internal static class StateMachinePool<S> { internal static string Name = string.Empty; }

#endif // FOR_DEBUG || UNITY_EDITOR

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

#if STORY_FAST || STORY_NO_DEBUG // 製品版でパフォーマンスを重視したいとき

        [Conditional("DUMMY")] internal static void Assert(bool condition, string message) {}
        [Conditional("DUMMY")] internal static void Assert(bool condition) {}

#if STORY_NO_DEBUG

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogException(System.Exception exception) => UnityEngine.Debug.LogException(exception);
        internal static void AssertIsTrue(bool condition, string message) => NUnit.Framework.Assert.IsTrue(condition, message);

#else // (STORY_NO_DEBUG) == false

        [Conditional("DUMMY")] internal static void LogException(System.Exception exception) {}

#endif // STORY_NO_DEBUG

#else // (STORY_FAST || STORY_NO_DEBUG) == false 製品版でリスクヘッジしたいとき

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Assert(bool condition, string message) { if (!condition) { throw new Exception(message); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void Assert(bool condition) { if (!condition) { throw new Exception(); } }
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static void LogException(System.Exception exception) => UnityEngine.Debug.LogException(exception);

#endif // STORY_FAST || STORY_NO_DEBUG

        [Conditional("DUMMY")] internal static void Log(object message) {}
        [Conditional("DUMMY")] internal static void LogWarning(object message) {}
    }

#endif // (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG

}
