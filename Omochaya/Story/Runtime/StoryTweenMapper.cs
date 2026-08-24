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

    using IXY = HiddenStory.Mover.ICarrier<UnityEngine.Vector2>;
    using X_ = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisIgnore>;
    using _Y = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisIgnore,
        HiddenStory.Mover.AxisUse>;
    using XY = StoryVector2.GenericMapper<
        HiddenStory.Mover.AxisUse,
        HiddenStory.Mover.AxisUse>;

    using IXYZ = HiddenStory.Mover.ICarrier<UnityEngine.Vector3>;
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

    using IRGBA = HiddenStory.Mover.ICarrier<UnityEngine.Color>;
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

    using IXYWH = HiddenStory.Mover.ICarrier<UnityEngine.Rect>;
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
        public static Mover.Plan<C, Mapper, float> To<C>(this C self, float p) where C : struct, Mover.ICarrier<float> => new(self, false, default, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Plan<C, Mapper, float> By<C>(this C self, float p) where C : struct, Mover.ICarrier<float> => new(self, true, default, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Mapper : Mover.IMapper<float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(float to, float from) => Mathf.Abs(to - from);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Lerp(float current, float to, float diff, float rt) => to + diff * rt;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Set(float current, float to) => to;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (float, float) GetParam(float from, float to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }
        }
    }

    /// <summary>Don't touch! Only for system.</summary>
    [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
#if STORY_MOVER_FAST
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, XY, Vector2> To<C>(this C self, Vector2 p = default, bool isDelta = false, XY _ = default) where C : struct, IXY => new(self, isDelta, _, p);
        public static Mover.Plan<C, X_, Vector2> To<C>(this C self, float x, bool isDelta = false, X_ _ = default) where C : struct, IXY => new(self, isDelta, _, new(x, 0));
        public static Mover.Plan<C, _Y, Vector2> To<C>(this C self, float y, bool isDelta = false, _Y _ = default) where C : struct, IXY => new(self, isDelta, _, new(0, y));
        public static Mover.Plan<C, XY, Vector2> To<C>(this C self, float x, float y, bool isDelta = false, XY _ = default) where C : struct, IXY => new(self, isDelta, _, new(x, y));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, XY, Vector2> By<C>(this C self, Vector2 p = default, XY _ = default) where C : struct, IXY => To(self, p, true);
        public static Mover.Plan<C, X_, Vector2> By<C>(this C self, float x, X_ _ = default) where C : struct, IXY => To(self, x:x, true);
        public static Mover.Plan<C, _Y, Vector2> By<C>(this C self, float y, _Y _ = default) where C : struct, IXY => To(self, y:y, true);
        public static Mover.Plan<C, XY, Vector2> By<C>(this C self, float x, float y, XY _ = default) where C : struct, IXY => To(self, x, y, true);
#else
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, Mapper, Vector2> To<C>(this C self, Vector2 p = default, bool isDelta = false, XY _ = default) where C : struct, IXY => new(self, isDelta, new(Comb.XY), p);
        public static Mover.Plan<C, Mapper, Vector2> To<C>(this C self, float x, bool isDelta = false, X_ _ = default) where C : struct, IXY => new(self, isDelta, new(Comb.X_), new(x, 0));
        public static Mover.Plan<C, Mapper, Vector2> To<C>(this C self, float y, bool isDelta = false, _Y _ = default) where C : struct, IXY => new(self, isDelta, new(Comb._Y), new(0, y));
        public static Mover.Plan<C, Mapper, Vector2> To<C>(this C self, float x, float y, bool isDelta = false, XY _ = default) where C : struct, IXY => new(self, isDelta, new(Comb.XY), new(x, y));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, Mapper, Vector2> By<C>(this C self, Vector2 p = default, XY _ = default) where C : struct, IXY => To(self, p, true);
        public static Mover.Plan<C, Mapper, Vector2> By<C>(this C self, float x, X_ _ = default) where C : struct, IXY => To(self, x:x, true);
        public static Mover.Plan<C, Mapper, Vector2> By<C>(this C self, float y, _Y _ = default) where C : struct, IXY => To(self, y:y, true);
        public static Mover.Plan<C, Mapper, Vector2> By<C>(this C self, float x, float y, XY _ = default) where C : struct, IXY => To(self, x, y, true);
#endif

        /// <summary></summary>
        public static void Set<C>(this C self, float x, X_ _ = default) where C : struct, IXY => To(self, x:x).SetEnd();
        public static void Set<C>(this C self, float y, _Y _ = default) where C : struct, IXY => To(self, y:y).SetEnd();

        internal enum Comb
        {
            None,
            X_, _Y,
            XY
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Mapper : Mover.IMapper<Vector2>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
                => this.comb switch
                {
                    Comb.X_ => default(X_).GetLength(to, from),
                    Comb._Y => default(_Y).GetLength(to, from),
                    Comb.XY => default(XY).GetLength(to, from),
                    _ => default
                };

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
                => this.comb switch
                {
                    Comb.X_ => default(X_).Lerp(current, to, diff, rt),
                    Comb._Y => default(_Y).Lerp(current, to, diff, rt),
                    Comb.XY => default(XY).Lerp(current, to, diff, rt),
                    _ => current
                };

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Set(Vector2 current, Vector2 to)
                => this.comb switch
                {
                    Comb.X_ => default(X_).Set(current, to),
                    Comb._Y => default(_Y).Set(current, to),
                    Comb.XY => default(XY).Set(current, to),
                    _ => current
                };

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector2, Vector2) GetParam(Vector2 from, Vector2 to, bool isDelta) => default(XY).GetParam(from, to, isDelta);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct GenericMapper<X, Y> : Mover.IMapper<Vector2>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                return Mathf.Sqrt(xx + yy);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Set(Vector2 current, Vector2 to)
            {
                current.x = default(X).Set(current.x, to.x);
                current.y = default(Y).Set(current.y, to.y);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
#if STORY_MOVER_FAST
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, XYZ, Vector3> To<C>(this C self, Vector3 p = default, bool isDelta = false, XYZ _ = default) where C : struct, IXYZ => new(self, isDelta, _, p);
        public static Mover.Plan<C, X__, Vector3> To<C>(this C self, float x, bool isDelta = false, X__ _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(x, 0, 0));
        public static Mover.Plan<C, _Y_, Vector3> To<C>(this C self, float y, bool isDelta = false, _Y_ _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(0, y, 0));
        public static Mover.Plan<C, __Z, Vector3> To<C>(this C self, float z, bool isDelta = false, __Z _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(0, 0, z));
        public static Mover.Plan<C, XY_, Vector3> To<C>(this C self, float x, float y, bool isDelta = false, XY_ _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(x, y, 0));
        public static Mover.Plan<C, _YZ, Vector3> To<C>(this C self, float y, float z, bool isDelta = false, _YZ _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(0, y, z));
        public static Mover.Plan<C, X_Z, Vector3> To<C>(this C self, float x, float z, bool isDelta = false, X_Z _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(x, 0, z));
        public static Mover.Plan<C, XYZ, Vector3> To<C>(this C self, float x, float y, float z, bool isDelta = false, XYZ _ = default) where C : struct, IXYZ => new(self, isDelta, _, new(x, y, z));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, XYZ, Vector3> By<C>(this C self, Vector3 p = default, XYZ _ = default) where C : struct, IXYZ => To(self, p, true);
        public static Mover.Plan<C, X__, Vector3> By<C>(this C self, float x, X__ _ = default) where C : struct, IXYZ => To(self, x:x, true);
        public static Mover.Plan<C, _Y_, Vector3> By<C>(this C self, float y, _Y_ _ = default) where C : struct, IXYZ => To(self, y:y, true);
        public static Mover.Plan<C, __Z, Vector3> By<C>(this C self, float z, __Z _ = default) where C : struct, IXYZ => To(self, z:z, true);
        public static Mover.Plan<C, XY_, Vector3> By<C>(this C self, float x, float y, XY_ _ = default) where C : struct, IXYZ => To(self, x:x, y:y, true);
        public static Mover.Plan<C, _YZ, Vector3> By<C>(this C self, float y, float z, _YZ _ = default) where C : struct, IXYZ => To(self, y:y, z:z, true);
        public static Mover.Plan<C, X_Z, Vector3> By<C>(this C self, float x, float z, X_Z _ = default) where C : struct, IXYZ => To(self, x:x, z:z, true);
        public static Mover.Plan<C, XYZ, Vector3> By<C>(this C self, float x, float y, float z, XYZ _ = default) where C : struct, IXYZ => To(self, x, y, z, true);
#else
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, Vector3 p = default, bool isDelta = false, XYZ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.XYZ), p);
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float x, bool isDelta = false, X__ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.X__), new(x, 0, 0));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float y, bool isDelta = false, _Y_ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb._Y_), new(0, y, 0));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float z, bool isDelta = false, __Z _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.__Z), new(0, 0, z));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float x, float y, bool isDelta = false, XY_ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.XY_), new(x, y, 0));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float y, float z, bool isDelta = false, _YZ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb._YZ), new(0, y, z));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float x, float z, bool isDelta = false, X_Z _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.X_Z), new(x, 0, z));
        public static Mover.Plan<C, Mapper, Vector3> To<C>(this C self, float x, float y, float z, bool isDelta = false, XYZ _ = default) where C : struct, IXYZ => new(self, isDelta, new(Comb.XYZ), new(x, y, z));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, Vector3 p = default, XYZ _ = default) where C : struct, IXYZ => To(self, p, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float x, X__ _ = default) where C : struct, IXYZ => To(self, x:x, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float y, _Y_ _ = default) where C : struct, IXYZ => To(self, y:y, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float z, __Z _ = default) where C : struct, IXYZ => To(self, z:z, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float x, float y, XY_ _ = default) where C : struct, IXYZ => To(self, x:x, y:y, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float y, float z, _YZ _ = default) where C : struct, IXYZ => To(self, y:y, z:z, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float x, float z, X_Z _ = default) where C : struct, IXYZ => To(self, x:x, z:z, true);
        public static Mover.Plan<C, Mapper, Vector3> By<C>(this C self, float x, float y, float z, XYZ _ = default) where C : struct, IXYZ => To(self, x, y, z, true);
