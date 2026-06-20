// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryUtils.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// Provides utility and extension methods for the Omochaya Story task system, 
// enabling zero-allocation asynchronous operations such as time-based waiting, 
// parallel execution, and task chaining optimized for Unity's lifecycle.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using HiddenStory;

    // タスク制御
    public static partial class Story
    {
        /// <summary>Expands the underlying task info pools to the specified initial capacity.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void SetInitialTaskCount(int count)
        {
            Pool<TaskInfo, TaskInfo2>.Shared.Expand(count);
            TaskManager.Shared.Expand(count); // 概ねプールと連動する。大抵は下回るが特殊な条件では上回る。
        }

        /// <summary>Replaces the current task with a new one, anchoring it to a master component and booting it.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static bool Boot(ref this Task self, Component master, Task task)
        {
            self.Free();
            self = task;
            return self.Boot(master);
        }

        /// <summary>Suspends the execution for the specified amount of seconds.</summary>
        /// <remarks>
        /// 標準の <c>Task.Delay</c> とは異なり、メインスレッドのフレーム更新（<c>UnityEngine.Time</c>）に依存して時間を計測します。
        /// また、タスクがキャンセルされた場合でも <c>TaskCanceledException</c> のような例外はスローされず、安全に中断されます。
        /// 
        /// タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        /// var handle = new Story.WaitTimeHandle(seconds);
        /// while (handle.IsBusy()) { await Story.Yield; }
        /// 
        /// こちらはNGです。
        /// while (new WaitTimeHandle(seconds).IsBusy()) { await Story.Yield; }
        /// 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WaitTime(float seconds) => WaitTimeCore(new WaitTimeHandle(seconds));

        /// <summary>Provides the internal asynchronous loop for time-based waiting using a custom wait handle.</summary>
        public static async Task WaitTimeCore(WaitTimeHandle handle) { while (handle.IsBusy()) { await Yield; } }

        /// <summary>Provides the internal asynchronous loop for time-based waiting until the specified end time is reached.</summary>
        public static async Task WaitTimeCore(double end) { while (WaitTimeHandle.IsBusy(end)) { await Yield; } }

        /// <summary>Represents a handle used to track a time-based wait operation without allocations.</summary>
        public readonly struct WaitTimeHandle
        {
            /// <summary>Calculates the target end time based on the current time and the specified duration in seconds.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static double End(float seconds) => Time.timeAsDouble + seconds;

            /// <summary>Determines whether the current time has not yet reached the specified end time.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsBusy(double end) => Time.timeAsDouble < end;

            // fields
            readonly double end;

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public WaitTimeHandle(float seconds) { this.end = End(seconds); }

            // methods

            /// <summary>Determines whether this handle is still waiting for the target duration to elapse.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsBusy() => IsBusy(this.end);
        }

        /// <summary>Suspends the execution for the specified amount of unscaled seconds.</summary>
        /// <remarks>
        /// 標準の <c>Task.Delay</c> とは異なり、メインスレッドのフレーム更新（<c>UnityEngine.Time</c>）に依存して時間を計測します。
        /// また、タスクがキャンセルされた場合でも <c>TaskCanceledException</c> のような例外はスローされず、安全に中断されます。
        /// 
        /// タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        /// var handle = new Story.WaitTimeUnscaledHandle(seconds);
        /// while (handle.IsBusy()) { await Story.Yield; }
        /// 
        /// こちらはNGです。
        /// while (new WaitTimeUnscaledHandle(seconds).IsBusy()) { await Story.Yield; }
        /// 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WaitTimeUnscaled(float seconds) => WaitTimeUnscaledCore(new WaitTimeUnscaledHandle(seconds));

        /// <summary>Provides the internal asynchronous loop for unscaled time-based waiting using a custom wait handle.</summary>
        public static async Task WaitTimeUnscaledCore(WaitTimeUnscaledHandle handle) { while (handle.IsBusy()) { await Yield; } }

        /// <summary>Provides the internal asynchronous loop for unscaled time-based waiting until the specified unscaled end time is reached.</summary>
        public static async Task WaitTimeUnscaledCore(double end) { while (WaitTimeUnscaledHandle.IsBusy(end)) { await Yield; } }

        /// <summary>Represents a handle used to track an unscaled time-based wait operation without allocations.</summary>
        public readonly struct WaitTimeUnscaledHandle
        {
            /// <summary>Calculates the target end time based on the current unscaled time and the specified duration in seconds.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static double End(float seconds) => Time.unscaledTimeAsDouble + seconds;

            /// <summary>Determines whether the current unscaled time has not yet reached the specified end time.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsBusy(double end) => Time.unscaledTimeAsDouble < end;

            // fields
            readonly double end;

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public WaitTimeUnscaledHandle(float seconds) { this.end = End(seconds); }

            // methods

            /// /// <summary>Determines whether this handle is still waiting for the target unscaled duration to elapse.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsBusy() => IsBusy(this.end);
        }

        /// <summary>Suspends the execution for the specified number of frames.</summary>
        /// <remarks>
        /// Unityのコルーチンと同様に、完全にメインスレッドの更新タイミングに同期して動作します。
        /// 
        /// タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        /// var handle = new Story.WaitFrameHandle(frames);
        /// while (handle.IsBusy) { await Story.Yield; }
        /// 
        /// こちらはNGです。
        /// while (new WaitFrameHandle(frames).IsBusy) { await Story.Yield; }
        /// 
        /// </remarks>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task WaitFrame(int frames) => WaitFrameCore(new WaitFrameHandle(frames));

        /// <summary>Provides the internal asynchronous loop for frame-based waiting using a custom wait handle.</summary>
        public static async Task WaitFrameCore(WaitFrameHandle handle) { while (handle.IsBusy()) { await Yield; } }

        /// <summary>Provides the internal asynchronous loop for frame-based waiting until the specified end frame index is reached.</summary>
        public static async Task WaitFrameCore(int end) { while (Time.frameCount < end) { await Yield; } }

        /// <summary>Represents a handle used to track a frame-based wait operation without allocations.</summary>
        public readonly struct WaitFrameHandle
        {
            /// <summary>Calculates the target end frame index based on the current frame count and the specified number of frames.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static int End(int frames) => Time.frameCount + frames;

            /// <summary>Determines whether the current frame count has not yet reached the specified end frame.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static bool IsBusy(int end) => Time.frameCount < end;

            // fields
            readonly int end;

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public WaitFrameHandle(int frames) { this.end = End(frames); }

            // methods

            /// <summary>Determines whether this handle is still waiting for the target number of frames to elapse.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public bool IsBusy() => IsBusy(this.end);
        }

        /// <summary>Suspends the execution until the provided condition evaluates to true.</summary>
        /// <remarks>
        /// 条件が <c>true</c> になるまで待機します。
        /// ラムダ式の外部変数キャプチャ（クロージャ）による意図しないメモリアロケーションを防ぐため、
        /// 判定に使用する外部変数は必ず <paramref name="args"/> 引数経由で渡してください。
        /// 複数必要な場合はタプルを使用してください。
        /// 
        /// タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        /// while (!condition(args)) { await Story.Yield; }
        /// </remarks>
        public static async Task WaitUntil<T>(T args, Func<T, bool> condition)
        {
            while (!condition(args)) { await Yield; }
        }

        /// <summary>Suspends the execution until the provided condition evaluates to false.</summary>
        /// <remarks>
        /// 条件が <c>false</c> になるまで待機します。
        /// <c>WaitUntil</c> と同様に、アロケーションを防ぐため判定変数は <paramref name="args"/> を経由して渡してください。
        /// 
        /// タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        /// while (condition(args)) { await Story.Yield; }
        /// </remarks>
        public static async Task WaitWhile<T>(T args, Func<T, bool> condition)
        {
            while (condition(args)) { await Yield; }
        }

        /// <summary>Creates a task that will complete when both the current task and the specified task have completed.</summary>
        /// <remarks>
        /// <b>[重要]</b> 標準の <c>Task.WhenAll</c> とは挙動が異なります。
        /// 実行中のいずれかのタスクが外部から強制キャンセル（<c>Free()</c>）されても、残りのタスクを道連れにしてキャンセルすることはありません。
        /// ゲームのキャラクター制御（例：足の移動と腕の振りを独立して実行する）などを想定し、それぞれが独立して完了・中断するのを最後まで待機します。
        /// </remarks>
        public static async Task With(this Task self, Task other)
        {
            while (true)
            {
                if (!self.MoveNext())
                {
                    while (other.MoveNext()) { await Yield; }
                    return;
                }
                if (!other.MoveNext())
                {
                    do { await Yield; } while (self.MoveNext());
                    return;
                }
                await Yield;
            }
        }
        /// <summary>Creates a task that will complete when both the current task and the specified task have completed.</summary>
        public static async Task With(this Task a, Task b, Task c)
        {
            while ( // 面倒なので終わったタスクへの無駄な IsValid は許容する
                a.MoveNext() | // || じゃないよ
                b.MoveNext() |
                c.MoveNext()) { await Yield; }
        }

        /// <summary>Creates a task that will complete when both the current task and the specified task have completed.</summary>
        public static async Task With(this Task a, Task b, Task c, Task d)
        {
            while (
                a.MoveNext() |
                b.MoveNext() |
                c.MoveNext() |
                d.MoveNext()) { await Yield; }
        }

        /// <summary>Executes the current task until the specified interrupting task completes.</summary>
        /// <remarks>
        /// <b>[重要]</b> 標準の <c>Task.WhenAny</c> とは挙動が異なります。
        /// どちらか一方が完了または中断した瞬間、もう一方のタスク（敗者）は自動的に道連れとして <c>Free()</c> され、即座に終了します。
        /// この際、敗者のタスク内で例外はスローされず、安全に <c>finally</c> ブロックへ遷移してクリーンアップされます。
        /// </remarks>
        public static async Task Until(this Task a, Task b)
        {
            while (true)
            {
                if (!a.MoveNext()) { b.Free(); return; }
                if (!b.MoveNext()) { a.Free(); return; }
                await Yield;
            }
        }

        /// <summary>Executes the current task until the specified interrupting task completes.</summary>
        public static async Task Until(this Task a, Task b, Task c)
        {
            while (true)
            {
                if (!a.MoveNext()) { b.Free(); c.Free(); return; }
                if (!b.MoveNext()) { c.Free(); a.Free(); return; }
                if (!c.MoveNext()) { a.Free(); b.Free(); return; }
                await Yield;
            }
        }

        /// <summary>Executes the current task until the specified interrupting task completes.</summary>
        public static async Task Until(this Task a, Task b, Task c, Task d)
        {
            while (true)
            {
                if (!a.MoveNext()) { b.Free(); c.Free(); d.Free(); return; }
                if (!b.MoveNext()) { c.Free(); d.Free(); a.Free(); return; }
                if (!c.MoveNext()) { d.Free(); a.Free(); b.Free(); return; }
                if (!d.MoveNext()) { a.Free(); b.Free(); c.Free(); return; }
                await Yield;
            }
        }

        /// <summary>Suspends the execution until any of the provided tasks complete, returning the result of the first completed task.</summary>
        public static async Task<R> Until<R>(this Task<R> a, Task<R> b)
        {
            var memory = PoolMemory.Alloc<R>();
            try
            {
                await Until(
                    UntilResult(a, memory),
                    UntilResult(b, memory));
                // ここでは a も b も確実に終わっているハズ
                return memory.Get<R>();
            }
            finally
            {
                memory.Free();
            }
        }
        static async Task UntilResult<R>(Task<R> task, PoolMemory weakMemory)
        {
            var result = await task;
            weakMemory.Get<R>() = result;
        }

        /// <summary>Suspends the execution until any of the provided tasks complete, returning the result of the first completed task.</summary>
        public static async Task<R> Until<R>(this Task<R> a, Task<R> b, Task<R> c)
        {
            var memory = PoolMemory.Alloc<R>();
            try
            {
                await Until(
                    UntilResult(a, memory),
                    UntilResult(b, memory),
                    UntilResult(c, memory));
                return memory.Get<R>();
            }
            finally
            {
                memory.Free();
            }
        }

        /// <summary>Suspends the execution until any of the provided tasks complete, returning the result of the first completed task.</summary>
        public static async Task<R> Until<R>(this Task<R> a, Task<R> b, Task<R> c, Task<R> d)
        {
            var memory = PoolMemory.Alloc<R>();
            try
            {
                await Until(
                    UntilResult(a, memory),
                    UntilResult(b, memory),
                    UntilResult(c, memory),
                    UntilResult(d, memory));
                return memory.Get<R>();
            }
            finally
            {
                memory.Free();
            }
        }

        /// <summary>Suspends the execution until the task completes or the specified timeout duration elapses.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Timeout(this Task self, float seconds) => Until(self, WaitTime(seconds));

        /// <summary>Suspends the execution until the task completes or the specified unscaled timeout duration elapses.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task TimeoutUnscaled(this Task self, float seconds) => Until(self, WaitTimeUnscaled(seconds));

        /// <summary>Executes the specified task sequentially after the current task completes.</summary>
        // タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        // await self;
        // await other;
        public static async Task Then(this Task self, Task other)
        {
            await self;
            await other;
        }

        /// <summary>
        /// Executes the specified action sequentially after the current task completes.
        /// Accepts an argument to prevent closure memory allocations.
        /// </summary>
        // タスク内ではこのメソッドを使うより以下の書き方をしたほうが無駄がありません。
        // await self;
        // action(args);
        public static async Task Then<T>(this Task self, T args, Action<T> action)
        {
            await self;
            action(args);
        }
    }
}
