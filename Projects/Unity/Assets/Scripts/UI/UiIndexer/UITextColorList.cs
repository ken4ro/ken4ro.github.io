using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace UI {
    /// <summary>
    /// TextMeshProカラー切り替えリスト
    /// m_ColorListのカラーを指定してm_TextListに登録したTextMeshProに反映します
    /// </summary>
    public class UITextColorList : MonoBehaviour {
        [System.Serializable]
        private class Gradient {
            public ColorMode ColorMode;
            public Color TopLeft;
            public Color TopRight;
            public Color BtmLeft;
            public Color BtmRight;

            public Gradient() {
                // 一応初期化してみたものの、InsertArrayElementAtIndexでは呼ばれないようだ
                // UITextColorListEditor.OnAddButtonで直に設定している
                ColorMode = ColorMode.Single;
                TopLeft = TopRight = BtmLeft = BtmRight = Color.white;
            }
        }

        [SerializeField]
        private Gradient[] m_ColorList;
        [SerializeField]
        private TextMeshProUGUI[] m_TextList;

        private VertexGradient m_GradientWork = new VertexGradient();
        private int m_Index = -1;

        public int GetColorListCount() {
            if (m_ColorList == null) {
                return 0;
            }
            return m_ColorList.Length;
        }
        public int GetIndex() {
            return m_Index;
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
            switch (col.ColorMode) {
            case ColorMode.Single:
                m_GradientWork.topLeft = m_GradientWork.topRight = m_GradientWork.bottomLeft = m_GradientWork.bottomRight = col.TopLeft;
                break;
            case ColorMode.HorizontalGradient:
                m_GradientWork.topLeft = m_GradientWork.bottomLeft = col.TopLeft;
                m_GradientWork.topRight = m_GradientWork.bottomRight = col.TopRight;
                break;
            case ColorMode.VerticalGradient:
                m_GradientWork.topLeft = m_GradientWork.topRight = col.TopLeft;
                m_GradientWork.bottomLeft = m_GradientWork.bottomRight = col.BtmLeft;
                break;
            case ColorMode.FourCornersGradient:
                m_GradientWork.topLeft = col.TopLeft;
                m_GradientWork.topRight = col.TopRight;
                m_GradientWork.bottomLeft = col.BtmLeft;
                m_GradientWork.bottomRight = col.BtmRight;
                break;
            }
            foreach (var text in m_TextList) {
                if (text == null) {
                    continue;
                }
                text.enableVertexGradient = true;
                text.colorGradient = m_GradientWork;
            }
            m_Index = index;
        }
    }
}