#endif

        /// <summary></summary>
        public static void Set<C>(this C self, float x, X__ _ = default) where C : struct, IXYZ => To(self, x:x).SetEnd();
        public static void Set<C>(this C self, float y, _Y_ _ = default) where C : struct, IXYZ => To(self, y:y).SetEnd();
        public static void Set<C>(this C self, float z, __Z _ = default) where C : struct, IXYZ => To(self, z:z).SetEnd();
        public static void Set<C>(this C self, float x, float y, XY_ _ = default) where C : struct, IXYZ => To(self, x:x, y:y).SetEnd();
        public static void Set<C>(this C self, float y, float z, _YZ _ = default) where C : struct, IXYZ => To(self, y:y, z:z).SetEnd();
        public static void Set<C>(this C self, float x, float z, X_Z _ = default) where C : struct, IXYZ => To(self, x:x, z:z).SetEnd();

        internal enum Comb
        {
            None,
            X__, _Y_, __Z,
            _YZ, X_Z, XY_,
            XYZ
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Mapper : Mover.IMapper<Vector3>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Set(Vector3 current, Vector3 to)
                => this.comb switch
                {
                    Comb.X__ => default(X__).Set(current, to),
                    Comb._Y_ => default(_Y_).Set(current, to),
                    Comb.__Z => default(__Z).Set(current, to),
                    Comb._YZ => default(_YZ).Set(current, to),
                    Comb.X_Z => default(X_Z).Set(current, to),
                    Comb.XY_ => default(XY_).Set(current, to),
                    Comb.XYZ => default(XYZ).Set(current, to),
                    _ => current
                };
            
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta) => default(XYZ).GetParam(from, to, isDelta);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct GenericMapper<X, Y, Z> : Mover.IMapper<Vector3>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
            where Z : struct, Mover.IAxisFlag
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector3 to, Vector3 from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                var zz = default(Z).GetSqDiff(to.z, from.z);
                return Mathf.Sqrt(xx + yy + zz);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                current.z = default(Z).Lerp(current.z, to.z, diff.z, rt);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Set(Vector3 current, Vector3 to)
            {
                current.x = default(X).Set(current.x, to.x);
                current.y = default(Y).Set(current.y, to.y);
                current.z = default(Z).Set(current.z, to.z);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
#if STORY_MOVER_FAST
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, RGBA, Color> To<C>(this C self, Color p = default, bool isDelta = false, RGBA _ = default) where C : struct, IRGBA => new(self, isDelta, _, p);
        public static Mover.Plan<C, R___, Color> To<C>(this C self, float r, bool isDelta = false, R___ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, 0, 0, 0));
        public static Mover.Plan<C, _G__, Color> To<C>(this C self, float g, bool isDelta = false, _G__ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, g, 0, 0));
        public static Mover.Plan<C, __B_, Color> To<C>(this C self, float b, bool isDelta = false, __B_ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, 0, b, 0));
        public static Mover.Plan<C, ___A, Color> To<C>(this C self, float a, bool isDelta = false, ___A _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, 0, 0, a));
        public static Mover.Plan<C, RG__, Color> To<C>(this C self, float r, float g, bool isDelta = false, RG__ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, g, 0, 0));
        public static Mover.Plan<C, __BA, Color> To<C>(this C self, float b, float a, bool isDelta = false, __BA _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, 0, b, a));
        public static Mover.Plan<C, R_B_, Color> To<C>(this C self, float r, float b, bool isDelta = false, R_B_ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, 0, b, 0));
        public static Mover.Plan<C, _G_A, Color> To<C>(this C self, float g, float a, bool isDelta = false, _G_A _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, g, 0, a));
        public static Mover.Plan<C, R__A, Color> To<C>(this C self, float r, float a, bool isDelta = false, R__A _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, 0, 0, a));
        public static Mover.Plan<C, _GB_, Color> To<C>(this C self, float g, float b, bool isDelta = false, _GB_ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, g, b, 0));
        public static Mover.Plan<C, _GBA, Color> To<C>(this C self, float g, float b, float a, bool isDelta = false, _GBA _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(0, g, b, a));
        public static Mover.Plan<C, R_BA, Color> To<C>(this C self, float r, float b, float a, bool isDelta = false, R_BA _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, 0, b, a));
        public static Mover.Plan<C, RG_A, Color> To<C>(this C self, float r, float g, float a, bool isDelta = false, RG_A _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, g, 0, a));
        public static Mover.Plan<C, RGB_, Color> To<C>(this C self, float r, float g, float b, bool isDelta = false, RGB_ _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, g, b, 0));
        public static Mover.Plan<C, RGBA, Color> To<C>(this C self, float r, float g, float b, float a, bool isDelta = false, RGBA _ = default) where C : struct, IRGBA => new(self, isDelta, _, new(r, g, b, a));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, RGBA, Color> By<C>(this C self, Color p = default, RGBA _ = default) where C : struct, IRGBA => To(self, p, true);
        public static Mover.Plan<C, R___, Color> By<C>(this C self, float r, R___ _ = default) where C : struct, IRGBA => To(self, r:r, true);
        public static Mover.Plan<C, _G__, Color> By<C>(this C self, float g, _G__ _ = default) where C : struct, IRGBA => To(self, g:g, true);
        public static Mover.Plan<C, __B_, Color> By<C>(this C self, float b, __B_ _ = default) where C : struct, IRGBA => To(self, b:b, true);
        public static Mover.Plan<C, ___A, Color> By<C>(this C self, float a, ___A _ = default) where C : struct, IRGBA => To(self, a:a, true);
        public static Mover.Plan<C, RG__, Color> By<C>(this C self, float r, float g, RG__ _ = default) where C : struct, IRGBA => To(self, r:r, g:g, true);
        public static Mover.Plan<C, __BA, Color> By<C>(this C self, float b, float a, __BA _ = default) where C : struct, IRGBA => To(self, b:b, a:a, true);
        public static Mover.Plan<C, R_B_, Color> By<C>(this C self, float r, float b, R_B_ _ = default) where C : struct, IRGBA => To(self, r:r, b:b, true);
        public static Mover.Plan<C, _G_A, Color> By<C>(this C self, float g, float a, _G_A _ = default) where C : struct, IRGBA => To(self, g:g, a:a, true);
        public static Mover.Plan<C, R__A, Color> By<C>(this C self, float r, float a, R__A _ = default) where C : struct, IRGBA => To(self, r:r, a:a, true);
        public static Mover.Plan<C, _GB_, Color> By<C>(this C self, float g, float b, _GB_ _ = default) where C : struct, IRGBA => To(self, g:g, b:b, true);
        public static Mover.Plan<C, _GBA, Color> By<C>(this C self, float g, float b, float a, _GBA _ = default) where C : struct, IRGBA => To(self, g:g, b:b, a:a, true);
        public static Mover.Plan<C, R_BA, Color> By<C>(this C self, float r, float b, float a, R_BA _ = default) where C : struct, IRGBA => To(self, r:r, b:b, a:a, true);
        public static Mover.Plan<C, RG_A, Color> By<C>(this C self, float r, float g, float a, RG_A _ = default) where C : struct, IRGBA => To(self, r:r, g:g, a:a, true);
        public static Mover.Plan<C, RGB_, Color> By<C>(this C self, float r, float g, float b, RGB_ _ = default) where C : struct, IRGBA => To(self, r:r, g:g, b:b, true);
        public static Mover.Plan<C, RGBA, Color> By<C>(this C self, float r, float g, float b, float a, RGBA _ = default) where C : struct, IRGBA => To(self, r, g, b, a, true);
