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
        // デフォルトでは（0歩目ではなく）1歩目から始まり到達したら終わる。0歩目からなど開始位置を指定したいときは ref start で指定すること。
        public static double GetStart() => Time.timeAsDouble - Time.deltaTime;

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Task Tween<U, E>(float interval, in U updater, in E ease, ref double start)
            where U : struct, IUpdater
            where E : struct, IEase
        {
            Dev.Assert(0f <= interval);
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
            Dev.Assert(0f <= speed);
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
            while (passer.Step(updater, ease)) { await Yield; }
        }

        internal struct Passer
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
                get { Dev.Assert(float.Epsilon < interval); return seek / interval; }
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
                prev = timeAsDouble;

                var seek = (float)(timeAsDouble - this.start);
#if UNITY_EDITOR
                if (seek < 0f) { Dev.LogWarning($"巻き戻ってるので実行しない(B)：{seek}"); }
#endif
                this.seek = seek;
                return;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal bool Step<U, E>(in U updater, in E ease)
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

        /// <summary></summary>
        public static Task Interval<P, E>(this P plan, float interval, in E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(interval, 0f, ease, ref start);
        }

        /// <summary></summary>
        public static Task Interval<P, E>(this P plan, float interval, in E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(interval, 0f, ease, ref start);
        }

        /// <summary></summary>
        public static Task Interval<P>(this P plan, float interval, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(interval, 0f, ease, ref start);
        }

        /// <summary></summary>
        public static Task Interval<P>(this P plan, float interval)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(interval, 0f, ease, ref start);
        }

        /// <summary></summary>
        public static Task Speed<P, E>(this P plan, float speed, in E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            return plan.CreateTask(0f, speed, ease, ref start);
        }

        /// <summary></summary>
        public static Task Speed<P, E>(this P plan, float speed, in E ease)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
            var start = GetStart();
            return plan.CreateTask(0f, speed, ease, ref start);
        }

        /// <summary></summary>
        public static Task Speed<P>(this P plan, float speed, ref double start)
            where P : struct, Mover.IPlan
        {
            var ease = Ease.None;
            return plan.CreateTask(0f, speed, ease, ref start);
        }

        /// <summary></summary>
        public static Task Speed<P>(this P plan, float speed)
            where P : struct, Mover.IPlan
        {
            var start = GetStart();
            var ease = Ease.None;
            return plan.CreateTask(0f, speed, ease, ref start);
        }

#if STORY_TIME_CACHE

        static class Time
        {
            public static double timeAsDouble;
            public static double unscaledTimeAsDouble;
            public static float deltaTime;
            public static int frameCount;
            public static void UpdateCache()
            {
                timeAsDouble = UnityEngine.Time.timeAsDouble;
                unscaledTimeAsDouble = UnityEngine.Time.unscaledTimeAsDouble;
                deltaTime = UnityEngine.Time.deltaTime;
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
            Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start) where E : struct, Story.IEase;
        }

        internal static Story.Task Create<T, P, M, C, E>(M _, P __, T to, C carrier, bool isDelta, float interval, float speed, in E ease, ref double start)
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
            where E : struct, Story.IEase
            => Task(new Builder<T, P, M, C>(new Target<T, P, M>(to), carrier, isDelta, interval, speed, ref start), ease);

        static async Story.Task Task<T, P, M, C, E>(Builder<T, P, M, C> builder, E ease)
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
            internal Target(T to)
            {
                this.to = new M().Get(to);
            }
            internal P Get(T p) => new M().Get(p);
            internal float Distance(T p) => this.To.Sub(this.Get(p)).Length;
        }

        readonly struct Builder<T, P, M, C>
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
        {
            readonly Target<T, P, M> target;
            readonly C carrier;
            readonly bool isDelta;
            readonly float interval;
            readonly float speed;
            readonly double start;
            internal Builder(Target<T, P, M> target, C carrier, bool isDelta, float interval, float speed, ref double start)
            {
                this.target = target;
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.interval = interval;
                this.speed = speed;
                this.start = start;
                if (float.Epsilon < speed)
                {
                    var length = isDelta ? this.target.To.Length : this.target.Distance(carrier.Current);
                    interval = length / speed;
                }
                start += interval;
            }

            internal Updater<T, P, M, C> Build()
            {
                // owner 確定
                TryKeep(this.carrier.Self);

                // from 確定
                var from = this.target.Get(this.carrier.Current);
                var to = this.target.To;

                // interval 確定
                var interval = this.interval;
                if (float.Epsilon < this.speed)
                {
                    if (this.isDelta) { interval = to.Length / this.speed; }
                    else { interval = from.Sub(to).Length / this.speed; }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.interval) && this.interval != interval) { Dev.LogWarning("移動期間が変化しました"); }
#endif

                }

                // to 確定
                if (this.isDelta) { to = from.Add(to); }

                // 生成
                return new(start, interval, this.carrier, from, to);
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

        struct Updater<T, P, M, C> : Story.IUpdater
            where P : struct, IParam<P>
            where M : struct, IMapper<T, P>
            where C : struct, ICarrier<T>
        {
            Story.Passer passer;
            C carrier; // 読み取り専用にできるらしいが、したら Update で更新されなくならないか？
            readonly P from, to;
            internal Updater(double start, float interval, C carrier, P from, P to)
            {
                this.passer = new Story.Passer(interval, ref start);
                this.carrier = carrier;
                this.from = from;
                this.to = to;
            }
            internal bool Step<E>(in E ease)
                where E : struct, Story.IEase
                => this.passer.Step(this, ease);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void Update(float now)
                => this.carrier.SetCurrent(new M().Set(this.carrier.Current, this.from.Lerp(this.to, now)));
        }
    }
}
