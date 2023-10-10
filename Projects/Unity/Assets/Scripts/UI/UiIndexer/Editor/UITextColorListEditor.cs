using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.U2D;
using UnityEditorInternal;

namespace UI {
    [CustomEditor(typeof(UITextColorList), true)]
    [CanEditMultipleObjects]
    public class UITextColorListEditor : Editor {
        private SerializedProperty m_ColorList;
        private ReorderableList m_ReorderableList;
        private int m_SelectIndex;

        private string m_TestIndex;

        private void OnEnable() {
            m_ColorList = serializedObject.FindProperty("m_ColorList");
            m_ReorderableList = new ReorderableList(serializedObject, m_ColorList);
            m_ReorderableList.drawHeaderCallback = rect => EditorGUI.LabelField(rect, "ColorList");
            m_ReorderableList.drawElementCallback = OnDrawElement;
            m_ReorderableList.onAddCallback = OnAddButton;
            m_ReorderableList.onRemoveCallback = OnRemoveButton;
            m_ReorderableList.elementHeightCallback = index => EditorGUIUtility.singleLineHeight * 4f;

            m_SelectIndex = -1;
            m_TestIndex = "0";
        }

        public override void OnInspectorGUI() {
            serializedObject.Update();
            m_ReorderableList.DoLayoutList();

            var textlist = serializedObject.FindProperty("m_TextList");
            EditorGUILayout.PropertyField(textlist);

            serializedObject.ApplyModifiedProperties();

            var str = EditorGUILayout.TextField("Test Index", m_TestIndex);
            if (int.TryParse(str, out int index)) {
                m_TestIndex = str;
            }
            if (GUILayout.Button("確認")) {
                var root = target as UITextColorList;
                var idx = int.Parse(m_TestIndex);
                root.Apply(idx);
                EditorUtility.SetDirty(target);
            }
        }

        /// <summary>
        /// リスト1個描画
        /// </summary>
        private void OnDrawElement(Rect rect, int index, bool isActive, bool isFocused) {
            // 選択インデックス→削除に使う
            if (isActive) {
                m_SelectIndex = index;
            }

            //rect.xMin += 10;    // めり込むので右にずらす

            rect.height = EditorGUIUtility.singleLineHeight;

            EditorGUI.LabelField(rect, "Index:" + index);
            rect.y += EditorGUIUtility.singleLineHeight;

            var data = m_ColorList.GetArrayElementAtIndex(index);
            var mode = data.FindPropertyRelative("ColorMode");
            EditorGUI.PropertyField(rect, mode, true);
            rect.y += EditorGUIUtility.singleLineHeight;

            var colorMode = (TMPro.ColorMode)mode.enumValueIndex;
            var rect2 = rect;
            rect2.width = 100f;
            float xoffset = 120f;
            EditorGUI.BeginChangeCheck();
            switch (colorMode) {
            case TMPro.ColorMode.Single: {
                    var color0 = data.FindPropertyRelative("TopLeft");
                    color0.colorValue = EditorGUI.ColorField(rect2, color0.colorValue);
                }
                break;
            case TMPro.ColorMode.HorizontalGradient: {
                    var color0 = data.FindPropertyRelative("TopLeft");
                    color0.colorValue = EditorGUI.ColorField(rect2, color0.colorValue);
                    rect2.x += xoffset;
                    var color1 = data.FindPropertyRelative("TopRight");
                    color1.colorValue = EditorGUI.ColorField(rect2, color1.colorValue);
                }
                break;
            case TMPro.ColorMode.VerticalGradient: {
                    var color0 = data.FindPropertyRelative("TopLeft");
                    color0.colorValue = EditorGUI.ColorField(rect2, color0.colorValue);
                    rect2.y += EditorGUIUtility.singleLineHeight;
                    var color1 = data.FindPropertyRelative("BtmLeft");
                    color1.colorValue = EditorGUI.ColorField(rect2, color1.colorValue);
                }
                break;
            case TMPro.ColorMode.FourCornersGradient: {
                    var color0 = data.FindPropertyRelative("TopLeft");
                    color0.colorValue = EditorGUI.ColorField(rect2, color0.colorValue);
                    rect2.x += xoffset;
                    var color1 = data.FindPropertyRelative("TopRight");
                    color1.colorValue = EditorGUI.ColorField(rect2, color1.colorValue);
                    rect2.y += EditorGUIUtility.singleLineHeight;
                    rect2.x -= xoffset;
                    var color2 = data.FindPropertyRelative("BtmLeft");
                    color2.colorValue = EditorGUI.ColorField(rect2, color2.colorValue);
                    rect2.x += xoffset;
                    var color3 = data.FindPropertyRelative("BtmRight");
                    color3.colorValue = EditorGUI.ColorField(rect2, color3.colorValue);
                }
                break;
            }
            if (EditorGUI.EndChangeCheck()) {
                EditorUtility.SetDirty(target);
            }
        }
        /// <summary>
        /// ＋ボタン
        /// </summary>
        private void OnAddButton(ReorderableList list) {
            var size = m_ColorList.arraySize;
            m_ColorList.InsertArrayElementAtIndex(m_ColorList.arraySize);
            // 1個目なら白設定（2個目以降は末尾のコピー）
            // …InsertArrayElementAtIndexでコンストラクタは呼んでくれないらしい（全部黒/アルファ0になる）
            if (size <= 0) {
                var data = m_ColorList.GetArrayElementAtIndex(0);
                var color0 = data.FindPropertyRelative("TopLeft");
                var color1 = data.FindPropertyRelative("TopRight");
                var color2 = data.FindPropertyRelative("BtmLeft");
                var color3 = data.FindPropertyRelative("BtmRight");
                color0.colorValue = color1.colorValue = color2.colorValue = color3.colorValue = Color.white;
            }
            EditorUtility.SetDirty(target);
        }
        /// <summary>
        /// －ボタン
        /// </summary>
        private void OnRemoveButton(ReorderableList list) {
            var size = m_ColorList.arraySize;
            if (size > 0) {
                // 未選択なら末尾
                int del = size - 1;
                if (m_SelectIndex >= 0 & m_SelectIndex < size) {
                    del = m_SelectIndex;
                }
                m_ColorList.DeleteArrayElementAtIndex(del);
                EditorUtility.SetDirty(target);
            }
        }
    }
}
