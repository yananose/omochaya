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
        internal static Component GetOwner(Component self)  => self.GetComponent<Story.ITaskOwner>() as Component ?? self;

        // InfoTo - interval ===================================================================================

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float interval, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float interval, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f, start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float interval, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float interval, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float interval, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float interval)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, interval, 0f).GetTask(Ease.None);

        // InfoTo - speed ======================================================================================

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float speed, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float speed, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed, start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoTo<T, C, H, P> info, float speed, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float speed, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float speed, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoTo<T, C, H, P> info, float speed)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoTo<T, C, H, P>, T, C, H, P>(info, 0f, speed).GetTask(Ease.None);

        // InfoAdd - interval ===================================================================================

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float interval, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float interval, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f, start).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float interval, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f).GetTask(ease);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float interval, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float interval, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float interval)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, interval, 0f).GetTask(Ease.None);

        // InfoAdd - speed ======================================================================================

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float speed, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed, ref start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float speed, in E ease, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed, start).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this in Mover.InfoAdd<T, C, H, P> info, float speed, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed).GetTask(ease);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float speed, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed, ref start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float speed, double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed, start).GetTask(Ease.None);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this in Mover.InfoAdd<T, C, H, P> info, float speed)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => new Mover.Builder<Mover.InfoAdd<T, C, H, P>, T, C, H, P>(info, 0f, speed).GetTask(Ease.None);
    }

    static class StoryFloat
    {
        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<float> {}

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly CanvasGroup self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.alpha = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier0 MoveAlpha(this CanvasGroup self) => new(self);

        // ToDo. TextMeshPro.alpha
        // ToDo. TextMeshProUGUI.alpha

        // changer ~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

        /// <summary></summary>
        public static Mover.InfoTo<float, C, Changer.To, Mover.Param1> To<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<float, C, Changer.To, Mover.Param1> Add<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector2
    {
        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector2> {}

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
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

        // ToDo. RectTransform.pivot

        // changer ~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

        // Mover ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary></summary>
        public static Mover.InfoTo<Vector2, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector2, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoTo<Vector2, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector2, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoTo<Vector2, C, Changer.To, Mover.Param2> To<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector2, C, Changer.To, Mover.Param2> Add<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector3
    {

        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector3> {}

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
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

        // ToDo. Transform.localScale
        // ToDo. Transform.localEulerAngles

        // ToDo. Transform.position
        // ToDo. Transform.scale
        // ToDo. Transform.eulerAngles

        // changer ~~~~~~~~~~~~~~~~~~~~~~~~~~~

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

        // Mover ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.ZTo, Mover.Param1> ZTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.ZTo, Mover.Param1> ZAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.XYTo, Mover.Param2> XYTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.XYTo, Mover.Param2> XYAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.YZTo, Mover.Param2> YZTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.YZTo, Mover.Param2> YZAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.ZXTo, Mover.Param2> ZXTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.ZXTo, Mover.Param2> ZXAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.InfoTo<Vector3, C, Changer.To, Mover.Param3> To<C>(this C carrier, Vector3 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.InfoAdd<Vector3, C, Changer.To, Mover.Param3> Add<C>(this C secarrierf, Vector3 to)
            where C : struct, ICarrier => new(secarrierf, new(), Mover.CreateParam(to));
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
            Component Owner { get; }
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

        // info ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface IInfo<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            /// <summary>Don't touch! Only for system.</summary>
            Component Owner { get; }
            /// <summary>Don't touch! Only for system.</summary>
            Updater<T, C, H, P> CreateUpdater();
            /// <summary>Don't touch! Only for system.</summary>
            float GetInterval(float speed);
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct InfoTo<T, C, H, P> : IInfo<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly C carrier;
            readonly H changer;
            readonly P to;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => this.carrier.Owner;
            /// <summary>Don't touch! Only for system.</summary>
            internal InfoTo(C carrier, H changer, in P to)
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
        public readonly struct InfoAdd<T, C, H, P> : IInfo<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly C carrier;
            readonly H changer;
            readonly P delta;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => this.carrier.Owner;
            /// <summary>Don't touch! Only for system.</summary>
            internal InfoAdd(C carrier, H changer, in P delta)
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
                => this.carrier.SetCurrent(this.changer.Set(this.carrier.Current, this.from.Lerp(this.to, now))); // 防御的コピー発生しない...ハズ
        }


        // Builder ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        internal readonly struct Builder<I, T, C, H, P>
            where I : struct, IInfo<T, C, H, P>
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            readonly I info;
            readonly double start;
            readonly float interval;
            readonly float speed;

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I info, float interval, float speed)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.info = info;
                this.start = Story.GetStart();

                this.interval = interval;
                this.speed = speed;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I info, float interval, float speed, double start)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.info = info;
                this.start = start;

                this.interval = interval;
                this.speed = speed;
            }

            /// <summary>Don't touch! Only for system.</summary>
            internal Builder(in I info, float interval, float speed, ref double start)
            {
                Dev.Assert((float.Epsilon < interval) != (float.Epsilon < speed));
                this.info = info;
                this.start = start;

                if (float.Epsilon < speed) { interval = info.GetInterval(speed); }
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
                var interval = this.interval;
                if (float.Epsilon < speed)
                {
                    interval = info.GetInterval(this.speed);

#if (FOR_DEBUG || UNITY_EDITOR) && !STORY_NO_DEBUG
                    // 変化したら警告
                    if ((float.Epsilon < this.interval) && this.interval != interval) { Dev.LogWarning("移動期間が変化しました"); }
#endif

                }
                var start = this.start;
                var passer = new Story.Passer(interval, ref start);
                var updater = this.info.CreateUpdater();
                while (passer.Bundle(updater, ease)) { await Story.Yield; }
            }
        }
    }
}
