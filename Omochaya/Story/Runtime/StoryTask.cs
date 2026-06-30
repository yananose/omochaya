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

[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("com.omochaya.story.Editor")]
[assembly: System.Runtime.CompilerServices.InternalsVisibleTo("com.omochaya.story.Tests")]

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
    ・タスクAがタスクBをawaitするとタスクAの代わりにタスクBが実行されるようになる。タスクBが終了するとタスクAが実行されるようになる。

    【注意点】
    ・メインスレッド限定。
    ・他の非同期タスクで await Story.Task することはできない。
    ・Story.Task で他の非同期タスクを await することはできない。await できるのは Story.Task, Story.Yield系、Story.Void のみ。
    ・Story.Task 外で Start する場合はコンポーネントを owner として指定する必要がある。
    ・await されたタスク及び Start したタスクを await することはできない。 待つ場合は while (task.IsValid) { await Story.Yield; } すること。なお、結果を受け取ることはできない。
    ・await が終了したタスクから結果を受け取ることはできない。
    ・高速化のため、可能であれば owner に指定するコンポーネントは ITaskOwner インターフェイスを実装すること。Story.TaskBehaviour を継承してもよい。
    ・Story.Task がキャンセル（Task.Stop 実行 / foreach 中の beak / owner 等消失による削除）された場合は finally ブロックを実行する。
    ・finally ブロックで owner に指定したコンポーネントを参照してはならない。
    ・キャンセル発生以降は owner を無視するので await する場合は使用者が責任を持って解放すること。
    ・子がキャンセルされても親はキャンセルされずに続きの処理を再開する。その際、親が受け取る結果は default となる。await の直後であれば Story.HasValidResult でキャンセルされたかどうか検知できる。

    */
    public static partial class Story
    {
        /// <summary>The default number of execution bands (Auto, Late, and Fixed) allocated for the task manager.</summary>
        public const int DEFAULT_BAND_COUNT = 3;

        /// <summary>The default initial capacity allocated for task pools and execution band arrays.</summary>
        public const int DEFAULT_TASK_COUNT = 1024;

        /// <summary>Configures and expands the capacity of the global task pools.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warmup(int taskCount) => TaskManager.Shared.Custom(DEFAULT_BAND_COUNT, taskCount);

        /// <summary>Configures and expands the capacity of the global task execution bands and pools.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Custom(int bandCount = DEFAULT_BAND_COUNT, int taskCount = DEFAULT_TASK_COUNT) => TaskManager.Shared.Custom(bandCount, taskCount);

        /// <summary>Drives the global task manager loop forward, executing all registered automated tasks for the current frame.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Update() => TaskManager.Shared.Update();

        /// <summary>Drives the late task manager loop forward, executing all registered automated late tasks.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void LateUpdate() => TaskManager.Shared.BandUpdate(1);

        /// <summary>Drives the fixed task manager loop forward, executing all registered automated fixed tasks.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void FixedUpdate() => TaskManager.Shared.BandUpdate(2);

        /// <summary>Drives a specific execution band loop forward based on the provided band index.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void BandUpdate(int bandNo) => TaskManager.Shared.BandUpdate(bandNo);

        /// <summary>A globally accessible token to await a single-frame yield.</summary>
        public static YieldCore Yield => new(0);

        /// <summary>A globally accessible token to await a late-frame yield.</summary>
        public static YieldCore YieldLate => new(1);

        /// <summary>A globally accessible token to await a fixed-frame yield.</summary>
        public static YieldCore YieldFixed => new(2);

        /// <summary>A globally accessible token to await a same-frame yield.</summary>
        public static YieldCore YieldSame => new(TaskManager.SAME_BAND);

        /// <summary>Creates a custom yield token bound to a specific execution band index.</summary>
        public static YieldCore YieldNo(int bandNo) => new(bandNo);

        // 即返却（await しない async メソッドの遅延実行用）
        /// <summary>A globally accessible token to await an immediate void or finished state.</summary>
        public static VoidCore Void => default;

        /// <summary>Determines whether the last awaited task completed successfully and returned a valid result.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool HasValidResult() => TaskManager.Shared.HasValidResult;

        /// <summary>Determines whether the specified exception indicates a task cancellation.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool IsCanceledException(Exception e) => TaskManager.Shared.IsCanceledException(e);

        /// <summary>Specifies the initial pool capacity to be allocated for the state machine associated with an asynchronous task method.</summary>
        [AttributeUsage(AttributeTargets.Method, Inherited = false)]
        [UnityEngine.Scripting.Preserve]
        public class CapacityAttribute : Attribute
        {
            public int Capacity { get; }
            public CapacityAttribute(int capacity)
            {
                Capacity = capacity;
            }
        }

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

            /// <summary>Gets a value indicating whether this task is marked or requested for cancellation.</summary>
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

            /// <summary>Anchors the task to a owner component and registers it to the automation loop.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Start(Component owner)
            {
                Keep(owner);
                return Start();
            }

            /// <summary>Registers the task to the automation loop using its pre-assigned owner component.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Start() => TaskManager.Shared.Boot(this);

            /// <summary>Explicitly releases and cancels the task, recycling its resources.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Stop()
            {
                if (IsEmpty) { return; }
                TaskManager.Shared.Free(this);
            }

            /// <summary>Anchors the task to a specific owner component to govern its lifecycle.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep(Component owner) => this.Info().Keep(owner);

            /// <summary>Anchors the task to the currently running task's owner component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep()
            {
                Dev.Assert(TaskManager.Shared.IsRunningValid);
                Keep(TaskManager.Shared.GetRunningInfo().Owner);
            }

            /// <summary>Drives the task state machine forward manually by one step.</summary>
            // [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext()
            {
                if (IsEmpty) { return false; }
                return TaskManager.Shared.MoveNext(this);
            }

            /// <summary>Determines whether this task matches the specified task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Task a) => this.Id.Matches(a.Id);

            /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Warmup(int length = 0)
            {
                ref var info = ref this.Info();
                Dev.Assert(info.IsValid);
                if (0 < length) { info.Warmup(length); }
            }

            /// <summary>Creates a task handle directly from a raw pool index without validation.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Returns a string that represents the current task status and identification.</summary>
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

            /// <summary>Gets a value indicating whether this task is marked or requested for cancellation.</summary>
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

            /// <summary>Anchors the task to a owner component and registers it to the automation loop.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Start(Component owner) => this.rawTask.Start(owner);

            /// <summary>Registers the task to the automation loop using its pre-assigned owner component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Start() => this.rawTask.Start();

            /// <summary>Explicitly releases and cancels the task, recycling its resources.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Stop() => this.rawTask.Stop();

            /// <summary>Anchors the task to the currently running task's owner component.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep(Component owner) => this.rawTask.Keep(owner);

            /// <summary>Anchors the task to a specific owner component to govern its lifecycle.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Keep() => this.rawTask.Keep();

            /// <summary>Drives the task state machine forward manually by one step.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool MoveNext() => this.rawTask.MoveNext();

            /// <summary>Determines whether this task matches the specified task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool Matches(Task<R> a) => this.rawTask.Matches(a.rawTask);

            /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this task.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Warmup(int length) => this.rawTask.Warmup(length);

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

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
            /// <summary>Returns a string that represents the current task status and identification.</summary>
            public override string ToString() => this.rawTask.ToString();
#endif
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // 偽装nullチェック回避と一時停止対応用

        /// <summary>Defines a owner object that governs the lifecycle and destruction state of associated tasks.</summary>
        public interface ITaskOwner
        {
            /// <summary>Gets or sets a value indicating whether the owner object has been destroyed.</summary>
            bool IsDestroyed { get; set; }
            /// <summary>Gets a value indicating whether the owner object is currently paused or inactive.</summary>
            bool IsPaused { get; }
        }

        /// <summary>A base MonoBehaviour that implements ITaskOwner to manage tasks bound to its lifecycle.</summary>
        public class TaskBehaviour : MonoBehaviour, ITaskOwner // 実装漏れ回避用
        {
            /// <summary>Invoked when the component is destroyed, allowing derived classes to perform custom cleanup.</summary>
            protected virtual void OnDestroyed() {}

            /// <summary>Invoked when the component becomes enabled and active, allowing derived classes to handle initialization.</summary>
            protected virtual void OnEnabled() {}

            /// <summary>Invoked when the component becomes disabled or inactive, allowing derived classes to handle pausing logic.</summary>
            protected virtual void OnDisabled() {}

            /// <summary>Gets or sets a value indicating whether this component has been destroyed.</summary>
            public bool IsDestroyed { get; set; }
            /// <summary>Gets or sets a value indicating whether this component is currently paused or inactive.</summary>
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
        // オプション機能。タスク内外でアクセスできるゼロアロケーション領域。タスク毎に１つのみ。

        /// <summary>Gets a reference to the custom extra metadata structure bound to the currently executing task.</summary>
        public static ref E GetExtra<E>()
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

    // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
    // タスク（中身）

    [StructLayout(LayoutKind.Sequential, Size = 32)]
    struct TaskInfo
    {
        // static

        public static TaskInfo Invalid;

        static TaskInfo()
        {
            Dev.Assert(Unsafe.SizeOf<TaskInfo>() == 32);
            Invalid.Offset = TaskManager.INVALID_OFFSET;
        }

        // inner classes
        [Flags] enum Flags : ushort
        {
            None = 0,
            IsValid = 1 << 0,
            IsStarted = 1 << 1,
            IsFastOwner = 1 << 2,
            IsPinned = 1 << 3,
            IsRunning = 1 << 4,
            WillCancel = 1 << 5,
        }

        // fields
        StateMachine stateMachine; // 12 バイトしか使ってないが 16 バイトを占有
        Component owner;
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
        public bool IsStarted
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsStarted) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsStarted; } else { this.flags &= ~Flags.IsStarted; } }
        }

        bool IsFastOwner
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsFastOwner) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set { if (value) { this.flags |= Flags.IsFastOwner; } else { this.flags &= ~Flags.IsFastOwner; } }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public bool IsPinned
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            readonly get => (this.flags & Flags.IsPinned) != 0;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            set
            {
                if (value)
                {
                    this.flags |= Flags.IsPinned;
                    // オーナーからも解放（※無用な参照を残さないためだが問題が出たらやめる）
                    IsFastOwner = false;
                    this.owner = null;
                }
                else { this.flags &= ~Flags.IsPinned; }
            }
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
        public Component Owner => this.owner;

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool IsTop
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get => this.Offset != TaskManager.INVALID_OFFSET;
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool ShouldCancel // 偽装nullチェック
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (WillCancel) { return true; }
                if (IsPinned) { return false; }
                if (IsFastOwner) { return ((Story.ITaskOwner)this.owner).IsDestroyed; }
                return this.owner == null;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly bool IsPaused
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            get
            {
                if (IsFastOwner) { return ((Story.ITaskOwner)this.owner).IsPaused; }
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
        public void Keep(Component owner)
        {
            if (!IsValid) { throw new Exception(Messages.Exceptions.CannotOperateInvalidTask); }
            Dev.Assert(!(owner is null), Messages.Exceptions.OwnerCannotBeNull);
            this.owner = owner;
            IsFastOwner = owner is Story.ITaskOwner;
            IsPinned = false; // ピン留めを外してオーナー依存に戻す
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
            IsStarted = true;
            this.stateMachine.MoveNext();
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public void Warmup(int length) => this.stateMachine.Warmup(length);

        /// <summary>Don't touch! Only for system.</summary>
#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
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

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
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
}
