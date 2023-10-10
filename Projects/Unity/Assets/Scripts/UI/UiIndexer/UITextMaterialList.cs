using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI {
    /// <summary>
    /// TextMeshProマテリアル切り替えリスト
    /// m_MaterialListのカラーを指定してm_TextListに登録したオブジェクトに反映します
    /// </summary>
    public class UITextMaterialList : MonoBehaviour {
        [SerializeField]
        private Material[] m_MaterialList;
        [SerializeField]
        private TextMeshProUGUI[] m_TextList;

        public int GetMaterialListCount() {
            if (m_MaterialList == null) {
                return 0;
            }
            return m_MaterialList.Length;
        }

        /// <summary>
        /// マテリアル反映
        /// </summary>
        /// <param name="index">m_MaterialListのインデックス</param>
        public void Apply(int index) {
            if (m_MaterialList == null || index >= m_MaterialList.Length) {
                return;
            }
            var mat = m_MaterialList[index];
            foreach (var list in m_TextList) {
                if (list == null) {
                    continue;
                }
                list.fontSharedMaterial = mat;
            }
        }
    }
}
