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

        /// <summary></summary>
        public static Task Interval<P, E>(this P plan, float interval, in E ease, ref double start)
            where P : struct, Mover.IPlan
            where E : struct, IEase
        {
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
    }
}

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using UnityEngine;

    /// <summary>Don't touch! Only for system.</summary>
    public static class Mover
    {
        // param ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            float Length { get; }
            /// <summary>Don't touch! Only for system.</summary>
            P Lerp(in P b, float now);
            /// <summary>Don't touch! Only for system.</summary>
            P Add(in P b);
            /// <summary>Don't touch! Only for system.</summary>
            P Sub(in P b);
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param1 : IParam<Param1>
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly float P0;
            /// <summary>Don't touch! Only for system.</summary>
            public float Length => Mathf.Abs(this.P0);
            /// <summary>Don't touch! Only for system.</summary>
            internal Param1(float p0)
            {
                this.P0 = p0;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param1 Lerp(in Param1 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t));
            /// <summary>Don't touch! Only for system.</summary>
            public Param1 Add(in Param1 b) => new(this.P0 + b.P0);
            /// <summary>Don't touch! Only for system.</summary>
            public Param1 Sub(in Param1 b) => new(this.P0 - b.P0);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param2 : IParam<Param2>
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly float P0, P1;
            /// <summary>Don't touch! Only for system.</summary>
            public float Length => new Vector2(this.P0, this.P1).magnitude; // 記述量減らしたいので（重くもないはず）
            /// <summary>Don't touch! Only for system.</summary>
            internal Param2(float p0, float p1)
            {
                this.P0 = p0;
                this.P1 = p1;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param2 Lerp(in Param2 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t));
            /// <summary>Don't touch! Only for system.</summary>
            public Param2 Add(in Param2 b) => new(this.P0 + b.P0, this.P1 + b.P1);
            /// <summary>Don't touch! Only for system.</summary>
            public Param2 Sub(in Param2 b) => new(this.P0 - b.P0, this.P1 - b.P1);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param3 : IParam<Param3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly float P0, P1, P2;
            /// <summary>Don't touch! Only for system.</summary>
            public float Length => new Vector3(this.P0, this.P1, this.P2).magnitude;
            /// <summary>Don't touch! Only for system.</summary>
            internal Param3(float p0, float p1, float p2)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param3 Lerp(in Param3 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t));
            /// <summary>Don't touch! Only for system.</summary>
            public Param3 Add(in Param3 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2);
            /// <summary>Don't touch! Only for system.</summary>
            public Param3 Sub(in Param3 b) => new(this.P0 - b.P0, this.P1 - b.P1, this.P2 - b.P2);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param4 : IParam<Param4>
        {
            /// <summary>Don't touch! Only for system.</summary>
            internal readonly float P0, P1, P2, P3;
            /// <summary>Don't touch! Only for system.</summary>
            public float Length => new Vector4(this.P0, this.P1, this.P2, this.P3).magnitude;
            /// <summary>Don't touch! Only for system.</summary>
            internal Param4(float p0, float p1, float p2, float p3)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
                this.P3 = p3;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param4 Lerp(in Param4 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t),
                Mathf.LerpUnclamped(this.P3, b.P3, t));
            /// <summary>Don't touch! Only for system.</summary>
            public Param4 Add(in Param4 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2, this.P3 + b.P3);
            /// <summary>Don't touch! Only for system.</summary>
            public Param4 Sub(in Param4 b) => new(this.P0 - b.P0, this.P1 - b.P1, this.P2 - b.P2, this.P3 - b.P3);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct ParamQ : IParam<ParamQ>
        {
            internal readonly Quaternion Q;
            /// <summary>Don't touch! Only for system.</summary>
            public float Length => Quaternion.Angle(Quaternion.identity, this.Q); // 角度にしておく
            /// <summary>Don't touch! Only for system.</summary>
            internal ParamQ(Quaternion q) => this.Q = q;
            /// <summary>Don't touch! Only for system.</summary>
            public ParamQ Lerp(in ParamQ b, float t) 
                => new(Quaternion.SlerpUnclamped(this.Q, b.Q, t));
            /// <summary>Don't touch! Only for system.</summary>
            public ParamQ Add(in ParamQ b) => new(this.Q * b.Q); // 順変換にしておく
            /// <summary>Don't touch! Only for system.</summary>
            public ParamQ Sub(in ParamQ b) => new(this.Q * Quaternion.Inverse(b.Q)); // 逆変換にしておく
        }

        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier<T>
        {
            /// <summary>Don't touch! Only for system.</summary>
            Component Self { get; }
            /// <summary>Don't touch! Only for system.</summary>
            T Current { get; }
            /// <summary>Don't touch! Only for system.</summary>
            void SetCurrent(T current);
        }

        // changer ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface IChanger<T, P>
            where P : struct, IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            P Get(T current);
            /// <summary>Don't touch! Only for system.</summary>
            T Set(T current, P prm);
        }

        // plan ~~~~~~~~~~~~~~~~~~~~~~~~~~~
        public interface IPlan
        {
            Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start) where E : struct, Story.IEase;
        }

        public static Story.Task CreateTaskCore<T, C, H, P, E>(H changer, P dummy, T to, C carrier, bool isDelta, float interval, float speed, E ease, ref double start)
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
            where E : struct, Story.IEase
        {
            var ret = Task<T, C, H, P, E>(changer, changer.Get(to), carrier, isDelta, interval, speed, ease, start);
            if (float.Epsilon < speed)
            {
                var length = isDelta ? changer.Get(to).Length : changer.Get(to).Sub(changer.Get(carrier.Current)).Length;
                interval = length / speed;
            }
            start += interval;
            return ret;
        }

        static async Story.Task Task<T, C, H, P, E>(H changer, P to, C carrier, bool isDelta, float interval, float speed, E ease, double start)
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
            where E : struct, Story.IEase
        {
            TryKeep(carrier.Self);

            var from = changer.Get(carrier.Current);

            var trueInterval = interval;
            if (float.Epsilon < speed)
            {
                if (isDelta) { trueInterval = to.Length / speed; }
                else { trueInterval = from.Sub(to).Length / speed; }

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                // 変化したら警告
                if ((float.Epsilon < interval) && interval != trueInterval) { Dev.LogWarning("移動期間が変化しました"); }
#endif

            }
            var passer = new Story.Passer(trueInterval, ref start);

            if (isDelta) { to = from.Add(to); }
            var updater = new Updater<T, C, H, P>(carrier, changer, from, to);

            while (passer.Bundle(updater, ease)) { await Story.Yield; }
        }

        static void TryKeep(Component self)
        {
            if (Story.IsTryKeeped)
            {
                var owner = self.GetComponent<Story.ITaskOwner>() as Component ?? self;
                if (owner != null) { Story.At(owner); }
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        readonly struct Updater<T, C, H, P> : Story.IUpdater
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly C carrier;
            readonly H changer;
            readonly P from, to;

            /// <summary>Don't touch! Only for system.</summary>
            internal Updater(C carrier, H changer, in P from, in P to)
            {
                this.carrier = carrier;
                this.changer = changer;
                this.from = from;
                this.to = to;
            }

            /// <summary>Don't touch! Only for system.</summary>
            public void Update(float now)
                => this.carrier.SetCurrent(this.changer.Set(this.carrier.Current, this.from.Lerp(this.to, now)));
                // 防御的コピー発生するが carrier が持つ参照に対する操作なので問題にならない
                // というか問題にならないように参照に対する操作になるように実装すること！
        }
    }
}
