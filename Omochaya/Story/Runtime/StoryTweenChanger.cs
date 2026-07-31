// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenChanger.cs" company="Omochaya">
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

    public static partial class StoryFloat ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, float p)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, float p)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<float> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly float p;

            internal Plan(in C carrier, bool isDelta, float p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
                => Mover.CreateTaskCore(new Changer(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Changer : Mover.IChanger<float, Mover.Param1>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Mover.Param1 Get(float current) => new(current);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Set(float current, Mover.Param1 prm) => prm.P0;
        }
    }

    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, false, x, y);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, true, x, y);

        /// <summary></summary>
        public static Plan<C> To<C>(this C self, Vector2 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, Vector2 p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<Vector2> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly Comb comb;
            readonly Vector2 p;

            internal Plan(in C carrier, bool isDelta, float? x, float? y)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                var result = Analyze(x, y);
                this.comb = result.Item1;
                this.p = result.Item2;
            }

            internal Plan(in C carrier, bool isDelta, Vector2 p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XY;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.X_: return Mover.CreateTaskCore(new Changer.X_(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._Y: return Mover.CreateTaskCore(new Changer._Y(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XY: return Mover.CreateTaskCore(new Changer.XY(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                }
                return default;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static (Comb, Vector2) Analyze(float? x, float? y)
        {
            if (x != null && y == null) { return (Comb.X_, new((float)x, default)); }
            if (x == null && y != null) { return (Comb._Y, new(default, (float)y)); }
            if (x != null && y != null) { return (Comb.XY, new((float)x, (float)y)); }
            Dev.LogError("引数の指定が不正です");
            return default;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        enum Comb
        {
            None,
            X_, _Y,
            XY
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Changer
        {
            internal readonly struct X_ : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Vector2 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            internal readonly struct _Y : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Vector2 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            internal readonly struct XY : Mover.IChanger<Vector2, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Vector2 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector2 Set(Vector2 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
        }
    }

    public static partial class StoryVector3 ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, false, x, y, z);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, true, x, y, z);

        /// <summary></summary>
        public static Plan<C> To<C>(this C self, in Vector3 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, in Vector3 p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<Vector3> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly Comb comb;
            readonly Vector3 p;

            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? z)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                var result = Analyze(x, y, z);
                this.comb = result.Item1;
                this.p = result.Item2;
            }

            internal Plan(in C carrier, bool isDelta, in Vector3 p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XYZ;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.X__: return Mover.CreateTaskCore(new Changer.X__(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._Y_: return Mover.CreateTaskCore(new Changer._Y_(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.__Z: return Mover.CreateTaskCore(new Changer.__Z(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._YZ: return Mover.CreateTaskCore(new Changer._YZ(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.X_Z: return Mover.CreateTaskCore(new Changer.X_Z(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XY_: return Mover.CreateTaskCore(new Changer.XY_(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XYZ: return Mover.CreateTaskCore(new Changer.XYZ(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                }
                return default;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static (Comb, Vector3) Analyze(float? x, float? y, float? z)
        {
            if (x != null && y == null && z == null) { return (Comb.X__, new((float)x, default, default)); }
            if (x == null && y != null && z == null) { return (Comb._Y_, new(default, (float)y, default)); }
            if (x == null && y == null && z != null) { return (Comb.__Z, new(default, default, (float)z)); }
            if (x == null && y != null && z != null) { return (Comb._YZ, new(default, (float)y, (float)z)); }
            if (x != null && y == null && z != null) { return (Comb.X_Z, new((float)x, default, (float)z)); }
            if (x != null && y != null && z == null) { return (Comb.XY_, new((float)x, (float)y, default)); }
            if (x != null && y != null && z != null) { return (Comb.XYZ, new((float)x, (float)y, (float)z)); }
            Dev.LogError("引数の指定が不正です");
            return default;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        enum Comb
        {
            None,
            X__, _Y_, __Z,
            _YZ, X_Z, XY_,
            XYZ
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static class Changer
        {
            internal readonly struct X__ : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Vector3 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            internal readonly struct _Y_ : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Vector3 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            internal readonly struct __Z : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Vector3 current) => new(current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.z = prm.P0;
                    return current;
                }
            }
            internal readonly struct _YZ : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Vector3 current) => new(current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X_Z : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            internal readonly struct XY_ : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            internal readonly struct XYZ : Mover.IChanger<Vector3, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Vector3 current) => new(current.x, current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Vector3 Set(Vector3 current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.z = prm.P2;
                    return current;
                }
            }
        }
    }

    public static partial class StoryColor ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, bool _ = false, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, false, r, g, b, a);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, bool _ = true, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, true, r, g, b, a);

        /// <summary></summary>
        public static Plan<C> To<C>(this C self, in Color p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, in Color p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<Color> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly Comb comb;
            readonly Color p;

            internal Plan(in C carrier, bool isDelta, float? r, float? g, float? b, float? a)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                var result = Analyze(r, g, b, a);
                this.comb = result.Item1;
                this.p = result.Item2;
            }

            internal Plan(in C carrier, bool isDelta, Color p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.RGBA;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.R___: return Mover.CreateTaskCore(new Changer.R___(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._G__: return Mover.CreateTaskCore(new Changer._G__(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.__B_: return Mover.CreateTaskCore(new Changer.__B_(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.___A: return Mover.CreateTaskCore(new Changer.___A(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.RG__: return Mover.CreateTaskCore(new Changer.RG__(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.__BA: return Mover.CreateTaskCore(new Changer.__BA(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.R_B_: return Mover.CreateTaskCore(new Changer.R_B_(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._G_A: return Mover.CreateTaskCore(new Changer._G_A(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.R__A: return Mover.CreateTaskCore(new Changer.R__A(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._GB_: return Mover.CreateTaskCore(new Changer._GB_(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._GBA: return Mover.CreateTaskCore(new Changer._GBA(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.R_BA: return Mover.CreateTaskCore(new Changer.R_BA(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.RG_A: return Mover.CreateTaskCore(new Changer.RG_A(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.RGB_: return Mover.CreateTaskCore(new Changer.RGB_(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.RGBA: return Mover.CreateTaskCore(new Changer.RGBA(), new Mover.Param4(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                }
                return default;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static (Comb, Color) Analyze(float? r, float? g, float? b, float? a)
        {
            if (r != null && g == null && b == null && a == null) { return (Comb.R___, new((float)r, default, default, default)); }
            if (r == null && g != null && b == null && a == null) { return (Comb._G__, new(default, (float)g, default, default)); }
            if (r == null && g == null && b != null && a == null) { return (Comb.__B_, new(default, default, (float)b, default)); }
            if (r == null && g == null && b == null && a != null) { return (Comb.___A, new(default, default, default, (float)a)); }
            if (r != null && g != null && b == null && a == null) { return (Comb.RG__, new((float)r, (float)g, default, default)); }
            if (r == null && g == null && b != null && a != null) { return (Comb.__BA, new(default, default, (float)b, (float)a)); }
            if (r != null && g == null && b != null && a == null) { return (Comb.R_B_, new((float)r, default, (float)b, default)); }
            if (r == null && g != null && b == null && a != null) { return (Comb._G_A, new(default, (float)g, default, (float)a)); }
            if (r != null && g == null && b == null && a != null) { return (Comb.R__A, new((float)r, default, default, (float)a)); }
            if (r == null && g != null && b != null && a == null) { return (Comb._GB_, new(default, (float)g, (float)b, default)); }
            if (r == null && g != null && b != null && a != null) { return (Comb._GBA, new(default, (float)g, (float)b, (float)a)); }
            if (r != null && g == null && b != null && a != null) { return (Comb.R_BA, new((float)r, default, (float)b, (float)a)); }
            if (r != null && g != null && b == null && a != null) { return (Comb.RG_A, new((float)r, (float)g, default, (float)a)); }
            if (r != null && g != null && b != null && a == null) { return (Comb.RGB_, new((float)r, (float)g, (float)b, default)); }
            if (r != null && g != null && b != null && a != null) { return (Comb.RGBA, new((float)r, (float)g, (float)b, (float)a)); }
            Dev.LogError("引数の指定が不正です");
            return default;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public enum Comb
        {
            None,
            R___, _G__, __B_, ___A,
            RG__, __BA,
            R_B_, _G_A,
            R__A, _GB_,
            _GBA, R_BA, RG_A, RGB_,
            RGBA
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static class Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R___ : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Color current) => new(current.r);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.r = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _G__ : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Color current) => new(current.g);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.g = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __B_ : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Color current) => new(current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.b = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ___A : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Color current) => new(current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.a = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct RG__ : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.r, current.g);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __BA : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.b = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R_B_ : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.r, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.b = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _G_A : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.g, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R__A : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.r, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _GB_ : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Color current) => new(current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.b = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _GBA : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Color current) => new(current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.g = prm.P0;
                    current.b = prm.P1;
                    current.a = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R_BA : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Color current) => new(current.r, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.r = prm.P0;
                    current.b = prm.P1;
                    current.a = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct RG_A : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Color current) => new(current.r, current.g, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    current.a = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct RGB_ : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Color current) => new(current.r, current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    current.b = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct RGBA : Mover.IChanger<Color, Mover.Param4>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param4 Get(Color current) => new(current.r, current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Color Set(Color current, Mover.Param4 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    current.b = prm.P2;
                    current.a = prm.P3;
                    return current;
                }
            }
        }
    }

    public static partial class StoryQuaternion ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, in Quaternion p)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, in Quaternion p)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<Quaternion> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly Quaternion p;

            internal Plan(in C carrier, bool isDelta, in Quaternion p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
                => Mover.CreateTaskCore(new Changer(), new Mover.ParamQ(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Changer : Mover.IChanger<Quaternion, Mover.ParamQ>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Mover.ParamQ Get(Quaternion current) => new(current);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Set(Quaternion current, Mover.ParamQ prm) => prm.Q;
        }
    }

    public static partial class StoryRect ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, false, x, y, width, height);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, true, x, y, width, height);

        /// <summary></summary>
        public static Plan<C> To<C>(this C self, in Rect p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        public static Plan<C> By<C>(this C self, in Rect p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<Rect> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly C carrier;
            readonly bool isDelta;
            readonly Comb comb;
            readonly Rect p;

            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? width, float? height)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                var result = Analyze(x, y, height, width);
                this.comb = result.Item1;
                this.p = result.Item2;
            }

            internal Plan(in C carrier, bool isDelta, Rect p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XYWH;
                this.p = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<E>(float interval, float speed, E ease, ref double start)
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.X___: return Mover.CreateTaskCore(new Changer.X___(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._Y__: return Mover.CreateTaskCore(new Changer._Y__(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.__W_: return Mover.CreateTaskCore(new Changer.__W_(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.___H: return Mover.CreateTaskCore(new Changer.___H(), new Mover.Param1(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XY__: return Mover.CreateTaskCore(new Changer.XY__(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.__WH: return Mover.CreateTaskCore(new Changer.__WH(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.X_W_: return Mover.CreateTaskCore(new Changer.X_W_(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._Y_H: return Mover.CreateTaskCore(new Changer._Y_H(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.X__H: return Mover.CreateTaskCore(new Changer.X__H(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._YW_: return Mover.CreateTaskCore(new Changer._YW_(), new Mover.Param2(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb._YWH: return Mover.CreateTaskCore(new Changer._YWH(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.X_WH: return Mover.CreateTaskCore(new Changer.X_WH(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XY_H: return Mover.CreateTaskCore(new Changer.XY_H(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XYW_: return Mover.CreateTaskCore(new Changer.XYW_(), new Mover.Param3(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                    case Comb.XYWH: return Mover.CreateTaskCore(new Changer.XYWH(), new Mover.Param4(), this.p, this.carrier, this.isDelta, interval, speed, ease, ref start);
                }
                return default;
            }
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static (Comb, Rect) Analyze(float? x, float? y, float? width, float? height)
        {
            if (x != null && y == null && width == null && height == null) { return (Comb.X___, new((float)x, default, default, default)); }
            if (x == null && y != null && width == null && height == null) { return (Comb._Y__, new(default, (float)y, default, default)); }
            if (x == null && y == null && width != null && height == null) { return (Comb.__W_, new(default, default, (float)width, default)); }
            if (x == null && y == null && width == null && height != null) { return (Comb.___H, new(default, default, default, (float)height)); }
            if (x != null && y != null && width == null && height == null) { return (Comb.XY__, new((float)x, (float)y, default, default)); }
            if (x == null && y == null && width != null && height != null) { return (Comb.__WH, new(default, default, (float)width, (float)height)); }
            if (x != null && y == null && width != null && height == null) { return (Comb.X_W_, new((float)x, default, (float)width, default)); }
            if (x == null && y != null && width == null && height != null) { return (Comb._Y_H, new(default, (float)y, default, (float)height)); }
            if (x != null && y == null && width == null && height != null) { return (Comb.X__H, new((float)x, default, default, (float)height)); }
            if (x == null && y != null && width != null && height == null) { return (Comb._YW_, new(default, (float)y, (float)width, default)); }
            if (x == null && y != null && width != null && height != null) { return (Comb._YWH, new(default, (float)y, (float)width, (float)height)); }
            if (x != null && y == null && width != null && height != null) { return (Comb.X_WH, new((float)x, default, (float)width, (float)height)); }
            if (x != null && y != null && width == null && height != null) { return (Comb.XY_H, new((float)x, (float)y, default, (float)height)); }
            if (x != null && y != null && width != null && height == null) { return (Comb.XYW_, new((float)x, (float)y, (float)width, default)); }
            if (x != null && y != null && width != null && height != null) { return (Comb.XYWH, new((float)x, (float)y, (float)width, (float)height)); }
            Dev.LogError("引数の指定が不正です");
            return default;
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public enum Comb
        {
            None,
            X___, _Y__, __W_, ___H,
            XY__, __WH,
            X_W_, _Y_H,
            X__H, _YW_,
            _YWH, X_WH, XY_H, XYW_,
            XYWH
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        static class Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X___ : Mover.IChanger<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Rect current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _Y__ : Mover.IChanger<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Rect current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __W_ : Mover.IChanger<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Rect current) => new(current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.width = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ___H : Mover.IChanger<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param1 Get(Rect current) => new(current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.height = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct XY__ : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __WH : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.width = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X_W_ : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.width = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _Y_H : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.y, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X__H : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _YW_ : Mover.IChanger<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param2 Get(Rect current) => new(current.y, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.width = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _YWH : Mover.IChanger<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Rect current) => new(current.y, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param3 prm)
                {
                    current.y = prm.P0;
                    current.width = prm.P1;
                    current.height = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X_WH : Mover.IChanger<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.width = prm.P1;
                    current.height = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct XY_H : Mover.IChanger<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.y, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.height = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct XYW_ : Mover.IChanger<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.y, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.width = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct XYWH : Mover.IChanger<Rect, Mover.Param4>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Mover.Param4 Get(Rect current) => new(current.x, current.y, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                public Rect Set(Rect current, Mover.Param4 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.width = prm.P2;
                    current.height = prm.P3;
                    return current;
                }
            }
        }
    }
}
