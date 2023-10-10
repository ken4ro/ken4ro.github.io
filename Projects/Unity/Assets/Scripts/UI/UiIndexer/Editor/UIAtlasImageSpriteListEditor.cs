using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.U2D;
using UnityEditorInternal;

namespace UI {
    [CustomEditor(typeof(UIAtlasImageSpriteList), true)]
    [CanEditMultipleObjects]
    public class UIAtlasImageSpriteListEditor : Editor {
        private const float THUMBNAIL_HEIGHT = 64f;

        private SerializedProperty m_List;
        private ReorderableList m_ReorderableList;
        private AtlasImage m_AtlasImage;
        private SpriteAtlas m_SpriteAtlas;
        private string[] m_AtlasSpriteNames;
        private bool m_IsEnumSelect;
        private int m_SelectIndex;

        private void OnEnable() {
            m_List = serializedObject.FindProperty("List");
            m_ReorderableList = new ReorderableList(serializedObject, m_List);
            m_ReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "List");
            m_ReorderableList.drawElementCallback = OnDrawElement;
            m_ReorderableList.onAddCallback = OnAddButton;
            m_ReorderableList.onRemoveCallback = OnRemoveButton;
            //m_ReorderableList.elementHeightCallback = index => EditorGUI.GetPropertyHeight(m_List.GetArrayElementAtIndex(index));
            m_ReorderableList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 4f + THUMBNAIL_HEIGHT;

            var ailist = target as UIAtlasImageSpriteList;
            m_AtlasImage = ailist.GetComponent<AtlasImage>();
            m_IsEnumSelect = IsEnableEnumSelect();
            m_SelectIndex = -1;

            if (m_IsEnumSelect) {
                m_AtlasSpriteNames = AtlasImageEditor.GetAtlasSpriteNames(m_AtlasImage.atlas);
            }
        }
        /// <summary>
        /// SpriteAtlasが入っていれば列挙選択可能
        /// </summary>
        private bool IsEnableEnumSelect() {
            return m_AtlasImage.atlas != null;
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();

            bool isEnableEnum = IsEnableEnumSelect();
            GUI.enabled = isEnableEnum;
            m_IsEnumSelect = EditorGUILayout.Toggle("列挙で選択する", m_IsEnumSelect);
            GUI.enabled = true;
            if (!isEnableEnum) {
                EditorGUILayout.HelpBox("SpriteAtlas設定で列挙選択が可能になります", MessageType.Warning);
            }
            // Atlas入ったら名前リスト取得
            if (isEnableEnum) {
                if (m_AtlasSpriteNames == null | m_SpriteAtlas != m_AtlasImage.atlas) {
                    m_AtlasSpriteNames = AtlasImageEditor.GetAtlasSpriteNames(m_AtlasImage.atlas);
                    m_SpriteAtlas = m_AtlasImage.atlas;
                }
            }
            else {
                m_AtlasSpriteNames = null;
            }

            m_ReorderableList.DoLayoutList();

            serializedObject.ApplyModifiedProperties();

            //base.OnInspectorGUI();
        }

        /// <summary>
        /// リスト1個描画
        /// </summary>
        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            // 選択インデックス→削除に使う
            if (isActive) {
                m_SelectIndex = index;
            }

            rect.xMin += 10;    // めり込むので右にずらす

            //var elementProperty = m_List.GetArrayElementAtIndex(index);
            //EditorGUI.PropertyField(rect, elementProperty, true);

            rect.height = EditorGUIUtility.singleLineHeight;

            var ailist = target as UIAtlasImageSpriteList;
            var data = ailist.List[index];

            EditorGUI.LabelField(rect, "Index:" + index);

            rect.y += EditorGUIUtility.singleLineHeight;
            var col = GUI.color;
            var enable = IsEnableEnumSelect();
            int nameIdx = -1;
            if (m_AtlasSpriteNames != null) {
                // 見つからなければ赤表示
                nameIdx = FindSpriteIndex(data.SpriteName);
                if (nameIdx < 0) {
                    GUI.color = Color.red;
                }
            }
            if (m_IsEnumSelect & enable) {
                // 列挙選択
                EditorGUI.BeginChangeCheck();
                nameIdx = EditorGUI.Popup(rect, "SpriteName", nameIdx, m_AtlasSpriteNames);
                if (EditorGUI.EndChangeCheck()) {
                    data.SpriteName = m_AtlasSpriteNames[nameIdx];
                    EditorUtility.SetDirty(target);
                }
            }
            else {
                EditorGUI.BeginChangeCheck();
                data.SpriteName = EditorGUI.TextField(rect, "SpriteName", data.SpriteName);
                if (EditorGUI.EndChangeCheck()) {
                    EditorUtility.SetDirty(target);
                }
            }
            GUI.color = col;
            rect.y += EditorGUIUtility.singleLineHeight;

            EditorGUI.BeginChangeCheck();
            data.Color = EditorGUI.ColorField(rect, "Color", data.Color);
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }
            rect.y += EditorGUIUtility.singleLineHeight;

            // 有効ならサムネイル表示
            if (enable & nameIdx >= 0) {
                var newSprite = m_AtlasImage.atlas.GetSprite(data.SpriteName);
                var rect2 = rect;
                rect2.height = THUMBNAIL_HEIGHT;
                EditorGUI.LabelField(rect2, new GUIContent(newSprite.texture));
            }
            rect.y += THUMBNAIL_HEIGHT;

            // アトラス有効で、スプライト名が有効ならテスト可能
            GUI.enabled = enable & nameIdx >= 0;
            if (GUI.Button(rect, "確認")) {
                ailist.ApplyEditor(index);
            }
            GUI.enabled = true;
        }
        /// <summary>
        /// ＋ボタン
        /// </summary>
        private void OnAddButton(ReorderableList list) {
            var ailist = target as UIAtlasImageSpriteList;
            var data = new UIAtlasImageSpriteList.SpriteData();
            ailist.List.Add(data);

            EditorUtility.SetDirty(target);
        }
        /// <summary>
        /// －ボタン
        /// </summary>
        private void OnRemoveButton(ReorderableList list) {
            var ailist = target as UIAtlasImageSpriteList;
            if (ailist.List.Count > 0) {
                // 未選択なら末尾
                int del = ailist.List.Count - 1;
                if (m_SelectIndex >= 0 & m_SelectIndex < ailist.List.Count) {
                    del = m_SelectIndex;
                }
                ailist.List.RemoveAt(del);
                EditorUtility.SetDirty(target);
            }
        }

        private int FindSpriteIndex(string name) {
            if (m_AtlasSpriteNames == null) {
                return -1;
            }
            for (int i = 0; i < m_AtlasSpriteNames.Length; ++i) {
                if (m_AtlasSpriteNames[i].Equals(name)) {
                    return i;
                }
            }
            return -1;
        }
    }
}
