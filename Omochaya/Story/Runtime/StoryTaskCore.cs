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
            if (TaskWarmup.IsValid) { TaskWarmup.Shared.Setup<S>(); }
            else { this.task = TaskManager.Shared.Entry(StateMachine.Alloc(in s)); } // 遅延起動のため常にプールする
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
            if (TaskWarmup.IsValid) { TaskWarmup.Shared.Setup<S>(); }
            else { this.task = TaskManager.Shared.Entry<R>(StateMachine.Alloc(in s)); } // 遅延起動のため常にプールする
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

        // inner classes
        internal enum FuncType { IsValid, Expand, Free }

        // interfaces

        /// <summary>Don't touch! Only for system.</summary>
        internal interface IStateMachinePool
        {
            bool Func(FuncType funcType, int param);
            void MoveNext(int index);
        }

        // constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        StateMachine(IStateMachinePool pool, int index) { this.pool = pool; this.index = index; }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Warmup(int count) => this.pool.Func(FuncType.Expand, count);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void Free() => this.pool.Func(FuncType.Free, this.index);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal void MoveNext() => this.pool.MoveNext(this.index);

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

        /// <summary>Don't touch! Only for system.</summary>
        internal class StateMachinePool<S> : IStateMachinePool
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            , IPoolMonitorForDebug
#endif
            where S : struct, IAsyncStateMachine
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal static readonly StateMachinePool<S> Shared = new();

            // fields
            Story.PoolCore core;
            S[] array;

            // constructors
            StateMachinePool() => Dev.PoolMonitorRegister(this);

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            internal int Alloc(in S value)
            {
                int index;
                var need = 0;

#if !STORY_NO_PRE_CAPACITY
                if (this.array == null) { need = TaskWarmup.GetCapacity(typeof(S)); }

                if (0 < need)
                {
                    index = 0;
                    this.core.FirstAlloc(need);
                }
                else
#endif
                {
                    var result = this.core.AllocBasedOnItemSize(Unsafe.SizeOf<S>());
                    index = result.Index;
                    need = result.Need;
                }

                if (0 < need) { Story.Pool.Expand(ref this.array, need); }
                this.array[index] = value;
                return index;
            }

            // for IStateMachinePool

            /// <summary>Don't touch! Only for system.</summary>
            public bool Func(FuncType funcType, int param)
            {
                switch (funcType)
                {
                    case FuncType.Expand:
                        if (this.core.TryExpand(param)) { Story.Pool.Expand(ref this.array, param); }
                        return true;
                    case FuncType.Free:
                        this.core.Free(param);
                        this.array[param] = default;
                        return true;
                    default:
                        return this.core.IsValid;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            public void MoveNext(int index)
            {
                var array = this.array;
                array[index].MoveNext();
                if (this.array != array) { this.array[index] = array[index]; } // 配列拡張時に新しい配列へ情報を反映
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            public string PoolName => Dev.StateMachinePool<S>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            public int ActiveCount => this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int WorstCount => this.core.WorstCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int FreeCount => this.core.TotalCount - this.core.ActiveCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int TotalBytes => this.core.ArraySize + Unsafe.SizeOf<S>() * (this.array?.Length ?? 0) + Unsafe.SizeOf<StateMachinePool<S>>();
#endif
        }

// ↑↑↑↑↑↑ ここまでステートマシンのジェネリクスによるコードブロート対象 ↑↑↑↑↑↑

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
