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
            readonly Mover.PlanArg<C, Mapper, float> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float p) => this.planArg = new(carrier, new Mapper(), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, float, E>(this.planArg, timeArg, ease, ref start);
        }

        readonly struct Mapper : Mover.IMapper<float>
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
            public (float, float) GetParam(float from, float to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
            }
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
            readonly Mover.PlanArg<C, Mapper, Vector2> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y)
            {
                var result = Analyze(x, y);
                this.planArg = new(carrier, new Mapper(result.Item1), result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Vector2 p)
                => this.planArg = new(carrier, new Mapper(Comb.XY), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Vector2, E>(this.planArg, timeArg, ease, ref start);
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

        enum Comb
        {
            None,
            X_, _Y,
            XY
        }

        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        readonly struct Mapper : Mover.IMapper<Vector2>
        {
            readonly Comb comb;
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float GetLength(Vector2 to, Vector2 from)
            {
                var diff = to - from;
                switch (this.comb)
                {
                    case Comb.X_: return Mathf.Abs(diff.x);
                    case Comb._Y: return Mathf.Abs(diff.y);
                    case Comb.XY: return diff.magnitude;
                    default: return default;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Lerp(Vector2 current, Vector2 to, Vector2 diff, float rt)
            {
                switch (this.comb)
                {
                    case Comb.X_:
                        current.x = to.x + diff.x * rt;
                        return current;
                    case Comb._Y:
                        current.y = to.y + diff.y * rt;
                        return current;
                    case Comb.XY:
                        return to + diff * rt;
                    default: return current;
                }
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
            readonly Mover.PlanArg<C, Mapper, Vector3> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? z)
            {
                var result = Analyze(x, y, z);
                this.planArg = new(carrier, new Mapper(result.Item1), result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Vector3 p)
                => this.planArg = new(carrier, new Mapper(Comb.XYZ), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Vector3, E>(this.planArg, timeArg, ease, ref start);


            // これだと芋づる式に全部のステートマシンのコードが作られる...ヤバすぎ
            // 毎回分岐を挟むとしてもまとめるしかないのか...（仮想メソッドよりはマシだけど）
            // To(x:10f) とかでなく ToX(10f) と書いてもらうなら対象のステートマシンのみコードが作られるようにできるけど、折角のオシャレ感が...
            // STORY_EASE_COMPACT があるならそれほど膨らまないのでこのままにする手も...
            // 
            // public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
            //     where S : struct, Story.IStepper
            //     where E : struct, Story.IEase
            // {
            //     switch (this.comb)
            //     {
            //         case Comb.X__: return Mover.Create<S, Vector3, Mover.Param1, Mapper.X__, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb._Y_: return Mover.Create<S, Vector3, Mover.Param1, Mapper._Y_, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb.__Z: return Mover.Create<S, Vector3, Mover.Param1, Mapper.__Z, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb._YZ: return Mover.Create<S, Vector3, Mover.Param2, Mapper._YZ, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb.X_Z: return Mover.Create<S, Vector3, Mover.Param2, Mapper.X_Z, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb.XY_: return Mover.Create<S, Vector3, Mover.Param2, Mapper.XY_, C, E>(this.planArg, timeArg, ease, ref start);
            //         case Comb.XYZ: return Mover.Create<S, Vector3, Mover.Param3, Mapper.XYZ, C, E>(this.planArg, timeArg, ease, ref start);
            //     }
            //     return default;
            // }

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
        readonly struct Mapper : Mover.IMapper<Vector3>
        {
            readonly Comb comb;
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float GetLength(Vector3 to, Vector3 from)
            {
                var diff = to - from;
                switch (this.comb)
                {
                    case Comb.X__: return Mathf.Abs(diff.x);
                    case Comb._Y_: return Mathf.Abs(diff.y);
                    case Comb.__Z: return Mathf.Abs(diff.z);
                    case Comb._YZ: return Mathf.Sqrt(diff.y * diff.y + diff.z * diff.z);
                    case Comb.X_Z: return Mathf.Sqrt(diff.x * diff.x + diff.z * diff.z);
                    case Comb.XY_: return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
                    case Comb.XYZ: return diff.magnitude;
                    default: return default;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Lerp(Vector3 current, Vector3 to, Vector3 diff, float rt)
            {
                switch (this.comb)
                {
                    case Comb.X__:
                        current.x = to.x + diff.x * rt;
                        return current;
                    case Comb._Y_:
                        current.y = to.y + diff.y * rt;
                        return current;
                    case Comb.__Z:
                        current.z = to.z + diff.z * rt;
                        return current;
                    case Comb._YZ:
                        current.y = to.y + diff.y * rt;
                        current.z = to.z + diff.z * rt;
                        return current;
                    case Comb.X_Z:
                        current.x = to.x + diff.x * rt;
                        current.z = to.z + diff.z * rt;
                        return current;
                    case Comb.XY_:
                        current.x = to.x + diff.x * rt;
                        current.y = to.y + diff.y * rt;
                        return current;
                    case Comb.XYZ:
                        return to + diff * rt;
                    default: return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public (Vector3, Vector3) GetParam(Vector3 from, Vector3 to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
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
            readonly Mover.PlanArg<C, Mapper, Color> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? r, float? g, float? b, float? a)
            {
                var result = Analyze(r, g, b, a);
                this.planArg = new(carrier, new Mapper(result.Item1), result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Color p)
                => this.planArg = new(carrier, new Mapper(Comb.RGBA), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Color, E>(this.planArg, timeArg, ease, ref start);
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
        readonly struct Mapper : Mover.IMapper<Color>
        {
            readonly Comb comb;
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float GetLength(Color to, Color from)
            {
                var diff = to - from;
                switch (this.comb)
                {
                    case Comb.R___: return Mathf.Abs(diff.r);
                    case Comb._G__: return Mathf.Abs(diff.g);
                    case Comb.__B_: return Mathf.Abs(diff.b);
                    case Comb.___A: return Mathf.Abs(diff.a);
                    case Comb.RG__: return Mathf.Sqrt(diff.r * diff.r + diff.g * diff.g);
                    case Comb.__BA: return Mathf.Sqrt(diff.b * diff.b + diff.a * diff.a);
                    case Comb.R_B_: return Mathf.Sqrt(diff.r * diff.r + diff.b * diff.b);
                    case Comb._G_A: return Mathf.Sqrt(diff.g * diff.g + diff.a * diff.a);
                    case Comb.R__A: return Mathf.Sqrt(diff.r * diff.r + diff.a * diff.a);
                    case Comb._GB_: return Mathf.Sqrt(diff.g * diff.g + diff.b * diff.b);
                    case Comb._GBA: return Mathf.Sqrt(diff.g * diff.g + diff.b * diff.b + diff.a * diff.a);
                    case Comb.R_BA: return Mathf.Sqrt(diff.r * diff.r + diff.b * diff.b + diff.a * diff.a);
                    case Comb.RG_A: return Mathf.Sqrt(diff.r * diff.r + diff.g * diff.g + diff.a * diff.a);
                    case Comb.RGB_: return Mathf.Sqrt(diff.r * diff.r + diff.g * diff.g + diff.b * diff.b);
                    case Comb.RGBA: return Mathf.Sqrt(diff.r * diff.r + diff.g * diff.g + diff.b * diff.b + diff.a * diff.a);
                    default: return default;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Lerp(Color current, Color to, Color diff, float rt)
            {
                switch (this.comb)
                {
                    case Comb.R___:
                        current.r = to.r + diff.r * rt;
                        return current;
                    case Comb._G__:
                        current.g = to.g + diff.g * rt;
                        return current;
                    case Comb.__B_:
                        current.b = to.b + diff.b * rt;
                        return current;
                    case Comb.___A:
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.RG__:
                        current.r = to.r + diff.r * rt;
                        current.g = to.g + diff.g * rt;
                        return current;
                    case Comb.__BA:
                        current.b = to.b + diff.b * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.R_B_:
                        current.r = to.r + diff.r * rt;
                        current.b = to.b + diff.b * rt;
                        return current;
                    case Comb._G_A:
                        current.g = to.g + diff.g * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.R__A:
                        current.r = to.r + diff.r * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb._GB_:
                        current.g = to.g + diff.g * rt;
                        current.b = to.b + diff.b * rt;
                        return current;
                    case Comb._GBA:
                        current.g = to.g + diff.g * rt;
                        current.b = to.b + diff.b * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.R_BA:
                        current.r = to.r + diff.r * rt;
                        current.b = to.b + diff.b * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.RG_A:
                        current.r = to.r + diff.r * rt;
                        current.g = to.g + diff.g * rt;
                        current.a = to.a + diff.a * rt;
                        return current;
                    case Comb.RGB_:
                        current.r = to.r + diff.r * rt;
                        current.g = to.g + diff.g * rt;
                        current.b = to.b + diff.b * rt;
                        return current;
                    case Comb.RGBA:
                        return to + diff * rt;
                    default: return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public (Color, Color) GetParam(Color from, Color to, bool isDelta)
            {
                if (isDelta) { to += from; }
                return (to, from - to);
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
            readonly Mover.PlanArg<C, Mapper, Quaternion> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Quaternion p) => this.planArg = new(carrier, new Mapper(), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Quaternion, E>(this.planArg, timeArg, ease, ref start);
        }

        // ToDo. 合ってるか要確認！！
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
            readonly Mover.PlanArg<C, Mapper, Rect> planArg;

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, float? x, float? y, float? width, float? height)
            {
                var result = Analyze(x, y, width, height);
                this.planArg = new(carrier, new Mapper(result.Item1), result.Item2, isDelta);
            }

            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Plan(in C carrier, bool isDelta, Rect p) => this.planArg = new(carrier, new Mapper(), p, isDelta);

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Story.Task CreateTask<S, E>(in S _, in Mover.TimeArg timeArg, E ease, ref double start)
                where S : struct, Story.IStepper
                where E : struct, Story.IEase
                => Mover.Create<S, C, Mapper, Rect, E>(this.planArg, timeArg, ease, ref start);
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
        readonly struct Mapper : Mover.IMapper<Rect>
        {
            readonly Comb comb;
            internal Mapper(Comb comb) => this.comb = comb;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float GetLength(Rect to, Rect from)
            {
                var diff = to;
                diff.position -= from.position;
                diff.size -= from.size;
                switch (this.comb)
                {
                    case Comb.X___: return Mathf.Abs(diff.x);
                    case Comb._Y__: return Mathf.Abs(diff.y);
                    case Comb.__W_: return Mathf.Abs(diff.width);
                    case Comb.___H: return Mathf.Abs(diff.height);
                    case Comb.XY__: return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y);
                    case Comb.__WH: return Mathf.Sqrt(diff.width * diff.width + diff.height * diff.height);
                    case Comb.X_W_: return Mathf.Sqrt(diff.x * diff.x + diff.width * diff.width); // position と size が混じった時の長さはこれでいいのか？
                    case Comb._Y_H: return Mathf.Sqrt(diff.y * diff.y + diff.height * diff.height);
                    case Comb.X__H: return Mathf.Sqrt(diff.x * diff.x + diff.height * diff.height);
                    case Comb._YW_: return Mathf.Sqrt(diff.y * diff.y + diff.width * diff.width);
                    case Comb._YWH: return Mathf.Sqrt(diff.y * diff.y + diff.width * diff.width + diff.height * diff.height);
                    case Comb.X_WH: return Mathf.Sqrt(diff.x * diff.x + diff.width * diff.width + diff.height * diff.height);
                    case Comb.XY_H: return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.height * diff.height);
                    case Comb.XYW_: return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.width * diff.width);
                    case Comb.XYWH: return Mathf.Sqrt(diff.x * diff.x + diff.y * diff.y + diff.width * diff.width + diff.height * diff.height);
                    default: return default;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Rect Lerp(Rect current, Rect to, Rect diff, float rt)
            {
                switch (this.comb)
                {
                    case Comb.X___:
                        current.x = to.x + diff.x * rt;
                        return current;
                    case Comb._Y__:
                        current.y = to.y + diff.y * rt;
                        return current;
                    case Comb.__W_:
                        current.width = to.width + diff.width * rt;
                        return current;
                    case Comb.___H:
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.XY__:
                        current.x = to.x + diff.x * rt;
                        current.y = to.y + diff.y * rt;
                        return current;
                    case Comb.__WH:
                        current.width = to.width + diff.width * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.X_W_:
                        current.x = to.x + diff.x * rt;
                        current.width = to.width + diff.width * rt;
                        return current;
                    case Comb._Y_H:
                        current.y = to.y + diff.y * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.X__H:
                        current.x = to.x + diff.x * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb._YW_:
                        current.y = to.y + diff.y * rt;
                        current.width = to.width + diff.width * rt;
                        return current;
                    case Comb._YWH:
                        current.y = to.y + diff.y * rt;
                        current.width = to.width + diff.width * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.X_WH:
                        current.x = to.x + diff.x * rt;
                        current.width = to.width + diff.width * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.XY_H:
                        current.x = to.x + diff.x * rt;
                        current.y = to.y + diff.y * rt;
                        current.height = to.height + diff.height * rt;
                        return current;
                    case Comb.XYW_:
                        current.x = to.x + diff.x * rt;
                        current.y = to.y + diff.y * rt;
                        current.width = to.width + diff.width * rt;
                        return current;
                    case Comb.XYWH:
                        current.position = to.position + diff.position * rt;
                        current.size = to.size + diff.size * rt;
                        return current;
                    default: return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
