// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenCarrier.cs" company="Omochaya">
//   Copyright (t) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// Defines zero-allocation carrier structures and extension methods to bridge Unity component properties with the tweening system.
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using System.Runtime.CompilerServices;

    // #if UNITY_EDITOR
    //     static class StorySample
    //     {
    //         public static async Story.Task Main(bool _
    //             , RectTransform rt
    //             , CanvasGroup cg
    //             , SpriteRenderer sr
    //             , TextMeshPro tmp
    //         ){
    //             var start = Story.GetStart();
    //             await rt.TweenLocalPosition().To(x:1f).Interval(2f, Story.Ease.None, ref start);
    //             await rt.TweenLocalPosition().By(z:10f, y:30f).Speed(2f);
    //             await rt.TweenAnchoredPosition().To(Vector2.up * 10f).Speed(2f);
    //             await cg.TweenAlpha().To(0.5f).Interval(2f);
    //             await sr.TweenColor().To(Color.red).Interval(2f);
    //             await rt.TweenLocalRotation().By(Quaternion.FromToRotation(rt.forward, Vector3.right)).Speed(30f);
    //             await tmp.TweenColor().To(a:0.5f).Interval(2f);
    //             await tmp.TweenAlpha().To(0.5f).Interval(2f);
    //         }
    //     }
    // #endif

    public static partial class StoryFloat ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:CanvasGroup.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : ICarrier
        {
            readonly CanvasGroup self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.alpha = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the CanvasGroup.alpha property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier0 TweenAlpha(this CanvasGroup self) => new(self);

        // Carrier1:CanvasGroup.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.alpha = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.alpha property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier1 TweenAlpha(this TMP_Text self) => new(self);

        // Carrier2:TMP_Text.fontSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier2(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fontSize;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.fontSize = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.fontSize property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier2 TweenFontSize(this TMP_Text self) => new(self);

        // Carrier3:TMP_Text.maxVisibleCharacters -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier3(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.maxVisibleCharacters;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.maxVisibleCharacters = Mathf.CeilToInt(value);
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.maxVisibleCharacters property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier3 TweenMaxVisibleCharacters(this TMP_Text self) => new(self);

        // Carrier4:AudioSource.volume -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier4 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier4(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.volume;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.volume = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the AudioSource.volume property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier4 TweenVolume(this AudioSource self) => new(self);

        // Carrier5:AudioSource.pitch -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier5 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier5(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.pitch;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.pitch = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the AudioSource.pitch property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier5 TweenPitch(this AudioSource self) => new(self);

        // Carrier6:Camera.fieldOfView -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier6 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier6(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fieldOfView;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.fieldOfView = Mathf.Clamp(value, 0.1f, 179.9f);
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.fieldOfView property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier6 TweenFieldOfView(this Camera self) => new(self);

        // Carrier7:Camera.orthographicSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier7 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier7(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.orthographicSize;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.orthographicSize = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.orthographicSize property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier7 TweenOrthographicSize(this Camera self) => new(self);

        // Carrier8:Image.fillAmount -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier8 : ICarrier
        {
            readonly Image self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier8(Image self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fillAmount;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.fillAmount = value; 
        }

        /// <summary>Creates a zero-allocation tween carrier for the Image.fillAmount property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier8 TweenFillAmount(this Image self) => new(self);

        // Carrier9:Slider.value -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier9 : ICarrier
        {
            readonly Slider self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier9(Slider self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.value;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(float value) => this.self.value = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Slider.value property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier9 TweenValue(this Slider self) => new(self);
    }

    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:RectTransform.anchoredPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier0(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.anchoredPosition;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector2 value) => this.self.anchoredPosition = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.anchoredPosition property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier0 TweenAnchoredPosition(this RectTransform self)
            => new(self);

        // Carrier1:RectTransform.sizeDelta -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier1(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.sizeDelta;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector2 value) => this.self.sizeDelta = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.sizeDelta property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier1 TweenSizeDelta(this RectTransform self)
            => new(self);

        // Carrier2:RectTransform.pivot -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier2(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.pivot;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector2 value) => this.self.pivot = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.pivot property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier2 TweenPivot(this RectTransform self)
            => new(self);
    }

    public static partial class StoryVector3 ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Transform.localPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localPosition;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector3 value) => this.self.localPosition = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localPosition property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier0 TweenLocalPosition(this Transform self)
            => new(self);

        // Carrier1:Transform.localScale -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localScale;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector3 value) => this.self.localScale = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localScale property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier1 TweenLocalScale(this Transform self)
            => new(self);

        // Carrier2:Transform.localEulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier2(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localEulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector3 value) => this.self.localEulerAngles = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localEulerAngles property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier2 TweenLocalEulerAngles(this Transform self)
            => new(self);

        // Carrier3:Transform.position -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier3(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.position;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector3 value) => this.self.position = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.position property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier3 TweenPosition(this Transform self)
            => new(self);

        // Carrier4:Transform.eulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier4 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier4(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.eulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Vector3 value) => this.self.eulerAngles = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.eulerAngles property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier4 TweenEulerAngles(this Transform self)
            => new(self);
    }

    public static partial class StoryColor ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Graphic.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : ICarrier
        {
            readonly Graphic self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier0(Graphic self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Graphic.color property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier0 TweenColor(this Graphic self)
            => new(self);

        // Carrier1:SpriteRenderer.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly SpriteRenderer self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier1(SpriteRenderer self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the SpriteRenderer.color property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier1 TweenColor(this SpriteRenderer self)
            => new(self);

        // Carrier2:Camera.backgroundColor -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier2(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.backgroundColor;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Color value) => this.self.backgroundColor = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.backgroundColor property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier2 TweenBackgroundColor(this Camera self)
            => new(self);
    }

    public static partial class StoryQuaternion ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Transform.localRotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Current => this.self.localRotation;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Quaternion value) => this.self.localRotation = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localRotation property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier0 TweenLocalRotation(this Transform self) => new(self);

        // Carrier1:Transform.rotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Current => this.self.rotation;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void SetCurrent(Quaternion value) => this.self.rotation = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.rotation property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Carrier1 TweenRotation(this Transform self) => new(self);
    }
}
