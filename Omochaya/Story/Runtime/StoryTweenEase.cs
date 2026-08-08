// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenEase.cs" company="Omochaya">
//   Copyright (c) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// Defines customizable easing functions and modifier structures for value interpolation.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using System.Runtime.CompilerServices;
    using UnityEngine;
    using HiddenStory;

    public static partial class Story
    {
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public interface IEase
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            float Calc(float now);
        }

        // ------------------------------------------------------------------------------------------------------------
        /// <summary>Provides factory methods and static instances for easing functions.</summary>
#if STORY_EASE_COMPACT
        public class Ease : EaseCompact
#else
        public class Ease : EaseFast
#endif
        {
            // shared

            // 〜〜 引数アリの ease（これらは共通化できない） 〜〜

            /// <summary>Creates an easing implementation based on an AnimationCurve.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static CurveImpl Curve(AnimationCurve curve) => new(curve);

            /// <summary>Creates a custom power acceleration easing implementation.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PowImpl PowAcc(float pow) => new(pow);

            /// <summary>Creates a custom power deceleration easing implementation.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static InverseImpl<PowImpl> PowDec(float pow) => new(new(pow));

            /// <summary>Creates an easing implementation that remaps the evaluation result between specified minimum and maximum values.</summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static FromToImpl FromTo(float from, float to) => new(from, to);

            // implementations

            // 加工用

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal DirectImpl(E prev, C calc)
                {
                    this.prev = prev;
                    this.calc = calc;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ReverseImpl<C> : IEase
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
                    return now;
                }

                // fields
                readonly C calc;

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal ReverseImpl(C calc)
                {
                    this.calc = calc;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal InverseImpl(C calc)
                {
                    this.calc = calc;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal InverseImpl(E prev, C calc)
                {
                    this.prev = prev;
                    this.calc = calc;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal FromToImpl(float from, float to)
                {
                    this.from = from;
                    this.to = to;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal FixImpl(E prev)
                {
                    this.prev = prev;
                    this.start = prev.Calc(0f);
                    this.length = prev.Calc(1f) - this.start;
                    if (Mathf.Approximately(this.length, 0f))
                    {
                        Dev.LogError(Messages.Exceptions.CannotNormalizeEase);
                        this.length = this.length < 0f ? -float.Epsilon : float.Epsilon;
                    }
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct StitchImpl<E, C> : IEase
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
                    if (now < this.seam)
                    {
                        // 前半
                        var t = now / this.seam;
                        return this.prev.Calc(t) * this.weight;
                    }
                    else
                    {
                        // 後半
                        var t = (now - this.seam) / (1f - this.seam);
                        return this.weight + this.calc.Calc(t) * (1f - this.weight);
                    }
                }

                // fields
                readonly E prev;
                readonly C calc;
                readonly float seam;
                readonly float weight;

                // properties

                /// <summary>Calculates the optimal seam point for stitching multiple easing functions evenly.</summary>
                internal float EvenSeam
                {
                    get
                    {
                        var b = 1f - this.seam;
                        if (Mathf.Approximately(b, 0f)) { return 1f; }
                        var count = Mathf.RoundToInt(1f / b);
                        return 1f - 1f / (count + 1);
                    }
                }

                // constructors
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                internal StitchImpl(E prev, C calc, float seam)
                {
                    this.prev = prev;
                    this.calc = calc;
                    this.weight = GetWeight(ref seam, prev, calc);
                    this.seam = seam;
                }
                static float GetWeight(ref float seam, in E prev, in C calc)
                {
                    if (seam <= 0f) { seam = 0f; return 0f; }
                    if (1f <= seam) { seam = 1f; return 1f; }

                    const float tip = 1f / 128;
                    var p = prev.Calc(1f) - prev.Calc(1f - tip);
                    var q = calc.Calc(tip) - calc.Calc(0f);
                    p /= tip;
                    q /= tip;

                    // 傾きがゼロに近いときは計算誤差が暴発するので seam をそのまま適用する
                    if (-tip < p*q && p*q < tip) { return seam; }

                    var a = q * seam;
                    var b = p - p * seam + q * seam;
                    if (!Mathf.Approximately(b, 0f)) { return a / b; }
                    Dev.LogError(Messages.Exceptions.CannotStitchEase);
                    return seam;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal CurveImpl(AnimationCurve curve)
                {
                    this.curve = curve;
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
                internal PowImpl(float pow)
                {
                    this.pow = pow;
                }
            }
         }

        // ------------------------------------------------------------------------------------------------------------
        /// <summary>Provides high-performance, statically allocated easing instances.</summary>
        public class EaseFast
        {
            // shared

            /// <summary>Linear easing with no acceleration or deceleration.</summary>
            public static readonly NoneImpl None = default;

            /// <summary>Inverted linear easing running from 1 to 0.</summary>
            public static readonly ReverseImpl Reverse = default;

            /// <summary>Sinusoidal acceleration easing.</summary>
            public static readonly Ease.InverseImpl<SineImpl> SineAcc = default;

            /// <summary>Sinusoidal deceleration easing.</summary>
            public static readonly SineImpl SineDec = default;

            /// <summary>Quadratic acceleration easing.</summary>
            public static readonly QuadImpl QuadAcc = default;

            /// <summary>Quadratic deceleration easing.</summary>
            public static readonly Ease.InverseImpl<QuadImpl> QuadDec = default;

            /// <summary>Cubic acceleration easing.</summary>
            public static readonly CubicImpl CubicAcc = default;

            /// <summary>Cubic deceleration easing.</summary>
            public static readonly Ease.InverseImpl<CubicImpl> CubicDec = default;

            /// <summary>Quartic acceleration easing.</summary>
            public static readonly QuartImpl QuartAcc = default;

            /// <summary>Quartic deceleration easing.</summary>
            public static readonly Ease.InverseImpl<QuartImpl> QuartDec = default;

            /// <summary>Square root acceleration easing.</summary>
            public static readonly Ease.InverseImpl<SqrtImpl> SqrtAcc = default;

            /// <summary>Square root deceleration easing.</summary>
            public static readonly SqrtImpl SqrtDec = default;

            /// <summary>Exponential acceleration easing.</summary>
            public static readonly ExpoImpl ExpoAcc = default;

            /// <summary>Exponential deceleration easing.</summary>
            public static readonly Ease.InverseImpl<ExpoImpl> ExpoDec = default;

            /// <summary>Circular acceleration easing.</summary>
            public static readonly Ease.InverseImpl<CircImpl> CircAcc = default;

            /// <summary>Circular deceleration easing.</summary>
            public static readonly CircImpl CircDec = default;

            /// <summary>Backing acceleration easing that slightly overshoots the initial point.</summary>
            public static readonly BackImpl BackAcc = default;

            /// <summary>Backing deceleration easing that slightly overshoots the target point.</summary>
            public static readonly Ease.InverseImpl<BackImpl> BackDec = default;

            /// <summary>Elastic acceleration easing simulating an oscillating spring.</summary>
            public static readonly ElasticImpl ElasticAcc = default;

            /// <summary>Elastic deceleration easing simulating an oscillating spring.</summary>
            public static readonly Ease.InverseImpl<ElasticImpl> ElasticDec = default;

            /// <summary>Bouncing acceleration easing simulating an inverted bounce effect.</summary>
            public static readonly Ease.InverseImpl<BounceImpl> BounceAcc = default;

            /// <summary>Bouncing deceleration easing simulating a decay bounce effect.</summary>
            public static readonly BounceImpl BounceDec = default;

            // implementations

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct NoneImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ReverseImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => 1f - now;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct SineImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => Mathf.Sin(now * Mathf.PI * 0.5f);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct QuadImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now * now;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct CubicImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now * now * now;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct QuartImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now * now * now * now;
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct SqrtImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => Mathf.Sqrt(now);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct ExpoImpl : IEase
            {
                /// <summary>Don't touch! Only for system.</summary>
                [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
                [MethodImpl(MethodImplOptions.AggressiveInlining)]
                public readonly float Calc(float now) => now == 0f ? 0f : Mathf.Pow(2f, 10f * now - 10f);
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
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
       }

        // ------------------------------------------------------------------------------------------------------------
        /// <summary>Provides memory-compact enum-based easing instances.</summary>
        public class EaseCompact
        {
            // shared

            /// <summary>Linear easing with no acceleration or deceleration.</summary>
            public static readonly Impl None = new(Type.None);

            /// <summary>Inverted linear easing running from 1 to 0.</summary>
            public static readonly Impl Reverse = new(Type.Reverse);

            /// <summary>Sinusoidal acceleration easing.</summary>
            public static readonly Impl SineAcc = new(Type.SineAcc);

            /// <summary>Sinusoidal deceleration easing.</summary>
            public static readonly Impl SineDec = new(Type.SineDec);

            /// <summary>Quadratic acceleration easing.</summary>
            public static readonly Impl QuadAcc = new(Type.QuadAcc);

            /// <summary>Quadratic deceleration easing.</summary>
            public static readonly Impl QuadDec = new(Type.QuadDec);

            /// <summary>Cubic acceleration easing.</summary>
            public static readonly Impl CubicAcc = new(Type.CubicAcc);

            /// <summary>Cubic deceleration easing.</summary>
            public static readonly Impl CubicDec = new(Type.CubicDec);

            /// <summary>Quartic acceleration easing.</summary>
            public static readonly Impl QuartAcc = new(Type.QuartAcc);

            /// <summary>Quartic deceleration easing.</summary>
            public static readonly Impl QuartDec = new(Type.QuartDec);

            /// <summary>Square root acceleration easing.</summary>
            public static readonly Impl SqrtAcc = new(Type.SqrtAcc);

            /// <summary>Square root deceleration easing.</summary>
            public static readonly Impl SqrtDec = new(Type.SqrtDec);

            /// <summary>Exponential acceleration easing.</summary>
            public static readonly Impl ExpoAcc = new(Type.ExpoAcc);

            /// <summary>Exponential deceleration easing.</summary>
            public static readonly Impl ExpoDec = new(Type.ExpoDec);

            /// <summary>Circular acceleration easing.</summary>
            public static readonly Impl CircAcc = new(Type.CircAcc);

            /// <summary>Circular deceleration easing.</summary>
            public static readonly Impl CircDec = new(Type.CircDec);

            /// <summary>Backing acceleration easing that slightly overshoots the initial point.</summary>
            public static readonly Impl BackAcc = new(Type.BackAcc);

            /// <summary>Backing deceleration easing that slightly overshoots the target point.</summary>
            public static readonly Impl BackDec = new(Type.BackDec);

            /// <summary>Elastic acceleration easing simulating an oscillating spring.</summary>
            public static readonly Impl ElasticAcc = new(Type.ElasticAcc);

            /// <summary>Elastic deceleration easing simulating an oscillating spring.</summary>
            public static readonly Impl ElasticDec = new(Type.ElasticDec);

            /// <summary>Bouncing acceleration easing simulating an inverted bounce effect.</summary>
            public static readonly Impl BounceAcc = new(Type.BounceAcc);

            /// <summary>Bouncing deceleration easing simulating a decay bounce effect.</summary>
            public static readonly Impl BounceDec = new(Type.BounceDec);

            // implementations

            public enum Type
            {
                None,
                Reverse,
                SineAcc,
                SineDec,
                QuadAcc,
                QuadDec,
                CubicAcc,
                CubicDec,
                QuartAcc,
                QuartDec,
                SqrtAcc,
                SqrtDec,
                ExpoAcc,
                ExpoDec,
                CircAcc,
                CircDec,
                BackAcc,
                BackDec,
                ElasticAcc,
                ElasticDec,
                BounceAcc,
                BounceDec,
            }
            public readonly struct Impl : IEase
            {
                readonly Type type;
                public Impl(Type type) => this.type = type;

                public float Calc(float now)
                {
                    switch (this.type)
                    {
                        case Type.Reverse: return EaseFast.Reverse.Calc(now);
                        case Type.SineAcc: return EaseFast.SineAcc.Calc(now);
                        case Type.SineDec: return EaseFast.SineDec.Calc(now);
                        case Type.QuadAcc: return EaseFast.QuadAcc.Calc(now);
                        case Type.QuadDec: return EaseFast.QuadDec.Calc(now);
                        case Type.CubicAcc: return EaseFast.CubicAcc.Calc(now);
                        case Type.CubicDec: return EaseFast.CubicDec.Calc(now);
                        case Type.QuartAcc: return EaseFast.QuartAcc.Calc(now);
                        case Type.QuartDec: return EaseFast.QuartDec.Calc(now);
                        case Type.SqrtAcc: return EaseFast.SqrtAcc.Calc(now);
                        case Type.SqrtDec: return EaseFast.SqrtDec.Calc(now);
                        case Type.ExpoAcc: return EaseFast.ExpoAcc.Calc(now);
                        case Type.ExpoDec: return EaseFast.ExpoDec.Calc(now);
                        case Type.CircAcc: return EaseFast.CircAcc.Calc(now);
                        case Type.CircDec: return EaseFast.CircDec.Calc(now);
                        case Type.BackAcc: return EaseFast.BackAcc.Calc(now);
                        case Type.BackDec: return EaseFast.BackDec.Calc(now);
                        case Type.ElasticAcc: return EaseFast.ElasticAcc.Calc(now);
                        case Type.ElasticDec: return EaseFast.ElasticDec.Calc(now);
                        case Type.BounceAcc: return EaseFast.BounceAcc.Calc(now);
                        case Type.BounceDec: return EaseFast.BounceDec.Calc(now);
                        default: return now;
                    }
                }
            }
        }

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // ease を加工する拡張メソッド。
        // 加工を3つ以上重ねるとパフォーマンスが低下する場合があるため注意
        // （ジェネリックの階層が7を超えるとフォールバック処理に置き換わるため）

        /// <summary>Chains an easing function to remap its output range between specified minimum and maximum values.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.FromToImpl> FromTo<E>(this E prev, float from, float to) where E : struct, IEase => new(prev, new(from, to));

        /// <summary>Chains an easing function to normalize its boundary evaluation results.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.FixImpl<E> Fix<E>(this E prev) where E : struct, IEase => new(prev);

        /// <summary>Stitches two easing functions seamlessly at the default midpoint seam ratio of 0.5.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.StitchImpl<E, C> Stitch<E, C>(this E prev, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary>Stitches two easing functions seamlessly at a custom seam ratio.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.StitchImpl<E, C> Stitch<E, C>(this E prev, float seam, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, seam);

        /// <summary>Stitches a third easing function into an existing stitched easing composition using even seam distribution.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.StitchImpl<Ease.StitchImpl<A, B>, C> Stitch<A, B, C>(this Ease.StitchImpl<A, B> prev, C calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            => new(prev, calc, prev.EvenSeam);

        /// <summary>Stitches two existing stitched easing pairs into a four-stage easing sequence.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.StitchImpl<Ease.StitchImpl<A, B>, Ease.StitchImpl<C, D>> Stitch<A, B, C, D>(this Ease.StitchImpl<A, B> prev, Ease.StitchImpl<C, D> calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            where D : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary>Applies a reversal modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.ReverseImpl<E> Reverse<E>(this E prev) where E : struct, IEase => new(prev);

        /// <summary>Applies sinusoidal acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.SineImpl> SineAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies sinusoidal deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.SineImpl> SineDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies quadratic acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.QuadImpl> QuadAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies quadratic deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.QuadImpl> QuadDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies cubic acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.CubicImpl> CubicAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies cubic deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.CubicImpl> CubicDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies quartic acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.QuartImpl> QuartAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies quartic deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.QuartImpl> QuartDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies square root acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.SqrtImpl> SqrtAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies square root deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.SqrtImpl> SqrtDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies a custom power acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.PowImpl> PowAcc<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary>Applies a custom power deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.PowImpl> PowDec<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary>Applies exponential acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.ExpoImpl> ExpoAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies exponential deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.ExpoImpl> ExpoDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies circular acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.CircImpl> CircAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies circular deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.CircImpl> CircDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies backing acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.BackImpl> BackAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies backing deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.BackImpl> BackDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies elastic acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.ElasticImpl> ElasticAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies elastic deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.ElasticImpl> ElasticDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies bouncing acceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.BounceImpl> BounceAcc<E>(this E prev) where E : struct, IEase => new(prev, default);

        /// <summary>Applies bouncing deceleration as a modifier to an existing easing function.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.BounceImpl> BounceDec<E>(this E prev) where E : struct, IEase => new(prev, default);

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // utils

        /// <summary>Converts an AnimationCurve into an easing function instance.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CurveImpl ToEase(this AnimationCurve self) => new Ease.CurveImpl(self);
    }
}

