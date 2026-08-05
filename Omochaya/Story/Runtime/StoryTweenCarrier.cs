// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryTweenCarrier.cs" company="Omochaya">
//   Copyright (t) 2026 Omochaya. All rights reserved.
//   Licensed under the MIT License. See LICENSE in the project root for license information.
// </copyright>
// <summary>
// </summary>
// --------------------------------------------------------------------------------------------------------------------
namespace Omochaya
{
    using UnityEngine;
    using UnityEngine.UI;
    using TMPro;
    using Omochaya.HiddenStory;

// #if UNITY_EDITOR
//     static class StorySample
//     {
//         public static async Story.Task Main(RectTransform rt, CanvasGroup cg, SpriteRenderer sr)
//         {
//             var start = Story.GetStart();
//             await rt.TweenLocalPosition().To(x:1f).Interval(2f, Story.Ease.None, ref start);
//             await rt.TweenLocalPosition().By(z:10f, y:30f).Speed(2f);
//             await rt.TweenAnchoredPosition().To(Vector2.up * 10f).Speed(2f);
//             await cg.TweenAlpha().To(0.5f).Interval(2f);
//             await sr.TweenColor().To(Color.red).Interval(2f);
//             await rt.TweenLocalRotation().By(Quaternion.FromToRotation(rt.forward, Vector3.right)).Speed(30f);
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
            internal Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.alpha = value;
        }

        /// <summary></summary>
        public static Carrier0 TweenAlpha(this CanvasGroup self) => new(self);

        // Carrier1:TMP_Text.fontSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fontSize;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.fontSize = value;
        }

        /// <summary></summary>
        public static Carrier1 TweenFontSize(this TMP_Text self) => new(self);

        // Carrier2:TMP_Text.maxVisibleCharacters -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier2(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.maxVisibleCharacters;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.maxVisibleCharacters = Mathf.CeilToInt(value);
        }

        /// <summary></summary>
        public static Carrier2 TweenVisibleCount(this TMP_Text self) => new(self);

        // Carrier3:AudioSource.volume -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier3(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.volume;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.volume = value;
        }

        /// <summary></summary>
        public static Carrier3 TweenVolume(this AudioSource self) => new(self);

        // Carrier4:AudioSource.pitch -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier4 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier4(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.pitch;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.pitch = value;
        }

        /// <summary></summary>
        public static Carrier4 TweenPitch(this AudioSource self) => new(self);

        // Carrier5:Camera.fieldOfView -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier5 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier5(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fieldOfView;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.fieldOfView = Mathf.Clamp(value, 0.1f, 179.9f);
        }

        /// <summary></summary>
        public static Carrier5 TweenFov(this Camera self) => new(self);

        // Carrier6:Camera.orthographicSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier6 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier6(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.orthographicSize;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.orthographicSize = value;
        }

        /// <summary></summary>
        public static Carrier6 TweenOrthoSize(this Camera self) => new(self);

        // Carrier7:Image.fillAmount -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier7 : ICarrier
        {
            readonly Image self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier7(Image self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.fillAmount;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.fillAmount = value; 
        }

        /// <summary></summary>
        public static Carrier7 TweenFillAmount(this Image self) => new(self);

        // Carrier8:Slider.value -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier8 : ICarrier
        {
            readonly Slider self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier8(Slider self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public float Current => this.self.value;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(float value) => this.self.value = value;
        }

        /// <summary></summary>
        public static Carrier8 TweenValue(this Slider self) => new(self);
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
            internal Carrier0(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.anchoredPosition;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector2 value) => this.self.anchoredPosition = value;
        }

        /// <summary></summary>
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
            internal Carrier1(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.sizeDelta;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector2 value) => this.self.sizeDelta = value;
        }

        /// <summary></summary>
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
            internal Carrier2(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector2 Current => this.self.pivot;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector2 value) => this.self.pivot = value;
        }

        /// <summary></summary>
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
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localPosition;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector3 value) => this.self.localPosition = value;
        }

        /// <summary></summary>
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
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localScale;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector3 value) => this.self.localScale = value;
        }

        /// <summary></summary>
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
            internal Carrier2(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.localEulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector3 value) => this.self.localEulerAngles = value;
        }

        /// <summary></summary>
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
            internal Carrier3(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.position;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector3 value) => this.self.position = value;
        }

        /// <summary></summary>
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
            internal Carrier4(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Vector3 Current => this.self.eulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Vector3 value) => this.self.eulerAngles = value;
        }

        /// <summary></summary>
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
            internal Carrier0(Graphic self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier0 TweenColor(this Graphic self)
            => new(self);

        // Carrier1:TMP_Text.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier1 TweenColor(this TMP_Text self)
            => new(self);

        // Carrier2:SpriteRenderer.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : ICarrier
        {
            readonly SpriteRenderer self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier2(SpriteRenderer self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier2 TweenColor(this SpriteRenderer self)
            => new(self);

        // Carrier3:Camera.backgroundColor -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Component Self => this.self;
            internal Carrier3(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Color Current => this.self.backgroundColor;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Color value) => this.self.backgroundColor = value;
        }

        /// <summary></summary>
        public static Carrier3 TweenBackgroundColor(this Camera self)
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
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Current => this.self.localRotation;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Quaternion value) => this.self.localRotation = value;
        }

        /// <summary></summary>
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
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public Quaternion Current => this.self.rotation;
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            public void SetCurrent(Quaternion value) => this.self.rotation = value;
        }

        /// <summary></summary>
        public static Carrier1 TweenRotation(this Transform self) => new(self);
    }
}
