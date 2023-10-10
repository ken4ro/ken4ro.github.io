using System.Linq;
using UnityEngine;
using UnityEngine.UI;
using UnityEditor;
using System.Collections;
using System.Collections.Generic;
using System.IO;

public class UISystemEditorUtility : EditorWindow {

    // TODO : とりあえずここに持ってきたが他でも使いそうならEditor外に移動
    static System.Text.StringBuilder mSbWork = new System.Text.StringBuilder(256);
    /// <summary>
    /// 親Transformから子供Transformまでの相対パスを取得する
    /// parent.find()で子供が見つけられるパス
    /// childがparentの下になければ失敗する
    /// 失敗した場合は空文字を返す
    /// </summary>
    /// <param name="parent">基準となる親Transform</param>
    /// <param name="child">目標となる子Transform</param>
    /// <returns>相対パスの文字列 見つからなければ空文字</returns>
    public static string GetRelativeTransformPath(Transform parent, Transform child) {
        bool isFind = false;
        mSbWork.Clear();
        Transform part = child;
        if (part != null) {
            //相対パスを取得
            Transform tmp = part;
            while (tmp != null) {
                mSbWork.Insert(0, tmp.name);
                tmp = tmp.parent;
                //ルートを見つけたなら抜ける
                if (tmp == parent) {
                    isFind = true;
                    break;
                }
                else {
                    mSbWork.Insert(0, "/");
                }
            }
        }
        if (isFind) {
            return mSbWork.ToString();
        }
        else {
            return "";
        }
    }

    /// <summary>
    /// 選択した２つのオブジェクトの相対パスをクリップボードにコピーする
    /// ２つのオブジェクトは親子関係でなければならない
    /// </summary>
    [MenuItem("GameObject/相対パスをクリップボードにコピー", false, 35)]
    public static void GetHierarchyRelativePath() {
        var gameObjects = Selection.gameObjects;
        if (gameObjects.Length == 2) {
            string path = "";
            var trans1 = gameObjects[0].transform;
            var trans2 = gameObjects[1].transform;
            path = GetRelativeTransformPath(trans1, trans2);
            if (path.Length <= 0) {
                path = GetRelativeTransformPath(trans2, trans1);
            }

            if (path.Length >= 0) {
                GUIUtility.systemCopyBuffer = path;
            }
        }
    }

    /// <summary>
    /// 選択したオブジェクト以下のRaycastTargetをOFFにする
    /// 対象はImage/RawImage/AtlasImage/TextMeshPro
    /// </summary>
    [MenuItem("Assets/RaycastTargetの一括OFF")]
    [MenuItem("GameObject/RaycastTargetの一括OFF")]
    public static void ClearRaycastTarget() {
        var gameObjects = Selection.gameObjects;
        foreach (var obj in gameObjects) {
            var path = UnityEditor.AssetDatabase.GetAssetPath(obj);
            // prefabでないなら変更するだけ
            if (string.IsNullOrEmpty(path)) {
                SetRaycastTargetInner(obj, false);
                Debug.Log(string.Format("RaycastTargetの一括OFF GameObject更新:{0}", obj.name));
            }
            // prefabなら更新する
            else {
                // 内部的なシーンにPrefabをロードする
                var contentsRoot = PrefabUtility.LoadPrefabContents(path);

                SetRaycastTargetInner(contentsRoot, false);

                // 保存する
                PrefabUtility.SaveAsPrefabAsset(contentsRoot, path);
                PrefabUtility.UnloadPrefabContents(contentsRoot);
                Debug.Log(string.Format("RaycastTargetの一括OFF prefab更新:{0}", path));
            }
        }
    }
    /// <summary>
    /// 指定のオブジェクト以下のRaycastTargetの設定
    /// 対象はImage/RawImage/AtlasImage/TextMeshPro
    /// </summary>
    private static void SetRaycastTargetInner(GameObject go, bool on) {
        // AtlasImageも含まれるはず
        var images = go.GetComponentsInChildren<Image>(true);
        foreach (var image in images) {
            image.raycastTarget = on;
        }
        var rawimages = go.GetComponentsInChildren<RawImage>(true);
        foreach (var rawimage in rawimages) {
            rawimage.raycastTarget = on;
        }
        var textMeshPros = go.GetComponentsInChildren<TMPro.TextMeshProUGUI>(true);
        foreach (var text in textMeshPros) {
            text.raycastTarget = on;
        }
    }
}
