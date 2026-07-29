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
    using UnityEngine;
    using TMPro;
    using Omochaya.HiddenStory;

#if UNITY_EDITOR
    static class StorySample
    {
        public static async Story.Task Main(RectTransform rt, CanvasGroup cg)
        {
            var start = Story.GetStart();
            await rt.MoveLocalPosition().XTo(1f).Interval(2f, Story.Ease.None, ref start);
            await rt.MoveLocalPosition().YZAdd(10f, 30f).Speed(2f);
            await rt.MoveAnchoredPosition().To(Vector2.up * 10f).Speed(2f);
            await cg.MoveAlpha().To(0.5f).Interval(2f);
        }
    }
#endif

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

    static class StoryFloat
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<float> {}

        // Carrier0:CanvasGroup.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly CanvasGroup self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.alpha = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier0 MoveAlpha(this CanvasGroup self) => new(self);

        // Carrier1:TMP_Text.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.alpha = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier1 MoveAlpha(this TMP_Text self) => new(self);

        // Carrier2:TMP_Text.fontSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.fontSize;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.fontSize = value;
        }

        /// <summary></summary>
        public static Carrier2 MoveFontSize(this TMP_Text self) => new(self);

        // Carrier3:TMP_Text.maxVisibleCharacters -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier3 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier3(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.maxVisibleCharacters;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.maxVisibleCharacters = Mathf.FloorToInt(value);
        }

        /// <summary></summary>
        public static Carrier3 MoveVisibleCount(this TMP_Text self) => new(self);

        // Carrier4:AudioSource.volume -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier4 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier4(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.volume;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.volume = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier4 MoveVolume(this AudioSource self) => new(self);


        // Carrier5:AudioSource.pitch -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier5 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier5(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.pitch;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.pitch = value;
        }

        /// <summary></summary>
        public static Carrier5 MovePitch(this AudioSource self) => new(self);

        // Carrier6:Camera.fieldOfView -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier6 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier6(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.fieldOfView;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.fieldOfView = Mathf.Clamp(value, 0.1f, 179.9f);
        }

        /// <summary></summary>
        public static Carrier6 MoveFov(this Camera self) => new(self);

        // Carrier7:Camera.orthographicSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier7 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier7(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.orthographicSize;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.orthographicSize = value;
        }

        /// <summary></summary>
        public static Carrier7 MoveOrthoSize(this Camera self) => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<float, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(float current) => new(current);
                /// <summary>Don't touch! Only for system.</summary>
                public float Set(float current, Mover.Param1 prm)
                    => prm.P0;
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<float, C, Changer.To, Mover.Param1> To<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<float, C, Changer.To, Mover.Param1> Add<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector2
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector2> {}

        // Carrier0:RectTransform.anchoredPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.anchoredPosition;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.anchoredPosition = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveAnchoredPosition(this RectTransform self)
            => new(self);

        // Carrier1:RectTransform.sizeDelta -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.sizeDelta;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.sizeDelta = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveSizeDelta(this RectTransform self)
            => new(self);

        // Carrier2:RectTransform.pivot -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.pivot;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.pivot = value;
        }

        /// <summary></summary>
        public static Carrier2 MovePivot(this RectTransform self)
            => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XTo : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector2 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 To(float p0) => new(p0);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YTo : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector2 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Vector2, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector2 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.To, Mover.Param2> To<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.To, Mover.Param2> Add<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector3
    {

        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector3> {}

        // Carrier0:Transform.localPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localPosition;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localPosition = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveLocalPosition(this Transform self)
            => new(self);

        // Carrier1:Transform.localScale -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localScale;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localScale = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveLocalScale(this Transform self)
            => new(self);

        // Carrier2:Transform.localEulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localEulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localEulerAngles = value;
        }

        /// <summary></summary>
        public static Carrier2 MoveLocalEulerAngles(this Transform self)
            => new(self);

        // Carrier3:Transform.position -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier3 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier3(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.position;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.position = value;
        }

        /// <summary></summary>
        public static Carrier3 MovePosition(this Transform self)
            => new(self);

        // Carrier4:Transform.eulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier4 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier4(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.eulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.eulerAngles = value;
        }

        /// <summary></summary>
        public static Carrier4 MoveEulerAngles(this Transform self)
            => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ZTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.z = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XYTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YZTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ZXTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.z, current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.z = prm.P0;
                    current.x = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Vector3, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Vector3 current) => new(current.x, current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.z = prm.P2;
                    return current;
                }
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.ZTo, Mover.Param1> ZTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.ZTo, Mover.Param1> ZAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.XYTo, Mover.Param2> XYTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.XYTo, Mover.Param2> XYAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.YZTo, Mover.Param2> YZTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.YZTo, Mover.Param2> YZAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.ZXTo, Mover.Param2> ZXTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.ZXTo, Mover.Param2> ZXAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.To, Mover.Param3> To<C>(this C carrier, Vector3 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.To, Mover.Param3> Add<C>(this C carrier, Vector3 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    // ToDo...
    static class StoryColor
    {
        // ToDo. SpriteRenderer.color

        // ToDo. TextMeshPro.color
        // ToDo. TextMeshProUGUI.color
    }

    // ToDo...
    static class StoryQuaternion
    {
        // ToDo. Transform.localRotation
        // ToDo. Transform.rotation
    }

    // ToDo...（たぶん非公開にする）
    static class StoryRect
    {
    }
}

// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
// これ以降は間接的に使用されます。利用者が直接使用することは想定していません
// 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
namespace Omochaya.HiddenStory
{
    using System;
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
        public static Param1 CreateParam(float p0) => new Param1(p0);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param2 CreateParam(float p0, float p1) => new Param2(p0, p1);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param3 CreateParam(float p0, float p1, float p2) => new Param3(p0, p1, p2);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(float p0, float p1, float p2, float p3) => new Param4(p0, p1, p2, p3);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param2 CreateParam(Vector2 p) => new Param2(p.x, p.y);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param3 CreateParam(Vector3 p) => new Param3(p.x, p.y, p.z);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(Rect p) => new Param4(p.x, p.y, p.width, p.height);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(Quaternion p) => new Param4(p.x, p.y, p.z, p.w);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param2 CreateParam(ValueTuple<float, float> p) => new Param2(p.Item1, p.Item2);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param3 CreateParam(ValueTuple<float, float, float> p) => new Param3(p.Item1, p.Item2, p.Item3);
        /// <summary>Don't touch! Only for system.</summary>
        public static Param4 CreateParam(ValueTuple<float, float, float, float> p) => new Param4(p.Item1, p.Item2, p.Item3, p.Item4);

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
