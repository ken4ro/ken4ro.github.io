using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEditor;
using UnityEngine.U2D;

namespace UI {
    /// <summary>
    /// UIAtlasImageSpriteListの一括追加ウインドウ
    /// </summary>
    public class UIAtlasImageSpriteListAddWindow : EditorWindow {
        private UIAtlasImageSpriteList m_TargetList;
        private SpriteAtlas m_Atlas;
        private string m_SearchText;

        private class SpriteName {
            public string Name;
            public bool IsAdd;
        }
        private List<SpriteName> m_NameList;
        private List<SpriteName> m_AddNameList;

        private Vector2 m_ScrollPos;

        /// <summary>
        /// コンテキストメニューから呼び出し
        /// </summary>
        [MenuItem("CONTEXT/UIAtlasImageSpriteList/一括追加")]
        private static void Open(MenuCommand menuCommand) {
            var window = CreateInstance<UIAtlasImageSpriteListAddWindow>();
            window.Init(menuCommand);
            window.ShowModal();
        }

        /// <summary>
        /// メニュー有効判定
        /// SpriteAtlasがなければどうしようもない
        /// </summary>
        [MenuItem("CONTEXT/UIAtlasImageSpriteList/一括追加", validate = true)]
        private static bool IsValid(MenuCommand menuCommand) {
            var splist = menuCommand.context as UIAtlasImageSpriteList;
            var image = splist.GetComponent<AtlasImage>();
            return image.atlas != null;
        }

        private void Init(MenuCommand menuCommand) {
            m_TargetList = menuCommand.context as UIAtlasImageSpriteList;
            var image = m_TargetList.GetComponent<AtlasImage>();
            m_Atlas = image.atlas;
            var names = AtlasImageEditor.GetAtlasSpriteNames(m_Atlas);

            m_NameList = new List<SpriteName>(names.Length);
            foreach (var name in names) {
                var data = new SpriteName() {
                    Name = name,
                    IsAdd = false,
                };
                m_NameList.Add(data);
            }
            // 最初は無条件
            m_AddNameList = m_NameList;
            m_SearchText = "";
        }

        private void OnGUI() {
            if (GUILayout.Button("すべてON")) {
                foreach (var name in m_NameList) {
                    name.IsAdd = true;
                }
            }
            if (GUILayout.Button("すべてOFF")) {
                foreach (var name in m_NameList) {
                    name.IsAdd = false;
                }
            }
            if (GUILayout.Button("選択中をON")) {
                foreach (var name in m_AddNameList) {
                    name.IsAdd = true;
                }
            }
            if (GUILayout.Button("選択中をOFF")) {
                foreach (var name in m_AddNameList) {
                    name.IsAdd = false;
                }
            }

            int count = GetAddCount(m_NameList);
            EditorGUILayout.LabelField(string.Format("チェック中:{0}/{1}", count, m_NameList.Count));
            GUI.enabled = count > 0;
            if (GUILayout.Button("リストに追加する")) {
                Execute();
                Close();
            }
            GUI.enabled = true;
            EditorGUILayout.Separator();
            EditorGUILayout.LabelField(string.Format("絞り込み（ワイルドカード*使用可能）{0}/{1}", m_AddNameList.Count, m_NameList.Count));
            EditorGUI.BeginChangeCheck();
            m_SearchText = EditorGUILayout.TextField(m_SearchText);
            if (EditorGUI.EndChangeCheck()) {
                m_AddNameList = SearchName(m_NameList, m_SearchText);
            }

            m_ScrollPos = EditorGUILayout.BeginScrollView(m_ScrollPos);
            foreach (var name in m_AddNameList) {
                name.IsAdd = GUILayout.Toggle(name.IsAdd, name.Name);
            }
            EditorGUILayout.EndScrollView();
        }

        /// <summary>
        /// 絞り込み
        /// </summary>
        private List<SpriteName> SearchName(List<SpriteName> nameList, string searchText) {
            // 無条件
            if (string.IsNullOrEmpty(searchText)) {
                return nameList;
            }

            searchText = searchText.Replace("*", ".*");
            searchText = "^" + searchText + "$";
            List<SpriteName> list = new List<SpriteName>(nameList.Count);
            foreach (var name in nameList) {
                if (Regex.IsMatch(name.Name, searchText)) {
                    list.Add(name);
                }
            }
            return list;
        }
        /// <summary>
        /// 追加チェック数を求める
        /// </summary>
        private int GetAddCount(List<SpriteName> nameList) {
            int count = 0;
            foreach (var name in nameList) {
                if (name.IsAdd) {
                    ++count;
                }
            }
            return count;
        }
        /// <summary>
        /// リスト追加の実行
        /// </summary>
        private void Execute() {
            foreach (var name in m_NameList) {
                if (!name.IsAdd) {
                    continue;
                }
                var data = new UIAtlasImageSpriteList.SpriteData() {
                    SpriteName = name.Name,
                };
                m_TargetList.List.Add(data);
            }
            EditorUtility.SetDirty(m_TargetList.gameObject);
        }
    }
}
