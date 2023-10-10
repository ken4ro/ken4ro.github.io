using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace UI {
    /// <summary>
    /// AtlasImageのSprite切り替えリストのルート
    /// 複数のパーツで同じ並びの切り替えリストが存在して
    /// 一括で同じインデックスで切り替えたい場合に使用
    /// </summary>
    public class UIAtlasImageSpriteListRoot : MonoBehaviour {
        [SerializeField]
        private UIAtlasImageSpriteList[] m_SpriteLists;

        /// <summary>
        /// 全部まとめてCreateSpriteList
        /// </summary>
        public void CreateSpriteList() {
            foreach (var list in m_SpriteLists) {
                list.CreateSpriteList();
            }
        }
        /// <summary>
        /// 全部まとめてDestroySpriteList
        /// </summary>
        public void DestroySpriteList() {
            foreach (var list in m_SpriteLists) {
                list.DestroySpriteList();
            }
        }

        /// <summary>
        /// リスト全部のSprite切り替え
        /// </summary>
        /// <param name="index"></param>
        public void Apply(int index) {
            foreach (var list in m_SpriteLists) {
                list.Apply(index);
            }
        }
    }
}
