using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// UI用シェーダの操作を行う
    /// Animationからマテリアルを操作するためのコンポーネント
    /// マテリアルを直接触ると他のオブジェクトにも影響が出るため、
    /// 変更時にマテリアルを複製する必要がある
    /// プログラム側からマテリアルを触る場合もこれを通す前提
    /// </summary>
    public class UIShaderControl : UIShaderControlBase {
        private readonly int PropertyIdBlurU = Shader.PropertyToID("_BlurU");
        private readonly int PropertyIdBlurV = Shader.PropertyToID("_BlurV");
        private readonly int PropertyIdSingleColor = Shader.PropertyToID("_SingleColor");

        // Animationで触るためにpublic、プログラムから直接触らない前提
        public Color Color = new Color(0.5f, 0.5f, 0.5f, 1f);
        public float BlurU;
        public float BlurV;
        public bool IsBlur;
        public bool IsGrayScale;
        public Color SingleColor = new Color(1f, 1f, 1f, 1f);
        public bool IsSingleColor;

        /// <summary>
        /// 白黒表示のON/OFF
        /// </summary>
        public void EnableGrayScale(bool enable) {
            this.IsGrayScale = enable;
            SetMaterialDirty();
        }
        /// <summary>
        /// ブラー設定
        /// この後EnableBlurを呼び出す
        /// </summary>
        public void SetBlurParameter(float blurU, float blurV) {
            this.BlurU = blurU;
            this.BlurV = blurV;
        }
        /// <summary>
        /// ブラーのON/OFF
        /// </summary>
        public void EnableBlur(bool enable) {
            this.IsBlur = enable;
            SetMaterialDirty();
        }

        protected override bool IsValidMaterial(Material material) {
            bool ret = material.HasProperty(PropertyIdBlurU) & material.HasProperty(PropertyIdBlurV) & material.HasProperty(PropertyIdSingleColor);
            return ret;
        }
        protected override void ApplyMaterial(Material material) {
            material.color = this.Color;
            material.SetFloat(PropertyIdBlurU, this.BlurU);
            material.SetFloat(PropertyIdBlurV, this.BlurV);
            //material.SetFloat("_UseBlur", IsBlur ? 1f : 0f);
            if (IsBlur) {
                material.EnableKeyword("BLUR");
            }
            else {
                material.DisableKeyword("BLUR");
            }
            //material.SetFloat("_UseGlayScale", IsGlayScale ? 1f : 0f);
            if (IsGrayScale) {
                material.EnableKeyword("GRAYSCALE");
            }
            else {
                material.DisableKeyword("GRAYSCALE");
            }
            if (IsSingleColor) {
                material.EnableKeyword("SINGLECOLOR");
                material.SetColor(PropertyIdSingleColor, this.SingleColor);
            }
            else {
                material.DisableKeyword("SINGLECOLOR");
            }
        }
    }
}