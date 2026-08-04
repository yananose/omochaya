// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenMapper.cs" company="Omochaya">
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
    using System.Runtime.CompilerServices;

    public static partial class StoryFloat ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, float p) where C : struct, ICarrier => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, float p) where C : struct, ICarrier => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface ICarrier : Mover.ICarrier<float> {}

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, ICarrier
        {
            readonly Mover.PlanArg<float, C> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float p) => this.planArg = new(carrier, p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, float, Mover.Param1, Mapper, C, E>(this.planArg, timeArg, ease, ref start);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<float, Mover.Param1>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Mover.Param1 Get(float current) => new(current);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Set(float current, Mover.Param1 prm) => prm.P0;
        }
    }

    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, false, x, y);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, true, x, y);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, Vector2 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly Mover.PlanArg<Vector2, C> planArg;
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y)
            {
                var result = Analyze(x, y);
                this.comb = result.Item1;
                this.planArg = new(carrier, result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Vector2 p)
            {
                this.comb = Comb.XY;
                this.planArg = new(carrier, p, isDelta);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.X_: return Mover.Create<S, Vector2, Mover.Param1, Mapper.X_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._Y: return Mover.Create<S, Vector2, Mover.Param1, Mapper._Y, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XY: return Mover.Create<S, Vector2, Mover.Param2, Mapper.XY, C, E>(this.planArg, timeArg, ease, ref start);
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
            Dev.LogException(new System.ArgumentException());
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
        readonly struct Mapper
        {
            internal readonly struct X_ : Mover.IMapper<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Vector2 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            internal readonly struct _Y : Mover.IMapper<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Vector2 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            internal readonly struct XY : Mover.IMapper<Vector2, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Vector2 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, false, x, y, z);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, true, x, y, z);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Vector3 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly Mover.PlanArg<Vector3, C> planArg;
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? z)
            {
                var result = Analyze(x, y, z);
                this.comb = result.Item1;
                this.planArg = new(carrier, result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, in Vector3 p)
            {
                this.comb = Comb.XYZ;
                this.planArg = new(carrier, p, isDelta);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
            {
                // 芋づる式にステートマシンが全部作られる...ヤバすぎ
                switch (this.comb)
                {
                    case Comb.X__: return Mover.Create<S, Vector3, Mover.Param1, Mapper.X__, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._Y_: return Mover.Create<S, Vector3, Mover.Param1, Mapper._Y_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.__Z: return Mover.Create<S, Vector3, Mover.Param1, Mapper.__Z, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._YZ: return Mover.Create<S, Vector3, Mover.Param2, Mapper._YZ, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.X_Z: return Mover.Create<S, Vector3, Mover.Param2, Mapper.X_Z, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XY_: return Mover.Create<S, Vector3, Mover.Param2, Mapper.XY_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XYZ: return Mover.Create<S, Vector3, Mover.Param3, Mapper.XYZ, C, E>(this.planArg, timeArg, ease, ref start);
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
            Dev.LogException(new System.ArgumentException());
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
        static class Mapper
        {
            internal readonly struct X__ : Mover.IMapper<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Vector3 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            internal readonly struct _Y_ : Mover.IMapper<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Vector3 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            internal readonly struct __Z : Mover.IMapper<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Vector3 current) => new(current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.z = prm.P0;
                    return current;
                }
            }
            internal readonly struct _YZ : Mover.IMapper<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Vector3 current) => new(current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X_Z : Mover.IMapper<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            internal readonly struct XY_ : Mover.IMapper<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            internal readonly struct XYZ : Mover.IMapper<Vector3, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Vector3 current) => new(current.x, current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, false, r, g, b, a);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, true, r, g, b, a);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Color p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly Mover.PlanArg<Color, C> planArg;
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? r, float? g, float? b, float? a)
            {
                var result = Analyze(r, g, b, a);
                this.comb = result.Item1;
                this.planArg = new(carrier, result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Color p)
            {
                this.comb = Comb.RGBA;
                this.planArg = new(carrier, p, isDelta);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.R___: return Mover.Create<S, Color, Mover.Param1, Mapper.R___, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._G__: return Mover.Create<S, Color, Mover.Param1, Mapper._G__, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.__B_: return Mover.Create<S, Color, Mover.Param1, Mapper.__B_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.___A: return Mover.Create<S, Color, Mover.Param1, Mapper.___A, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.RG__: return Mover.Create<S, Color, Mover.Param2, Mapper.RG__, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.__BA: return Mover.Create<S, Color, Mover.Param2, Mapper.__BA, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.R_B_: return Mover.Create<S, Color, Mover.Param2, Mapper.R_B_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._G_A: return Mover.Create<S, Color, Mover.Param2, Mapper._G_A, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.R__A: return Mover.Create<S, Color, Mover.Param2, Mapper.R__A, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._GB_: return Mover.Create<S, Color, Mover.Param2, Mapper._GB_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._GBA: return Mover.Create<S, Color, Mover.Param3, Mapper._GBA, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.R_BA: return Mover.Create<S, Color, Mover.Param3, Mapper.R_BA, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.RG_A: return Mover.Create<S, Color, Mover.Param3, Mapper.RG_A, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.RGB_: return Mover.Create<S, Color, Mover.Param3, Mapper.RGB_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.RGBA: return Mover.Create<S, Color, Mover.Param4, Mapper.RGBA, C, E>(this.planArg, timeArg, ease, ref start);
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
            Dev.LogException(new System.ArgumentException());
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
        static class Mapper
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R___ : Mover.IMapper<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Color current) => new(current.r);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.r = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _G__ : Mover.IMapper<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Color current) => new(current.g);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.g = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __B_ : Mover.IMapper<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Color current) => new(current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.b = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ___A : Mover.IMapper<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Color current) => new(current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.a = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct RG__ : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.r, current.g);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __BA : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.b = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R_B_ : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.r, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.b = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _G_A : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.g, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct R__A : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.r, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _GB_ : Mover.IMapper<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Color current) => new(current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.b = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _GBA : Mover.IMapper<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Color current) => new(current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct R_BA : Mover.IMapper<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Color current) => new(current.r, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct RG_A : Mover.IMapper<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Color current) => new(current.r, current.g, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct RGB_ : Mover.IMapper<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Color current) => new(current.r, current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct RGBA : Mover.IMapper<Color, Mover.Param4>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param4 Get(Color current) => new(current.r, current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Quaternion p)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly Mover.PlanArg<Quaternion, C> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, in Quaternion p)
            {
                this.planArg = new(carrier, p, isDelta);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, Quaternion, Mover.ParamQ, Mapper, C, E>(this.planArg, timeArg, ease, ref start);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Quaternion, Mover.ParamQ>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Mover.ParamQ Get(Quaternion current) => new(current);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Set(Quaternion current, Mover.ParamQ prm) => prm.Q;
        }
    }

    public static partial class StoryRect ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, false, x, y, width, height);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, true, x, y, width, height);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Rect p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            readonly Mover.PlanArg<Rect, C> planArg;
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? width, float? height)
            {
                var result = Analyze(x, y, width, height);
                this.comb = result.Item1;
                this.planArg = new(carrier, result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Rect p)
            {
                this.comb = Comb.XYWH;
                this.planArg = new(carrier, p, isDelta);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
            {
                switch (this.comb)
                {
                    case Comb.X___: return Mover.Create<S, Rect, Mover.Param1, Mapper.X___, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._Y__: return Mover.Create<S, Rect, Mover.Param1, Mapper._Y__, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.__W_: return Mover.Create<S, Rect, Mover.Param1, Mapper.__W_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.___H: return Mover.Create<S, Rect, Mover.Param1, Mapper.___H, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XY__: return Mover.Create<S, Rect, Mover.Param2, Mapper.XY__, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.__WH: return Mover.Create<S, Rect, Mover.Param2, Mapper.__WH, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.X_W_: return Mover.Create<S, Rect, Mover.Param2, Mapper.X_W_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._Y_H: return Mover.Create<S, Rect, Mover.Param2, Mapper._Y_H, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.X__H: return Mover.Create<S, Rect, Mover.Param2, Mapper.X__H, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._YW_: return Mover.Create<S, Rect, Mover.Param2, Mapper._YW_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb._YWH: return Mover.Create<S, Rect, Mover.Param3, Mapper._YWH, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.X_WH: return Mover.Create<S, Rect, Mover.Param3, Mapper.X_WH, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XY_H: return Mover.Create<S, Rect, Mover.Param3, Mapper.XY_H, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XYW_: return Mover.Create<S, Rect, Mover.Param3, Mapper.XYW_, C, E>(this.planArg, timeArg, ease, ref start);
                    case Comb.XYWH: return Mover.Create<S, Rect, Mover.Param4, Mapper.XYWH, C, E>(this.planArg, timeArg, ease, ref start);
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
            Dev.LogException(new System.ArgumentException());
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
        static class Mapper
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X___ : Mover.IMapper<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Rect current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _Y__ : Mover.IMapper<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Rect current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __W_ : Mover.IMapper<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Rect current) => new(current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.width = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ___H : Mover.IMapper<Rect, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param1 Get(Rect current) => new(current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param1 prm)
                {
                    current.height = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct XY__ : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct __WH : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.width = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X_W_ : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.width = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _Y_H : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.y, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct X__H : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.x, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.height = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _YW_ : Mover.IMapper<Rect, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param2 Get(Rect current) => new(current.y, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Set(Rect current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.width = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct _YWH : Mover.IMapper<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Rect current) => new(current.y, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct X_WH : Mover.IMapper<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct XY_H : Mover.IMapper<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.y, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct XYW_ : Mover.IMapper<Rect, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param3 Get(Rect current) => new(current.x, current.y, current.width);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
            public readonly struct XYWH : Mover.IMapper<Rect, Mover.Param4>
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Mover.Param4 Get(Rect current) => new(current.x, current.y, current.width, current.height);
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
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
