// --------------------------------------------------------------------------------------------------------------------
// <copyright file="StoryMoverTypes.cs" company="Omochaya">
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

    static class StoryFloat
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<float> {}

        // Carrier0:CanvasGroup.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly CanvasGroup self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(CanvasGroup self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.alpha = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier0 MoveAlpha(this CanvasGroup self) => new(self);

        // Carrier1:TMP_Text.alpha -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.alpha;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.alpha = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier1 MoveAlpha(this TMP_Text self) => new(self);

        // Carrier2:TMP_Text.fontSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.fontSize;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.fontSize = value;
        }

        /// <summary></summary>
        public static Carrier2 MoveFontSize(this TMP_Text self) => new(self);

        // Carrier3:TMP_Text.maxVisibleCharacters -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier3 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier3(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.maxVisibleCharacters;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.maxVisibleCharacters = Mathf.CeilToInt(value);
        }

        /// <summary></summary>
        public static Carrier3 MoveVisibleCount(this TMP_Text self) => new(self);

        // Carrier4:AudioSource.volume -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier4 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier4(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.volume;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.volume = Mathf.Clamp01(value);
        }

        /// <summary></summary>
        public static Carrier4 MoveVolume(this AudioSource self) => new(self);


        // Carrier5:AudioSource.pitch -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier5 : ICarrier
        {
            readonly AudioSource self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier5(AudioSource self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.pitch;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.pitch = value;
        }

        /// <summary></summary>
        public static Carrier5 MovePitch(this AudioSource self) => new(self);

        // Carrier6:Camera.fieldOfView -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier6 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier6(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.fieldOfView;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.fieldOfView = Mathf.Clamp(value, 0.1f, 179.9f);
        }

        /// <summary></summary>
        public static Carrier6 MoveFov(this Camera self) => new(self);

        // Carrier7:Camera.orthographicSize -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier7 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier7(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.orthographicSize;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.orthographicSize = value;
        }

        /// <summary></summary>
        public static Carrier7 MoveOrthoSize(this Camera self) => new(self);

        // Carrier8:Image.fillAmount -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier8 : ICarrier
        {
            readonly Image self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier8(Image self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.fillAmount;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.fillAmount = value; 
        }

        /// <summary></summary>
        public static Carrier8 MoveFillAmount(this Image self) => new(self);

        // Carrier9:Slider.value -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier9 : ICarrier
        {
            readonly Slider self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier9(Slider self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public float Current => this.self.value;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(float value) => this.self.value = value;
        }

        /// <summary></summary>
        public static Carrier9 MoveValue(this Slider self) => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<float, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(float current) => new(current);
                /// <summary>Don't touch! Only for system.</summary>
                public float Set(float current, Mover.Param1 prm)
                    => prm.P0;
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<float, C, Changer.To, Mover.Param1> To<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<float, C, Changer.To, Mover.Param1> Add<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector2
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector2> {}

        // Carrier0:RectTransform.anchoredPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.anchoredPosition;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.anchoredPosition = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveAnchoredPosition(this RectTransform self)
            => new(self);

        // Carrier1:RectTransform.sizeDelta -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.sizeDelta;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.sizeDelta = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveSizeDelta(this RectTransform self)
            => new(self);

        // Carrier2:RectTransform.pivot -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly RectTransform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(RectTransform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector2 Current => this.self.pivot;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector2 value) => this.self.pivot = value;
        }

        /// <summary></summary>
        public static Carrier2 MovePivot(this RectTransform self)
            => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XTo : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector2 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YTo : Mover.IChanger<Vector2, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector2 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Vector2, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector2 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector2 Set(Vector2 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector2, C, Changer.To, Mover.Param2> To<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector2, C, Changer.To, Mover.Param2> Add<C>(this C carrier, Vector2 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryVector3
    {

        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Vector3> {}

        // Carrier0:Transform.localPosition -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localPosition;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localPosition = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveLocalPosition(this Transform self)
            => new(self);

        // Carrier1:Transform.localScale -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localScale;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localScale = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveLocalScale(this Transform self)
            => new(self);

        // Carrier2:Transform.localEulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.localEulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.localEulerAngles = value;
        }

        /// <summary></summary>
        public static Carrier2 MoveLocalEulerAngles(this Transform self)
            => new(self);

        // Carrier3:Transform.position -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier3 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier3(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.position;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.position = value;
        }

        /// <summary></summary>
        public static Carrier3 MovePosition(this Transform self)
            => new(self);

        // Carrier4:Transform.eulerAngles -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier4 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier4(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Vector3 Current => this.self.eulerAngles;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Vector3 value) => this.self.eulerAngles = value;
        }

        /// <summary></summary>
        public static Carrier4 MoveEulerAngles(this Transform self)
            => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.x = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.y = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ZTo : Mover.IChanger<Vector3, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Vector3 current) => new(current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param1 prm)
                {
                    current.z = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct XYTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.x, current.y);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct YZTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.y = prm.P0;
                    current.z = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ZXTo : Mover.IChanger<Vector3, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Vector3 current) => new(current.z, current.x);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param2 prm)
                {
                    current.z = prm.P0;
                    current.x = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Vector3, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Vector3 current) => new(current.x, current.y, current.z);
                /// <summary>Don't touch! Only for system.</summary>
                public Vector3 Set(Vector3 current, Mover.Param3 prm)
                {
                    current.x = prm.P0;
                    current.y = prm.P1;
                    current.z = prm.P2;
                    return current;
                }
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.XTo, Mover.Param1> XTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.XTo, Mover.Param1> XAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.YTo, Mover.Param1> YTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.YTo, Mover.Param1> YAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.ZTo, Mover.Param1> ZTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.ZTo, Mover.Param1> ZAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.XYTo, Mover.Param2> XYTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.XYTo, Mover.Param2> XYAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.YZTo, Mover.Param2> YZTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.YZTo, Mover.Param2> YZAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.ZXTo, Mover.Param2> ZXTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.ZXTo, Mover.Param2> ZXAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Vector3, C, Changer.To, Mover.Param3> To<C>(this C carrier, Vector3 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Vector3, C, Changer.To, Mover.Param3> Add<C>(this C carrier, Vector3 to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryColor
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Color> {}

        // Carrier0:Graphic.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly Graphic self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(Graphic self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveColor(this Graphic self)
            => new(self);

        // Carrier1:TMP_Text.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly TMP_Text self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(TMP_Text self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveColor(this TMP_Text self)
            => new(self);

        // Carrier2:SpriteRenderer.color -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier2 : ICarrier
        {
            readonly SpriteRenderer self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier2(SpriteRenderer self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Color Current => this.self.color;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Color value) => this.self.color = value;
        }

        /// <summary></summary>
        public static Carrier2 MoveColor(this SpriteRenderer self)
            => new(self);

        // Carrier3:Camera.backgroundColor -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier3 : ICarrier
        {
            readonly Camera self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier3(Camera self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Color Current => this.self.backgroundColor;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Color value) => this.self.backgroundColor = value;
        }

        /// <summary></summary>
        public static Carrier3 MoveBackgroundColor(this Camera self)
            => new(self);

        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct RTo : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Color current) => new(current.r);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.r = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct GTo : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Color current) => new(current.g);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.g = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct BTo : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Color current) => new(current.b);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.b = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ATo : Mover.IChanger<Color, Mover.Param1>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param1 Get(Color current) => new(current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param1 prm)
                {
                    current.a = prm.P0;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct RGTo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.r, current.g);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct GBTo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.b = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct BRTo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.b, current.r);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.b = prm.P0;
                    current.r = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct RATo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.r, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.r = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct GATo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.g, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.g = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct BATo : Mover.IChanger<Color, Mover.Param2>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param2 Get(Color current) => new(current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param2 prm)
                {
                    current.b = prm.P0;
                    current.a = prm.P1;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct RGBTo : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Color current) => new(current.r, current.g, current.b);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    current.b = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct GBATo : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Color current) => new(current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.g = prm.P0;
                    current.b = prm.P1;
                    current.a = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct BARTo : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Color current) => new(current.b, current.a, current.r);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.b = prm.P0;
                    current.a = prm.P1;
                    current.r = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct ARGTo : Mover.IChanger<Color, Mover.Param3>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param3 Get(Color current) => new(current.a, current.r, current.g);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param3 prm)
                {
                    current.a = prm.P0;
                    current.r = prm.P1;
                    current.g = prm.P2;
                    return current;
                }
            }
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Color, Mover.Param4>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.Param4 Get(Color current) => new(current.r, current.g, current.b, current.a);
                /// <summary>Don't touch! Only for system.</summary>
                public Color Set(Color current, Mover.Param4 prm)
                {
                    current.r = prm.P0;
                    current.g = prm.P1;
                    current.b = prm.P2;
                    current.a = prm.P3;
                    return current;
                }
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.RTo, Mover.Param1> RTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.RTo, Mover.Param1> RAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.GTo, Mover.Param1> GTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.GTo, Mover.Param1> GAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.BTo, Mover.Param1> BTo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.BTo, Mover.Param1> BAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.ATo, Mover.Param1> ATo<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.ATo, Mover.Param1> AAdd<C>(this C carrier, float to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.RGTo, Mover.Param2> RGTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.RGTo, Mover.Param2> RGAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.GBTo, Mover.Param2> GBTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.GBTo, Mover.Param2> GBAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.BRTo, Mover.Param2> BRTo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.BRTo, Mover.Param2> BRAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.RATo, Mover.Param2> RATo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.RATo, Mover.Param2> RAAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.GATo, Mover.Param2> GATo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.GATo, Mover.Param2> GAAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.BATo, Mover.Param2> BATo<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.BATo, Mover.Param2> BAAdd<C>(this C carrier, float to0, float to1)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.RGBTo, Mover.Param3> RGBTo<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.RGBTo, Mover.Param3> RGBAdd<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.GBATo, Mover.Param3> GBATo<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.GBATo, Mover.Param3> GBAAdd<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.BARTo, Mover.Param3> BARTo<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.BARTo, Mover.Param3> BARAdd<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.ARGTo, Mover.Param3> ARGTo<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.ARGTo, Mover.Param3> ARGAdd<C>(this C carrier, float to0, float to1, float to2)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to0, to1, to2));

        /// <summary></summary>
        public static Mover.PlanTo<Color, C, Changer.To, Mover.Param4> To<C>(this C carrier, Color to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Color, C, Changer.To, Mover.Param4> Add<C>(this C carrier, Color to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    static class StoryQuaternion
    {
        // carrier ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public interface ICarrier : Mover.ICarrier<Quaternion> {}

        // Carrier0:Transform.localRotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier0 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier0(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Quaternion Current => this.self.localRotation;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Quaternion value) => this.self.localRotation = value;
        }

        /// <summary></summary>
        public static Carrier0 MoveLocalRotation(this Transform self) => new(self);

        // Carrier1:Transform.rotation -------------------------------------------------
        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Carrier1 : ICarrier
        {
            readonly Transform self;
            /// <summary>Don't touch! Only for system.</summary>
            public Component Self => this.self;
            /// <summary>Don't touch! Only for system.</summary>
            internal Carrier1(Transform self) => this.self = self;
            /// <summary>Don't touch! Only for system.</summary>
            public Quaternion Current => this.self.rotation;
            /// <summary>Don't touch! Only for system.</summary>
            public void SetCurrent(Quaternion value) => this.self.rotation = value;
        }

        /// <summary></summary>
        public static Carrier1 MoveRotation(this Transform self) => new(self);


        // changer ===============================================================================================

        /// <summary>Don't touch! Only for system.</summary>
        public readonly struct Changer
        {
            /// <summary>Don't touch! Only for system.</summary>
            public readonly struct To : Mover.IChanger<Quaternion, Mover.ParamQ>
            {
                /// <summary>Don't touch! Only for system.</summary>
                public Mover.ParamQ Get(Quaternion current) => new(current);
                /// <summary>Don't touch! Only for system.</summary>
                public Quaternion Set(Quaternion current, Mover.ParamQ prm)
                    => prm.Q;
            }
        }

        // plan ==================================================================================================

        /// <summary></summary>
        public static Mover.PlanTo<Quaternion, C, Changer.To, Mover.ParamQ> To<C>(this C carrier, Quaternion to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));

        /// <summary></summary>
        public static Mover.PlanAdd<Quaternion, C, Changer.To, Mover.ParamQ> Add<C>(this C carrier, Quaternion to)
            where C : struct, ICarrier => new(carrier, new(), Mover.CreateParam(to));
    }

    // ToDo...（たぶん非公開にする）
    static class StoryRect
    {
    }
}
