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

    using X_ = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _Y = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using XY = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;

    using X__ = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using _Y_ = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using __Z = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using _YZ = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using X_Z = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using XY_ = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using XYZ = StoryVector3.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;

    using R___ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using _G__ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using __B_ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using ___A = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using RG__ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using __BA = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using R_B_ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _G_A = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using R__A = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using _GB_ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _GBA = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using R_BA = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using RG_A = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using RGB_ = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using RGBA = StoryColor.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;

    using X___ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using _Y__ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using __W_ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using ___H = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using XY__ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore>;
    using __WH = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using X_W_ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _Y_H = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using X__H = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using _YW_ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _YWH = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using X_WH = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;
    using XY_H = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using XYW_ = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using XYWH = StoryRect.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryFloat ///////////////////////////////////////////////////////////////////////////////////
    {
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, float p) where C : struct, Mover.ICarrier<float> => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, float p) where C : struct, Mover.ICarrier<float> => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<float>
        {
            readonly Mover.PlanArg<C, Mapper, float> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float p) => this.planArg = new(carrier, p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
                => Mover.CreateTask<TS, C, Mapper, float, E>(this.planArg, timeArg, ease, ref start);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
                => Mover.CreateDummy<TS, C, Mapper, float, E>(ease);
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
            where C : struct, Mover.ICarrier<Vector2>
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, Vector2 p = default)
            where C : struct, Mover.ICarrier<Vector2>
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null)
            where C : struct, Mover.ICarrier<Vector2>
            => new(self, false, x, y);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null)
            where C : struct, Mover.ICarrier<Vector2>
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
            Dev.Assert(bits != 0, Messages.Exceptions.InvalidArguments);
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
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<Vector2>
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
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X_ => Mover.CreateTask<TS, C, X_, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y => Mover.CreateTask<TS, C, _Y, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY => Mover.CreateTask<TS, C, XY, Vector2, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.CreateTask<TS, C, Mapper, Vector2, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X_ => Mover.CreateDummy<TS, C, X_, Vector2, E>(ease),
                    Comb._Y => Mover.CreateDummy<TS, C, _Y, Vector2, E>(ease),
                    Comb.XY => Mover.CreateDummy<TS, C, XY, Vector2, E>(ease),
                    _ => default
                };
#else
                => Mover.CreateDummy<TS, C, Mapper, Vector2, E>(ease);
#endif
        }

        readonly struct Mapper : Mover.IMapper<Vector2>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
                => this.comb switch
                {
                    Comb.X_ => default(X_).GetLength(to, from),
                    Comb._Y => default(_Y).GetLength(to, from),
                    Comb.XY => default(XY).GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                => this.comb switch
                {
                    Comb.X_ => default(X_).Lerp(current, to, diff, rt),
                    Comb._Y => default(_Y).Lerp(current, to, diff, rt),
                    Comb.XY => default(XY).Lerp(current, to, diff, rt),
                    _ => current
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => default(XY).GetParam(from, to, isDelta);
        }

        internal readonly struct GenericMapper<X, Y> : Mover.IMapper<Vector2>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                return Mathf.Sqrt(xx + yy);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                return current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
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
            where C : struct, Mover.ICarrier<Vector3>
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Vector3 p = default)
            where C : struct, Mover.ICarrier<Vector3>
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? z = null)
            where C : struct, Mover.ICarrier<Vector3>
            => new(self, false, x, y, z);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? z = null)
            where C : struct, Mover.ICarrier<Vector3>
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
            Dev.Assert(bits != 0, Messages.Exceptions.InvalidArguments);
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
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<Vector3>
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
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X__ => Mover.CreateTask<TS, C, X__, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y_ => Mover.CreateTask<TS, C, _Y_, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__Z => Mover.CreateTask<TS, C, __Z, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YZ => Mover.CreateTask<TS, C, _YZ, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_Z => Mover.CreateTask<TS, C, X_Z, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY_ => Mover.CreateTask<TS, C, XY_, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYZ => Mover.CreateTask<TS, C, XYZ, Vector3, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.CreateTask<TS, C, Mapper, Vector3, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X__ => Mover.CreateDummy<TS, C, X__, Vector3, E>(ease),
                    Comb._Y_ => Mover.CreateDummy<TS, C, _Y_, Vector3, E>(ease),
                    Comb.__Z => Mover.CreateDummy<TS, C, __Z, Vector3, E>(ease),
                    Comb._YZ => Mover.CreateDummy<TS, C, _YZ, Vector3, E>(ease),
                    Comb.X_Z => Mover.CreateDummy<TS, C, X_Z, Vector3, E>(ease),
                    Comb.XY_ => Mover.CreateDummy<TS, C, XY_, Vector3, E>(ease),
                    Comb.XYZ => Mover.CreateDummy<TS, C, XYZ, Vector3, E>(ease),
                    _ => default
                };