#else
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, Color p = default, bool isDelta = false, RGBA _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.RGBA), p);
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, bool isDelta = false, R___ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.R___), new(r, 0, 0, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float g, bool isDelta = false, _G__ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb._G__), new(0, g, 0, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float b, bool isDelta = false, __B_ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.__B_), new(0, 0, b, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float a, bool isDelta = false, ___A _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.___A), new(0, 0, 0, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float g, bool isDelta = false, RG__ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.RG__), new(r, g, 0, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float b, float a, bool isDelta = false, __BA _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.__BA), new(0, 0, b, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float b, bool isDelta = false, R_B_ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.R_B_), new(r, 0, b, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float g, float a, bool isDelta = false, _G_A _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb._G_A), new(0, g, 0, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float a, bool isDelta = false, R__A _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.R__A), new(r, 0, 0, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float g, float b, bool isDelta = false, _GB_ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb._GB_), new(0, g, b, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float g, float b, float a, bool isDelta = false, _GBA _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb._GBA), new(0, g, b, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float b, float a, bool isDelta = false, R_BA _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.R_BA), new(r, 0, b, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float g, float a, bool isDelta = false, RG_A _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.RG_A), new(r, g, 0, a));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float g, float b, bool isDelta = false, RGB_ _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.RGB_), new(r, g, b, 0));
        public static Mover.Plan<C, Mapper, Color> To<C>(this C self, float r, float g, float b, float a, bool isDelta = false, RGBA _ = default) where C : struct, IRGBA => new(self, isDelta, new(Comb.RGBA), new(r, g, b, a));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, Color p = default, RGBA _ = default) where C : struct, IRGBA => To(self, p, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, R___ _ = default) where C : struct, IRGBA => To(self, r:r, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float g, _G__ _ = default) where C : struct, IRGBA => To(self, g:g, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float b, __B_ _ = default) where C : struct, IRGBA => To(self, b:b, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float a, ___A _ = default) where C : struct, IRGBA => To(self, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float g, RG__ _ = default) where C : struct, IRGBA => To(self, r:r, g:g, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float b, float a, __BA _ = default) where C : struct, IRGBA => To(self, b:b, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float b, R_B_ _ = default) where C : struct, IRGBA => To(self, r:r, b:b, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float g, float a, _G_A _ = default) where C : struct, IRGBA => To(self, g:g, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float a, R__A _ = default) where C : struct, IRGBA => To(self, r:r, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float g, float b, _GB_ _ = default) where C : struct, IRGBA => To(self, g:g, b:b, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float g, float b, float a, _GBA _ = default) where C : struct, IRGBA => To(self, g:g, b:b, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float b, float a, R_BA _ = default) where C : struct, IRGBA => To(self, r:r, b:b, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float g, float a, RG_A _ = default) where C : struct, IRGBA => To(self, r:r, g:g, a:a, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float g, float b, RGB_ _ = default) where C : struct, IRGBA => To(self, r:r, g:g, b:b, true);
        public static Mover.Plan<C, Mapper, Color> By<C>(this C self, float r, float g, float b, float a, RGBA _ = default) where C : struct, IRGBA => To(self, r, g, b, a, true);
