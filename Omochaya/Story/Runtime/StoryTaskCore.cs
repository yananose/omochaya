// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTaskCcore.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Provides the custom async method builders and low-level execution context wiring 
//   necessary to enable zero-allocation async/await operations within the framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
    using System.Reflection;
    using System.Runtime.CompilerServices;

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // builder

    /// <summary>Don't touch! Only for system.</summary>
    public struct TaskMethodBuilder
    {
        // fields
        Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskMethodBuilder Create() => default;

        /// <summary>Don't touch! Only for system.</summary>
        public void Start<S>(ref S s)
            where S : struct, IAsyncStateMachine
        {
            this.task = TaskManager.Shared.Entry(StateMachine.Alloc(in s)); // 常にプールする。AwaitOnCompleted 内でやると直前のステートマシンのコピーを避けられないため。
        }

        /// <summary>Don't touch! Only for system.</summary>
        public Story.Task Task => this.task;

        /// <summary>Don't touch! Only for system.</summary>
        public void SetResult() => TaskManager.Shared.SetResult();

        /// <summary>Don't touch! Only for system.</summary>
        public void SetException(Exception e) => TaskManager.Shared.SetException(e);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<A, S>(ref A a, ref S s)
            where A : INotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<A, S>(ref A a, ref S s)
            where A : ICriticalNotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        public void SetStateMachine(IAsyncStateMachine s) { }
    }

    /// <summary>Don't touch! Only for system.</summary>
    public struct TaskMethodBuilder<R>
    {
        // fields
        Story.Task<R> task;

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskMethodBuilder<R> Create() => default;

        /// <summary>Don't touch! Only for system.</summary>
        public void Start<S>(ref S s)
            where S : struct, IAsyncStateMachine
        {
            this.task = TaskManager.Shared.Entry<R>(StateMachine.Alloc(in s));
        }

        /// <summary>Don't touch! Only for system.</summary>
        public Story.Task<R> Task => this.task;

        /// <summary>Don't touch! Only for system.</summary>
        public void SetResult(R result) => TaskManager.Shared.SetResult(result);

        /// <summary>Don't touch! Only for system.</summary>
        public void SetException(Exception e) => TaskManager.Shared.SetException(e);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitOnCompleted<A, S>(ref A a, ref S s)
            where A : INotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void AwaitUnsafeOnCompleted<A, S>(ref A a, ref S s)
            where A : ICriticalNotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        public void SetStateMachine(IAsyncStateMachine s) { }
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // ステートマシン
    // UnsafePool相当。コードブロートを軽減するための独自定義。

    /// <summary>Don't touch! Only for system.</summary>
    internal readonly struct StateMachine
    {
        // fields
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        /// <summary>Don't touch! Only for system.</summary>
        internal
#endif
        readonly IStateMachinePool pool;
        readonly int index;

        // interfaces

        /// <summary>Don't touch! Only for system.</summary>
        internal interface IStateMachinePool
        {
            void Free(int index);
            void MoveNext(int index);
            void Expand(int length);
        }

        // constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        StateMachine(IStateMachinePool pool, int index) { this.pool = pool; this.index = index; }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Free() => this.pool.Free(this.index);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MoveNext() => this.pool.MoveNext(this.index);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Expand(int length) => this.pool.Expand(length);

        // ↓↓↓↓↓↓ ここからステートマシンのジェネリクスによるコードブロート対象 ↓↓↓↓↓↓

        // creators

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // ジェネリクスによるコードブロート防止のため明示的にインライン化しない
        internal static StateMachine Alloc<S>(in S value) where S : struct, IAsyncStateMachine
        {
            var pool = StateMachinePool<S>.Shared;
            return new StateMachine(pool, pool.Alloc(value));
        }

        // inner classes

        class StateMachinePool<S> : IStateMachinePool
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            , IPoolMonitorForDebug
#endif
            where S : struct, IAsyncStateMachine
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal static readonly StateMachinePool<S> Shared;

            static StateMachinePool()
            {
                Shared = new StateMachinePool<S>();
#if !STORY_NO_PRE_CAPACITY
                var capacity = GetCapacity(typeof(S));
                if (0 < capacity) { Shared.Expand(capacity); }
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                else if (capacity < 0) { Dev.LogWarning($"Pool capacity initialization failed for {typeof(S).Name}"); }
#endif
#endif
            }

            // fields
            Core core;
            S[] array;

            StateMachinePool() => Dev.PoolMonitorRegister(this);

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            internal int Alloc(in S value)
            {
                if (this.core.IsUsedUp)
                {
                    var count = this.core.ExpandAtAlloc(Unsafe.SizeOf<S>());
                    if (this.array == null) { this.array = new S[count]; }
                    else
                    {
                        Dev.Assert(this.array.Length < count);
                        Dev.SetInt(this.array.Length);
                        System.Array.Resize(ref this.array, count);
                        Dev.LogWarning(string.Format(Messages.Warnings.ArrayExpanded_StateMachine, Dev.GetInt(), array.Length, Dev.FormatMemorySize(Unsafe.SizeOf<S>() * array.Length), typeof(S).Name));
                    }
                }

                var ret = this.core.Alloc();
                this.array[ret] = value;
                return ret;
            }

            // for IStateMachinePool

            /// <summary>Don't touch! Only for system.</summary>
            public void Free(int index)
            {
                this.array[index] = default;
                this.core.Free(index);
            }

            /// <summary>Don't touch! Only for system.</summary>
            public void MoveNext(int index)
            {
                var array = this.array;
                array[index].MoveNext();
                if (this.array != array) { this.array[index] = array[index]; } // 配列拡張時に新しい配列へ情報を反映
            }

            /// <summary>Don't touch! Only for system.</summary>
            public void Expand(int length)
            {
                if (this.core.Expand(length))
                {
                    if (this.array == null) { this.array = new S[length]; }
                    else { System.Array.Resize(ref this.array, length); }
                }
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public string PoolName => Dev.StateMachinePool<S>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            public int ActiveCount => this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int WorstCount { get; set; }
            /// <summary>Don't touch! Only for system.</summary>
            public int FreeCount => this.core.TotalCount - this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int TotalBytes => this.core.ArraySize + Unsafe.SizeOf<S>() * (this.array?.Length ?? 0) + Unsafe.SizeOf<StateMachinePool<S>>();
#endif
        }

        // ↑↑↑↑↑↑ ここまでステートマシンのジェネリクスによるコードブロート対象 ↑↑↑↑↑↑

        /// <summary>Don't touch! Only for system.</summary>
        // StateMachinePool用のコア機能
        internal struct Core
        {
            // fields
            int[] nextFree;
            int freeHead;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            int useCount;
#endif

            // properties

            /// <summary>Don't touch! Only for system.</summary>
            internal readonly bool IsUsedUp
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.nextFree == null || this.freeHead == -1;
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            internal int ActiveCount => this.useCount;
            /// <summary>Don't touch! Only for system.</summary>
            internal int TotalCount => this.nextFree?.Length ?? 0;
            /// <summary>Don't touch! Only for system.</summary>
            internal int ArraySize => TotalCount * Unsafe.SizeOf<int>();
#endif

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal int Alloc()
            {
                var index = this.freeHead;
                this.freeHead = this.nextFree[index];

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                this.useCount++;
#endif

                return index;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal void Free(int index)
            {
                this.nextFree[index] = this.freeHead;
                this.freeHead = index;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                this.useCount--;
#endif

            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // コードブロートの影響を抑えるため明示的にインライン化しない
            internal int ExpandAtAlloc(int itemSize)
            {
                if (this.nextFree != null && this.freeHead != -1) { return 0; }

                int oldLength;
                if (this.nextFree == null)
                {
                    oldLength = 0;
                    this.freeHead = -1;
                    Story.Pool.CreateBasedOnItemSize(ref this.nextFree, itemSize);
                }
                else
                {
                    oldLength = this.nextFree.Length;
                    Story.Pool.ExpandBasedOnItemSize(ref this.nextFree, itemSize);
                }

                MakeLink(oldLength);

                return this.nextFree.Length;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // コードブロートの影響を抑えるため明示的にインライン化しない
            internal bool Expand(int length)
            {
                if (this.nextFree == null)
                {
                    this.freeHead = -1;
                    this.nextFree = new int[length];
                    MakeLink(0);
                    return true;
                }

                var oldLength = this.nextFree.Length;
                if (length <= oldLength)
                {
                    Dev.LogWarning(string.Format(Messages.Warnings.ExpandOnly, oldLength, length));
                    return false;
                }

                System.Array.Resize(ref this.nextFree, length);
                MakeLink(oldLength);

                return true;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void MakeLink(int oldLength)
            {
                var newLength = this.nextFree.Length;
                for (var i = oldLength; i < newLength-1; ++i) { this.nextFree[i] = i + 1; }
                this.nextFree[newLength - 1] = this.freeHead;
                this.freeHead = oldLength;
            }
        }

        static int GetCapacity(Type stateMachineType)
        {
            try
            {
                var declaringType = stateMachineType.DeclaringType;
                if (declaringType == null) { return -1; }


                var smName = stateMachineType.Name;
                if (!smName.StartsWith("<")) { return -1; }
                var index = smName.IndexOf('>');
                if (index < 3) { return -1; }
                var targetMethodName = smName.Substring(1, index - 1);
                var methods = declaringType.GetMethods(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.NonPublic | System.Reflection.BindingFlags.Instance | System.Reflection.BindingFlags.Static);
                foreach (var method in methods)
                {
                    if (method.Name != targetMethodName) continue;
                    var asyncAttr = method.GetCustomAttribute<System.Runtime.CompilerServices.AsyncStateMachineAttribute>();
                    if (asyncAttr == null) { continue; }
                    if (asyncAttr.StateMachineType != stateMachineType) { continue; }
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

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // awaiter

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct Awaiter : INotifyCompletion
    {
        /// <summary>Don't touch! Only for system.</summary>
        readonly Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return !TaskManager.Shared.IsNotCompleted(this.task);
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void GetResult() => TaskManager.Shared.GetResult();

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnCompleted(Action continuation) { }
    }

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct Awaiter<R> : INotifyCompletion
    {
        /// <summary>Don't touch! Only for system.</summary>
        readonly Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return !TaskManager.Shared.IsNotCompleted(this.task);
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R GetResult() => TaskManager.Shared.GetResult<R>();

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // yield

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct YieldCore : INotifyCompletion
    {
        readonly int bandNo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal YieldCore(int bandNo) { this.bandNo = bandNo; }

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
                TaskManager.Shared.LastAwaitBandNo = this.bandNo;
                return false;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetResult() => TaskManager.Shared.GetResult();

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YieldCore GetAwaiter() => this;
    }

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct VoidCore : INotifyCompletion
    {
        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
                return true;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetResult() { }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VoidCore GetAwaiter() => this;
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // enumerator

    /// <summary>Don't touch! Only for system.</summary>
    public struct TaskEnumerator
    {
        Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TaskEnumerator(Story.Task task)
        {
            this.task = task;

            // 事前計算（とりあえず Keep だけ。ToDo.他に高速化できる余地があるかは後で考える）
            ref var info = ref task.Info();
            Dev.Assert(info.IsValid);
            TaskManager.Shared.TryKeep(ref info);
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly object Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => null;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => TaskManager.Shared.MoveNext(this.task);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Dispose() => this.task.Stop();
    }

/*

[CompilerGenerated]
private sealed class <<Main>$>d__0 : IAsyncStateMachine
{
	public int <>1__state;

	public AsyncTaskMethodBuilder <>t__builder;

	public string[] args;

	private TaskAwaiter <>u__1;

	private void MoveNext()
	{
		int num = <>1__state;
		try
		{
			TaskAwaiter awaiter;
			if (num != 0)
			{
				awaiter = Task.Delay(100).GetAwaiter();
				if (!awaiter.IsCompleted)
				{
					num = (<>1__state = 0);
					<>u__1 = awaiter;
					<<Main>$>d__0 stateMachine = this;
					<>t__builder.AwaitUnsafeOnCompleted(ref awaiter, ref stateMachine);
					return;
				}
			}
			else
			{
				awaiter = <>u__1;
				<>u__1 = default(TaskAwaiter);
				num = (<>1__state = -1);
			}
			var ret = awaiter.GetResult();
			Console.WriteLine("waited:" + ret);
		}
		catch (Exception exception)
		{
			<>1__state = -2;
			<>t__builder.SetException(exception);
			return;
		}
		<>1__state = -2;
		<>t__builder.SetResult();
	}

	void IAsyncStateMachine.MoveNext()
	{
		//ILSpy generated this explicit interface implementation from .override directive in MoveNext
		this.MoveNext();
	}

	[DebuggerHidden]
	private void SetStateMachine(IAsyncStateMachine stateMachine)
	{
	}

	void IAsyncStateMachine.SetStateMachine(IAsyncStateMachine stateMachine)
	{
		//ILSpy generated this explicit interface implementation from .override directive in SetStateMachine
		this.SetStateMachine(stateMachine);
	}
}

*/
}
