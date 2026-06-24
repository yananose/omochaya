// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTask.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
//   Defines the core asynchronous task abstractions, handles, and custom awaiters 
//   for the Omochaya Story framework.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using HiddenStory;

    /*

    【目的・特徴】
    ・コルーチン(IEnumerator)から非同期タスク(async)への置き換え。
    ・遅延起動。
    ・手動での MoveNext や foreach での実行が可能。

    【前提】
    ・Story.Update() を毎フレーム実行すること。
    ・自動タスクは毎フレーム TaskManager.Update でステートマシンが MoveNext される。順番は後で登録された（自動タスクになった）ものほど先に実行される。
    ・手動タスクは任意のタイミングで MoveNext や foreach 等で実行する。
    ・awaitされたタスクはawaitしたタスクの実行位置で実行されるようになる。
    ・awaitされたタスクが存在している間はawaitしたタスクは実行されない。

    【注意点】
    ・メインスレッド限定。
    ・Story.Task で await できるのは Story.Task, Story.Yield、Story.Void のみ。
    ・他の非同期タスクで await Story.Task することはできない。
    ・Story.Task 外で Boot する場合はコンポーネントを master として指定する必要がある。
    ・await されたタスク及びBootしたタスクを await することはできない。 待つ場合は while (task.IsValid) { await Story.Yield; } すること。なお、結果を受け取ることはできない。
    ・await が終了したタスクから結果を受け取ることはできない。
    ・高速化のため、可能であれば master に指定するコンポーネントは ITaskMaster インターフェイスを実装すること。Story.TaskBehaviour を継承してもよい。
    ・Story.Task がキャンセル（Task.Free 実行 / foreach 中の beak / master 等消失による削除）された場合は finally ブロックを実行する。
    ・finally ブロックで master に指定したコンポーネントを参照してはならない。
    ・キャンセル発生以降は master を無視するので await する場合は使用者が責任を持って解放すること。
    ・子がキャンセルされても親はキャンセルされずに続きの処理を再開する。その際、親が受け取る結果は default となる。（←必ずしもそうはならない？）
    ・Extra<E> は１タスクにつき１つしか持てない。
    ・Extra<E> を ref で持ち続けることは避け、必要な時に都度取得して使用すること。

    */
    public static partial class Story
    {
        /// <summary>Drives the global task manager loop forward, executing all registered automated tasks for the current frame.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update() => TaskManager.Shared.Update();

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LateUpdate() => TaskManager.Shared.BandUpdate(1);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FixedUpdate() => TaskManager.Shared.BandUpdate(2);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BandUpdate(int bandNo) => TaskManager.Shared.BandUpdate(bandNo);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsResultError() => TaskManager.Shared.IsResultError;

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // タスク（ハンドラ）

        /// <summary>Represents a zero-allocation asynchronous task handle.</summary>
        [AsyncMethodBuilder(typeof(TaskMethodBuilder))]
        public readonly struct Task : IEquatable<Task>
        {
            // fields

            /// <summary>The unique pool identifier of this task.</summary>
            public readonly Pool.Id Id;

            // properties

            /// <summary>Gets a value indicating whether the task is unassigned or empty.</summary>
            public bool IsEmpty
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => !this.Id.IsValid; // 未割り当て
            }

            /// <summary>Gets a value indicating whether the task handle is valid and active in the pool.</summary>
            public bool IsValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => !IsEmpty && Pool<TaskInfo, TaskInfo2>.Shared.IsValid(this.Id);
            }

            /// <summary></summary>
            public bool WillCancel
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.Info().WillCancel;
            }

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Task(Pool.Id id)
            {
                this.Id = id;
            }

            // methods

            /// <summary>Determines whether this task matches the specified task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Task a) => this.Id.Matches(a.Id);

            /// <summary>Explicitly releases and cancels the task, recycling its resources.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free()
            {
                if (IsEmpty) { return; }
                TaskManager.Shared.Free(this);
            }

            /// <summary>Anchors the task to a specific master component to govern its lifecycle.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep(Component master) => this.Info().Keep(master);

            /// <summary>Anchors the task to the currently running task's master component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep()
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
                this.Info().Keep(TaskManager.Shared.GetRunningInfo().Master);
            }

            /// <summary>Anchors the task to a master component and registers it to the automation loop.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Boot(Component master)
            {
                Keep(master);
                return Boot();
            }

            /// <summary>Registers the task to the automation loop using its pre-assigned master component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Boot() => TaskManager.Shared.Boot(this);

            /// <summary>Drives the task state machine forward manually by one step.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (IsEmpty) { return false; }
                ref var info = ref this.Info();
                if (!info.IsValid) { return false; }

                // keep
                if (info.Master is null && TaskManager.Shared.IsRunningValid)
                {
                    var master = TaskManager.Shared.GetRunningInfo().Master;
                    Dev.Assert(!(master is null));
                    info.Keep(master);
                }

                return TaskManager.Shared.MoveNext(this);
            }

            /// <summary>Expands the global pool capacity for the underlying state machine type associated with this task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Expand(int length)
            {
                ref var info = ref this.Info();
                Dev.Assert(info.IsValid);
                info.Expand(length);
            }

            /// <summary>Creates a task handle directly from a raw pool index without validation.</summary>
            public static Task UnsafeCreate(int index) => new Task(Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGetId(index));

            // for awaiter（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public Awaiter GetAwaiter() => new Awaiter(this);

            // for collection（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public bool Equals(Task other) => Matches(other);

            /// <summary>Don't touch! Only for system.</summary>
            public override bool Equals(object obj) => obj is Task other && Equals(other);

            /// <summary>Don't touch! Only for system.</summary>
            public override int GetHashCode() => HashCode.Combine(Id);

            // for foreach（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public TaskEnumerator GetEnumerator() => new TaskEnumerator(this);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
            public override string ToString() => Dev.ToString(this);
