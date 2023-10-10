using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.UI;
using UnityEngine.U2D;
using UnityEditorInternal;

namespace UI {
    [CustomEditor(typeof(UIAtlasImageSpriteListRoot), true)]
    [CanEditMultipleObjects]
    public class UIAtlasImageSpriteListRootEditor : Editor {
        private string m_TestIndex;

        private void OnEnable() {
            m_TestIndex = "0";
        }

        public override void OnInspectorGUI() {
            base.OnInspectorGUI();

            var str = EditorGUILayout.TextField("Test Index", m_TestIndex);
            if (int.TryParse(str, out int index)) {
                m_TestIndex = str;
            }
            if (GUILayout.Button("確認")) {
                var root = target as UIAtlasImageSpriteListRoot;
                var idx = int.Parse(m_TestIndex);
                root.CreateSpriteList();
                root.Apply(idx);
            }
        }
    }
}