#endif


        /// <summary></summary>
        public static void Set<C>(this C self, float r, R___ _ = default) where C : struct, IRGBA => To(self, r:r).SetEnd();
        public static void Set<C>(this C self, float g, _G__ _ = default) where C : struct, IRGBA => To(self, g:g).SetEnd();
        public static void Set<C>(this C self, float b, __B_ _ = default) where C : struct, IRGBA => To(self, b:b).SetEnd();
        public static void Set<C>(this C self, float a, ___A _ = default) where C : struct, IRGBA => To(self, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float g, RG__ _ = default) where C : struct, IRGBA => To(self, r:r, g:g).SetEnd();
        public static void Set<C>(this C self, float b, float a, __BA _ = default) where C : struct, IRGBA => To(self, b:b, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float b, R_B_ _ = default) where C : struct, IRGBA => To(self, r:r, b:b).SetEnd();
        public static void Set<C>(this C self, float g, float a, _G_A _ = default) where C : struct, IRGBA => To(self, g:g, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float a, R__A _ = default) where C : struct, IRGBA => To(self, r:r, a:a).SetEnd();
        public static void Set<C>(this C self, float g, float b, _GB_ _ = default) where C : struct, IRGBA => To(self, g:g, b:b).SetEnd();
        public static void Set<C>(this C self, float g, float b, float a, _GBA _ = default) where C : struct, IRGBA => To(self, g:g, b:b, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float b, float a, R_BA _ = default) where C : struct, IRGBA => To(self, r:r, b:b, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float g, float a, RG_A _ = default) where C : struct, IRGBA => To(self, r:r, g:g, a:a).SetEnd();
        public static void Set<C>(this C self, float r, float g, float b, RGB_ _ = default) where C : struct, IRGBA => To(self, r:r, g:g, b:b).SetEnd();

        internal enum Comb
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
        public readonly struct Mapper : Mover.IMapper<Color>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Set(Color current, Color to)
                => this.comb switch
                {
                    Comb.R___ => default(R___).Set(current, to),
                    Comb._G__ => default(_G__).Set(current, to),
                    Comb.__B_ => default(__B_).Set(current, to),
                    Comb.___A => default(___A).Set(current, to),
                    Comb.RG__ => default(RG__).Set(current, to),
                    Comb.__BA => default(__BA).Set(current, to),
                    Comb.R_B_ => default(R_B_).Set(current, to),
                    Comb._G_A => default(_G_A).Set(current, to),
                    Comb.R__A => default(R__A).Set(current, to),
                    Comb._GB_ => default(_GB_).Set(current, to),
                    Comb._GBA => default(_GBA).Set(current, to),
                    Comb.R_BA => default(R_BA).Set(current, to),
                    Comb.RG_A => default(RG_A).Set(current, to),
                    Comb.RGB_ => default(RGB_).Set(current, to),
                    Comb.RGBA => default(RGBA).Set(current, to),
                    _ => current
                };
            
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Color, Color) GetParam(Color from, Color to, bool isDelta) => default(RGBA).GetParam(from, to, isDelta);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct GenericMapper<R, G, B, A> : Mover.IMapper<Color>
            where R : struct, Mover.IAxisFlag
            where G : struct, Mover.IAxisFlag
            where B : struct, Mover.IAxisFlag
            where A : struct, Mover.IAxisFlag
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Color to, Color from)
            {
                var rr = default(R).GetSqDiff(to.r, from.r);
                var gg = default(G).GetSqDiff(to.g, from.g);
                var bb = default(B).GetSqDiff(to.b, from.b);
                var aa = default(A).GetSqDiff(to.a, from.a);
                return Mathf.Sqrt(rr + gg + bb + aa);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Lerp(Color current, Color to, Color diff, float rt)
            {
                current.r = default(R).Lerp(current.r, to.r, diff.r, rt);
                current.g = default(G).Lerp(current.g, to.g, diff.g, rt);
                current.b = default(B).Lerp(current.b, to.b, diff.b, rt);
                current.a = default(A).Lerp(current.a, to.a, diff.a, rt);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Set(Color current, Color to)
            {
                current.r = default(R).Set(current.r, to.r);
                current.g = default(G).Set(current.g, to.g);
                current.b = default(B).Set(current.b, to.b);
                current.a = default(A).Set(current.a, to.a);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
        public static Mover.Plan<C, Mapper, Quaternion> To<C>(this C self, Quaternion p) where C : struct, Mover.ICarrier<Quaternion> => new(self, false, default, p);

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Plan<C, Mapper, Quaternion> By<C>(this C self, Quaternion p) where C : struct, Mover.ICarrier<Quaternion> => new(self, true, default, p);

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Mapper : Mover.IMapper<Quaternion>
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
            public Quaternion Set(Quaternion current, Quaternion to) => to;

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
#if STORY_MOVER_FAST
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, XYWH, Rect> To<C>(this C self, Rect p = default, bool isDelta = false, XYWH _ = default) where C : struct, IXYWH => new(self, isDelta, _, p);
        public static Mover.Plan<C, X___, Rect> To<C>(this C self, float x, bool isDelta = false, X___ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, 0, 0, 0));
        public static Mover.Plan<C, _Y__, Rect> To<C>(this C self, float y, bool isDelta = false, _Y__ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, y, 0, 0));
        public static Mover.Plan<C, __W_, Rect> To<C>(this C self, float w, bool isDelta = false, __W_ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, 0, w, 0));
        public static Mover.Plan<C, ___H, Rect> To<C>(this C self, float h, bool isDelta = false, ___H _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, 0, 0, h));
        public static Mover.Plan<C, XY__, Rect> To<C>(this C self, float x, float y, bool isDelta = false, XY__ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, y, 0, 0));
        public static Mover.Plan<C, __WH, Rect> To<C>(this C self, float w, float h, bool isDelta = false, __WH _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, 0, w, h));
        public static Mover.Plan<C, X_W_, Rect> To<C>(this C self, float x, float w, bool isDelta = false, X_W_ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, 0, w, 0));
        public static Mover.Plan<C, _Y_H, Rect> To<C>(this C self, float y, float h, bool isDelta = false, _Y_H _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, y, 0, h));
        public static Mover.Plan<C, X__H, Rect> To<C>(this C self, float x, float h, bool isDelta = false, X__H _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, 0, 0, h));
        public static Mover.Plan<C, _YW_, Rect> To<C>(this C self, float y, float w, bool isDelta = false, _YW_ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, y, w, 0));
        public static Mover.Plan<C, _YWH, Rect> To<C>(this C self, float y, float w, float h, bool isDelta = false, _YWH _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(0, y, w, h));
        public static Mover.Plan<C, X_WH, Rect> To<C>(this C self, float x, float w, float h, bool isDelta = false, X_WH _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, 0, w, h));
        public static Mover.Plan<C, XY_H, Rect> To<C>(this C self, float x, float y, float h, bool isDelta = false, XY_H _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, y, 0, h));
        public static Mover.Plan<C, XYW_, Rect> To<C>(this C self, float x, float y, float w, bool isDelta = false, XYW_ _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, y, w, 0));
        public static Mover.Plan<C, XYWH, Rect> To<C>(this C self, float x, float y, float w, float h, bool isDelta = false, XYWH _ = default) where C : struct, IXYWH => new(self, isDelta, _, new(x, y, w, h));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, XYWH, Rect> By<C>(this C self, Rect p = default, XYWH _ = default) where C : struct, IXYWH => To(self, p, true);
        public static Mover.Plan<C, X___, Rect> By<C>(this C self, float x, X___ _ = default) where C : struct, IXYWH => To(self, x:x, true);
        public static Mover.Plan<C, _Y__, Rect> By<C>(this C self, float y, _Y__ _ = default) where C : struct, IXYWH => To(self, y:y, true);
        public static Mover.Plan<C, __W_, Rect> By<C>(this C self, float w, __W_ _ = default) where C : struct, IXYWH => To(self, w:w, true);
        public static Mover.Plan<C, ___H, Rect> By<C>(this C self, float h, ___H _ = default) where C : struct, IXYWH => To(self, h:h, true);
        public static Mover.Plan<C, XY__, Rect> By<C>(this C self, float x, float y, XY__ _ = default) where C : struct, IXYWH => To(self, x:x, y:y, true);
        public static Mover.Plan<C, __WH, Rect> By<C>(this C self, float w, float h, __WH _ = default) where C : struct, IXYWH => To(self, w:w, h:h, true);
        public static Mover.Plan<C, X_W_, Rect> By<C>(this C self, float x, float w, X_W_ _ = default) where C : struct, IXYWH => To(self, x:x, w:w, true);
        public static Mover.Plan<C, _Y_H, Rect> By<C>(this C self, float y, float h, _Y_H _ = default) where C : struct, IXYWH => To(self, y:y, h:h, true);
        public static Mover.Plan<C, X__H, Rect> By<C>(this C self, float x, float h, X__H _ = default) where C : struct, IXYWH => To(self, x:x, h:h, true);
        public static Mover.Plan<C, _YW_, Rect> By<C>(this C self, float y, float w, _YW_ _ = default) where C : struct, IXYWH => To(self, y:y, w:w, true);
        public static Mover.Plan<C, _YWH, Rect> By<C>(this C self, float y, float w, float h, _YWH _ = default) where C : struct, IXYWH => To(self, y:y, w:w, h:h, true);
        public static Mover.Plan<C, X_WH, Rect> By<C>(this C self, float x, float w, float h, X_WH _ = default) where C : struct, IXYWH => To(self, x:x, w:w, h:h, true);
        public static Mover.Plan<C, XY_H, Rect> By<C>(this C self, float x, float y, float h, XY_H _ = default) where C : struct, IXYWH => To(self, x:x, y:y, h:h, true);
        public static Mover.Plan<C, XYW_, Rect> By<C>(this C self, float x, float y, float w, XYW_ _ = default) where C : struct, IXYWH => To(self, x:x, y:y, w:w, true);
        public static Mover.Plan<C, XYWH, Rect> By<C>(this C self, float x, float y, float w, float h, XYWH _ = default) where C : struct, IXYWH => To(self, x, y, w, h, true);
