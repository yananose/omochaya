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
    using HiddenStory;

    public static partial class Story
    {
        /// <summary>Enters a disposable warmup scope to safely pre-allocate pool capacities for tasks initialized within the block.</summary>
        public static TaskWarmuper WarmupMode() => TaskWarmup.Create();

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this task.</summary>
        public static void Warmup(this Task self, int count = 0)
        {
            if (TaskWarmup.IsValid)
            {
                Dev.Assert(!self.IsValid, string.Format(Messages.Exceptions.CannotWarmupAllocatedPool, self));
                TaskWarmup.Shared.Complete(count);
            }
            else
            {
                ref var info = ref self.Info();
                Dev.Assert(info.IsValid, Messages.Exceptions.MustWarmupInBlock);
                Dev.LogWarning(string.Format(Messages.Warnings.ResizingAllocatedPool, self));
                info.Warmup(count);
            }
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

    /// <summary>Don't touch! Only for system.</summary>
    internal static class TaskWarmup
    {
        static TaskWarmuper shared;

        /// <summary>Don't touch! Only for system.</summary>
        internal static TaskWarmuper Shared => TaskWarmup.shared;

        /// <summary>Don't touch! Only for system.</summary>
        internal static bool IsValid => TaskWarmup.shared != null;

        /// <summary>Don't touch! Only for system.</summary>
        internal static TaskWarmuper Create()
        {
            var ret = new TaskWarmuper();
            ret.Parent = TaskWarmup.shared;
            TaskWarmup.shared = ret;
            return ret;
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal static void Destroy(this TaskWarmuper self)
        {
            Dev.Assert(TaskWarmup.shared == self);
            if (self != null) { TaskWarmup.shared = self.Parent; }
        }

        /// <summary>Don't touch! Only for system.</summary>
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
    public class TaskWarmuper : IDisposable
    {

        // fields
        StateMachine.IStateMachinePool pool;
        Type type;
        internal TaskWarmuper Parent;
        int size;

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        public void Dispose() => this.Destroy();

        /// <summary>Don't touch! Only for system.</summary>
        internal void Setup<S>() where S : struct, IAsyncStateMachine
        {
            Dev.Assert(this.pool == null, string.Format(Messages.Exceptions.MustWarmupImmediately, this.type));
            this.pool = StateMachine.StateMachinePool<S>.Shared;
            this.type = typeof(S);
            this.size = Unsafe.SizeOf<S>();
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal void Complete(int count)
        {
            Dev.Assert(this.pool != null, Messages.Exceptions.MustSpecifyTaskInWarmupMode);
            if (this.pool.Func(StateMachine.FuncType.IsValid, 0))
            {
                Dev.LogWarning(string.Format(Messages.Warnings.IgnoredWarmupForAllocatedPool, this.type));
            }
            else
            {
// #if !STORY_NO_PRE_CAPACITY // コメントアウト（避けたいのはタスク開始時のコストのはずなので事前確保時は残す）
                if (count <= 0) { count = TaskWarmup.GetCapacity(this.type); }
// #endif
                if (count <= 0) { count = Story.Pool.GetNeedCountAtCreate(this.size); }
                this.pool.Func(StateMachine.FuncType.Expand, count);
            }
            this.pool = null;
        }
    }
}
