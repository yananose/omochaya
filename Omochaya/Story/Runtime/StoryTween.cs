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

    public static partial class Story
    {
        /// <summary></summary>
        // デフォルトでは（0歩目ではなく）1歩目から始まり到達したら終わる。0歩目からなど開始位置を指定したいときは ref start で指定すること。
        public static double GetStart() => Time.timeAsDouble - Time.deltaTime;

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, in E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Debug.Assert(0f <= interval);
            return float.Epsilon < interval ? TweenTask(interval, updater, ease, ref start)
                : ImmediateTask(updater, ease.Calc(1f)); // interval = 0 : 即終了
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, in E ease)
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
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, in E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(interval, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<E, T>(float interval, T args, Action<T, float> update, in E ease)
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
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, in E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Debug.Assert(0f <= speed);
            return float.Epsilon < speed ? TweenTask(Mathf.Abs((to - from) / speed), updater, ease.FromTo(from, to), ref start)
                : ImmediateTask(updater, from); // speed = 0 : 始まらない
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float from, float to, float speed, in U updater, in E ease)
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
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, in E ease, ref double start)
            where E : struct, IEase
        {
            var updater = Updater(args, update);
            return Tween(from, to, speed, updater, ease, ref start);
        }

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<T, E>(float from, float to, float speed, T args, Action<T, float> update, in E ease)
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

        // updater

        /// <summary></summary>
        public interface IUpdater
        {
            /// <summary>Don't touch! Only for system.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            void Update(float now);
        }

        /// <summary></summary>
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
        static Task TweenTask<U, E>(float interval, in U updater, in E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            var passer = new Passer(interval, ref start);
            return IntervalTask(passer, updater, ease);
        }

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        static async Task IntervalTask<U, E>(Passer passer, U updater, E ease)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            while (passer.Bundle(updater, ease)) { await Yield; }
        }

        internal struct Passer
        {
            // fields
            double prev;
            double start;
            float interval;
            float seek;

            // properties
            readonly float Now
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                get { Debug.Assert(float.Epsilon < interval); return seek / interval; }
            }

            // constructors
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Passer(float interval, ref double start)
            {
                prev = this.start = start;
                this.interval = interval;
                seek = 0f;
                start = this.start + interval;
            }

            // methods
            void Proceed()
            {
                var timeAsDouble = Time.timeAsDouble;
                var diff = timeAsDouble - prev;
                if (diff <= 0)
                {
#if UNITY_EDITOR
                    if (diff == 0f) { Debug.Log($"巻き戻ってるので実行しない(interrupt？)：{diff}"); }
                    else if (timeAsDouble <= this.start) { Debug.Log($"巻き戻ってるので実行しない(SetStart で遅延起動？)：{diff}"); }
                    else { Debug.LogWarning($"巻き戻ってるので実行しない(A)：{diff}"); }
#endif
                    this.seek = -1f;
                    return;
                }

                double delta = Time.deltaTime;
                if (delta * 1.25 < diff)
                {
                    diff -= delta;
                    Debug.LogWarning($"実行してないフレームがあったので飛ばす：{diff}");
                    this.start += diff;
                }
                prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Debug.LogWarning($"巻き戻ってるので実行しない(B)：{seek}"); }
#endif
                this.seek = seek;
                return;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Bundle<U, E>(in U updater, in E ease)
                where U : struct, IUpdater
                where E : struct, IEase
            {
                Proceed();
                if (0f <= seek)
                {
                    if (interval <= seek)
                    {
                        updater.Update(ease.Calc(1f));
                        return false;
                    }
                    updater.Update(ease.Calc(Now));
                }
                return true;
            }
        }
    }
}

