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
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public struct TaskMethodBuilder
    {
        // fields
        Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TaskMethodBuilder Create() => default;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Start<S>(ref S s)
            where S : struct, IAsyncStateMachine
        {
            if (TaskWarmup.IsValid) { TaskWarmup.Shared.Setup<S>(); }
            else { this.task = TaskManager.Shared.Entry(StateMachine.Alloc(in s)); } // 遅延起動のため常にプールする
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Story.Task Task => this.task;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetResult() => TaskManager.Shared.SetResult();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetException(Exception e) => TaskManager.Shared.SetException(e);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 消してほしいので
        public void AwaitOnCompleted<A, S>(ref A a, ref S s)
            where A : INotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 消してほしいので
        public void AwaitUnsafeOnCompleted<A, S>(ref A a, ref S s)
            where A : ICriticalNotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetStateMachine(IAsyncStateMachine s) { }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public struct TaskMethodBuilder<R>
    {
        // fields
        Story.Task<R> task;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public static TaskMethodBuilder<R> Create() => default;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void Start<S>(ref S s)
            where S : struct, IAsyncStateMachine
        {
            if (TaskWarmup.IsValid) { TaskWarmup.Shared.Setup<S>(); }
            else { this.task = TaskManager.Shared.Entry<R>(StateMachine.Alloc(in s)); } // 遅延起動のため常にプールする
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public Story.Task<R> Task => this.task;

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetResult(R result) => TaskManager.Shared.SetResult(result);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetException(Exception e) => TaskManager.Shared.SetException(e);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 消してほしいので
        public void AwaitOnCompleted<A, S>(ref A a, ref S s)
            where A : INotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // 消してほしいので
        public void AwaitUnsafeOnCompleted<A, S>(ref A a, ref S s)
            where A : ICriticalNotifyCompletion
            where S : IAsyncStateMachine
            => Dev.ValidateAwaiter<A>();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public void SetStateMachine(IAsyncStateMachine s) { }
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // ステートマシン
    // UnsafePool相当。コードブロートを軽減するための独自定義。

    readonly struct StateMachine
    {
        // inner classes
        internal abstract class StateMachinePool : PoolCore
        {
            internal abstract void MoveNext(int index);
        }
 
        // fields
#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
        internal
#endif
        readonly StateMachinePool pool;
        readonly int index;

        // constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        StateMachine(StateMachinePool pool, int index) { this.pool = pool; this.index = index; }

        // methods

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        internal void Warmup(int count) => this.pool.Expand(count);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        internal void Free() => this.pool.UnsafeFree(this.index);

        [MethodImpl(MethodImplOptions.AggressiveInlining)] // サイズが小さい
        internal void MoveNext() => this.pool.MoveNext(this.index);

// ↓↓↓↓↓↓ ここからステートマシンのジェネリクスによるコードブロート対象 ↓↓↓↓↓↓

        // creators

        // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
        internal static StateMachine Alloc<S>(in S value) where S : struct, IAsyncStateMachine
        {
            var pool = StateMachinePool<S>.Shared;
            return new StateMachine(pool, pool.Alloc(value));
        }

        // inner classes

        internal class StateMachinePool<S> : StateMachinePool
            where S : struct, IAsyncStateMachine
        {
            internal static readonly StateMachinePool<S> Shared = new();
            StateMachinePool() {}

            // fields
            protected S[] array;

            // methods

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 仮想メソッド
            protected override int ExpandArray(int count)
            {
                if (count <= 0) { count = GetNeedCount(base.ItemSize + Unsafe.SizeOf<S>()); }
                Story.Pool.Expand(ref this.array, count);
                return count;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)] // StateMachine.Alloc からしか呼ばれないので
            internal int Alloc(in S value)
            {
#if !STORY_NO_PRE_CAPACITY
                if (this.array == null)
                {
                    var count = TaskWarmup.GetCapacity(typeof(S));
                    if (0 < count) { Expand(count); }
                }
#endif
                var index = Alloc();
                this.array[index] = value;
                return index;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public override void UnsafeFree(int index)
            {
                base.UnsafeFree(index);
                this.array[index] = default;
            }

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // 仮想メソッド
            internal override void MoveNext(int index)
            {
                var array = this.array;
                array[index].MoveNext();
                if (this.array != array) { this.array[index] = array[index]; } // 配列拡張時に新しい配列へ情報を反映
            }

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override string PoolName => Dev.StateMachinePool<S>.Name;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public override int TotalBytes => (base.ItemSize + Unsafe.SizeOf<S>()) * Length + Unsafe.SizeOf<StateMachinePool<S>>();
#endif

        }

// ↑↑↑↑↑↑ ここまでステートマシンのジェネリクスによるコードブロート対象 ↑↑↑↑↑↑

    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // awaiter

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public readonly struct Awaiter : INotifyCompletion
    {
        readonly Story.Task task;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return !TaskManager.Shared.IsNotCompleted(this.task);
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void GetResult() => TaskManager.Shared.GetResult();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public readonly void OnCompleted(Action continuation) { }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public readonly struct Awaiter<R> : INotifyCompletion
    {
        readonly Story.Task task;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                return !TaskManager.Shared.IsNotCompleted(this.task);
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public R GetResult() => TaskManager.Shared.GetResult<R>();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // yield

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public readonly struct YieldCore : INotifyCompletion
    {
        readonly int bandNo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal YieldCore(int bandNo) { this.bandNo = bandNo; }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetResult() => TaskManager.Shared.GetResult();

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YieldCore GetAwaiter() => this;
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public readonly struct VoidCore : INotifyCompletion
    {
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void GetResult() { }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void OnCompleted(Action continuation) { }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public VoidCore GetAwaiter() => this;
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // enumerator

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public struct TaskEnumerator
    {
        Story.Task task;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal TaskEnumerator(Story.Task task)
        {
            this.task = task;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly object Current
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => null;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public bool MoveNext() => TaskManager.Shared.MoveNext(this.task);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
