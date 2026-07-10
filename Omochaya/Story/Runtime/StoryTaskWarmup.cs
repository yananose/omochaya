// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskWarmup.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------

namespace Omochaya
{
    using HiddenStory;

    public static partial class Story
    {
        /// <summary></summary>
        public static TaskWarmuper WarmupMode() => TaskWarmup.Create();

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this task.</summary>
        public static void Warmup(this Task self, int count = 0)
        {
            if (TaskWarmup.IsValid)
            {
                Dev.Assert(!self.IsValid, $"using(WarmupMode()){{}} 内では割り当て済みのプールを Warmup() できません : {self}");
                TaskWarmup.Shared.Complete(count);
            }
            else
            {
                ref var info = ref self.Info();
                Dev.Assert(info.IsValid, "using(WarmupMode()){} 内で作成したタスクはブロック内で Warmup() してください");
                Dev.LogWarning($"割り当て済みのプールをリサイズします。初期容量を指定したい場合は using(WarmupMode()){{}} 内で Warmup() してください : {self}");
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
            if (count < 0) { Dev.LogWarning(string.Format("Pool capacity initialization failed for {0}", stateMachineType)); }
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
            Dev.Assert(this.pool == null, $"using(WarmupMode()){{}} 内で作成したタスクは Warmup() 以外の目的で使用することはできません。即座に Warmup() してください : {this.type}");
            this.pool = StateMachine.StateMachinePool<S>.Shared;
            this.type = typeof(S);
            this.size = Unsafe.SizeOf<S>();
        }

        /// <summary>Don't touch! Only for system.</summary>
        internal void Complete(int count)
        {
            Dev.Assert(this.pool != null, "Warmupモード中に作成したタスクを指定してください");
            if (this.pool.Core.IsValid)
            {
                Dev.LogWarning($"割り当て済みのプールを Warmup() しようとしたので無視します : {this.type}");
            }
            else
            {
// #if !STORY_NO_PRE_CAPACITY // コメントアウト（避けたいのはタスク開始時のコストのはずなので事前確保時は残す）
                if (count <= 0) { count = TaskWarmup.GetCapacity(this.type); }
// #endif
                if (count <= 0) { count = Story.Pool.GetNeedCountAtCreate(this.size); }
                this.pool.Expand(count);
            }
            this.pool = null;
        }
    }
}
