using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// シェーダ操作の基底クラス
    /// Animationからマテリアルを操作するためのコンポーネント
    /// マテリアルを直接触ると他のオブジェクトにも影響が出るため、
    /// 変更時にマテリアルを複製する必要がある
    /// プログラム側からマテリアルを触る場合もこれを通す前提
    /// </summary>
    [RequireComponent(typeof(Graphic))]
    [ExecuteAlways] // Animation編集中も反映するため
    [DisallowMultipleComponent]
    public abstract class UIShaderControlBase : MonoBehaviour, IMaterialModifier {
        [SerializeField]
        private Material m_Material;
        private Graphic m_Graphic;


        private Material m_instanceMaterial;

        /// <summary>
        /// マテリアル変更時に呼び出し
        /// </summary>
        protected void SetMaterialDirty() {
            m_Graphic?.SetMaterialDirty();
        }
        private void OnEnable() {
            if (m_Graphic == null) {
                m_Graphic = GetComponent<Graphic>();
            }
            m_Graphic?.SetMaterialDirty();   // GetModifiedMaterial呼び出しのため
        }
        private void OnDisable() {
            if (m_Material != null) {
                DestroyImmediate(m_Material);
                m_Material = null;
            }

            if (m_instanceMaterial != null) {
                DestroyImmediate(m_instanceMaterial);
                m_instanceMaterial = null;
            }

            m_Graphic?.SetMaterialDirty();   // GetModifiedMaterial呼び出しのため
        }

        /// <summary>
        /// 該当マテリアルかチェック
        /// HasPropertyなどを使って渡されたマテリアルが想定するものかチェックする
        /// </summary>
        /// <returns>該当するならtrue</returns>
        protected abstract bool IsValidMaterial(Material material);
        /// <summary>
        /// マテリアルに設定を行う
        /// </summary>
        protected abstract void ApplyMaterial(Material material);

        /// <summary>
        /// 動作が有効か
        /// </summary>
        private bool IsValid() {
            return isActiveAndEnabled & m_Graphic != null;
        }
        /// <summary>
        /// マテリアル変更時のコールバック
        /// </summary>
        /// <param name="baseMaterial">直前のマテリアル</param>
        /// <returns>変更後のマテリアル</returns>
        public Material GetModifiedMaterial(Material baseMaterial) {
            // 別のマテリアルは除外
            if (!IsValid() || !IsValidMaterial(baseMaterial)) {
                return baseMaterial;
            }

            if (m_Material == null) {
                if (m_instanceMaterial == null)
                    m_instanceMaterial = new Material(baseMaterial);

                m_Material = m_instanceMaterial;
                m_Material.hideFlags = HideFlags.HideAndDontSave;   // Animation編集時にも動かすため保存しない設定
#if UNITY_EDITOR
                m_Material.name += "(Clone)";   // Inspector確認用
#endif
            }
            // 基本は直前のマテリアルを引き継ぐ
            m_Material.CopyPropertiesFromMaterial(baseMaterial);

            // マテリアルに反映
            ApplyMaterial(m_Material);

            return m_Material;
        }

        /// <summary>
        /// Animationで変更されたときのコールバック
        /// </summary>
        private void OnDidApplyAnimationProperties() {
            if (!IsValid()) {
                return;
            }

            m_Graphic?.SetMaterialDirty();   // GetModifiedMaterial呼び出しのため
        }
#if UNITY_EDITOR
        /// <summary>
        /// Editor編集時のコールバック
        /// </summary>
        private void OnValidate() {
            OnDidApplyAnimationProperties();
        }
#endif
    }
}