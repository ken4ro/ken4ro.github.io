using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.U2D;

namespace UI {

    /// <summary>
    /// AtlasImageのSprite切り替えリスト
    /// SpriteAtlas内のSpriteの切り替え候補をリストアップする
    /// カラーも指定可能
    /// </summary>
    [RequireComponent(typeof(AtlasImage))]
    [ExecuteAlways] // Animation編集中も反映するため
    [DisallowMultipleComponent]
    public class UIAtlasImageSpriteList : MonoBehaviour {
        private AtlasImage m_AtlasImage;
        private AtlasSpriteList m_SpriteList;

        [System.Serializable]
        public class SpriteData {
            public string SpriteName;
            public Color Color = Color.white;   // デフォルト白にする
        }

        public List<SpriteData> List;

        /// <summary>
        /// AnimationからSprite変更する用
        /// </summary>
        public int Index = -1;

        private int m_Index = 1;    // 内部保存用
        /// <summary>
        /// 設定済みのIndexを返す
        /// </summary>
        public int GetIndex() {
            return m_Index;
        }

        private void OnDestroy() {
            // 念のため
            DestroySpriteList();
        }

        /// <summary>
        /// Spriteの適用
        /// </summary>
        public void Apply(int index) {
            Apply(index, m_AtlasImage);
        }
        public void Apply(int index, AtlasImage atlasImage) {
            if (atlasImage == null) {
                return;
            }
            if (index >= List.Count | index >= m_SpriteList.Count) {
                return;
            }
            var data = List[index];
            atlasImage.sprite = m_SpriteList[index];
            atlasImage.color = data.Color;
            m_Index = index;
        }
#if UNITY_EDITOR
        /// <summary>
        /// Editorでの確認用
        /// </summary>
        public void ApplyEditor(int index) {
            if (m_AtlasImage == null | m_SpriteList == null) {
                CreateSpriteList();
            }
            Apply(index);
        }
#endif

        /// <summary>
        /// Spriteのリストアップ
        /// 使用前に必ず呼び出し
        /// </summary>
        public void CreateSpriteList() {
            m_AtlasImage = GetComponent<AtlasImage>();
            if (m_AtlasImage == null) {
                return;
            }
            if (m_AtlasImage.atlas == null) {
                return;
            }
            m_SpriteList = new AtlasSpriteList(List.Count);
            foreach (var data in List) {
                m_SpriteList.Add(m_AtlasImage.atlas, data.SpriteName);
            }
        }
        /// <summary>
        /// Spriteの後片付け
        /// prefabを維持しつつSpriteAtlasだけ削除するような場合に呼び出し
        /// </summary>
        public void DestroySpriteList() {
            if (m_SpriteList == null) {
                return;
            }
            m_SpriteList.Destroy();
        }

        /// <summary>
        /// Animationで変更されたときのコールバック
        /// </summary>
        private void OnDidApplyAnimationProperties() {
            if (Index < 0) {
                return;
            }
#if UNITY_EDITOR
            // Animation編集中とか
            if (!UnityEditor.EditorApplication.isPlaying) {
                ApplyEditor(Index);
                return;
            }
#endif
            Apply(Index);
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
