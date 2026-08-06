// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenMapper.cs" company="Omochaya">
//   Copyright (t) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// Defines zero-allocation, type-specialized tween mapping extensions and structural plans for various Unity mathematical types.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using UnityEngine;
    using Omochaya.HiddenStory;
    using System.Runtime.CompilerServices;

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryFloat ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, float p) where C : struct, ICarrier => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
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
            readonly Mover.PlanArg<C, Mapper, float> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float p) => this.planArg = new(carrier, p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, float, E>(this.planArg, timeArg, ease, ref start);
        }

        readonly struct Mapper : Mover.IMapper<float>
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(float to, float from) => Mathf.Abs(to - from);
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (float, float) GetParam(float from, float to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Lerp(float current, float to, float diff, float rt) => to + diff * rt;
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, Vector2 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, Vector2 p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, false, x, y);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null)
            where C : struct, ICarrier
            => new(self, true, x, y);

        enum Comb
        {
            None,
            X_, _Y,
            XY
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
        static Comb Analyze(float? x, float? y)
        {
            var bits = 0;
            bits <<= 1; if (x != null) { bits |= 1; } 
            bits <<= 1; if (y != null) { bits |= 1; } 
            Dev.Assert(bits == 0, "引数が不正");
            return bits switch
                {
                    0b10 => Comb.X_,
                    0b01 => Comb._Y,
                    0b11 => Comb.XY,
                    _ => default
                };
        }

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
            readonly Vector2 to;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Analyze(x, y);
                this.to = new(x ?? default, y ?? default);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Vector2 p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XY;
                this.to = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X_ => Mover.Create<S, C, Mapper.X_, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y => Mover.Create<S, C, Mapper._Y, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY => Mover.Create<S, C, Mapper.XY, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.Create<S, C, Mapper, Vector2, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Vector2>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
                => this.comb switch
                {
                    Comb.X_ => new X_().GetLength(to, from),
                    Comb._Y => new _Y().GetLength(to, from),
                    Comb.XY => new XY().GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                => this.comb switch
                {
                    Comb.X_ => new X_().Lerp(current, to, diff, rt),
                    Comb._Y => new _Y().Lerp(current, to, diff, rt),
                    Comb.XY => new XY().Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => Param(from, to, isDelta);

            partial struct X_ { public float GetLength(Vector2 to, Vector2 from) => Mathf.Abs(to.x - from.x); }
            partial struct _Y { public float GetLength(Vector2 to, Vector2 from) => Mathf.Abs(to.y - from.y); }
            partial struct XY { public float GetLength(Vector2 to, Vector2 from) => (to - from).magnitude; }
            partial struct X_ { public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _Y { public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XY { public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => Param(from, to, isDelta); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (Vector2, Vector2) Param(Vector2 from, Vector2 to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }

            internal readonly partial struct X_ : Mover.IMapper<Vector2>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    return current;
                }
            }
            internal readonly partial struct _Y : Mover.IMapper<Vector2>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    return current;
                }
            }
            internal readonly partial struct XY : Mover.IMapper<Vector2>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                {
                    return to + diff * rt;
                }
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryVector3 ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Vector3 p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Vector3 p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, false, x, y, z);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? z = null)
            where C : struct, ICarrier
            => new(self, true, x, y, z);

        enum Comb
        {
            None,
            X__, _Y_, __Z,
            _YZ, X_Z, XY_,
            XYZ
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
        static Comb Analyze(float? x, float? y, float? z)
        {
            var bits = 0;
            bits <<= 1; if (x != null) { bits |= 1; } 
            bits <<= 1; if (y != null) { bits |= 1; } 
            bits <<= 1; if (z != null) { bits |= 1; } 
            Dev.Assert(bits == 0, "引数が不正");
            return bits switch
                {
                    0b100 => Comb.X__,
                    0b010 => Comb._Y_,
                    0b001 => Comb.__Z,
                    0b011 => Comb._YZ,
                    0b101 => Comb.X_Z,
                    0b110 => Comb.XY_,
                    0b111 => Comb.XYZ,
                    _ => default
                };
        }

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
            readonly Vector3 to;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? z)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Analyze(x, y, z);
                this.to = new(x ?? default, y ?? default, z ?? default);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Vector3 p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XYZ;
                this.to = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X__ => Mover.Create<S, C, Mapper.X__, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y_ => Mover.Create<S, C, Mapper._Y_, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__Z => Mover.Create<S, C, Mapper.__Z, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YZ => Mover.Create<S, C, Mapper._YZ, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_Z => Mover.Create<S, C, Mapper.X_Z, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY_ => Mover.Create<S, C, Mapper.XY_, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYZ => Mover.Create<S, C, Mapper.XYZ, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.Create<S, C, Mapper, Vector3, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Vector3>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector3 to, Vector3 from)
                => this.comb switch
                {
                    Comb.X__ => new X__().GetLength(to, from),
                    Comb._Y_ => new _Y_().GetLength(to, from),
                    Comb.__Z => new __Z().GetLength(to, from),
                    Comb._YZ => new _YZ().GetLength(to, from),
                    Comb.X_Z => new X_Z().GetLength(to, from),
                    Comb.XY_ => new XY_().GetLength(to, from),
                    Comb.XYZ => new XYZ().GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                => this.comb switch
                {
                    Comb.X__ => new X__().Lerp(current, to, diff, rt),
                    Comb._Y_ => new _Y_().Lerp(current, to, diff, rt),
                    Comb.__Z => new __Z().Lerp(current, to, diff, rt),
                    Comb._YZ => new _YZ().Lerp(current, to, diff, rt),
                    Comb.X_Z => new X_Z().Lerp(current, to, diff, rt),
                    Comb.XY_ => new XY_().Lerp(current, to, diff, rt),
                    Comb.XYZ => new XYZ().Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta);

            partial struct X__ { public float GetLength(Vector3 to, Vector3 from) => Mathf.Abs(to.x - from.x); }
            partial struct _Y_ { public float GetLength(Vector3 to, Vector3 from) => Mathf.Abs(to.y - from.y); }
            partial struct __Z { public float GetLength(Vector3 to, Vector3 from) => Mathf.Abs(to.z - from.z); }
            partial struct _YZ { public float GetLength(Vector3 to, Vector3 from) => Hidden.GetLength(to.y, from.y, to.z, from.z); }
            partial struct X_Z { public float GetLength(Vector3 to, Vector3 from) => Hidden.GetLength(to.x, from.x, to.z, from.z); }
            partial struct XY_ { public float GetLength(Vector3 to, Vector3 from) => Hidden.GetLength(to.x, from.x, to.y, from.y); }
            partial struct XYZ { public float GetLength(Vector3 to, Vector3 from) => (to - from).magnitude; }
            partial struct X__ { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _Y_ { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct __Z { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _YZ { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct X_Z { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XY_ { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XYZ { public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => Param(from, to, isDelta); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (Vector3, Vector3) Param(Vector3 from, Vector3 to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }

            internal readonly partial struct X__ : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    return current;
                }
            }
            internal readonly partial struct _Y_ : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    return current;
                }
            }
            internal readonly partial struct __Z : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.z = to.z + diff.z * rt;
                    return current;
                }
            }
            internal readonly partial struct _YZ : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    current.z = to.z + diff.z * rt;
                    return current;
                }
            }
            internal readonly partial struct X_Z : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.z = to.z + diff.z * rt;
                    return current;
                }
            }
            internal readonly partial struct XY_ : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.y = to.y + diff.y * rt;
                    return current;
                }
            }
            internal readonly partial struct XYZ : Mover.IMapper<Vector3>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                {
                    return to + diff * rt;
                }
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryColor ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Color p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Color p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, false, r, g, b, a);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, ICarrier
            => new(self, true, r, g, b, a);

        enum Comb
        {
            None,
            R___, _G__, __B_, ___A,
            RG__, __BA,
            R_B_, _G_A,
            R__A, _GB_,
            _GBA, R_BA, RG_A, RGB_,
            RGBA
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
        static Comb Analyze(float? r, float? g, float? b, float? a)
        {
            var bits = 0;
            bits <<= 1; if (r != null) { bits |= 1; } 
            bits <<= 1; if (g != null) { bits |= 1; } 
            bits <<= 1; if (b != null) { bits |= 1; } 
            bits <<= 1; if (a != null) { bits |= 1; } 
            Dev.Assert(bits == 0, "引数が不正");
            return bits switch
                {
                    0b1000 => Comb.R___,
                    0b0100 => Comb._G__,
                    0b0010 => Comb.__B_,
                    0b0001 => Comb.___A,
                    0b1100 => Comb.RG__,
                    0b0011 => Comb.__BA,
                    0b1010 => Comb.R_B_,
                    0b0101 => Comb._G_A,
                    0b1001 => Comb.R__A,
                    0b0110 => Comb._GB_,
                    0b0111 => Comb._GBA,
                    0b1011 => Comb.R_BA,
                    0b1101 => Comb.RG_A,
                    0b1110 => Comb.RGB_,
                    0b1111 => Comb.RGBA,
                    _ => default
                };
        }

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
            readonly Color to;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? r, float? g, float? b, float? a)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Analyze(r, g, b, a);
                this.to = new(r ?? default, g ?? default, b ?? default, a ?? default);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Color p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.RGBA;
                this.to = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.R___ => Mover.Create<S, C, Mapper.R___, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._G__ => Mover.Create<S, C, Mapper._G__, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__B_ => Mover.Create<S, C, Mapper.__B_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.___A => Mover.Create<S, C, Mapper.___A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RG__ => Mover.Create<S, C, Mapper.RG__, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__BA => Mover.Create<S, C, Mapper.__BA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R_B_ => Mover.Create<S, C, Mapper.R_B_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._G_A => Mover.Create<S, C, Mapper._G_A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R__A => Mover.Create<S, C, Mapper.R__A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._GB_ => Mover.Create<S, C, Mapper._GB_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._GBA => Mover.Create<S, C, Mapper._GBA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R_BA => Mover.Create<S, C, Mapper.R_BA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RG_A => Mover.Create<S, C, Mapper.RG_A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RGB_ => Mover.Create<S, C, Mapper.RGB_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RGBA => Mover.Create<S, C, Mapper.RGBA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.Create<S, C, Mapper, Color, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Color>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Color to, Color from)
                => this.comb switch
                {
                    Comb.R___ => new R___().GetLength(to, from),
                    Comb._G__ => new _G__().GetLength(to, from),
                    Comb.__B_ => new __B_().GetLength(to, from),
                    Comb.___A => new ___A().GetLength(to, from),
                    Comb.RG__ => new RG__().GetLength(to, from),
                    Comb.__BA => new __BA().GetLength(to, from),
                    Comb.R_B_ => new R_B_().GetLength(to, from),
                    Comb._G_A => new _G_A().GetLength(to, from),
                    Comb.R__A => new R__A().GetLength(to, from),
                    Comb._GB_ => new _GB_().GetLength(to, from),
                    Comb._GBA => new _GBA().GetLength(to, from),
                    Comb.R_BA => new R_BA().GetLength(to, from),
                    Comb.RG_A => new RG_A().GetLength(to, from),
                    Comb.RGB_ => new RGB_().GetLength(to, from),
                    Comb.RGBA => new RGBA().GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Lerp(Color current, Color to, Color diff, float rt)
                => this.comb switch
                {
                    Comb.R___ => new R___().Lerp(current, to, diff, rt),
                    Comb._G__ => new _G__().Lerp(current, to, diff, rt),
                    Comb.__B_ => new __B_().Lerp(current, to, diff, rt),
                    Comb.___A => new ___A().Lerp(current, to, diff, rt),
                    Comb.RG__ => new RG__().Lerp(current, to, diff, rt),
                    Comb.__BA => new __BA().Lerp(current, to, diff, rt),
                    Comb.R_B_ => new R_B_().Lerp(current, to, diff, rt),
                    Comb._G_A => new _G_A().Lerp(current, to, diff, rt),
                    Comb.R__A => new R__A().Lerp(current, to, diff, rt),
                    Comb._GB_ => new _GB_().Lerp(current, to, diff, rt),
                    Comb._GBA => new _GBA().Lerp(current, to, diff, rt),
                    Comb.R_BA => new R_BA().Lerp(current, to, diff, rt),
                    Comb.RG_A => new RG_A().Lerp(current, to, diff, rt),
                    Comb.RGB_ => new RGB_().Lerp(current, to, diff, rt),
                    Comb.RGBA => new RGBA().Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta);

            partial struct R___ { public float GetLength(Color to, Color from) => Mathf.Abs(to.r - from.r); }
            partial struct _G__ { public float GetLength(Color to, Color from) => Mathf.Abs(to.g - from.g); }
            partial struct __B_ { public float GetLength(Color to, Color from) => Mathf.Abs(to.b - from.b); }
            partial struct ___A { public float GetLength(Color to, Color from) => Mathf.Abs(to.a - from.a); }
            partial struct RG__ { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.g, from.g); }
            partial struct __BA { public float GetLength(Color to, Color from) => Hidden.GetLength(to.b, from.b, to.a, from.a); }
            partial struct R_B_ { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.b, from.b); }
            partial struct _G_A { public float GetLength(Color to, Color from) => Hidden.GetLength(to.g, from.g, to.a, from.a); }
            partial struct R__A { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.a, from.a); }
            partial struct _GB_ { public float GetLength(Color to, Color from) => Hidden.GetLength(to.g, from.g, to.b, from.b); }
            partial struct _GBA { public float GetLength(Color to, Color from) => Hidden.GetLength(to.g, from.g, to.b, from.b, to.a, from.a); }
            partial struct R_BA { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.b, from.b, to.a, from.a); }
            partial struct RG_A { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.g, from.g, to.a, from.a); }
            partial struct RGB_ { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.g, from.g, to.b, from.b); }
            partial struct RGBA { public float GetLength(Color to, Color from) => Hidden.GetLength(to.r, from.r, to.g, from.g, to.b, from.b, to.a, from.a); }
            partial struct R___ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _G__ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct __B_ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct ___A { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct RG__ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct __BA { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct R_B_ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _G_A { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct R__A { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _GB_ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _GBA { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct R_BA { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct RG_A { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct RGB_ { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }
            partial struct RGBA { public (Color, Color) GetParam(Color from, Color to, bool isDelta) => Param(from, to, isDelta); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (Color, Color) Param(Color from, Color to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }

            internal readonly partial struct R___ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    return current;
                }
            }
            internal readonly partial struct _G__ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.g = to.g + diff.g * rt;
                    return current;
                }
            }
            internal readonly partial struct __B_ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.b = to.b + diff.b * rt;
                    return current;
                }
            }
            internal readonly partial struct ___A : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct RG__ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.g = to.g + diff.g * rt;
                    return current;
                }
            }
            internal readonly partial struct __BA : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.b = to.b + diff.b * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct R_B_ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.b = to.b + diff.b * rt;
                    return current;
                }
            }
            internal readonly partial struct _G_A : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.g = to.g + diff.g * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct R__A : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct _GB_ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.g = to.g + diff.g * rt;
                    current.b = to.b + diff.b * rt;
                    return current;
                }
            }
            internal readonly partial struct _GBA : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.g = to.g + diff.g * rt;
                    current.b = to.b + diff.b * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct R_BA : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.b = to.b + diff.b * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct RG_A : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.g = to.g + diff.g * rt;
                    current.a = to.a + diff.a * rt;
                    return current;
                }
            }
            internal readonly partial struct RGB_ : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    current.r = to.r + diff.r * rt;
                    current.g = to.g + diff.g * rt;
                    current.b = to.b + diff.b * rt;
                    return current;
                }
            }
            internal readonly partial struct RGBA : Mover.IMapper<Color>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Color Lerp(Color current, Color to, Color diff, float rt)
                {
                    return to + diff * rt;
                }
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryQuaternion ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Quaternion p)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
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
            readonly Mover.PlanArg<C, Mapper, Quaternion> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Quaternion p) => this.planArg = new(carrier, p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Quaternion, E>(this.planArg, timeArg, ease, ref start);
        }

        readonly struct Mapper : Mover.IMapper<Quaternion>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Quaternion to, Quaternion from) => Quaternion.Angle(from, to);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Quaternion Lerp(Quaternion current, Quaternion to, Quaternion from, float rt) => Quaternion.SlerpUnclamped(to, from, rt);
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Quaternion, Quaternion) GetParam(Quaternion from, Quaternion to, bool isDelta)
            {
                if (isDelta) { to = from * to; }
                return (to, from);
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryRect ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, in Rect p = default)
            where C : struct, ICarrier
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Rect p = default)
            where C : struct, ICarrier
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, false, x, y, width, height);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, ICarrier
            => new(self, true, x, y, width, height);

        enum Comb
        {
            None,
            X___, _Y__, __W_, ___H,
            XY__, __WH,
            X_W_, _Y_H,
            X__H, _YW_,
            _YWH, X_WH, XY_H, XYW_,
            XYWH
        }

        [MethodImpl(MethodImplOptions.NoInlining)] // インライン化禁止
        static Comb Analyze(float? x, float? y, float? width, float? height)
        {
            var bits = 0;
            bits <<= 1; if (x != null) { bits |= 1; } 
            bits <<= 1; if (y != null) { bits |= 1; } 
            bits <<= 1; if (width != null) { bits |= 1; } 
            bits <<= 1; if (height != null) { bits |= 1; } 
            Dev.Assert(bits == 0, "引数が不正");
            return bits switch
                {
                    0b1000 => Comb.X___,
                    0b0100 => Comb._Y__,
                    0b0010 => Comb.__W_,
                    0b0001 => Comb.___H,
                    0b1100 => Comb.XY__,
                    0b0011 => Comb.__WH,
                    0b1010 => Comb.X_W_,
                    0b0101 => Comb._Y_H,
                    0b1001 => Comb.X__H,
                    0b0110 => Comb._YW_,
                    0b0111 => Comb._YWH,
                    0b1011 => Comb.X_WH,
                    0b1101 => Comb.XY_H,
                    0b1110 => Comb.XYW_,
                    0b1111 => Comb.XYWH,
                    _ => default
                };
        }

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
            readonly Rect to;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? width, float? height)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Analyze(x, y, width, height);
                this.to = new(x ?? default, y ?? default, width ?? default, height ?? default);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Rect p)
            {
                this.carrier = carrier;
                this.isDelta = isDelta;
                this.comb = Comb.XYWH;
                this.to = p;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X___ => Mover.Create<S, C, Mapper.X___, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y__ => Mover.Create<S, C, Mapper._Y__, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__W_ => Mover.Create<S, C, Mapper.__W_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.___H => Mover.Create<S, C, Mapper.___H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY__ => Mover.Create<S, C, Mapper.XY__, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__WH => Mover.Create<S, C, Mapper.__WH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_W_ => Mover.Create<S, C, Mapper.X_W_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y_H => Mover.Create<S, C, Mapper._Y_H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X__H => Mover.Create<S, C, Mapper.X__H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YW_ => Mover.Create<S, C, Mapper._YW_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YWH => Mover.Create<S, C, Mapper._YWH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_WH => Mover.Create<S, C, Mapper.X_WH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY_H => Mover.Create<S, C, Mapper.XY_H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYW_ => Mover.Create<S, C, Mapper.XYW_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYWH => Mover.Create<S, C, Mapper.XYWH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.Create<S, C, Mapper, Rect, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Rect>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Rect to, Rect from)
                => this.comb switch
                {
                    Comb.X___ => new X___().GetLength(to, from),
                    Comb._Y__ => new _Y__().GetLength(to, from),
                    Comb.__W_ => new __W_().GetLength(to, from),
                    Comb.___H => new ___H().GetLength(to, from),
                    Comb.XY__ => new XY__().GetLength(to, from),
                    Comb.__WH => new __WH().GetLength(to, from),
                    Comb.X_W_ => new X_W_().GetLength(to, from),
                    Comb._Y_H => new _Y_H().GetLength(to, from),
                    Comb.X__H => new X__H().GetLength(to, from),
                    Comb._YW_ => new _YW_().GetLength(to, from),
                    Comb._YWH => new _YWH().GetLength(to, from),
                    Comb.X_WH => new X_WH().GetLength(to, from),
                    Comb.XY_H => new XY_H().GetLength(to, from),
                    Comb.XYW_ => new XYW_().GetLength(to, from),
                    Comb.XYWH => new XYWH().GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                => this.comb switch
                {
                    Comb.X___ => new X___().Lerp(current, to, diff, rt),
                    Comb._Y__ => new _Y__().Lerp(current, to, diff, rt),
                    Comb.__W_ => new __W_().Lerp(current, to, diff, rt),
                    Comb.___H => new ___H().Lerp(current, to, diff, rt),
                    Comb.XY__ => new XY__().Lerp(current, to, diff, rt),
                    Comb.__WH => new __WH().Lerp(current, to, diff, rt),
                    Comb.X_W_ => new X_W_().Lerp(current, to, diff, rt),
                    Comb._Y_H => new _Y_H().Lerp(current, to, diff, rt),
                    Comb.X__H => new X__H().Lerp(current, to, diff, rt),
                    Comb._YW_ => new _YW_().Lerp(current, to, diff, rt),
                    Comb._YWH => new _YWH().Lerp(current, to, diff, rt),
                    Comb.X_WH => new X_WH().Lerp(current, to, diff, rt),
                    Comb.XY_H => new XY_H().Lerp(current, to, diff, rt),
                    Comb.XYW_ => new XYW_().Lerp(current, to, diff, rt),
                    Comb.XYWH => new XYWH().Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta);

            partial struct X___ { public float GetLength(Rect to, Rect from) => Mathf.Abs(to.x - from.x); }
            partial struct _Y__ { public float GetLength(Rect to, Rect from) => Mathf.Abs(to.y - from.y); }
            partial struct __W_ { public float GetLength(Rect to, Rect from) => Mathf.Abs(to.width - from.width); }
            partial struct ___H { public float GetLength(Rect to, Rect from) => Mathf.Abs(to.height - from.height); }
            partial struct XY__ { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.y, from.y); }
            partial struct __WH { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.width, from.width, to.height, from.height); }
            partial struct X_W_ { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.width, from.width); }
            partial struct _Y_H { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.y, from.y, to.height, from.height); }
            partial struct X__H { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.height, from.height); }
            partial struct _YW_ { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.y, from.y, to.width, from.width); }
            partial struct _YWH { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.y, from.y, to.width, from.width, to.height, from.height); }
            partial struct X_WH { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.width, from.width, to.height, from.height); }
            partial struct XY_H { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.y, from.y, to.height, from.height); }
            partial struct XYW_ { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.y, from.y, to.width, from.width); }
            partial struct XYWH { public float GetLength(Rect to, Rect from) => Hidden.GetLength(to.x, from.x, to.y, from.y, to.width, from.width, to.height, from.height); }
            partial struct X___ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _Y__ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct __W_ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct ___H { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XY__ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct __WH { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct X_W_ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _Y_H { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct X__H { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _YW_ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct _YWH { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct X_WH { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XY_H { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XYW_ { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }
            partial struct XYWH { public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => Param(from, to, isDelta); }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            static (Rect, Rect) Param(Rect from, Rect to, bool isDelta)
            {
                if (isDelta)
                {
                    to.position += from.position;
                    to.size += from.size;
                }
                from.position -= to.position;
                from.size -= to.size;
                return (to, from);
            }

            internal readonly partial struct X___ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    return current;
                }
            }
            internal readonly partial struct _Y__ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    return current;
                }
            }
            internal readonly partial struct __W_ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.width = to.width + diff.width * rt;
                    return current;
                }
            }
            internal readonly partial struct ___H : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct XY__ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.y = to.y + diff.y * rt;
                    return current;
                }
            }
            internal readonly partial struct __WH : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.width = to.width + diff.width * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct X_W_ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.width = to.width + diff.width * rt;
                    return current;
                }
            }
            internal readonly partial struct _Y_H : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct X__H : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct _YW_ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    current.width = to.width + diff.width * rt;
                    return current;
                }
            }
            internal readonly partial struct _YWH : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.y = to.y + diff.y * rt;
                    current.width = to.width + diff.width * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct X_WH : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.width = to.width + diff.width * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct XY_H : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.y = to.y + diff.y * rt;
                    current.height = to.height + diff.height * rt;
                    return current;
                }
            }
            internal readonly partial struct XYW_ : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.x = to.x + diff.x * rt;
                    current.y = to.y + diff.y * rt;
                    current.width = to.width + diff.width * rt;
                    return current;
                }
            }
            internal readonly partial struct XYWH : Mover.IMapper<Rect>
            {
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                {
                    current.position = to.position + diff.position * rt;
                    current.size = to.size + diff.size * rt;
                    return current;
                }
            }
        }
    }

    internal static partial class Hidden
    {
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetLength(float a0, float b0, float a1, float b1)
            => Mathf.Sqrt((a0 - b0) * (a0 - b0) + (a1 - b1) * (a1 - b1));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetLength(float a0, float b0, float a1, float b1, float a2, float b2)
            => Mathf.Sqrt((a0 - b0) * (a0 - b0) + (a1 - b1) * (a1 - b1) + (a2 - b2) * (a2 - b2));

        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        internal static float GetLength(float a0, float b0, float a1, float b1, float a2, float b2, float a3, float b3)
            => Mathf.Sqrt((a0 - b0) * (a0 - b0) + (a1 - b1) * (a1 - b1) + (a2 - b2) * (a2 - b2) + (a3 - b3) * (a3 - b3));
    }
}
