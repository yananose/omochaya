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
    using Omochaya.HiddenStory;

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
        public readonly struct Carrier0 : Mover.ICarrierProperty<CanvasGroup, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(CanvasGroup self) => self.alpha;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(CanvasGroup self, float value) => self.alpha = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the CanvasGroup.alpha property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<CanvasGroup, float, Carrier0> TweenAlpha(this CanvasGroup self, Component owner = null) => new(self, owner);

        // Carrier1:TMP_Text.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : Mover.ICarrierProperty<TMP_Text, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(TMP_Text self) => self.alpha;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(TMP_Text self, float value) => self.alpha = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.alpha property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<TMP_Text, float, Carrier1> TweenAlpha(this TMP_Text self, Component owner = null) => new(self, owner);

        // Carrier2:TMP_Text.fontSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : Mover.ICarrierProperty<TMP_Text, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(TMP_Text self) => self.fontSize;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(TMP_Text self, float value) => self.fontSize = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.fontSize property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<TMP_Text, float, Carrier2> TweenFontSize(this TMP_Text self, Component owner = null) => new(self, owner);

        // Carrier3:TMP_Text.maxVisibleCharacters -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : Mover.ICarrierProperty<TMP_Text, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(TMP_Text self) => self.maxVisibleCharacters;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(TMP_Text self, float value) => self.maxVisibleCharacters = Mathf.CeilToInt(value);
        }

        /// <summary>Creates a zero-allocation tween carrier for the TMP_Text.maxVisibleCharacters property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<TMP_Text, float, Carrier3> TweenMaxVisibleCharacters(this TMP_Text self, Component owner = null) => new(self, owner);

        // Carrier4:AudioSource.volume -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier4 : Mover.ICarrierProperty<AudioSource, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(AudioSource self) => self.volume;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(AudioSource self, float value) => self.volume = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the AudioSource.volume property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<AudioSource, float, Carrier4> TweenVolume(this AudioSource self, Component owner = null) => new(self, owner);

        // Carrier5:AudioSource.pitch -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier5 : Mover.ICarrierProperty<AudioSource, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(AudioSource self) => self.pitch;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(AudioSource self, float value) => self.pitch = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the AudioSource.pitch property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<AudioSource, float, Carrier5> TweenPitch(this AudioSource self, Component owner = null) => new(self, owner);

        // Carrier6:Camera.fieldOfView -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier6 : Mover.ICarrierProperty<Camera, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(Camera self) => self.fieldOfView;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Camera self, float value) => self.fieldOfView = Mathf.Clamp(value, 0.1f, 179.9f);
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.fieldOfView property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Camera, float, Carrier6> TweenFieldOfView(this Camera self, Component owner = null) => new(self, owner);

        // Carrier7:Camera.orthographicSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier7 : Mover.ICarrierProperty<Camera, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(Camera self) => self.orthographicSize;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Camera self, float value) => self.orthographicSize = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.orthographicSize property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Camera, float, Carrier7> TweenOrthographicSize(this Camera self, Component owner = null) => new(self, owner);

        // Carrier8:Image.fillAmount -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier8 : Mover.ICarrierProperty<Image, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(Image self) => self.fillAmount;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Image self, float value) => self.fillAmount = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Image.fillAmount property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Image, float, Carrier8> TweenFillAmount(this Image self, Component owner = null) => new(self, owner);

        // Carrier9:Slider.value -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier9 : Mover.ICarrierProperty<Slider, float>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public float Get(Slider self) => self.value;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Slider self, float value) => self.value = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Slider.value property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Slider, float, Carrier9> TweenValue(this Slider self, Component owner = null) => new(self, owner);
    }

    public static partial class StoryVector2 ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:RectTransform.anchoredPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : Mover.ICarrierProperty<RectTransform, Vector2>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Get(RectTransform self) => self.anchoredPosition;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(RectTransform self, Vector2 value) => self.anchoredPosition = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.anchoredPosition property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<RectTransform, Vector2, Carrier0> TweenAnchoredPosition(this RectTransform self, Component owner = null) => new(self, owner);

        // Carrier1:RectTransform.sizeDelta -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : Mover.ICarrierProperty<RectTransform, Vector2>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Get(RectTransform self) => self.sizeDelta;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(RectTransform self, Vector2 value) => self.sizeDelta = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.sizeDelta property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<RectTransform, Vector2, Carrier1> TweenSizeDelta(this RectTransform self, Component owner = null) => new(self, owner);

        // Carrier2:RectTransform.pivot -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : Mover.ICarrierProperty<RectTransform, Vector2>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector2 Get(RectTransform self) => self.pivot;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(RectTransform self, Vector2 value) => self.pivot = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the RectTransform.pivot property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<RectTransform, Vector2, Carrier2> TweenPivot(this RectTransform self, Component owner = null) => new(self, owner);
    }

    public static partial class StoryVector3 ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Transform.localPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : Mover.ICarrierProperty<Transform, Vector3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Get(Transform self) => self.localPosition;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Vector3 value) => self.localPosition = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localPosition property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Vector3, Carrier0> TweenLocalPosition(this Transform self, Component owner = null) => new(self, owner);

        // Carrier1:Transform.localScale -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : Mover.ICarrierProperty<Transform, Vector3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Get(Transform self) => self.localScale;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Vector3 value) => self.localScale = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localScale property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Vector3, Carrier1> TweenLocalScale(this Transform self, Component owner = null) => new(self, owner);

        // Carrier2:Transform.localEulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : Mover.ICarrierProperty<Transform, Vector3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Get(Transform self) => self.localEulerAngles;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Vector3 value) => self.localEulerAngles = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localEulerAngles property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Vector3, Carrier2> TweenLocalEulerAngles(this Transform self, Component owner = null) => new(self, owner);

        // Carrier3:Transform.position -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier3 : Mover.ICarrierProperty<Transform, Vector3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Get(Transform self) => self.position;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Vector3 value) => self.position = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.position property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Vector3, Carrier3> TweenPosition(this Transform self, Component owner = null) => new(self, owner);

        // Carrier4:Transform.eulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier4 : Mover.ICarrierProperty<Transform, Vector3>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Vector3 Get(Transform self) => self.eulerAngles;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Vector3 value) => self.eulerAngles = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.eulerAngles property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Vector3, Carrier4> TweenEulerAngles(this Transform self, Component owner = null) => new(self, owner);
    }

    public static partial class StoryColor ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Graphic.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : Mover.ICarrierProperty<Graphic, Color>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Get(Graphic self) => self.color;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Graphic self, Color value) => self.color = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Graphic.color property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Graphic, Color, Carrier0> TweenColor(this Graphic self, Component owner = null) => new(self, owner);

        // Carrier1:SpriteRenderer.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : Mover.ICarrierProperty<SpriteRenderer, Color>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Get(SpriteRenderer self) => self.color;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(SpriteRenderer self, Color value) => self.color = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the SpriteRenderer.color property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<SpriteRenderer, Color, Carrier1> TweenColor(this SpriteRenderer self, Component owner = null) => new(self, owner);

        // Carrier2:Camera.backgroundColor -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier2 : Mover.ICarrierProperty<Camera, Color>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Color Get(Camera self) => self.backgroundColor;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Camera self, Color value) => self.backgroundColor = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Camera.backgroundColor property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Camera, Color, Carrier2> TweenBackgroundColor(this Camera self, Component owner = null) => new(self, owner);
    }

    public static partial class StoryQuaternion ///////////////////////////////////////////////////////////////////////////////////
    {
        // Carrier0:Transform.localRotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier0 : Mover.ICarrierProperty<Transform, Quaternion>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Quaternion Get(Transform self) => self.localRotation;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Quaternion value) => self.localRotation = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.localRotation property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Quaternion, Carrier0> TweenLocalRotation(this Transform self, Component owner = null) => new(self, owner);

        // Carrier1:Transform.rotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
        public readonly struct Carrier1 : Mover.ICarrierProperty<Transform, Quaternion>
        {
            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public Quaternion Get(Transform self) => self.rotation;

            /// <summary>Don't touch! Only for system.</summary>
            [System.ComponentModel.EditorBrowsable(System.ComponentModel.EditorBrowsableState.Never)]
            [MethodImpl(MethodImplOptions.AggressiveInlining)]
            public void Set(Transform self, Quaternion value) => self.rotation = value;
        }

        /// <summary>Creates a zero-allocation tween carrier for the Transform.rotation property.</summary>
        [MethodImpl(MethodImplOptions.AggressiveInlining)]
        public static Mover.Carrier<Transform, Quaternion, Carrier1> TweenRotation(this Transform self, Component owner = null) => new(self, owner);
    }
}
