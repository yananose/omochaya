// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenEase.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System.Runtime.CompilerServices;
    using UnityEngine;

    public static partial class Story
    {
        /// <summary></summary>
        public interface IEase
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            float Calc(float now);
        }

        /// <summary></summary>
        public static class Ease
        {
            // implementations

            /// <summary></summary>
            public readonly struct DirectImpl<E, C> : IEase
                where E : struct, IEase
                where C : struct, IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = this.prev.Calc(now);
                    now = this.calc.Calc(now);
                    return now;
                }

                // fields
                readonly E prev;
                readonly C calc;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public DirectImpl(E prev, C calc)
                {
                    this.prev = prev;
                    this.calc = calc;
                }
            }

            /// <summary></summary>
            public readonly struct InverseImpl<C> : IEase
                where C : struct, IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = 1f - now;
                    now = this.calc.Calc(now);
                    now = 1f - now;
                    return now;
                }

                // fields
                readonly C calc;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public InverseImpl(C calc)
                {
                    this.calc = calc;
                }
            }

            /// <summary></summary>
            public readonly struct InverseImpl<E, C> : IEase
                where E : struct, IEase
                where C : struct, IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = this.prev.Calc(now);
                    now = 1f - now;
                    now = this.calc.Calc(now);
                    now = 1f - now;
                    return now;
                }

                // fields
                readonly E prev;
                readonly C calc;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public InverseImpl(E prev, C calc)
                {
                    this.prev = prev;
                    this.calc = calc;
                }
            }

            /// <summary></summary>
            public readonly struct FromToImpl : IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => Mathf.LerpUnclamped(this.from, this.to, now);

                // fields
                readonly float from;
                readonly float to;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public FromToImpl(float from, float to)
                {
                    this.from = from;
                    this.to = to;
                }
            }

            /// <summary></summary>
            public readonly struct FixImpl<E> : IEase // 特殊
                where E : struct, IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = this.prev.Calc(now);
                    now = (now - this.start) / this.length;
                    return now;
                }

                // fields
                readonly E prev;
                readonly float start;
                readonly float length;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public FixImpl(E prev)
                {
                    this.prev = prev;
                    this.start = prev.Calc(0f);
                    this.length = prev.Calc(1f) - this.start;
                    if (Mathf.Approximately(this.length, 0f))
                    {
                        Debug.Assert(false);
                        this.length = this.length < 0f ? -float.Epsilon : float.Epsilon;
                    }
                }
            }

            /// <summary></summary>
            public readonly struct CombineImpl<E, C> : IEase
                where E : struct, IEase
                where C : struct, IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    if (now <= 0f) { return 0f; }
                    if (1f <= now) { return 1f; }
                    if (now < this.split)
                    {
                        // 前半
                        var t = now / this.split;
                        return this.prev.Calc(t) * this.weight;
                    }
                    else
                    {
                        // 後半
                        var t = (now - this.split) / (1f - this.split);
                        return this.weight + this.calc.Calc(t) * (1f - this.weight);
                    }
                }

                // fields
                readonly E prev;
                readonly C calc;
                readonly float split;
                readonly float weight;

                // properties

                /// <summary></summary>
                public float EvenSplit
                {
                    get
                    {
                        var b = 1f - this.split;
                        if (Mathf.Approximately(b, 0f)) { return 1f; }
                        var count = Mathf.RoundToInt(1f / b);
                        return 1f - 1f / (count + 1);
                    }
                }

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public CombineImpl(E prev, C calc, float split)
                {
                    this.prev = prev;
                    this.calc = calc;
                    this.weight = GetWeight(ref split, prev, calc);
                    this.split = split;
                }
                static float GetWeight(ref float split, in E prev, in C calc)
                {
                    if (split <= 0f) { split = 0f; return 0f; }
                    if (1f <= split) { split = 1f; return 1f; }

                    const float tip = 1f / 128;
                    var p = prev.Calc(1f) - prev.Calc(1f - tip);
                    var q = calc.Calc(tip) - calc.Calc(0f);
                    p /= tip;
                    q /= tip;

                    // 傾きがゼロに近いときは計算誤差が暴発するので split をそのまま適用する
                    if (-tip < p*q && p*q < tip) { return split; }

                    var a = q * split;
                    var b = p - p * split + q * split;

                    if (!Mathf.Approximately(b, 0f)) { return a / b; }
                    Debug.LogError("結合できない");
                    return 1f;
                }
            }

            /// <summary></summary>
            public readonly struct CurveImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public float Calc(float now)
                {
                    return this.curve != null ? this.curve.Evaluate(now) : now;
                }

                readonly AnimationCurve curve;

                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public CurveImpl(AnimationCurve curve)
                {
                    this.curve = curve;
                }
            }

            /// <summary></summary>
            public readonly struct NoneImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now;
            }

            /// <summary></summary>
            public readonly struct ReverseImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => 1f - now;
            }

            /// <summary></summary>
            public readonly struct SineImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => Mathf.Sin(now * Mathf.PI * 0.5f);
            }

            /// <summary></summary>
            public readonly struct QuadImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now * now;
            }

            /// <summary></summary>
            public readonly struct CubicImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now * now * now;
            }

            /// <summary></summary>
            public readonly struct PowImpl : IEase
            {
                // for iease

                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = Mathf.Pow(now, this.pow);
                    return now;
                }

                // fields
                readonly float pow;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public PowImpl(float pow)
                {
                    this.pow = pow;
                }
            }

            /// <summary></summary>
            public readonly struct ExpoImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now == 0f ? 0f : Mathf.Pow(2f, 10f * now - 10f);
            }

            /// <summary></summary>
            public readonly struct CircImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    now = (now % 4f + 4f) % 4f; // 0 <= now < 4f
                    now -= 1f;
                    if (now < 1f) { return Mathf.Sqrt(1f - now * now); } // -1f <= now < 1f
                    now -= 2f;
                    return - Mathf.Sqrt(1f - now * now);
                }
            }

            /// <summary></summary>
            public readonly struct BackImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    const float c1 = 1.70158f;
                    const float c3 = c1 + 1f;

                    return c3 * now * now * now - c1 * now * now;
                }
            }

            /// <summary></summary>
            public readonly struct ElasticImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    if (now <= 0f) { return 0f; }
                    if (1f <= now) { return 1f; }

                    const float c4 = 2f * Mathf.PI / 3f;
                    return -Mathf.Pow(2f, 10f * now - 10f) * Mathf.Sin((now * 10f - 10.75f) * c4);
                }
            }

            /// <summary></summary>
            public readonly struct BounceImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now)
                {
                    if (now <= 0f) { return 0f; }
                    if (1f <= now) { return 1f; }

                    const float n1 = 7.5625f;
                    const float d1 = 2.75f;

                    if (now < 1f / d1) {
                        return n1 * now * now;
                    } else if (now < 2f / d1) {
                        return n1 * (now -= 1.5f / d1) * now + 0.75f;
                    } else if (now < 2.5f / d1) {
                        return n1 * (now -= 2.25f / d1) * now + 0.9375f;
                    } else {
                        return n1 * (now -= 2.625f / d1) * now + 0.984375f;
                    }
                }
            }

            // shared

            /// <summary></summary>
            public static readonly NoneImpl None = new();

            /// <summary></summary>
            public static readonly ReverseImpl Reverse = new();

            /// <summary></summary>
            public static FromToImpl FromTo(float from, float to) => new(from, to);

            /// <summary></summary>
            public static readonly InverseImpl<SineImpl> SineAcc = new();

            /// <summary></summary>
            public static readonly SineImpl SineDec = new();

            /// <summary></summary>
            public static CurveImpl Curve(AnimationCurve curve) => new(curve);

            /// <summary></summary>
            public static readonly QuadImpl QuadAcc = new();

            /// <summary></summary>
            public static readonly InverseImpl<QuadImpl> QuadDec = new();

            /// <summary></summary>
            public static PowImpl PowAcc(float pow) => new(pow);

            /// <summary></summary>
            public static InverseImpl<PowImpl> PowDec(float pow) => new(new(pow));

            /// <summary></summary>
            public static readonly CubicImpl CubicAcc = new();

            /// <summary></summary>
            public static readonly InverseImpl<CubicImpl> CubicDec = new();

            /// <summary></summary>
            public static readonly ExpoImpl ExpoAcc = new();

            /// <summary></summary>
            public static readonly InverseImpl<ExpoImpl> ExpoDec = new();

            /// <summary></summary>
            public static readonly InverseImpl<CircImpl> CircAcc = new();

            /// <summary></summary>
            public static readonly CircImpl CircDec = new();

            /// <summary></summary>
            public static readonly BackImpl BackAcc = new();

            /// <summary></summary>
            public static readonly InverseImpl<BackImpl> BackDec = new();

            /// <summary></summary>
            public static readonly ElasticImpl ElasticAcc = new();

            /// <summary></summary>
            public static readonly InverseImpl<ElasticImpl> ElasticDec = new();

            /// <summary></summary>
            public static readonly InverseImpl<BounceImpl> BounceAcc = new();

            /// <summary></summary>
            public static readonly BounceImpl BounceDec = new();
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // 【公開】Ease 拡張メソッド

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.ReverseImpl> Reverse<E>(this E prev)
            where E : struct, IEase
            => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.FromToImpl> FromTo<E>(this E prev, float from, float to)
            where E : struct, IEase
            => new(prev, new(from, to));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.FixImpl<E> Fix<E>(this E prev)
            where E : struct, IEase
            => new(prev);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CombineImpl<E, C> Combine<E, C>(this E prev, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CombineImpl<E, C> Combine<E, C>(this E prev, float split, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, split);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CombineImpl<Ease.CombineImpl<A, B>, C> Combine<A, B, C>(this Ease.CombineImpl<A, B> prev, C calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            => new(prev, calc, prev.EvenSplit);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CombineImpl<Ease.CombineImpl<A, B>, Ease.CombineImpl<C, D>> Combine<A, B, C, D>(this Ease.CombineImpl<A, B> prev, Ease.CombineImpl<C, D> calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            where D : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.SineImpl> SineAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.SineImpl> SineDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.QuadImpl> QuadAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.QuadImpl> QuadDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.CubicImpl> CubicAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.CubicImpl> CubicDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.PowImpl> PowAcc<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.PowImpl> PowDec<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.ExpoImpl> ExpoAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.ExpoImpl> ExpoDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.CircImpl> CircAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.CircImpl> CircDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.BackImpl> BackAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.BackImpl> BackDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.ElasticImpl> ElasticAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.ElasticImpl> ElasticDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.BounceImpl> BounceAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.BounceImpl> BounceDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CurveImpl ToEase(this AnimationCurve self) => new Ease.CurveImpl(self);
    }
}