#else
        /// <summary>Creates a zero-allocation tween plan to interpolate the value towards an absolute target.</summary>
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, Rect p = default, bool isDelta = false, XYWH _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.XYWH), p);
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, bool isDelta = false, X___ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.X___), new(x, 0, 0, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float y, bool isDelta = false, _Y__ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb._Y__), new(0, y, 0, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float w, bool isDelta = false, __W_ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.__W_), new(0, 0, w, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float h, bool isDelta = false, ___H _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.___H), new(0, 0, 0, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float y, bool isDelta = false, XY__ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.XY__), new(x, y, 0, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float w, float h, bool isDelta = false, __WH _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.__WH), new(0, 0, w, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float w, bool isDelta = false, X_W_ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.X_W_), new(x, 0, w, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float y, float h, bool isDelta = false, _Y_H _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb._Y_H), new(0, y, 0, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float h, bool isDelta = false, X__H _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.X__H), new(x, 0, 0, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float y, float w, bool isDelta = false, _YW_ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb._YW_), new(0, y, w, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float y, float w, float h, bool isDelta = false, _YWH _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb._YWH), new(0, y, w, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float w, float h, bool isDelta = false, X_WH _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.X_WH), new(x, 0, w, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float y, float h, bool isDelta = false, XY_H _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.XY_H), new(x, y, 0, h));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float y, float w, bool isDelta = false, XYW_ _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.XYW_), new(x, y, w, 0));
        public static Mover.Plan<C, Mapper, Rect> To<C>(this C self, float x, float y, float w, float h, bool isDelta = false, XYWH _ = default) where C : struct, IXYWH => new(self, isDelta, new(Comb.XYWH), new(x, y, w, h));

        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, Rect p = default, XYWH _ = default) where C : struct, IXYWH => To(self, p, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, X___ _ = default) where C : struct, IXYWH => To(self, x:x, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float y, _Y__ _ = default) where C : struct, IXYWH => To(self, y:y, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float w, __W_ _ = default) where C : struct, IXYWH => To(self, w:w, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float h, ___H _ = default) where C : struct, IXYWH => To(self, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float y, XY__ _ = default) where C : struct, IXYWH => To(self, x:x, y:y, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float w, float h, __WH _ = default) where C : struct, IXYWH => To(self, w:w, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float w, X_W_ _ = default) where C : struct, IXYWH => To(self, x:x, w:w, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float y, float h, _Y_H _ = default) where C : struct, IXYWH => To(self, y:y, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float h, X__H _ = default) where C : struct, IXYWH => To(self, x:x, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float y, float w, _YW_ _ = default) where C : struct, IXYWH => To(self, y:y, w:w, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float y, float w, float h, _YWH _ = default) where C : struct, IXYWH => To(self, y:y, w:w, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float w, float h, X_WH _ = default) where C : struct, IXYWH => To(self, x:x, w:w, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float y, float h, XY_H _ = default) where C : struct, IXYWH => To(self, x:x, y:y, h:h, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float y, float w, XYW_ _ = default) where C : struct, IXYWH => To(self, x:x, y:y, w:w, true);
        public static Mover.Plan<C, Mapper, Rect> By<C>(this C self, float x, float y, float w, float h, XYWH _ = default) where C : struct, IXYWH => To(self, x, y, w, h, true);
