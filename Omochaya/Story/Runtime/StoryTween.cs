// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTween.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// Defines the core zero-allocation tweening engine, managing task scheduling, time steppers, and structural execution plans.
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
        /// <summary>Retrieves the current scaled time as a double-precision starting point for tween operations.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetStart() => Time.timeAsDouble;

        /// <summary>Retrieves the current unscaled time as a double-precision starting point for tween operations.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static double GetUnscaledStart() => Time.unscaledTimeAsDouble;

        /// <summary>Creates a zero-allocation tween task executed over a specific interval with a structural updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= interval);
            return float.Epsilon < interval ? TweenTask(new Stepper(), interval, updater, ease, ref start)
                : ImmediateTask(updater, ease.Calc(1f)); // interval = 0 : 即終了
        }

        /// <summary>Creates a zero-allocation tween task executed over a specific interval with a structural updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task executed over a specific interval with a structural updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float interval, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task executed over a specific interval with a structural updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float interval, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation tween task executed over a specific interval using a delegate-based updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation tween task executed over a specific interval using a delegate-based updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task executed over a specific interval using a delegate-based updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float interval, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return Tween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task executed over a specific interval using a delegate-based updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float interval, T args, Action<T, float> update)
        {
            var start = GetStart();
            var updater = Updater(args, update);
            return Tween(interval, updater, Ease.None, ref start);
        }

        // 【from/to/speed】

        /// <summary>Creates a zero-allocation tween task driven by movement speed from a start to a target value with easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= speed);
            return float.Epsilon < speed ? TweenTask(new Stepper(), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
                : ImmediateTask(updater, from); // speed = 0 : 始まらない
        }

        /// <summary>Creates a zero-allocation tween task driven by movement speed from a start to a target value with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetStart();
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task driven by movement speed from a start to a target value, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float from, float to, float speed, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task driven by movement speed from a start to a target value, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U>(float from, float to, float speed, in U updater)
            where U : struct, IUpdater
        {
            var start = GetStart();
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation tween task driven by movement speed using a delegate-based updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation tween task driven by movement speed using a delegate-based updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task driven by movement speed using a delegate-based updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float from, float to, float speed, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation linear tween task driven by movement speed using a delegate-based updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T>(float from, float to, float speed, T args, Action<T, float> update)
        {
            var updater = Updater(args, update);
            var start = GetStart();
            return Tween(from, to, speed, updater, Ease.None, ref start);
        }


        /// <summary>Creates a zero-allocation unscaled time tween task executed over a specific interval with a structural updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float interval, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= interval);
            return float.Epsilon < interval ? TweenTask(new UnscaledStepper(), interval, updater, ease, ref start)
                : ImmediateTask(updater, ease.Calc(1f)); // interval = 0 : 即終了
        }

        /// <summary>Creates a zero-allocation unscaled time tween task executed over a specific interval with a structural updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float interval, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task executed over a specific interval with a structural updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float interval, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task executed over a specific interval with a structural updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float interval, in U updater)
            where U : struct, IUpdater
        {
            var start = GetUnscaledStart();
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled time tween task executed over a specific interval using a delegate-based updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<E, T>(float interval, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled time tween task executed over a specific interval using a delegate-based updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<E, T>(float interval, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task executed over a specific interval using a delegate-based updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float interval, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task executed over a specific interval using a delegate-based updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float interval, T args, Action<T, float> update)
        {
            var start = GetUnscaledStart();
            var updater = Updater(args, update);
            return UnscaledTween(interval, updater, Ease.None, ref start);
        }

        // 【from/to/speed】

        /// <summary>Creates a zero-allocation unscaled time tween task driven by movement speed from a start to a target value with easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float from, float to, float speed, in U updater, E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= speed);
            return float.Epsilon < speed ? TweenTask(new UnscaledStepper(), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
                : ImmediateTask(updater, from); // speed = 0 : 始まらない
        }

        /// <summary>Creates a zero-allocation unscaled time tween task driven by movement speed from a start to a target value with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U, E>(float from, float to, float speed, in U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task driven by movement speed from a start to a target value, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float from, float to, float speed, in U updater, ref double start)
            where U : struct, IUpdater
        {
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task driven by movement speed from a start to a target value, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<U>(float from, float to, float speed, in U updater)
            where U : struct, IUpdater
        {
            var start = GetUnscaledStart();
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled time tween task driven by movement speed using a delegate-based updater and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled time tween task driven by movement speed using a delegate-based updater and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T, E>(float from, float to, float speed, T args, Action<T, float> update, E ease)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            var start = GetUnscaledStart();
            return UnscaledTween(from, to, speed, updater, ease, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task driven by movement speed using a delegate-based updater, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float from, float to, float speed, T args, Action<T, float> update, ref double start)
        {
            var updater = Updater(args, update);
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        /// <summary>Creates a zero-allocation unscaled linear time tween task driven by movement speed using a delegate-based updater, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledTween<T>(float from, float to, float speed, T args, Action<T, float> update)
        {
            var updater = Updater(args, update);
            var start = GetUnscaledStart();
            return UnscaledTween(from, to, speed, updater, Ease.None, ref start);
        }

        // updater

        /// <summary>Defines a zero-allocation interface for executing tween value updates based on normalized time.</summary>
        public interface IUpdater
        {
            /// <summary>Executes the tween update logic for the current normalized time.</summary>
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

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Update(float now) => this.action(this.args, now);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static UpdaterImpl<T> Updater<T>(T args, Action<T, float> action) => new UpdaterImpl<T>(args, action);

        // task
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

        // task
        static async Task IntervalTask<S, U, E>(S stepper, U updater, E ease)
            where S : struct, IStepper
            where U : struct, IUpdater
            where E : struct, IEase
        {
            while (stepper.Step(updater, ease)) { await Yield; }
        }

        internal struct Stepper : IStepper
        {

            // fields

            StepperImpl impl;

            // methods

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Setup(float interval, ref double start) => this.impl = new(interval, ref start);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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

            // fields

            StepperImpl impl;

            // methods

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Setup(float interval, ref double start) => this.impl = new(interval, ref start);

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            void Setup(float interval, ref double start);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
            internal void Proceed()
            {
                var timeAsDouble = Time.timeAsDouble;
                var diff = timeAsDouble - this.prev;
                if (diff <= 0)
                {
#if UNITY_EDITOR
                    if (timeAsDouble == this.start) { } // 初回
                    else if (timeAsDouble < this.start) { Dev.Log(string.Format(Messages.Warnings.TimeRewoundDelayedStart, diff)); }
                    else { Dev.LogWarning(string.Format(Messages.Warnings.TimeRewoundA, diff)); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = Time.deltaTime;
                if (delta * 1.25 < diff)
                {
                    diff -= delta;
                    Dev.LogWarning(string.Format(Messages.Warnings.SkippedUnexecutedFrames, diff));
                    this.start += diff;
                }
                this.prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Dev.LogWarning(string.Format(Messages.Warnings.TimeRewoundB, seek)); }
#endif
                this.seek = seek;
            }

            [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
            internal void UnscaledProceed()
            {
                var timeAsDouble = Time.unscaledTimeAsDouble;
                var diff = timeAsDouble - this.prev;
                if (diff <= 0)
                {
#if UNITY_EDITOR
                    if (timeAsDouble == this.start) { } // 初回
                    else if (timeAsDouble < this.start) { Dev.Log(string.Format(Messages.Warnings.TimeRewoundDelayedStart, diff)); }
                    else { Dev.LogWarning(string.Format(Messages.Warnings.TimeRewoundA, diff)); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = Time.unscaledDeltaTime;
                if (delta * 1.25 < diff)
                {
                    diff -= delta;
                    Dev.LogWarning(string.Format(Messages.Warnings.SkippedUnexecutedFrames, diff));
                    this.start += diff;
                }
                this.prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Dev.LogWarning(string.Format(Messages.Warnings.TimeRewoundB, seek)); }
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

        /// <summary>Converts a structural tween plan into a zero-allocation task executed over a specific interval with easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P, E>(this P plan, float interval, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new Stepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task executed over a specific interval with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(new Stepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task executed over a specific interval, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task executed over a specific interval, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task driven by movement speed and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new Stepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task driven by movement speed and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(new Stepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task driven by movement speed, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task driven by movement speed, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(new Stepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task executed over a specific interval with easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new UnscaledStepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task executed over a specific interval with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return plan.CreateTask(new UnscaledStepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task executed over a specific interval, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task executed over a specific interval, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetUnscaledStart();
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task driven by movement speed and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(new UnscaledStepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task driven by movement speed and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return plan.CreateTask(new UnscaledStepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task driven by movement speed, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(0f, speed), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task driven by movement speed, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetUnscaledStart();
            var ease = Ease.None;
            return plan.CreateTask(new UnscaledStepper(), new(0f, speed), ease, ref start);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Component GetOwner(Component self, Component owner)
        {
            if (owner != null) { return owner; }
            if (self.TryGetComponent<ITaskOwner>(out var other)) { return (Component)other; }
            return self;
        }

#if STORY_NO_TIME_CACHE
#else

        static class Time
        {

            // static

            public static double timeAsDouble;
            public static double unscaledTimeAsDouble;
            public static float deltaTime;
            public static float unscaledDeltaTime;
            public static int frameCount;

            // methods

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
                else { Dev.LogError(Messages.Exceptions.TimeCacheInjectionFailed); }
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
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier<T>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            Component Owner { get; }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            T Current { get; }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            void SetCurrent(T current);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IMapper<T>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            float GetLength(T to, T from);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            T Lerp(T current, T to, T diff, float rt);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            (T, T) GetParam(T from, T to, bool isDelta);
        }

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

        internal readonly struct PlanArg<C, M, T>
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
        {

            // fields

            internal readonly C Carrier;
            internal readonly M Mapper;
            internal readonly T To;
            internal readonly bool IsDelta;

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal PlanArg(C carrier, M mapper, T to, bool isDelta)
            {
                this.Carrier = carrier;
                this.Mapper = mapper;
                this.To = to;
                this.IsDelta = isDelta;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal PlanArg(C carrier, T to, bool isDelta)
            {
                this.Carrier = carrier;
                this.Mapper = default;
                this.To = to;
                this.IsDelta = isDelta;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct TimeArg
        {

            // fields

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly float Interval;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly float Speed;

            // constructors

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TimeArg(float interval = 0f, float speed = 0f)
            {
                this.Interval = interval;
                this.Speed = speed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Story.Task Create<S, C, M, T, E>(in PlanArg<C, M, T> planArg, in TimeArg timeArg, E ease, ref double start)
            where S : struct, Story.IStepper
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where E : struct, Story.IEase
            => Task(new Creator<S, C, M, T>(planArg, timeArg, ref start), ease).At(planArg.Carrier.Owner);

        // task
        static async Story.Task Task<S, C, M, T, E>(Creator<S, C, M, T> creator, E ease)
            where S : struct, Story.IStepper
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where E : struct, Story.IEase
        {
            var updater = creator.CreateUpdater();
            while (updater.Step(ease)) { await Story.Yield; }
        }

        readonly struct Creator<S, C, M, T>
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where S : struct, Story.IStepper
        {
            // fields

            readonly PlanArg<C, M, T> planArg;
            readonly TimeArg timeArg;
            readonly double start;

            // constructors

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる（インライン化するとeaseごとに生成されるので）
            internal Creator(in PlanArg<C, M, T> planArg, in TimeArg timeArg, ref double start)
            {
                this.planArg = planArg;
                this.timeArg = timeArg;
                this.start = start;
                if (float.Epsilon < this.timeArg.Speed)
                {
                    var length = planArg.Mapper.GetLength(planArg.To , planArg.IsDelta ? planArg.Carrier.Current : default);
                    start += length / this.timeArg.Speed;
                }
                else
                {
                    start += this.timeArg.Interval;
                }
            }

            // methods

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる（インライン化するとeaseごとに生成されるので）
            internal Updater<S, C, M, T> CreateUpdater()
            {
                // from 確定
                var from = this.planArg.Carrier.Current;
                var to = this.planArg.To;

                // interval 確定
                var interval = this.timeArg.Interval;
                if (float.Epsilon < this.timeArg.Speed)
                {
                    var length = this.planArg.Mapper.GetLength(this.planArg.To , this.planArg.IsDelta ? this.planArg.Carrier.Current : default);
                    interval = length / this.timeArg.Speed;

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.timeArg.Interval) && this.timeArg.Interval != interval) { Dev.LogWarning(Messages.Warnings.MovementDurationChanged); }
#endif

                }

                // 生成
                var stepper = new S();
                var start = this.start;
                stepper.Setup(interval, ref start);
                var prm = this.planArg.Mapper.GetParam(from, to, this.planArg.IsDelta);
                return new(stepper, this.planArg, prm.Item1, prm.Item2);
            }
        }

        struct Updater<S, C, M, T> : Story.IUpdater
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where S : struct, Story.IStepper
        {

            // fields

            S stepper;
            C carrier;
            readonly M mapper;
            readonly T to, diff;

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Updater(in S stepper, PlanArg<C, M, T> planArg, T to, T diff)
            {
                this.stepper = stepper;
                this.carrier = planArg.Carrier;
                this.mapper = planArg.Mapper;
                this.to = to;
                this.diff = diff;
            }

            // methods

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Step<E>(E ease)
                where E : struct, Story.IEase
                => this.stepper.Step(this, ease);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void Update(float now)
                => this.carrier.SetCurrent(this.mapper.Lerp(this.carrier.Current, this.to, this.diff, 1f - now));
        }
    }
}
