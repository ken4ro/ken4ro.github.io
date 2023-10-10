using UnityEngine;

using TMPro;


public class UserMessage : MonoBehaviour
{

    // ユーザーメッセージに属するコンポーネント
    [SerializeField]
    private TextMeshProUGUI _userMessageTextPro = null;

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
    }

    /// <summary>
    /// 有効にする
    /// </summary>
    public void Enable() { }

    /// <summary>
    /// 無効にする
    /// </summary>
    public void Disable() { }

    /// <summary>
    /// ユーザーメッセージのテキストをセット
    /// </summary>
    /// <param name="text"></param>
    /// <param name="isAnim"></param>
    /// <returns></returns>
    public void SetUserText(string text) => _userMessageTextPro.text = text;

    /// <summary>
    /// ユーザーメッセージのテキストを取得
    /// </summary>
    /// <returns></returns>
    public string GetUserText() => _userMessageTextPro.text;
}
