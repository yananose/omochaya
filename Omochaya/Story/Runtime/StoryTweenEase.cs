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
    using HiddenStory;

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

        // ------------------------------------------------------------------------------------------------------------
        /// <summary></summary>
#if STORY_EASE_COMPACT
        public class Ease : EaseCompact
#else
        public class Ease : EaseFast
#endif
        {
            // shared

            // 〜〜 引数アリの ease（これらは共通化できない） 〜〜

            /// <summary></summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static CurveImpl Curve(AnimationCurve curve) => new(curve);

            /// <summary></summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static PowImpl PowAcc(float pow) => new(pow);

            /// <summary></summary>
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public static InverseImpl<PowImpl> PowDec(float pow) => new(new(pow));

            /// <summary></summary>
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
                        Dev.LogError("始点と終点が近すぎるため正規化できません");
                        this.length = this.length < 0f ? -float.Epsilon : float.Epsilon;
                    }
                }
            }

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public readonly struct JoinImpl<E, C> : IEase
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
                internal float EvenSplit
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
                internal JoinImpl(E prev, C calc, float split)
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
                    Dev.LogError("結合できない");
                    return 1f;
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
        /// <summary></summary>
        public class EaseFast
        {
            // shared

            /// <summary></summary>
            public static readonly NoneImpl None = new();

            /// <summary></summary>
            public static readonly ReverseImpl Reverse = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<SineImpl> SineAcc = new();

            /// <summary></summary>
            public static readonly SineImpl SineDec = new();

            /// <summary></summary>
            public static readonly QuadImpl QuadAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<QuadImpl> QuadDec = new();

            /// <summary></summary>
            public static readonly CubicImpl CubicAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<CubicImpl> CubicDec = new();

            /// <summary></summary>
            public static readonly QuartImpl QuartAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<QuartImpl> QuartDec = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<SqrtImpl> SqrtAcc = new();

            /// <summary></summary>
            public static readonly SqrtImpl SqrtDec = new();

            /// <summary></summary>
            public static readonly ExpoImpl ExpoAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<ExpoImpl> ExpoDec = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<CircImpl> CircAcc = new();

            /// <summary></summary>
            public static readonly CircImpl CircDec = new();

            /// <summary></summary>
            public static readonly BackImpl BackAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<BackImpl> BackDec = new();

            /// <summary></summary>
            public static readonly ElasticImpl ElasticAcc = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<ElasticImpl> ElasticDec = new();

            /// <summary></summary>
            public static readonly Ease.InverseImpl<BounceImpl> BounceAcc = new();

            /// <summary></summary>
            public static readonly BounceImpl BounceDec = new();

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
        /// <summary></summary>
        public class EaseCompact
        {
            // shared

            /// <summary></summary>
            public static readonly Impl None = new(Type.None);

            /// <summary></summary>
            public static readonly Impl Reverse = new(Type.Reverse);

            /// <summary></summary>
            public static readonly Impl SineAcc = new(Type.SineAcc);

            /// <summary></summary>
            public static readonly Impl SineDec = new(Type.SineDec);

            /// <summary></summary>
            public static readonly Impl QuadAcc = new(Type.QuadAcc);

            /// <summary></summary>
            public static readonly Impl QuadDec = new(Type.QuadDec);

            /// <summary></summary>
            public static readonly Impl CubicAcc = new(Type.CubicAcc);

            /// <summary></summary>
            public static readonly Impl CubicDec = new(Type.CubicDec);

            /// <summary></summary>
            public static readonly Impl QuartAcc = new(Type.QuartAcc);

            /// <summary></summary>
            public static readonly Impl QuartDec = new(Type.QuartDec);

            /// <summary></summary>
            public static readonly Impl SqrtAcc = new(Type.SqrtAcc);

            /// <summary></summary>
            public static readonly Impl SqrtDec = new(Type.SqrtDec);

            /// <summary></summary>
            public static readonly Impl ExpoAcc = new(Type.ExpoAcc);

            /// <summary></summary>
            public static readonly Impl ExpoDec = new(Type.ExpoDec);

            /// <summary></summary>
            public static readonly Impl CircAcc = new(Type.CircAcc);

            /// <summary></summary>
            public static readonly Impl CircDec = new(Type.CircDec);

            /// <summary></summary>
            public static readonly Impl BackAcc = new(Type.BackAcc);

            /// <summary></summary>
            public static readonly Impl BackDec = new(Type.BackDec);

            /// <summary></summary>
            public static readonly Impl ElasticAcc = new(Type.ElasticAcc);

            /// <summary></summary>
            public static readonly Impl ElasticDec = new(Type.ElasticDec);

            /// <summary></summary>
            public static readonly Impl BounceAcc = new(Type.BounceAcc);

            /// <summary></summary>
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

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.FromToImpl> FromTo<E>(this E prev, float from, float to) where E : struct, IEase => new(prev, new(from, to));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.FixImpl<E> Fix<E>(this E prev) where E : struct, IEase => new(prev);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.JoinImpl<E, C> Join<E, C>(this E prev, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.JoinImpl<E, C> Join<E, C>(this E prev, float split, C calc)
            where E : struct, IEase
            where C : struct, IEase
            => new(prev, calc, split);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.JoinImpl<Ease.JoinImpl<A, B>, C> Join<A, B, C>(this Ease.JoinImpl<A, B> prev, C calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            => new(prev, calc, prev.EvenSplit);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.JoinImpl<Ease.JoinImpl<A, B>, Ease.JoinImpl<C, D>> Join<A, B, C, D>(this Ease.JoinImpl<A, B> prev, Ease.JoinImpl<C, D> calc)
            where A : struct, IEase
            where B : struct, IEase
            where C : struct, IEase
            where D : struct, IEase
            => new(prev, calc, 0.5f);

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.ReverseImpl> Reverse<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.SineImpl> SineAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.SineImpl> SineDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.QuadImpl> QuadAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.QuadImpl> QuadDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.CubicImpl> CubicAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.CubicImpl> CubicDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.QuartImpl> QuartAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.QuartImpl> QuartDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.SqrtImpl> SqrtAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.SqrtImpl> SqrtDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, Ease.PowImpl> PowAcc<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, Ease.PowImpl> PowDec<E>(this E prev, float pow) where E : struct, IEase => new(prev, new(pow));

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.ExpoImpl> ExpoAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.ExpoImpl> ExpoDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.CircImpl> CircAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.CircImpl> CircDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.BackImpl> BackAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.BackImpl> BackDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.ElasticImpl> ElasticAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.ElasticImpl> ElasticDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.InverseImpl<E, EaseFast.BounceImpl> BounceAcc<E>(this E prev) where E : struct, IEase => new(prev, new());

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.DirectImpl<E, EaseFast.BounceImpl> BounceDec<E>(this E prev) where E : struct, IEase => new(prev, new());

        // 〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜〜
        // utils

        /// <summary></summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Ease.CurveImpl ToEase(this AnimationCurve self) => new Ease.CurveImpl(self);
    }
}

