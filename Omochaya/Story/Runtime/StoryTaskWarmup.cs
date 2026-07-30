// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskWarmup.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides scoped initialization mechanisms and extension methods to safely pre-allocate 
//   state machine pool capacities, avoiding runtime expansion overhead.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Omochaya
{
    using System.Runtime.CompilerServices;
    using HiddenStory;

    public static partial class Story
    {
        /// <summary>Enters a disposable warmup scope to safely pre-allocate pool capacities for tasks initialized within the block.</summary>
        public static TaskWarmupper WarmupMode() => TaskWarmup.Create();

        /// <summary>Configures and expands the capacity of the global task pools.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warmup(int count) => TaskManager.Shared.Custom(DEFAULT_BAND_COUNT, count);

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this task.</summary>
        public static void Warmup(this Task self, int count = 0)
        {
            if (TaskWarmup.IsValid)
            {
                Dev.Assert(!self.IsValid, string.Format(Messages.Exceptions.CannotWarmupAllocatedPool, self));
                TaskWarmup.Shared.Warmup(count);
            }
            else
            {
                ref var info = ref self.Info();
                Dev.Assert(info.IsValid, Messages.Exceptions.MustWarmupInBlock);
                Dev.LogWarning(string.Format(Messages.Warnings.ResizingAllocatedPool, self));
                info.Warmup(count);
            }
        }

        /// <summary>Configures the capacity limits for the state machine pool associated with this task during the warmup phase.</summary>
        public static void Custom(this Task self, int createLimitSize = Story.Pool.CREATE_LIMIT_SIZE, int expandLimitSize = Story.Pool.EXPAND_LIMIT_SIZE)
        {
            Dev.Assert(TaskWarmup.IsValid);
            TaskWarmup.Shared.Custom(createLimitSize, expandLimitSize);
        }
    }
}

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    internal static class TaskWarmup
    {
        static TaskWarmupper shared;

        internal static TaskWarmupper Shared => TaskWarmup.shared;

        internal static bool IsValid => TaskWarmup.shared != null;

        internal static TaskWarmupper Create()
        {
            var ret = new TaskWarmupper();
            ret.Parent = TaskWarmup.shared;
            TaskWarmup.shared = ret;
            return ret;
        }

        internal static void Destroy(this TaskWarmupper self)
        {
            Dev.Assert(TaskWarmup.shared == self);
            if (self != null) { TaskWarmup.shared = self.Parent; }
        }

        internal static int GetCapacity(Type stateMachineType)
        {
            var count = GetCapacityCore(stateMachineType);
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            if (count < 0) { Dev.LogWarning(string.Format(Messages.Warnings.PoolCapacityInitFailed, stateMachineType)); }
#endif
            return count;
        }

        static int GetCapacityCore(Type stateMachineType)
        {
            try
            {
                var declaringType = stateMachineType.DeclaringType;
                if (declaringType == null) { return -1; }


                var smName = stateMachineType.Name;
                if (!smName.StartsWith("<")) { return -1; }
                var index = smName.IndexOf('>');
                if (index < 2) { return -1; }
                var targetMethodName = smName.Substring(1, index - 1);
                var methods = declaringType.GetMethods(
                    System.Reflection.BindingFlags.Public |
                    System.Reflection.BindingFlags.NonPublic |
                    System.Reflection.BindingFlags.Instance |
                    System.Reflection.BindingFlags.Static);

                var smTypeToCompare = stateMachineType.IsGenericType && !stateMachineType.IsGenericTypeDefinition
                                    ? stateMachineType.GetGenericTypeDefinition()
                                    : stateMachineType;

                // 通常の非同期メソッド
                foreach (var method in methods)
                {
                    if (method.Name != targetMethodName) continue;
                    var asyncAttr = method.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>();
                    if (asyncAttr == null) { continue; }
                    if (asyncAttr.StateMachineType != smTypeToCompare) { continue; }
                    var capacityAttr = method.GetCustomAttribute<Story.CapacityAttribute>();
                    return capacityAttr != null ? capacityAttr.Capacity : 0;
                }
                // ローカル関数等の名前が特殊な非同期メソッド
                foreach (var method in methods)
                {
                    // if (method.Name != targetMethodName) continue;
                    var asyncAttr = method.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>();
                    if (asyncAttr == null) { continue; }
                    if (asyncAttr.StateMachineType != smTypeToCompare) { continue; }
                    var capacityAttr = method.GetCustomAttribute<Story.CapacityAttribute>();
                    return capacityAttr != null ? capacityAttr.Capacity : 0;
                }

                return 0;
            }
            catch (System.Exception e)
            {
                Dev.LogWarning(e.Message);

                return -1;
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public class TaskWarmupper : IDisposable
    {

        // fields
        PoolCore pool;
        Type type;
        internal TaskWarmupper Parent;
        int size;

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Dispose() => this.Destroy();

        internal void Setup<S>() where S : struct, IAsyncStateMachine
        {
            Dev.Assert(this.pool == null, string.Format(Messages.Exceptions.MustWarmupImmediately, this.type));
            this.pool = StateMachine.StateMachinePool<S>.Shared;
            this.type = typeof(S);
            this.size = Unsafe.SizeOf<S>();
        }

        internal void Warmup(int count)
        {
            Dev.Assert(this.pool != null, Messages.Exceptions.MustSpecifyTaskInWarmupMode);
            if (this.pool.IsValid)
            {
                Dev.LogWarning(string.Format(Messages.Warnings.IgnoredWarmupForAllocatedPool, this.type));
            }
            else
            {
// #if !STORY_NO_PRE_CAPACITY // コメントアウト（避けたいのはタスク開始時のコストのはずなので事前確保時は残す）
                if (count <= 0) { count = TaskWarmup.GetCapacity(this.type); }
// #endif
                this.pool.Expand(count);
            }
            this.pool = null;
        }

        internal void Custom(int createLimitSize, int expandLimitSize)
        {
            Dev.Assert(this.pool != null, Messages.Exceptions.MustSpecifyTaskInWarmupMode);
            this.pool.Custom(createLimitSize, expandLimitSize);
            this.pool = null;
        }
    }
}
