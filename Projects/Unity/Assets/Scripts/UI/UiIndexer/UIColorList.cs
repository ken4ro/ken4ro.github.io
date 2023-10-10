using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

namespace UI {
    /// <summary>
    /// カラー切り替えリスト
    /// Image/RawImage/AtlasImageの一括カラー変更機能
    /// m_ColorListのカラーを指定してm_GraphicListに登録したオブジェクトに反映します
    /// </summary>
    public class UIColorList : MonoBehaviour {
        [SerializeField]
        private Color[] m_ColorList;
        [SerializeField]
        private Graphic[] m_GraphicList;

        public int GetColorListCount() {
            if (m_ColorList == null) {
                return 0;
            }
            return m_ColorList.Length;
        }

        /// <summary>
        /// カラー反映
        /// </summary>
        /// <param name="index">m_ColorListのインデックス</param>
        public void Apply(int index) {
            if (m_ColorList == null || index >= m_ColorList.Length) {
                return;
            }
            var col = m_ColorList[index];
            foreach (var list in m_GraphicList) {
                if (list == null) {
                    continue;
                }
                list.color = col;
            }
        }
    }
}