#endif


        /// <summary>Creates a zero-allocation tween plan to interpolate the value by a relative delta amount.</summary>
        public static void Set<C>(this C self, float x, X___ _ = default) where C : struct, IXYWH => To(self, x:x).SetEnd();
        public static void Set<C>(this C self, float y, _Y__ _ = default) where C : struct, IXYWH => To(self, y:y).SetEnd();
        public static void Set<C>(this C self, float w, __W_ _ = default) where C : struct, IXYWH => To(self, w:w).SetEnd();
        public static void Set<C>(this C self, float h, ___H _ = default) where C : struct, IXYWH => To(self, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float y, XY__ _ = default) where C : struct, IXYWH => To(self, x:x, y:y).SetEnd();
        public static void Set<C>(this C self, float w, float h, __WH _ = default) where C : struct, IXYWH => To(self, w:w, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float w, X_W_ _ = default) where C : struct, IXYWH => To(self, x:x, w:w).SetEnd();
        public static void Set<C>(this C self, float y, float h, _Y_H _ = default) where C : struct, IXYWH => To(self, y:y, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float h, X__H _ = default) where C : struct, IXYWH => To(self, x:x, h:h).SetEnd();
        public static void Set<C>(this C self, float y, float w, _YW_ _ = default) where C : struct, IXYWH => To(self, y:y, w:w).SetEnd();
        public static void Set<C>(this C self, float y, float w, float h, _YWH _ = default) where C : struct, IXYWH => To(self, y:y, w:w, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float w, float h, X_WH _ = default) where C : struct, IXYWH => To(self, x:x, w:w, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float y, float h, XY_H _ = default) where C : struct, IXYWH => To(self, x:x, y:y, h:h).SetEnd();
        public static void Set<C>(this C self, float x, float y, float w, XYW_ _ = default) where C : struct, IXYWH => To(self, x:x, y:y, w:w).SetEnd();

        internal enum Comb
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
        public readonly struct Mapper : Mover.IMapper<Rect>
        {
            readonly Comb comb;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Set(Rect current, Rect to)
                => this.comb switch
                {
                    Comb.X___ => default(X___).Set(current, to),
                    Comb._Y__ => default(_Y__).Set(current, to),
                    Comb.__W_ => default(__W_).Set(current, to),
                    Comb.___H => default(___H).Set(current, to),
                    Comb.XY__ => default(XY__).Set(current, to),
                    Comb.__WH => default(__WH).Set(current, to),
                    Comb.X_W_ => default(X_W_).Set(current, to),
                    Comb._Y_H => default(_Y_H).Set(current, to),
                    Comb.X__H => default(X__H).Set(current, to),
                    Comb._YW_ => default(_YW_).Set(current, to),
                    Comb._YWH => default(_YWH).Set(current, to),
                    Comb.X_WH => default(X_WH).Set(current, to),
                    Comb.XY_H => default(XY_H).Set(current, to),
                    Comb.XYW_ => default(XYW_).Set(current, to),
                    Comb.XYWH => default(XYWH).Set(current, to),
                    _ => current
                };
            
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public (Rect, Rect) GetParam(Rect from, Rect to, bool isDelta) => default(XYWH).GetParam(from, to, isDelta);
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct GenericMapper<X, Y, W, H> : Mover.IMapper<Rect>
            where X : struct, Mover.IAxisFlag
            where Y : struct, Mover.IAxisFlag
            where W : struct, Mover.IAxisFlag
            where H : struct, Mover.IAxisFlag
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Rect to, Rect from)
            {
                var xx = default(X).GetSqDiff(to.x, from.x);
                var yy = default(Y).GetSqDiff(to.y, from.y);
                var ww = default(W).GetSqDiff(to.width, from.width);
                var hh = default(H).GetSqDiff(to.height, from.height);
                return Mathf.Sqrt(xx + yy + ww + hh);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
            {
                current.x = default(X).Lerp(current.x, to.x, diff.x, rt);
                current.y = default(Y).Lerp(current.y, to.y, diff.y, rt);
                current.width = default(W).Lerp(current.width, to.width, diff.width, rt);
                current.height = default(H).Lerp(current.height, to.height, diff.height, rt);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Rect Set(Rect current, Rect to)
            {
                current.x = default(X).Set(current.x, to.x);
                current.y = default(Y).Set(current.y, to.y);
                current.width = default(W).Set(current.width, to.width);
                current.height = default(H).Set(current.height, to.height);
                return current;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
