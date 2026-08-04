// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTween.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System;
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using UnityEngine.LowLevel;
    using UnityEngine.PlayerLoop;
    using HiddenStory;

    public static partial class Story
    {
        /// <summary></summary>
        public static double GetStart() => Time.timeAsDouble;

        /// <summary></summary>
        public static double GetUnacaledStart() => Time.unscaledTimeAsDouble;

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= interval);
            return float.Epsilon < interval ? TweenTask(new Stepper(), interval, updater, ease, ref start)
                : ImmediateTask(updater, ease.Calc(1f)); // interval = 0 : 即終了
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float interval, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float interval, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float interval, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float interval, T args, Action<T, float> update)
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return Tween(interval, updater, Ease.None, ref start);
        }

        // 【from/to/speed】

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= speed);
            return float.Epsilon < speed ? TweenTask(new Stepper(), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
                : ImmediateTask(updater, from); // speed = 0 : 始まらない
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float from, float to, float speed, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float from, float to, float speed, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float from, float to, float speed, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float from, float to, float speed, T args, Action<T, float> update)
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }


        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float interval, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= interval);
            return float.Epsilon < interval ? TweenTask(new UnscaledStepper(), interval, updater, ease, ref start)
                : ImmediateTask(updater, ease.Calc(1f)); // interval = 0 : 即終了
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float interval, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float interval, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float interval, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<E, T>(float interval, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<E, T>(float interval, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float interval, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float interval, T args, Action<T, float> update)
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        // 【from/to/speed】

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float from, float to, float speed, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= speed);
            return float.Epsilon < speed ? TweenTask(new Stepper(), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
                : ImmediateTask(updater, from); // speed = 0 : 始まらない
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float from, float to, float speed, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float from, float to, float speed, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float from, float to, float speed, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float from, float to, float speed, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float from, float to, float speed, T args, Action<T, float> update)
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        // updater

        /// <summary></summary>
        public interface IUpdater
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Update(float now);
        }

        readonly struct UpdaterImpl<T> : IUpdater
        {
            // fields
            readonly Action<T, float> action;
            readonly T args;

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public UpdaterImpl(T args, Action<T, float> action)
            {
                this.args = args;
                this.action = action;
            }

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(float now) => this.action(this.args, now);
        }
        static UpdaterImpl<T> Updater<T>(T args, Action<T, float> action) => new UpdaterImpl<T>(args, action);

        // task
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task ImmediateTask<U>(U updater, float end)
            where U : struct, IUpdater
        {
            updater.Update(end);
            await Void;
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static Task TweenTask<S, U, E>(S stepper, float interval, in U updater, E ease, ref double start)
            where S : struct, IStepper
            where U : struct, IUpdater
            where E : struct, IEase
        {
            stepper.Setup(interval, ref start);
            return IntervalTask(stepper, updater, ease);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task IntervalTask<S, U, E>(S stepper, U updater, E ease)
            where S : struct, IStepper
            where U : struct, IUpdater
            where E : struct, IEase
        {
            while (stepper.Step(updater, ease)) { await Yield; }
        }

        internal struct Stepper : IStepper
        {
            StepperImpl impl;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void Setup(float interval, ref double start) => this.impl = new(interval, ref start);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public bool Step<U, E>(in U updater, E ease)
                where U : struct, IUpdater
                where E : struct, IEase
            {
                this.impl.Proceed();
                return this.impl.Step(updater, ease);
            }
        }
        internal struct UnscaledStepper : IStepper
        {
            StepperImpl impl;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void Setup(float interval, ref double start) => this.impl = new(interval, ref start);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public bool Step<U, E>(in U updater, E ease)
                where U : struct, IUpdater
                where E : struct, IEase
            {
                this.impl.UnscaledProceed();
                return this.impl.Step(updater, ease);
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IStepper
        {
            void Setup(float interval, ref double start);
            bool Step<U, E>(in U updater, E ease)
                where U : struct, IUpdater
                where E : struct, IEase;
        }

        struct StepperImpl
        {
            // fields
            double prev;
            double start;
            readonly float interval;
            float seek;

            // properties
            readonly float Now
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { Dev.Assert(float.Epsilon < this.interval); return this.seek / this.interval; }
            }

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal StepperImpl(float interval, ref double start)
            {
                prev = this.start = start;
                this.interval = interval;
                this.seek = 0f;
                start = this.start + interval;
            }

            // methods
            internal void Proceed()
            {
                var timeAsDouble = Time.timeAsDouble;
                var diff = timeAsDouble - this.prev;
                if (diff <= 0)
                {
#if UNITY_EDITOR
                    if (diff == 0f) { Dev.Log($"巻き戻ってるので実行しない(interrupt？)：{diff}"); }
                    else if (timeAsDouble <= this.start) { Dev.Log($"巻き戻ってるので実行しない(SetStart で遅延起動？)：{diff}"); }
                    else { Dev.LogWarning($"巻き戻ってるので実行しない(A)：{diff}"); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = Time.deltaTime;
                if (delta * 1.25 < diff)
                {
                    diff -= delta;
                    Dev.LogWarning($"実行してないフレームがあったので飛ばす：{diff}");
                    this.start += diff;
                }
                this.prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Dev.LogWarning($"巻き戻ってるので実行しない(B)：{seek}"); }
#endif
                this.seek = seek;
            }
            internal void UnscaledProceed()
            {
                var timeAsDouble = Time.unscaledTimeAsDouble;
                var diff = timeAsDouble - this.prev;
                if (diff <= 0)
                {
#if UNITY_EDITOR
                    if (diff == 0f) { Dev.Log($"巻き戻ってるので実行しない(interrupt？)：{diff}"); }
                    else if (timeAsDouble <= this.start) { Dev.Log($"巻き戻ってるので実行しない(SetStart で遅延起動？)：{diff}"); }
                    else { Dev.LogWarning($"巻き戻ってるので実行しない(A)：{diff}"); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = Time.unscaledDeltaTime;
                if (delta * 1.25 < diff)
                {
                    diff -= delta;
                    Dev.LogWarning($"実行してないフレームがあったので飛ばす：{diff}");
                    this.start += diff;
                }
                this.prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Dev.LogWarning($"巻き戻ってるので実行しない(B)：{seek}"); }
#endif
                this.seek = seek;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Step<U, E>(in U updater, E ease)
                where U : struct, IUpdater
                where E : struct, IEase
            {
                if (0f <= this.seek)
                {
                    if (this.interval <= this.seek)
                    {
                        updater.Update(ease.Calc(1f));
                        return false;
                    }
                    updater.Update(ease.Calc(Now));
                }
                return true;
            }
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P, E>(this P plan, float interval, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new Stepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(new Stepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new Stepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(new Stepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new UnscaledStepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnacaledStart();
            return plan.CreateTask(new UnscaledStepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetUnacaledStart();
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(interval:interval), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new UnscaledStepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnacaledStart();
            return plan.CreateTask(new UnscaledStepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(speed:speed), ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetUnacaledStart();
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(speed:speed), ease, ref start);
        }

#if !STORY_NO_TIME_CACHE

        static class Time
        {
            public static double timeAsDouble;
            public static double unscaledTimeAsDouble;
            public static float deltaTime;
            public static float unscaledDeltaTime;
            public static int frameCount;
            public static void UpdateCache()
            {
                timeAsDouble = UnityEngine.Time.timeAsDouble;
                unscaledTimeAsDouble = UnityEngine.Time.unscaledTimeAsDouble;
                deltaTime = UnityEngine.Time.deltaTime;
                unscaledDeltaTime = UnityEngine.Time.unscaledDeltaTime;
                frameCount = UnityEngine.Time.frameCount;
            }

            [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.SubsystemRegistration)]
            static void RegisterTimeCache()
            {
                var defaultLoop = PlayerLoop.GetCurrentPlayerLoop();
                var customLoop = new PlayerLoopSystem { type = typeof(Time), updateDelegate = Time.UpdateCache };

                if (PlayerLoopUtility.AppendToPhase<EarlyUpdate>(ref defaultLoop, customLoop)) { PlayerLoop.SetPlayerLoop(defaultLoop); }
                else { Dev.LogError("[TimeCache] プレイヤーループへの挿入に失敗しました。"); }
            }
        }

        static class PlayerLoopUtility
        {
            /// <summary>
            /// 指定したメインフェーズの「一番最後」にカスタムループを追加します。
            /// </summary>
            public static bool AppendToPhase<TTargetPhase>(ref PlayerLoopSystem rootLoop, PlayerLoopSystem customLoop)
                where TTargetPhase : struct
            {
                if (rootLoop.subSystemList == null) return false;

                for (int i = 0; i < rootLoop.subSystemList.Length; i++)
                {
                    if (rootLoop.subSystemList[i].type == typeof(TTargetPhase))
                    {
                        var subSystems = rootLoop.subSystemList[i].subSystemList ?? new PlayerLoopSystem[0];
                        var newSubSystems = new PlayerLoopSystem[subSystems.Length + 1];

                        // 既存のものをすべてコピー
                        System.Array.Copy(subSystems, 0, newSubSystems, 0, subSystems.Length);
                        // 最後尾にカスタムループを配置
                        newSubSystems[subSystems.Length] = customLoop;

                        rootLoop.subSystemList[i].subSystemList = newSubSystems;
                        return true;
                    }

                    if (AppendToPhase<TTargetPhase>(ref rootLoop.subSystemList[i], customLoop))
                    {
                        return true;
                    }
                }
                return false;
            }
        }
#endif
    }
}

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static class Mover
    {
        // param ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            float Length { get; }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            P Lerp(in P b, float now);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            P Add(in P b);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            P Sub(in P b);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Param1 : IParam<Param1>
        {
            internal readonly float P0;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Length => Mathf.Abs(this.P0);
            internal Param1(float p0)
            {
                this.P0 = p0;
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param1 Lerp(in Param1 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t));
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param1 Add(in Param1 b) => new(this.P0 + b.P0);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param1 Sub(in Param1 b) => new(this.P0 - b.P0);
        }
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Param2 : IParam<Param2>
        {
            internal readonly float P0, P1;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Length => new Vector2(this.P0, this.P1).magnitude; // 記述量減らしたいので（重くもないはず）
            internal Param2(float p0, float p1)
            {
                this.P0 = p0;
                this.P1 = p1;
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param2 Lerp(in Param2 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t));
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param2 Add(in Param2 b) => new(this.P0 + b.P0, this.P1 + b.P1);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param2 Sub(in Param2 b) => new(this.P0 - b.P0, this.P1 - b.P1);
        }
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Param3 : IParam<Param3>
        {
            internal readonly float P0, P1, P2;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Length => new Vector3(this.P0, this.P1, this.P2).magnitude;
            internal Param3(float p0, float p1, float p2)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param3 Lerp(in Param3 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t));
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param3 Add(in Param3 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param3 Sub(in Param3 b) => new(this.P0 - b.P0, this.P1 - b.P1, this.P2 - b.P2);
        }
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Param4 : IParam<Param4>
        {
            internal readonly float P0, P1, P2, P3;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Length => new Vector4(this.P0, this.P1, this.P2, this.P3).magnitude;
            internal Param4(float p0, float p1, float p2, float p3)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
                this.P3 = p3;
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param4 Lerp(in Param4 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t),
                Mathf.LerpUnclamped(this.P3, b.P3, t));
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param4 Add(in Param4 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2, this.P3 + b.P3);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Param4 Sub(in Param4 b) => new(this.P0 - b.P0, this.P1 - b.P1, this.P2 - b.P2, this.P3 - b.P3);
        }
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct ParamQ : IParam<ParamQ>
        {
            internal readonly Quaternion Q;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Length => Quaternion.Angle(Quaternion.identity, this.Q); // 角度にしておく
            internal ParamQ(Quaternion q) => this.Q = q;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public ParamQ Lerp(in ParamQ b, float t) 
                => new(Quaternion.SlerpUnclamped(this.Q, b.Q, t));
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public ParamQ Add(in ParamQ b) => new(this.Q * b.Q); // 順変換にしておく
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public ParamQ Sub(in ParamQ b) => new(this.Q * Quaternion.Inverse(b.Q)); // 逆変換にしておく
        }

        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier<T>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            Component Self { get; }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            T Current { get; }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            void SetCurrent(T current);
        }

        // mapper ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IMapper<T, P>
            where P : struct, IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            P Get(T current);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            T Set(T current, P prm);
        }

        // plan ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IPlan
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            Story.Task CreateTask<S, E>(in S spepper, in TimeArg arg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase;
        }

        internal readonly struct PlanArg<T, C>
            where C : struct, ICarrier<T>
        {
            internal readonly T To;
            internal readonly C Carrier;
            internal readonly bool IsDelta;
            internal PlanArg(C carrier, T to, bool isDelta)
            {
                this.To = to;
                this.Carrier = carrier;
                this.IsDelta = isDelta;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct TimeArg
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly float Interval;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly float Speed;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly bool IsUnscaled;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TimeArg(float interval = 0f, float speed = 0f, bool isUnscaled = false)
            {
                this.Interval = interval;
                this.Speed = speed;
                this.IsUnscaled = isUnscaled;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Story.Task Create<S, T, P, M, C, E>(in PlanArg<T, C> planArg, in TimeArg timeArg, E ease, ref double start)
            where S : struct, Story.IStepper
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
            where E : struct, Story.IEase
            => Task(new Builder<S, T, P, M, C>(planArg, timeArg, ref start), ease);

        static async Story.Task Task<S, T, P, M, C, E>(Builder<S, T, P, M, C> builder, E ease)
            where S : struct, Story.IStepper
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
            where E : struct, Story.IEase
        {
            var kit = builder.Build();
            while (kit.Step(ease)) { await Story.Yield; }
        }

        readonly struct Target<T, P, M>
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
        {
            readonly P to;
            internal P To => this.to;
            internal Target(T to) => this.to = new M().Get(to);
            internal P Get(T p) => new M().Get(p);
            internal float Distance(T p) => this.To.Sub(this.Get(p)).Length;
        }

        readonly struct Builder<S, T, P, M, C>
            where S : struct, Story.IStepper
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
        {
            readonly PlanArg<T, C> planArg;
            readonly TimeArg timeArg;
            readonly double start;
            // internal Builder(Target<T, P, M> target, C carrier, bool isDelta, in TimeArg arg, ref double start)
            internal Builder(in PlanArg<T, C> planArg, in TimeArg timeArg, ref double start)
            {
                this.planArg = planArg;
                this.timeArg = timeArg;
                this.start = start;
                if (float.Epsilon < this.timeArg.Speed)
                {
                    var target = new Target<T, P, M>(planArg.To);
                    var length = planArg.IsDelta ? target.To.Length : target.Distance(planArg.Carrier.Current);
                    start += length / this.timeArg.Speed;
                }
                else
                {
                    start += this.timeArg.Interval;
                }
            }

            internal Updater<S, T, P, M, C> Build()
            {
                // owner 確定
                TryKeep(this.planArg.Carrier.Self);

                // from 確定
                var target = new Target<T, P, M>(this.planArg.To);
                var from = target.Get(this.planArg.Carrier.Current);
                var to = target.To;

                // interval 確定
                var interval = this.timeArg.Interval;
                if (float.Epsilon < this.timeArg.Speed)
                {
                    if (this.planArg.IsDelta) { interval = to.Length / this.timeArg.Speed; }
                    else { interval = from.Sub(to).Length / this.timeArg.Speed; }

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.timeArg.Interval) && this.timeArg.Interval != interval) { Dev.LogWarning("移動期間が変化しました"); }
#endif

                }

                // to 確定
                if (this.planArg.IsDelta) { to = from.Add(to); }

                // 生成
                var stepper = new S();
                var start = this.start;
                stepper.Setup(interval, ref start);
                return new(stepper, this.planArg.Carrier, from, to);
            }
        }

        static void TryKeep(Component self)
        {
            if (Story.IsTryKeeped)
            {
                if (self.TryGetComponent<Story.ITaskOwner>(out var owner)) { Story.At((Component)owner); }
                else { Story.At(self); }
            }
        }

        struct Updater<S, T, P, M, C> : Story.IUpdater
            where S : struct, Story.IStepper
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
        {
            S stepper;
            C carrier; // 読み取り専用にできるらしいが、したら Update で更新されなくならないか？
            readonly P from, to;
            internal Updater(in S stepper, C carrier, P from, P to)
            {
                this.stepper = stepper;
                this.carrier = carrier;
                this.from = from;
                this.to = to;
            }
            internal bool Step<E>(E ease)
                where E : struct, Story.IEase
                => this.stepper.Step(this, ease);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void Update(float now)
                => this.carrier.SetCurrent(new M().Set(this.carrier.Current, this.from.Lerp(this.to, now)));
        }
    }
}
