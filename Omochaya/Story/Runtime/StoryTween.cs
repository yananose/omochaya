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
            return float.Epsilon < interval ? TweenTask(default(ScaledTime), interval, updater, ease, ref start)
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
            return float.Epsilon < speed ? TweenTask(default(ScaledTime), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
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
            return float.Epsilon < interval ? TweenTask(default(UnscaledTime), interval, updater, ease, ref start)
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
            return float.Epsilon < speed ? TweenTask(default(UnscaledTime), Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
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
            internal UpdaterImpl(T args, Action<T, float> action)
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
        static Task TweenTask<TS, U, E>(TS _, float interval, in U updater, E ease, ref double start)
            where TS : struct, ITimeSource
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var stepper = new Stepper<TS>(interval, ref start);
            return IntervalTask(stepper, updater, ease);
        }

        // task
        static async Task IntervalTask<TS, U, E>(Stepper<TS> stepper, U updater, E ease)
            where TS : struct, ITimeSource
            where U : struct, IUpdater
            where E : struct, IEase
        {
            TaskCancelMode = CancelMode.Drop;
            while (stepper.Step(updater, ease)) { await Yield; }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ITimeSource
        {
            double TimeAsDouble { get; }
            double DeltaTime { get; }
            bool IsSkip { get; }
        }

        internal readonly struct ScaledTime : ITimeSource
        {
            public double TimeAsDouble => Time.timeAsDouble;
            public double DeltaTime => Time.deltaTime;
            public bool IsSkip => false;
        }

        internal readonly struct UnscaledTime : ITimeSource
        {
            public double TimeAsDouble => Time.unscaledTimeAsDouble;
            public double DeltaTime => Time.unscaledDeltaTime;
            public bool IsSkip => false;
        }

        internal struct Stepper<TS>
            where TS : struct, ITimeSource
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
            internal Stepper(float interval, ref double start)
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
                if (default(TS).IsSkip)
                {
                    this.seek = 1f;
                    return;
                }

                var timeAsDouble = default(TS).TimeAsDouble;
                var diff = timeAsDouble - this.prev;
                if (diff < 0)
                {
#if UNITY_EDITOR
                    if (timeAsDouble < this.start) { /*Dev.Log(string.Format(Messages.Warnings.TimeRewoundDelayedStart, diff));*/ }
                    else { Dev.LogWarning(string.Format(Messages.Warnings.TimeRewoundA, diff)); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = default(TS).DeltaTime;
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
                Proceed();
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
            return plan.CreateTask(default(ScaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task executed over a specific interval with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(default(ScaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task executed over a specific interval, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(default(ScaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task executed over a specific interval, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Interval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(default(ScaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task driven by movement speed and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(default(ScaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation task driven by movement speed and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(default(ScaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task driven by movement speed, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(default(ScaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation linear task driven by movement speed, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Speed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(default(ScaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task executed over a specific interval with easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(default(UnscaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task executed over a specific interval with easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P, E>(this P plan, float interval, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return plan.CreateTask(default(UnscaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task executed over a specific interval, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(default(UnscaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task executed over a specific interval, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledInterval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetUnscaledStart();
            var ease = Ease.None;
            return plan.CreateTask(default(UnscaledTime), new(interval, 0f), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task driven by movement speed and easing, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(default(UnscaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled time task driven by movement speed and easing, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P, E>(this P plan, float speed, E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetUnscaledStart();
            return plan.CreateTask(default(UnscaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task driven by movement speed, starting at a designated time.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(default(UnscaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Converts a structural tween plan into a zero-allocation unscaled linear time task driven by movement speed, starting immediately.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task UnscaledSpeed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetUnscaledStart();
            var ease = Ease.None;
            return plan.CreateTask(default(UnscaledTime), new(0f, speed, true), ease, ref start);
        }

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this tween.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warmup<P, E>(this P plan, E ease, int count = 0)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            plan.CreateDummy(default(ScaledTime), ease).Warmup(count);
        }

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this tween.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void Warmup<P>(this P plan, int count = 0)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            plan.CreateDummy(default(ScaledTime), ease).Warmup(count);
        }

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this tween.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnscaledWarmup<P, E>(this P plan, E ease, int count = 0)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            plan.CreateDummy(default(UnscaledTime), ease).Warmup(count);
        }

        /// <summary>Warmups the global pool capacity for the underlying state machine type associated with this tween.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static void UnscaledWarmup<P>(this P plan, int count = 0)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            plan.CreateDummy(default(UnscaledTime), ease).Warmup(count);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Component GetOwner(Component self, Component owner)
        {
            if (self == null) { return null; }
            if (owner != null) { return owner; }
            if (self.TryGetComponent<ITaskOwner>(out var other)) { return (Component)other; }
            return self;
        }

#if STORY_NO_TIME_CACHE
#else

        /// <summary></summary>
        public static class Time
        {

            // static

            /// <summary>The double precision time at the beginning of this frame. This is the time in seconds since the start of the game.</summary>
            public static double timeAsDouble;
            /// <summary>The double precision timeScale-independent time for this frame. This is the time in seconds since the start of the game.</summary>
            public static double unscaledTimeAsDouble;
            /// <summary>The interval in seconds from the last frame to the current one.</summary>
            public static float deltaTime;
            /// <summary>The timeScale-independent interval in seconds from the last frame to the current one.</summary>
            public static float unscaledDeltaTime;
            /// <summary>The total number of frames since the start of the game.</summary>
            public static int frameCount;

            // methods

            static void UpdateCache()
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
            internal static bool AppendToPhase<TTargetPhase>(ref PlayerLoopSystem rootLoop, PlayerLoopSystem customLoop)
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
        public interface ICarrierProperty<C, T>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            T Get(C component);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            void Set(C component, T value);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier<CS, T, CP> : ICarrier<T>
            where CS : Component
            where CP : struct, ICarrierProperty<CS, T>
        {
            // fields

            readonly CS self;
            readonly Component owner;

            // properties

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Owner => this.owner;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public T Current => default(CP).Get(this.self);

            // constructors

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Carrier(CS self, Component owner) 
            { 
                this.self = self; 
                this.owner = Story.GetOwner(self, owner); 
            }

            // methods

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(T value) => default(CP).Set(this.self, value);
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

        internal interface IAxisFlag
        {
            float Lerp(float current, float to, float diff, float rt);
            float GetSqDiff(float to, float from);
        }

        internal readonly struct AxisUse : IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Lerp(float current, float to, float diff, float rt) => to + diff * rt;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetSqDiff(float to, float from)
            {
                var diff = to - from;
                return diff * diff;
            }
        }

        internal readonly struct AxisIgnore : IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Lerp(float current, float to, float diff, float rt) => current;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetSqDiff(float to, float from) => 0f;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IPlan
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            Story.Task CreateTask<TS, E>(in TS spepper, in TimeArg arg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            Story.Task CreateDummy<TS, E>(in TS spepper, E ease)
                where TS : struct, Story.ITimeSource
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
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly bool IsSpeed;

            // constructors

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public TimeArg(float interval = 0f, float speed = 0f, bool isSpeed = false)
            {
                Dev.Assert(0f <= interval && 0f <= speed);
                this.Interval = interval;
                this.Speed = speed;
                this.IsSpeed = isSpeed;
            }
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Story.Task CreateTask<TS, C, M, T, E>(in PlanArg<C, M, T> planArg, in TimeArg timeArg, E ease, ref double start)
            where TS : struct, Story.ITimeSource
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where E : struct, Story.IEase
            => Task(new Creator<TS, C, M, T>(planArg, timeArg, ref start), ease).At(planArg.Carrier.Owner);

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static Story.Task CreateDummy<TS, C, M, T, E>(E ease)
            where TS : struct, Story.ITimeSource
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where E : struct, Story.IEase
            => Task(default(Creator<TS, C, M, T>), ease);

        // task
        static async Story.Task Task<TS, C, M, T, E>(Creator<TS, C, M, T> creator, E ease)
            where TS : struct, Story.ITimeSource
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where E : struct, Story.IEase
        {
            var updater = creator.CreateUpdater();
            if (!updater.IsValid) { return; }
            Story.TaskCancelMode = Story.CancelMode.Drop;
            while (updater.Step(ease)) { await Story.Yield; }
        }

        readonly struct Creator<TS, C, M, T>
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where TS : struct, Story.ITimeSource
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
                if (!this.timeArg.IsSpeed)
                {
                    start += this.timeArg.Interval;
                }
                else if (float.Epsilon < this.timeArg.Speed)
                {
                    var length = planArg.Mapper.GetLength(planArg.To , planArg.IsDelta ? default : planArg.Carrier.Current);
                    start += length / this.timeArg.Speed;
                }
            }

            // methods

            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる（インライン化するとeaseごとに生成されるので）
            internal Updater<TS, C, M, T> CreateUpdater()
            {
                // from 確定
                var from = this.planArg.Carrier.Current;
                var to = this.planArg.To;

                // interval 確定
                var interval = this.timeArg.Interval;
                if (float.Epsilon < this.timeArg.Speed)
                {
                    var length = this.planArg.Mapper.GetLength(to , this.planArg.IsDelta ? default : from);
                    interval = length / this.timeArg.Speed;

#if (STORY_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.timeArg.Interval) && this.timeArg.Interval != interval) { Dev.LogWarning(Messages.Warnings.MovementDurationChanged); }
#endif
                }
                else if (this.timeArg.IsSpeed)
                {
                    // 速度が小さい場合は開始地点で終了させる
                    this.planArg.Carrier.SetCurrent(from);
                    return default;
                }
                else if (interval <= float.Epsilon)
                {
                    // 間隔が小さい場合は終了地点で終了させる
                    this.planArg.Carrier.SetCurrent(to);
                    return default;
                }

                // 生成
                var start = this.start;
                var stepper = new Story.Stepper<TS>(interval, ref start);
                var prm = this.planArg.Mapper.GetParam(from, to, this.planArg.IsDelta);
                return new(stepper, this.planArg, prm.Item1, prm.Item2);
            }
        }

        struct Updater<TS, C, M, T> : Story.IUpdater
            where C : struct, ICarrier<T>
            where M : struct, IMapper<T>
            where TS : struct, Story.ITimeSource
        {
            // fields

            Story.Stepper<TS> stepper;
            C carrier;
            readonly M mapper;
            readonly T to, diff;
            internal readonly bool IsValid;

            // constructors

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Updater(in Story.Stepper<TS> stepper, PlanArg<C, M, T> planArg, T to, T diff)
            {
                this.stepper = stepper;
                this.carrier = planArg.Carrier;
                this.mapper = planArg.Mapper;
                this.to = to;
                this.diff = diff;
                this.IsValid = true;
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