#else
                => Mover.CreateDummy<TS, C, Mapper, Vector3, E>(ease);
#endif
        }

        readonly struct Mapper : Mover.IMapper<Vector3>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector3 to, Vector3 from)
                => this.comb switch
                {
                    Comb.X__ => default(X__).GetLength(to, from),
                    Comb._Y_ => default(_Y_).GetLength(to, from),
                    Comb.__Z => default(__Z).GetLength(to, from),
                    Comb._YZ => default(_YZ).GetLength(to, from),
                    Comb.X_Z => default(X_Z).GetLength(to, from),
                    Comb.XY_ => default(XY_).GetLength(to, from),
                    Comb.XYZ => default(XYZ).GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
                => this.comb switch
                {
                    Comb.X__ => default(X__).Lerp(current, to, diff, rt),
                    Comb._Y_ => default(_Y_).Lerp(current, to, diff, rt),
                    Comb.__Z => default(__Z).Lerp(current, to, diff, rt),
                    Comb._YZ => default(_YZ).Lerp(current, to, diff, rt),
                    Comb.X_Z => default(X_Z).Lerp(current, to, diff, rt),
                    Comb.XY_ => default(XY_).Lerp(current, to, diff, rt),
                    Comb.XYZ => default(XYZ).Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => default(XYZ).GetParam(from, to, isDelta);
        }

        internal readonly struct GenericMapper<X, Y, Z> : Mover.IMapper<Vector3>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
            where Z : struct, Mover.IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector3 to, Vector3 from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                var zz = default(Z).GetSqDiff(to.z, from.z);
                return Mathf.Sqrt(xx + yy + zz);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                current.z = default(Z).Lerp(current.z, to.z, diff.z, rt);
                return current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
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
            where C : struct, Mover.ICarrier<Color>
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Color p = default)
            where C : struct, Mover.ICarrier<Color>
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, Mover.ICarrier<Color>
            => new(self, false, r, g, b, a);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? r = null, float? g = null, float? b = null, float? a = null)
            where C : struct, Mover.ICarrier<Color>
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
            Dev.Assert(bits != 0, Messages.Exceptions.InvalidArguments);
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
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<Color>
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
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.R___ => Mover.CreateTask<TS, C, R___, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._G__ => Mover.CreateTask<TS, C, _G__, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__B_ => Mover.CreateTask<TS, C, __B_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.___A => Mover.CreateTask<TS, C, ___A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RG__ => Mover.CreateTask<TS, C, RG__, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__BA => Mover.CreateTask<TS, C, __BA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R_B_ => Mover.CreateTask<TS, C, R_B_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._G_A => Mover.CreateTask<TS, C, _G_A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R__A => Mover.CreateTask<TS, C, R__A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._GB_ => Mover.CreateTask<TS, C, _GB_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._GBA => Mover.CreateTask<TS, C, _GBA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.R_BA => Mover.CreateTask<TS, C, R_BA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RG_A => Mover.CreateTask<TS, C, RG_A, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RGB_ => Mover.CreateTask<TS, C, RGB_, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.RGBA => Mover.CreateTask<TS, C, RGBA, Color, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.CreateTask<TS, C, Mapper, Color, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.R___ => Mover.CreateDummy<TS, C, R___, Color, E>(ease),
                    Comb._G__ => Mover.CreateDummy<TS, C, _G__, Color, E>(ease),
                    Comb.__B_ => Mover.CreateDummy<TS, C, __B_, Color, E>(ease),
                    Comb.___A => Mover.CreateDummy<TS, C, ___A, Color, E>(ease),
                    Comb.RG__ => Mover.CreateDummy<TS, C, RG__, Color, E>(ease),
                    Comb.__BA => Mover.CreateDummy<TS, C, __BA, Color, E>(ease),
                    Comb.R_B_ => Mover.CreateDummy<TS, C, R_B_, Color, E>(ease),
                    Comb._G_A => Mover.CreateDummy<TS, C, _G_A, Color, E>(ease),
                    Comb.R__A => Mover.CreateDummy<TS, C, R__A, Color, E>(ease),
                    Comb._GB_ => Mover.CreateDummy<TS, C, _GB_, Color, E>(ease),
                    Comb._GBA => Mover.CreateDummy<TS, C, _GBA, Color, E>(ease),
                    Comb.R_BA => Mover.CreateDummy<TS, C, R_BA, Color, E>(ease),
                    Comb.RG_A => Mover.CreateDummy<TS, C, RG_A, Color, E>(ease),
                    Comb.RGB_ => Mover.CreateDummy<TS, C, RGB_, Color, E>(ease),
                    Comb.RGBA => Mover.CreateDummy<TS, C, RGBA, Color, E>(ease),
                    _ => default
                };
