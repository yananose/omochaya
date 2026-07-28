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
        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this Mover.Updater<T, C, H, P> self, float interval, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => Tween(interval, self, ease, ref start).At(self.Owner);

        /// <summary></summary>
        public static Task Interval<T, C, H, P, E>(this Mover.Updater<T, C, H, P> self, float interval, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => Tween(interval, self, ease).At(self.Owner);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this Mover.Updater<T, C, H, P> self, float interval, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => Tween(interval, self, ref start).At(self.Owner);

        /// <summary></summary>
        public static Task Interval<T, C, H, P>(this Mover.Updater<T, C, H, P> self, float interval)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => Tween(interval, self).At(self.Owner);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this Mover.Updater<T, C, H, P> self, float speed, in E ease, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => Tween(self.GetInterval(speed), self, ease, ref start).At(self.Owner);

        /// <summary></summary>
        public static Task Speed<T, C, H, P, E>(this Mover.Updater<T, C, H, P> self, float speed, in E ease)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            where E : struct, IEase
            => Tween(self.GetInterval(speed), self, ease).At(self.Owner);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this Mover.Updater<T, C, H, P> self, float speed, ref double start)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => Tween(self.GetInterval(speed), self, ref start).At(self.Owner);

        /// <summary></summary>
        public static Task Speed<T, C, H, P>(this Mover.Updater<T, C, H, P> self, float speed)
            where C : struct, Mover.ICarrier<T>
            where H : struct, Mover.IChanger<T, P>
            where P : struct, Mover.IParam<P>
            => Tween(self.GetInterval(speed), self).At(self.Owner);

        internal static Component GetOwner(Component self)  => self.GetComponent<Story.ITaskOwner>() as Component ?? self;
    }

    static class StoryFloat
    {
        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<float> {}

        /// <summary>Don't touch! Only for system.</summary>
        public struct Carrier0 : ICarrier
        {
            CanvasGroup self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
            /// <summary>Don't touch! Only for system.</summary>
            public Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current
            {
                get => this.self.alpha;
                set => this.self.alpha = Mathf.Clamp01(value);
            }
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
            public struct To : Mover.IChanger<float, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(float current) => new(current);
                /// <summary>Don't touch! Only for system.</summary>
                public float Set(float current, Mover.Param1 prm)
                    => prm.P0;
            }
        }

        /// <summary></summary>
        public static Mover.Updater<float, C, Changer.To, Mover.Param1> To<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<float, C, Changer.To, Mover.Param1> Add<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);
    }

    static class StoryVector2
    {
        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector2> {}

        /// <summary>Don't touch! Only for system.</summary>
        public struct Carrier0 : ICarrier
        {
            RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
            /// <summary>Don't touch! Only for system.</summary>
            public Carrier0(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current
            {
                get => this.self.anchoredPosition;
                set => this.self.anchoredPosition = value;
            }
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
            public struct XTo : Mover.IChanger<Vector2, Mover.Param1>
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
            public struct YTo : Mover.IChanger<Vector2, Mover.Param1>
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
            public struct To : Mover.IChanger<Vector2, Mover.Param2>
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
        public static Mover.Updater<Vector2, C, Changer.XTo, Mover.Param1> XTo<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector2, C, Changer.XTo, Mover.Param1> XAdd<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);

        /// <summary></summary>
        public static Mover.Updater<Vector2, C, Changer.YTo, Mover.Param1> YTo<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector2, C, Changer.YTo, Mover.Param1> YAdd<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);

        /// <summary></summary>
        public static Mover.Updater<Vector2, C, Changer.To, Mover.Param2> To<C>(this C self, Vector2 to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector2, C, Changer.To, Mover.Param2> Add<C>(this C self, Vector2 to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);
    }

    static class StoryVector3
    {

        // carrier ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector3> {}

        /// <summary>Don't touch! Only for system.</summary>
        public struct Carrier0 : ICarrier
        {
            Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Owner => Story.GetOwner(this.self); // 重いので後で考える
            /// <summary>Don't touch! Only for system.</summary>
            public Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current
            {
                get => this.self.localPosition;
                set => this.self.localPosition = value;
            }
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
            public struct XTo : Mover.IChanger<Vector3, Mover.Param1>
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
            public struct YTo : Mover.IChanger<Vector3, Mover.Param1>
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
            public struct ZTo : Mover.IChanger<Vector3, Mover.Param1>
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
            public struct XYTo : Mover.IChanger<Vector3, Mover.Param2>
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
            public struct YZTo : Mover.IChanger<Vector3, Mover.Param2>
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
            public struct ZXTo : Mover.IChanger<Vector3, Mover.Param2>
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
            public struct To : Mover.IChanger<Vector3, Mover.Param3>
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
        public static Mover.Updater<Vector3, C, Changer.XTo, Mover.Param1> XTo<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.XTo, Mover.Param1> XAdd<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.YTo, Mover.Param1> YTo<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.YTo, Mover.Param1> YAdd<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.ZTo, Mover.Param1> ZTo<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.ZTo, Mover.Param1> ZAdd<C>(this C self, float to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.XYTo, Mover.Param2> XYTo<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.XYTo, Mover.Param2> XYAdd<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.YZTo, Mover.Param2> YZTo<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.YZTo, Mover.Param2> YZAdd<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.ZXTo, Mover.Param2> ZXTo<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.ZXTo, Mover.Param2> ZXAdd<C>(this C self, float to0, float to1)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to0, to1), true);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.To, Mover.Param3> To<C>(this C self, Vector3 to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), false);

        /// <summary></summary>
        public static Mover.Updater<Vector3, C, Changer.To, Mover.Param3> Add<C>(this C self, Vector3 to)
            where C : struct, ICarrier => new(self, new(), Mover.CreateParam(to), true);
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
            P Lerp(P b, float now);
            /// <summary>Don't touch! Only for system.</summary>
            float GetDistance(P b);
            /// <summary>Don't touch! Only for system.</summary>
            P Add(P b);
        }

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param1 : IParam<Param1>
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly float P0;
            /// <summary>Don't touch! Only for system.</summary>
            public Param1(float p0)
            {
                this.P0 = p0;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param1 Lerp(Param1 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t));
            /// <summary>Don't touch! Only for system.</summary>
            public float GetDistance(Param1 b) => Mathf.Abs(this.P0 - b.P0);
            /// <summary>Don't touch! Only for system.</summary>
            public Param1 Add(Param1 b) => new(this.P0 + b.P0);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param2 : IParam<Param2>
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly float P0, P1;
            /// <summary>Don't touch! Only for system.</summary>
            public Param2(float p0, float p1)
            {
                this.P0 = p0;
                this.P1 = p1;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param2 Lerp(Param2 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t));
            /// <summary>Don't touch! Only for system.</summary>
            public float GetDistance(Param2 b)
            {
                var d0 = this.P0 - b.P0;
                var d1 = this.P1 - b.P1;
                return Mathf.Sqrt(d0 * d0 + d1 * d1);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param2 Add(Param2 b) => new(this.P0 + b.P0, this.P1 + b.P1);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param3 : IParam<Param3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly float P0, P1, P2;
            /// <summary>Don't touch! Only for system.</summary>
            public Param3(float p0, float p1, float p2)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param3 Lerp(Param3 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t));
            /// <summary>Don't touch! Only for system.</summary>
            public float GetDistance(Param3 b)
            {
                var d0 = this.P0 - b.P0;
                var d1 = this.P1 - b.P1;
                var d2 = this.P2 - b.P2;
                return Mathf.Sqrt(d0 * d0 + d1 * d1 + d2 * d2);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param3 Add(Param3 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2);
        }
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Param4 : IParam<Param4>
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly float P0, P1, P2, P3;
            /// <summary>Don't touch! Only for system.</summary>
            public Param4(float p0, float p1, float p2, float p3)
            {
                this.P0 = p0;
                this.P1 = p1;
                this.P2 = p2;
                this.P3 = p3;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param4 Lerp(Param4 b, float t) => new(
                Mathf.LerpUnclamped(this.P0, b.P0, t),
                Mathf.LerpUnclamped(this.P1, b.P1, t),
                Mathf.LerpUnclamped(this.P2, b.P2, t),
                Mathf.LerpUnclamped(this.P3, b.P3, t));
            /// <summary>Don't touch! Only for system.</summary>
            public float GetDistance(Param4 b)
            {
                var d0 = this.P0 - b.P0;
                var d1 = this.P1 - b.P1;
                var d2 = this.P2 - b.P2;
                var d3 = this.P3 - b.P3;
                return Mathf.Sqrt(d0 * d0 + d1 * d1 + d2 * d2 + d3 * d3);
            }
            /// <summary>Don't touch! Only for system.</summary>
            public Param4 Add(Param4 b) => new(this.P0 + b.P0, this.P1 + b.P1, this.P2 + b.P2, this.P3 + b.P3);
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
            T Current { get; set; }
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

        // Updater ~~~~~~~~~~~~~~~~~~~~~~~~~~~

        /// <summary>Don't touch! Only for system.</summary>
        public struct Updater<T, C, H, P> : Story.IUpdater
            where C : struct, ICarrier<T>
            where H : struct, IChanger<T, P>
            where P : struct, IParam<P>
        {
            C carrier;
            H changer;
            P from, to;
            /// <summary>Don't touch! Only for system.</summary>
            internal Component Owner => this.carrier.Owner;
            /// <summary>Don't touch! Only for system.</summary>
            internal Updater(C carrier, H changer, P to, bool forAdd)
            {
                this.carrier = carrier;
                this.changer = changer;
                this.from = changer.Get(carrier.Current);
                this.to = forAdd ? this.from.Add(to) : to;
            }
            /// <summary>Don't touch! Only for system.</summary>
            public void Update(float now) => this.carrier.Current = this.changer.Set(this.carrier.Current, this.from.Lerp(this.to, now));
            /// <summary>Don't touch! Only for system.</summary>
            internal float GetInterval(float speed)
            {
                Dev.Assert(float.Epsilon < speed);
                return this.from.GetDistance(this.to) / speed;
            }
        }
    }
}
