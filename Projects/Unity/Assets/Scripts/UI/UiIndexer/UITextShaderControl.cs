using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI {
    /// <summary>
    /// TextMeshPro用シェーダの操作を行う
    /// Animationからマテリアルを操作するためのコンポーネント
    /// マテリアルを直接触ると他のオブジェクトにも影響が出るため、
    /// 変更時にマテリアルを複製する必要がある
    /// プログラム側からマテリアルを触る場合もこれを通す前提
    /// </summary>
    public class UITextShaderControl : UIShaderControlBase {
        private readonly int PropertyIdFaceColor = Shader.PropertyToID("_FaceColor");
        private readonly int PropertyIdOutlineColor = Shader.PropertyToID("_OutlineColor");
        private readonly int PropertyIdUnderlayColor = Shader.PropertyToID("_UnderlayColor");

        // Animationで触るためにpublic、プログラムから直接触らない前提
        public Color FaceColor = new Color(1f, 1f, 1f, 1f);
        public Color OutlineColor = new Color(1f, 1f, 1f, 1f);
        public Color UnderlayColor = new Color(1f, 1f, 1f, 1f);

        protected override bool IsValidMaterial(Material material) {
            bool ret = material.HasProperty(PropertyIdFaceColor) & material.HasProperty(PropertyIdOutlineColor) & material.HasProperty(PropertyIdUnderlayColor);
            return ret;
        }
        protected override void ApplyMaterial(Material material) {
            material.SetColor(PropertyIdFaceColor, this.FaceColor);
            material.SetColor(PropertyIdOutlineColor, this.OutlineColor);
            material.SetColor(PropertyIdUnderlayColor, this.UnderlayColor);
        }

        /// <summary>
        /// マテリアルから該当値を取得
        /// </summary>
        private void Reset() {
            var text = GetComponent<TextMeshProUGUI>();
            if (text == null) {
                return;
            }
            this.FaceColor = text.fontMaterial.GetColor(PropertyIdFaceColor);
            this.OutlineColor = text.fontMaterial.GetColor(PropertyIdOutlineColor);
            this.UnderlayColor = text.fontMaterial.GetColor(PropertyIdUnderlayColor);
        }
    }
}