#else
                => Mover.CreateDummy<TS, C, Mapper, Color, E>(ease);
#endif
        }

        readonly struct Mapper : Mover.IMapper<Color>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Color to, Color from)
                => this.comb switch
                {
                    Comb.R___ => default(R___).GetLength(to, from),
                    Comb._G__ => default(_G__).GetLength(to, from),
                    Comb.__B_ => default(__B_).GetLength(to, from),
                    Comb.___A => default(___A).GetLength(to, from),
                    Comb.RG__ => default(RG__).GetLength(to, from),
                    Comb.__BA => default(__BA).GetLength(to, from),
                    Comb.R_B_ => default(R_B_).GetLength(to, from),
                    Comb._G_A => default(_G_A).GetLength(to, from),
                    Comb.R__A => default(R__A).GetLength(to, from),
                    Comb._GB_ => default(_GB_).GetLength(to, from),
                    Comb._GBA => default(_GBA).GetLength(to, from),
                    Comb.R_BA => default(R_BA).GetLength(to, from),
                    Comb.RG_A => default(RG_A).GetLength(to, from),
                    Comb.RGB_ => default(RGB_).GetLength(to, from),
                    Comb.RGBA => default(RGBA).GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Lerp(Color current, Color to, Color diff, float rt)
                => this.comb switch
                {
                    Comb.R___ => default(R___).Lerp(current, to, diff, rt),
                    Comb._G__ => default(_G__).Lerp(current, to, diff, rt),
                    Comb.__B_ => default(__B_).Lerp(current, to, diff, rt),
                    Comb.___A => default(___A).Lerp(current, to, diff, rt),
                    Comb.RG__ => default(RG__).Lerp(current, to, diff, rt),
                    Comb.__BA => default(__BA).Lerp(current, to, diff, rt),
                    Comb.R_B_ => default(R_B_).Lerp(current, to, diff, rt),
                    Comb._G_A => default(_G_A).Lerp(current, to, diff, rt),
                    Comb.R__A => default(R__A).Lerp(current, to, diff, rt),
                    Comb._GB_ => default(_GB_).Lerp(current, to, diff, rt),
                    Comb._GBA => default(_GBA).Lerp(current, to, diff, rt),
                    Comb.R_BA => default(R_BA).Lerp(current, to, diff, rt),
                    Comb.RG_A => default(RG_A).Lerp(current, to, diff, rt),
                    Comb.RGB_ => default(RGB_).Lerp(current, to, diff, rt),
                    Comb.RGBA => default(RGBA).Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Color, Color) GetParam(Color from, Color to, bool isDelta) => default(RGBA).GetParam(from, to, isDelta);
        }

        internal readonly struct GenericMapper<R, G, B, A> : Mover.IMapper<Color>
            where R : struct, Mover.IAxisFlag
            where G : struct, Mover.IAxisFlag
            where B : struct, Mover.IAxisFlag
            where A : struct, Mover.IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Color to, Color from)
            {
                var rr = default(R).GetSqDiff(to.r, from.r);
                var gg = default(G).GetSqDiff(to.g, from.g);
                var bb = default(B).GetSqDiff(to.b, from.b);
                var aa = default(A).GetSqDiff(to.a, from.a);
                return Mathf.Sqrt(rr + gg + bb + aa);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Lerp(Color current, Color to, Color diff, float rt)
            {
                current.r = default(R).Lerp(current.r, to.r, diff.r, rt);
                current.g = default(G).Lerp(current.g, to.g, diff.g, rt);
                current.b = default(B).Lerp(current.b, to.b, diff.b, rt);
                current.a = default(A).Lerp(current.a, to.a, diff.a, rt);
                return current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Color, Color) GetParam(Color from, Color to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
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
            where C : struct, Mover.ICarrier<Quaternion>
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Quaternion p)
            where C : struct, Mover.ICarrier<Quaternion>
            => new(self, true, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<Quaternion>
        {
            readonly Mover.PlanArg<C, Mapper, Quaternion> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Quaternion p) => this.planArg = new(carrier, p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
                => Mover.CreateTask<TS, C, Mapper, Quaternion, E>(this.planArg, timeArg, ease, ref start);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
                => Mover.CreateDummy<TS, C, Mapper, Quaternion, E>(ease);
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
            where C : struct, Mover.ICarrier<Rect>
            => new(self, false, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, in Rect p = default)
            where C : struct, Mover.ICarrier<Rect>
            => new(self, true, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components towards an absolute target.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> To<C>(this C self, bool _ = false, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, Mover.ICarrier<Rect>
            => new(self, false, x, y, width, height);

        /// <summary>Creates a zero-allocation tween plan to interpolate specific components by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Plan<C> By<C>(this C self, bool _ = true, float? x = null, float? y = null, float? width = null, float? height = null)
            where C : struct, Mover.ICarrier<Rect>
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
            Dev.Assert(bits != 0, Messages.Exceptions.InvalidArguments);
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
        public readonly struct Plan<C> : Mover.IPlan
            where C : struct, Mover.ICarrier<Rect>
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
            public Story.Task CreateTask<TS, E>(in TS _, in Mover.TimeArg timeArg, E ease, ref double start)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X___ => Mover.CreateTask<TS, C, X___, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y__ => Mover.CreateTask<TS, C, _Y__, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__W_ => Mover.CreateTask<TS, C, __W_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.___H => Mover.CreateTask<TS, C, ___H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY__ => Mover.CreateTask<TS, C, XY__, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.__WH => Mover.CreateTask<TS, C, __WH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_W_ => Mover.CreateTask<TS, C, X_W_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._Y_H => Mover.CreateTask<TS, C, _Y_H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X__H => Mover.CreateTask<TS, C, X__H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YW_ => Mover.CreateTask<TS, C, _YW_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb._YWH => Mover.CreateTask<TS, C, _YWH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.X_WH => Mover.CreateTask<TS, C, X_WH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XY_H => Mover.CreateTask<TS, C, XY_H, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYW_ => Mover.CreateTask<TS, C, XYW_, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    Comb.XYWH => Mover.CreateTask<TS, C, XYWH, Rect, E>(new(this.carrier, this.to, this.isDelta), timeArg, ease, ref start),
                    _ => default
                };
#else
                => Mover.CreateTask<TS, C, Mapper, Rect, E>(new(this.carrier, new(this.comb), this.to, this.isDelta), timeArg, ease, ref start);
#endif

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            // [MethodImpl(MethodImplOptions.AggressiveInlining)] // コンパイラに任せる
            public Story.Task CreateDummy<TS, E>(in TS _, E ease)
                where TS : struct, Story.ITimeSource
                where E : struct, Story.IEase
#if STORY_MOVER_FAST
                => this.comb switch
                {
                    Comb.X___ => Mover.CreateDummy<TS, C, X___, Rect, E>(ease),
                    Comb._Y__ => Mover.CreateDummy<TS, C, _Y__, Rect, E>(ease),
                    Comb.__W_ => Mover.CreateDummy<TS, C, __W_, Rect, E>(ease),
                    Comb.___H => Mover.CreateDummy<TS, C, ___H, Rect, E>(ease),
                    Comb.XY__ => Mover.CreateDummy<TS, C, XY__, Rect, E>(ease),
                    Comb.__WH => Mover.CreateDummy<TS, C, __WH, Rect, E>(ease),
                    Comb.X_W_ => Mover.CreateDummy<TS, C, X_W_, Rect, E>(ease),
                    Comb._Y_H => Mover.CreateDummy<TS, C, _Y_H, Rect, E>(ease),
                    Comb.X__H => Mover.CreateDummy<TS, C, X__H, Rect, E>(ease),
                    Comb._YW_ => Mover.CreateDummy<TS, C, _YW_, Rect, E>(ease),
                    Comb._YWH => Mover.CreateDummy<TS, C, _YWH, Rect, E>(ease),
                    Comb.X_WH => Mover.CreateDummy<TS, C, X_WH, Rect, E>(ease),
                    Comb.XY_H => Mover.CreateDummy<TS, C, XY_H, Rect, E>(ease),
                    Comb.XYW_ => Mover.CreateDummy<TS, C, XYW_, Rect, E>(ease),
                    Comb.XYWH => Mover.CreateDummy<TS, C, XYWH, Rect, E>(ease),
                    _ => default
                };
#else
                => Mover.CreateDummy<TS, C, Mapper, Rect, E>(ease);
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
                    Comb.X___ => default(X___).GetLength(to, from),
                    Comb._Y__ => default(_Y__).GetLength(to, from),
                    Comb.__W_ => default(__W_).GetLength(to, from),
                    Comb.___H => default(___H).GetLength(to, from),
                    Comb.XY__ => default(XY__).GetLength(to, from),
                    Comb.__WH => default(__WH).GetLength(to, from),
                    Comb.X_W_ => default(X_W_).GetLength(to, from),
                    Comb._Y_H => default(_Y_H).GetLength(to, from),
                    Comb.X__H => default(X__H).GetLength(to, from),
                    Comb._YW_ => default(_YW_).GetLength(to, from),
                    Comb._YWH => default(_YWH).GetLength(to, from),
                    Comb.X_WH => default(X_WH).GetLength(to, from),
                    Comb.XY_H => default(XY_H).GetLength(to, from),
                    Comb.XYW_ => default(XYW_).GetLength(to, from),
                    Comb.XYWH => default(XYWH).GetLength(to, from),
                    _ => default
                };

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
                => this.comb switch
                {
                    Comb.X___ => default(X___).Lerp(current, to, diff, rt),
                    Comb._Y__ => default(_Y__).Lerp(current, to, diff, rt),
                    Comb.__W_ => default(__W_).Lerp(current, to, diff, rt),
                    Comb.___H => default(___H).Lerp(current, to, diff, rt),
                    Comb.XY__ => default(XY__).Lerp(current, to, diff, rt),
                    Comb.__WH => default(__WH).Lerp(current, to, diff, rt),
                    Comb.X_W_ => default(X_W_).Lerp(current, to, diff, rt),
                    Comb._Y_H => default(_Y_H).Lerp(current, to, diff, rt),
                    Comb.X__H => default(X__H).Lerp(current, to, diff, rt),
                    Comb._YW_ => default(_YW_).Lerp(current, to, diff, rt),
                    Comb._YWH => default(_YWH).Lerp(current, to, diff, rt),
                    Comb.X_WH => default(X_WH).Lerp(current, to, diff, rt),
                    Comb.XY_H => default(XY_H).Lerp(current, to, diff, rt),
                    Comb.XYW_ => default(XYW_).Lerp(current, to, diff, rt),
                    Comb.XYWH => default(XYWH).Lerp(current, to, diff, rt),
                    _ => current
                };
            
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => default(XYWH).GetParam(from, to, isDelta);
        }

        internal readonly struct GenericMapper<X, Y, W, H> : Mover.IMapper<Rect>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
            where W : struct, Mover.IAxisFlag
            where H : struct, Mover.IAxisFlag
        {
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Rect to, Rect from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                var ww = default(W).GetSqDiff(to.width, from.width);
                var hh = default(H).GetSqDiff(to.height, from.height);
                return Mathf.Sqrt(xx + yy + ww + hh);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                current.width = default(W).Lerp(current.width, to.width, diff.width, rt);
                current.height = default(H).Lerp(current.height, to.height, diff.height, rt);
                return current;
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta)
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
        }
    }
}