#endif
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // タスク（ハンドラ）：戻り値あり版

        /// <summary>Represents a zero-allocation asynchronous task handle that yields a result of type <typeparamref name="R"/>.</summary>
        [AsyncMethodBuilder(typeof(TaskMethodBuilder<>))]
        public readonly struct Task<R> : IEquatable<Task<R>>
        {
            // fields
            readonly Task rawTask;

            // properties

            /// <summary>The unique pool identifier of this task.</summary>
            public Pool.Id Id
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.rawTask.Id;
            }

            /// <summary>Gets a value indicating whether the task is unassigned or empty.</summary>
            public bool IsEmpty
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.rawTask.IsEmpty;
            }

            /// <summary>Gets a value indicating whether the task handle is valid and active in the pool.</summary>
            public bool IsValid
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.rawTask.IsValid;
            }

            /// <summary></summary>
            public bool WillCancel
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.rawTask.WillCancel;
            }

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Task(Task naked)
            {
                this.rawTask = naked;
            }

            // methods

            /// <summary>Determines whether this task matches the specified task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Task<R> a) => this.rawTask.Matches(a.rawTask);

            /// <summary>Explicitly releases and cancels the task, recycling its resources.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free() => this.rawTask.Free();

            /// <summary>Anchors the task to a specific master component to govern its lifecycle.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep() => this.rawTask.Keep();

            /// <summary>Anchors the task to the currently running task's master component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep(Component master) => this.rawTask.Keep(master);

            /// <summary>Anchors the task to a master component and registers it to the automation loop.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Boot(Component master) => this.rawTask.Boot(master);

            /// <summary>Drives the task state machine forward manually by one step.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => this.rawTask.MoveNext();

            /// <summary>Expands the global pool capacity for the underlying state machine type associated with this task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Expand(int length) => this.rawTask.Expand(length);

            // for awaiter（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public Awaiter<R> GetAwaiter() => new Awaiter<R>(this.rawTask);

            // for collection（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public bool Equals(Task<R> other) => Matches(other);

            /// <summary>Don't touch! Only for system.</summary>
            public override bool Equals(object obj) => obj is Task<R> other && Equals(other);

            /// <summary>Don't touch! Only for system.</summary>
            public override int GetHashCode() => HashCode.Combine(rawTask);

            // for foreach（利用者による呼び出し禁止）

            /// <summary>Don't touch! Only for system.</summary>
            public TaskEnumerator GetEnumerator() => this.rawTask.GetEnumerator();
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // 偽装nullチェック回避と一時停止対応用

        /// <summary>Defines a master object that governs the lifecycle and destruction state of associated tasks.</summary>
        public interface ITaskMaster { bool IsDestroyed { get; set; } bool IsPaused { get; } }

        /// <summary>A base MonoBehaviour that implements ITaskMaster to manage tasks bound to its lifecycle.</summary>
        public class TaskBehaviour : MonoBehaviour, ITaskMaster // 実装漏れ回避用
        {
            protected virtual void OnDestroyed() {}
            protected virtual void OnEnabled() {}
            protected virtual void OnDisabled() {}

            public bool IsDestroyed { get; set; }
            public bool IsPaused { get; set; }

            // 派生クラスで OnDestroy を定義する場合は base.OnDestroy を呼び出してください。あるいは OnDestroy の代わりに OnDestroyed を定義すれば base 呼び出しは不要です。
            protected void OnDestroy()
            {
                this.IsDestroyed = true;
                OnDestroyed();
            }

            // 派生クラスで OnEnable を定義する場合は base.OnEnable を呼び出してください。あるいは OnEnable の代わりに OnEnabled を定義すれば base 呼び出しは不要です。
            protected void OnEnable()
            {
                this.IsPaused = false;
                OnEnabled();
            }

            // 派生クラスで OnDisable を定義する場合は base.OnDisable を呼び出してください。あるいは OnDisable の代わりに OnDisabled を定義すれば base 呼び出しは不要です。
            protected void OnDisable()
            {
                this.IsPaused = true;
                OnDisabled();
            }
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // extra

        /// <summary>Gets a reference to the custom extra metadata structure bound to the currently executing task.</summary>
        public static ref E GetTaskExtra<E>()
        {
            Dev.Assert(TaskManager.Shared.IsRunningValid);
            return ref TaskManager.Shared.GetRunningInfo2().GetExtra<E>();
        }

        /// <summary>Gets a reference to the custom extra metadata structure bound to this specific task handle.</summary>
        public static ref E GetExtra<E>(this Story.Task self)
        {
            ref var info2 = ref self.Info2();
            Dev.Assert(info2.IsValid, Messages.Exceptions.InvalidExtraOperation);
            return ref info2.GetExtra<E>();
        }

        /// <summary>Assigns a custom extra metadata structure directly to the specified task handle.</summary>
        public static void SetExtra<E>(this Story.Task self, in E extra)
        {
            self.GetExtra<E>() = extra;
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // others

        /// <summary>A globally accessible token to await a single-frame yield.</summary>
        public static YieldCore Yield => new(0);

        /// <summary></summary>
        public static YieldCore YieldLate => new(1);

        /// <summary></summary>
        public static YieldCore YieldFixed => new(2);

        /// <summary></summary>
        public static YieldCore YieldNo(int bandNo) => new(bandNo);

        // 即返却（await しない async メソッドの遅延実行用）
        /// <summary>A globally accessible token to await an immediate void or finished state.</summary>
        public static VoidCore Void => default;
    }
}

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
    using System.Runtime.CompilerServices;
    using System.Runtime.InteropServices;
    using UnityEngine;

    static class Extensions
    {
        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TaskInfo Info(this Story.Task self)
        {
            if (Story.Pool<TaskInfo, TaskInfo2>.Shared.IsValid(self.Id)) { return ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet(self.Id.Index); }
            else { return ref TaskInfo.Invalid; }
        }
        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static ref TaskInfo2 Info2(this Story.Task self)
        {
            if (Story.Pool<TaskInfo, TaskInfo2>.Shared.IsValid(self.Id)) { return ref Story.Pool<TaskInfo, TaskInfo2>.Shared.UnsafeGet2(self.Id.Index); }
            else { return ref TaskInfo2.Invalid; }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct YieldCore : INotifyCompletion
    {
        readonly int bandNo;

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public YieldCore(int bandNo) { this.bandNo = bandNo; }

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
    // awaiter

    /// <summary>Don't touch! Only for system.</summary>
    public readonly struct Awaiter : INotifyCompletion
    {
        /// <summary>Don't touch! Only for system.</summary>
        public readonly Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
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
        public readonly Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public Awaiter(Story.Task task) =>  this.task = task;

        // for INotifyCompletion

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsCompleted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
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
    // enumerator

    /// <summary>Don't touch! Only for system.</summary>
    public struct TaskEnumerator
    {
        Story.Task task;

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public TaskEnumerator(Story.Task task)
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
        public void Dispose() => this.task.Free();
    }

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
    public readonly struct StateMachine
    {
        // fields
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        /// <summary>Don't touch! Only for system.</summary>
        public
#endif
        readonly IStateMachinePool pool;
        readonly int index;

        // interfaces

        /// <summary>Don't touch! Only for system.</summary>
        public interface IStateMachinePool { void Free(int index); void MoveNext(int index);  void Expand(int length); }

        // constructors
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        StateMachine(IStateMachinePool pool, int index) { this.pool = pool; this.index = index; }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free() => this.pool.Free(this.index);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void MoveNext() => this.pool.MoveNext(this.index);

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int length) => this.pool.Expand(length);

        // ↓↓↓↓↓↓ ここからステートマシンのジェネリクスによるコードブロート対象 ↓↓↓↓↓↓

        // creators

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.NoInlining)] // ジェネリクスによるコードブロート防止のため明示的にインライン化しない
        public static StateMachine Alloc<S>(in S value) where S : struct, IAsyncStateMachine
        {
            var pool = StateMachinePool<S>.Shared;
            return new StateMachine(pool, pool.Alloc(value));
        }

        // inner classes

        class StateMachinePool<S> : IStateMachinePool
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
            , IPoolMonitorForDebug
#endif
            where S : struct, IAsyncStateMachine
        {
            /// <summary>Don't touch! Only for system.</summary>
            public static readonly StateMachinePool<S> Shared = new();

            // fields
            Core core;
            S[] array;

            StateMachinePool() => Dev.PoolMonitorRegister(this);

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)] // 基本的にラップされて呼び出される（つまりT毎に1箇所からしか呼び出されない）のでここはインライン展開のほうがコードサイズ的にも有利になると判断。
            public int Alloc(in S value)
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
                if (this.core.Expand(length)) { System.Array.Resize(ref this.array, length); }
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
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
        public struct Core
        {
            // fields
            int[] nextFree;
            int freeHead;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
            int useCount;
#endif

            // properties

            /// <summary>Don't touch! Only for system.</summary>
            public readonly bool IsUsedUp
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get => this.nextFree == null || this.freeHead == -1;
            }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
            /// <summary>Don't touch! Only for system.</summary>
            public int ActiveCount => this.useCount;
            /// <summary>Don't touch! Only for system.</summary>
            public int TotalCount => this.nextFree?.Length ?? 0;
            /// <summary>Don't touch! Only for system.</summary>
            public int ArraySize => TotalCount * Unsafe.SizeOf<int>();
#endif

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public int Alloc()
            {
                var index = this.freeHead;
                this.freeHead = this.nextFree[index];

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
                this.useCount++;
#endif

                return index;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Free(int index)
            {
                this.nextFree[index] = this.freeHead;
                this.freeHead = index;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
                this.useCount--;
#endif

            }

            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.NoInlining)] // コードブロートの影響を抑えるため明示的にインライン化しない
            public int ExpandAtAlloc(int itemSize)
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
            public bool Expand(int length)
            {
                Dev.Assert(this.nextFree != null);

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
    }

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // タスク（中身）

    /// <summary>Don't touch! Only for system.</summary>
    [StructLayout(LayoutKind.Sequential, Size = 32)]
    struct TaskInfo
    {
        // static

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskInfo Invalid;

        static TaskInfo()
        {
            Dev.Assert(Unsafe.SizeOf<TaskInfo>() == 32);
            Invalid.Offset = -1;
        }

        // inner classes
        [Flags] enum Flags : ushort
        {
            None = 0,
            IsValid = 1 << 0,
            IsUsing = 1 << 1,
            HasException = 1 << 2,
            IsFastMaster = 1 << 3,
            IsPinned = 1 << 4,
            IsRunning = 1 << 5,
            WillCancel = 1 << 6,
        }

        // fields
        StateMachine stateMachine; // 12 バイトしか使ってないが 16 バイトを占有
        Component master;
        public int Offset;
        Flags flags; // 1 バイトで足りるが後々のため 2 バイト

        // properties

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsValid) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            private set { if (value) { this.flags |= Flags.IsValid; } else { this.flags &= ~Flags.IsValid; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsUsing
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsUsing) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsUsing; } else { this.flags &= ~Flags.IsUsing; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool HasException
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.HasException) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.HasException; } else { this.flags &= ~Flags.HasException; } }
        }

        bool IsFastMaster
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsFastMaster) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsFastMaster; } else { this.flags &= ~Flags.IsFastMaster; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsPinned
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsPinned) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsPinned; } else { this.flags &= ~Flags.IsPinned; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsRunning
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsRunning) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsRunning; } else { this.flags &= ~Flags.IsRunning; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool WillCancel
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.WillCancel) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.WillCancel; } else { this.flags &= ~Flags.WillCancel; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public Component Master => this.master;

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool HasOffset
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => 0 <= this.Offset;
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool IsOrphaned // 偽装nullチェック
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (WillCancel) { return true; }
                if (IsPinned) { return false; }
                if (IsFastMaster) { return ((Story.ITaskMaster)this.master).IsDestroyed; }
                return this.master == null;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool IsPaused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsFastMaster) { return ((Story.ITaskMaster)this.master).IsPaused; }
                return false;
            }
        }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Entry(in StateMachine stateMachine, int offset)
        {
            this.stateMachine = stateMachine;
            IsValid = true;
            Offset = offset;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Keep(Component master)
        {
            Dev.Assert(IsValid);
            Dev.Assert(!(master is null), Messages.Exceptions.MasterCannotBeNull);
            this.master = master;
            IsFastMaster = master is Story.ITaskMaster;
            IsPinned = false; // ピン留め外してマスター依存に戻す
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            this.stateMachine.Free();
            this.stateMachine = default; // TaskManager からしか呼ばれないので不要だけど、一応 stateMachine の多重解放を予防（null参照で止める）
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Run()
        {
            IsUsing = true;
            this.stateMachine.MoveNext();
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Expand(int length) => this.stateMachine.Expand(length);

        /// <summary>Don't touch! Only for system.</summary>
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        public string GetMethodName() => Dev.ToString(this.stateMachine.pool);
#else
        public string GetMethodName() => string.Empty;
#endif
    }

    /// <summary>Don't touch! Only for system.</summary>
    struct TaskInfo2
    {
        // static

        /// <summary>Don't touch! Only for system.</summary>
        public static TaskInfo2 Invalid;

        static TaskInfo2()
        {
            Invalid.Next = Invalid.Prev = -1;
        }

        // inner classes
        [Flags] enum Flags : byte
        {
            None = 0,
            IsValid = 1 << 0,
        }

        // fields
        Story.PoolMemory extra;
        public int Prev; // 子タスクの方向
        public int Next; // 親タスクの方向
        Flags flags;

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_FAST
        /// <summary>Don't touch! Only for system.</summary>
        public long SortKeyForDebug;
        /// <summary>Don't touch! Only for system.</summary>
        public string StateForDebug;
#endif

        // properties

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsValid
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsValid) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsValid; } else { this.flags &= ~Flags.IsValid; } }
        }

        // methods

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Entry(int index)
        {
            this.IsValid = true;
            this.Next = this.Prev = index;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Free()
        {
            this.extra.Free();
            this.extra = default;
        }

        // extra

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public ref E GetExtra<E>()
        {
            if (!this.extra.IsValid) { this.extra = Story.PoolMemory.Alloc<E>(); }
            return ref this.extra.Get<E>();
        }
    }

/* Task の実行イメージ

static int step;
Task Prepare()
{
    step = 0;
    return Root();
}
async Task Root()
{
    step = 1;
    var any = Any(); // step = 1 （変わらない）
    await any;       // step = 2 （次フレ step = 3）
    step = 4;
}
async Task Any()
{
    step = 2;
    await Yield; // 次フレにここから実行するように登録
    step = 3;
}

void main()
{
    var node = Prepare();   // step = 0
    node.Boot();            // step = 1(by node) -> 2(by any)
}
    次フレ                   // step = 3(by any) -> 4(by node)

*/

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
