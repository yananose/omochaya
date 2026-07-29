// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryMover.cs" company="Omochaya">
//   Copyright (t) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using Omochaya.HiddenStory;

    public static partial class Story
    {
        // PlanTo - interval ===================================================================================

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float interval, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float interval, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f, start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float interval, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float interval, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float interval, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float interval)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, interval, 0f).GetTask(Ease.None);

        // PlanTo - speed ======================================================================================

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float speed, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float speed, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed, start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanTo<T, C, H, P> plan, float speed, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float speed, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float speed, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanTo<T, C, H, P> plan, float speed)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanTo<T, C, H, P>, T, C, H, P>(plan, 0f, speed).GetTask(Ease.None);

        // PlanAdd - interval ===================================================================================

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float interval, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float interval, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f, start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float interval, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float interval, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float interval, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float interval)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, interval, 0f).GetTask(Ease.None);

        // PlanAdd - speed ======================================================================================

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float speed, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float speed, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed, start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.PlanAdd<T, C, H, P> plan, float speed, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float speed, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float speed, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.PlanAdd<T, C, H, P> plan, float speed)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.PlanAdd<T, C, H, P>, T, C, H, P>(plan, 0f, speed).GetTask(Ease.None);
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
            public ParamQ Add(in ParamQ b) => new(this.Q * b.Q); // 乗算にしておく
            /// <summary>Don't touch! Only for system.</summary>
            public ParamQ Sub(in ParamQ b) => new(this.Q * Quaternion.Inverse(b.Q)); // 除算にしておく
        }

        /// <summary>Don't touch! Only for system.</summary>
        public static Param1 CreateParam(float p0) => new(p0);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param2 CreateParam(float p0, float p1) => new(p0, p1);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param3 CreateParam(float p0, float p1, float p2) => new(p0, p1, p2);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(float p0, float p1, float p2, float p3) => new(p0, p1, p2, p3);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param2 CreateParam(Vector2 p) => new(p.x, p.y);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param3 CreateParam(Vector3 p) => new(p.x, p.y, p.z);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(Color p) => new(p.r, p.g, p.b, p.a);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(Rect p) => new(p.x, p.y, p.width, p.height);
        /// <summary>Don't touch! Only for system.</summary>
        public static ParamQ CreateParam(Quaternion p) => new(p);

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

        /// <summary>Don't touch! Only for system.</summary>
        public interface IPlan<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            Component Self { get; }
            /// <summary>Don't touch! Only for system.</summary>
            Updater<T, C, H, P> CreateUpdater();
            /// <summary>Don't touch! Only for system.</summary>
            float GetInterval(float speed);
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct PlanTo<T, C, H, P> : IPlan<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly C carrier;
            readonly H changer;
            readonly P to;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.carrier.Self;
            /// <summary>Don't touch! Only for system.</summary>
            internal PlanTo(C carrier, H changer, in P to)
            {
                this.carrier = carrier;
                this.changer = changer;
                this.to = to;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Updater<T, C, H, P> CreateUpdater()
            {
                var from = this.changer.Get(this.carrier.Current);
                return new(this.carrier, this.changer, from, this.to);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public float GetInterval(float speed)
                => this.changer.Get(this.carrier.Current).Sub(this.to).Length / speed;
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct PlanAdd<T, C, H, P> : IPlan<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly C carrier;
            readonly H changer;
            readonly P delta;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.carrier.Self;
            /// <summary>Don't touch! Only for system.</summary>
            internal PlanAdd(C carrier, H changer, in P delta)
            {
                this.carrier = carrier;
                this.changer = changer;
                this.delta = delta;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Updater<T, C, H, P> CreateUpdater()
            {
                var from = this.changer.Get(this.carrier.Current);
                var to = from.Add(this.delta);
                return new(this.carrier, this.changer, from, to);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public float GetInterval(float speed)
                => this.delta.Length / speed;
        }

        // Updater ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Updater<T, C, H, P> : Story.IUpdater
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


        // Builder ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        internal readonly struct Builder<I, T, C, H, P>
            where I : struct, IPlan<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly I plan;
            readonly double start;
            readonly float interval;
            readonly float speed;

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I plan, float interval, float speed)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.plan = plan;
                this.start = Story.GetStart();

                this.interval = interval;
                this.speed = speed;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I plan, float interval, float speed, double start)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.plan = plan;
                this.start = start;

                this.interval = interval;
                this.speed = speed;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I plan, float interval, float speed, ref double start)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.plan = plan;
                this.start = start;

                if (float.Epsilon < speed) { interval = plan.GetInterval(speed); }
                start += interval;

                this.interval = interval;
                this.speed = speed;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal Story.Task GetTask<E>(in E ease)
                where E : struct, Story.IEase => Task(ease);

            async Story.Task Task<E>(E ease)
                where E : struct, Story.IEase
            {
                TryKeep(this.plan.Self);
                var interval = this.interval;
                if (float.Epsilon < this.speed)
                {
                    interval = this.plan.GetInterval(this.speed);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.interval) && this.interval != interval) { Dev.LogWarning("移動期間が変化しました"); }
#endif

                }
                var start = this.start;
                var passer = new Story.Passer(interval, ref start);
                var updater = this.plan.CreateUpdater();
                while (passer.Bundle(updater, ease)) { await Story.Yield; }
            }
        }
        static void TryKeep(Component self)
        {
            if (Story.IsTryKeeped)
            {
                var owner = self.GetComponent<Story.ITaskOwner>() as Component ?? self;
                if (owner != null) { Story.At(owner); }
            }
        }
    }
}
