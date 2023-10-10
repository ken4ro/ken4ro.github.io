using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Video;
using UniRx;
using TMPro;
using DG.Tweening;
using Coffee.UIExtensions;
using Cysharp.Threading.Tasks;
using static BotManager;
using static SignageSettings;
using System.Linq;
using UI;

public class ScenarioWindow : MonoBehaviour
{
    private enum eSlectType
    { 
        ICON,
        CHAR,
    }

    /// <summary>
    /// 選択肢ボタン押下時コールバック
    /// </summary>
    public Action<string> OnClickSelectWord = null;

    /// <summary>
    /// テキストの文字送りディレイタイム
    /// </summary>
    public int TextAnimationDelayTimeMs { get; set; } = 60;

    /// <summary>
    /// 選択画面
    /// </summary>
    [SerializeField] Animator selectWindow = null;

    /// <summary>
    /// 詳細画面
    /// </summary>
    [SerializeField] Animator infoWindow = null;

    /// <summary>
    /// ウィンドウ画像
    /// </summary>
    [SerializeField] Animator questionWindow = null;

    /// <summary>
    /// テキスト
    /// </summary>
    [SerializeField] TextMeshProUGUI quesstionText = null;

    /// <summary>
    /// テキスト
    /// </summary>
    [SerializeField] TextMeshProUGUI messageText = null;

    /// <summary>
    /// 画像
    /// </summary>
    [SerializeField] RawImage messageImage = null;

    /// <summary>
    /// 動画
    /// </summary>
    [SerializeField] VideoPlayer messageMovie = null;

    /// <summary>
    /// ドットリスト
    /// </summary>
    [SerializeField] UIAtlasImageSpriteListRoot[] dotList = null;

    /// <summary>
    /// 選択肢オブジェクト
    /// </summary>
    [SerializeField] GameObject _iconSelect = null;
    [SerializeField] GameObject _charSelect = null;

    /// <summary>
    /// 左右ボタン
    /// </summary>
    [SerializeField] ButtonWrapper _leftButton = null;
    [SerializeField] ButtonWrapper _rightButton = null;

    /// <summary>
    /// 画像クリック
    /// </summary>
    public Action OnImageClick = null;

    private static readonly uint MaxEnableSelectButtonNum = 3;
    private static readonly Color _enableColor = new Color(1.0f, 1.0f, 1.0f, 1.0f);
    private static readonly Color _disableColor = new Color(1.0f, 1.0f, 1.0f, 0.0f);

    private ButtonWrapper[] _iconSelectButtons;
    private ButtonWrapper[] _charSelectButtons;

    private RectTransform _rectTransform = null;
    private Vector3 _defaultPos = Vector3.zero;
    private float _selectWindowBottomPos = 0.0f;

    private ReactiveProperty<int> _pageIndex = new ReactiveProperty<int>(-1);
    private Dictionary<int, List<BotResponseSelect>> _selectObjectsInPage = new Dictionary<int, List<BotResponseSelect>>();
    private Language _viewLanguage = Language.Japanese;
    private eSlectType _nowType;

    void Awake()
    {
        _rectTransform = GetComponent<RectTransform>();
        _defaultPos = _rectTransform.localPosition;
        _pageIndex.Subscribe(x => PageChanged(x)).AddTo(gameObject);

        foreach (var dot in dotList)
        {
            dot.CreateSpriteList();
        }

        _iconSelectButtons = _iconSelect.GetComponentsInChildren<ButtonWrapper>(true);
        foreach (var button in _iconSelectButtons)
        {
            button.Initialise();
        }
        _charSelectButtons = _charSelect.GetComponentsInChildren<ButtonWrapper>(true);
        foreach (var button in _charSelectButtons)
        {
            button.Initialise();
        }

        // 次ページ
        _rightButton.Initialise();
        _rightButton.onClick.AddListener(NextPage);

        // 前ページ
        _leftButton.Initialise();
        _leftButton.onClick.AddListener(PrevPage);
    }

    /// <summary>
    /// キャラクターメッセージの表示を有効にする
    /// </summary>
    public void Enable()
    {
    }

    /// <summary>
    /// キャラクターメッセージの表示を無効にする
    /// </summary>
    public void Disable()
    {
        RemoveButtonEvent();
    }

    /// <summary>
    /// フォントサイズセット
    /// </summary>
    /// <param name="size"></param>
    public void SetFontSize(int size)
    {
        messageText.fontSize = size;
    }

    /// <summary>
    /// キャラクターメッセージをセット
    /// </summary>
    /// <param name="message"></param>
    /// <param name="isAnim"></param>
    /// <param name="imageType"></param>
    /// <param name="imageFileName"></param>
    /// <param name="selectTexts"></param>
    /// <param name="selectImageFileName"></param>
    public async UniTask SetCharacterMessage(string message, bool isAnim, ImageAccessTypes imageType, string imageFileName, bool isSelect)
    {
        // 文字表示速度リセット
        TextAnimationDelayTimeMs = GlobalState.Instance.UserSettings.UI.TextSpeed;
        // 表示リセット
        ResetCharacterMessage();
        // 表示を有効化
        Enable();

        questionWindow.Play(isSelect ? "concierge_mess_in" : "concierge_mess_out");
        infoWindow.Play(isSelect ? "main_palet_out" : "main_palet_in");

        if (isSelect)
        {
            questionWindow.gameObject.SetActive(true);
        }
        else
        {
            infoWindow.gameObject.SetActive(true);
        }

        // テキストセット
        await SetText(message, isAnim, isSelect);
        // 画像セット
        await SetImage(imageType, imageFileName);
    }

    /// <summary>
    /// キャラクターメッセージ動画をセット
    /// </summary>
    /// <param name="movieFilePath"></param>
    public void SetCharacterMessageMovie(string movieFilePath)
    {
        SetMovie(movieFilePath);
    }

    /// <summary>
    /// 選択肢構造のセット
    /// </summary>
    /// <param name="selectObjects"></param>
    /// <param name="lang"></param>
    public void SetSelectObjects(List<BotResponseSelect> selectObjects, string type, Language lang)
    {
        selectWindow.gameObject.SetActive(true);
        selectWindow.Play("main_palet_in");

        InitializeSelectButton(selectObjects, type, lang);
    }

    /// <summary>
    /// キャラクターメッセージをリセット
    /// </summary>
    public void ResetCharacterMessage()
    {
        // コンテンツレイアウト設定リセット
        //GetComponent<VerticalLayoutGroup>().childControlHeight = true;
        //GetComponent<ContentSizeFitter>().verticalFit = ContentSizeFitter.FitMode.PreferredSize;
        // テキストリセット
        ResetText();
        // 画像リセット
        ResetImage();
        // 動画リセット
        ResetMovie();
        // 表示位置リセット
        ResetPosition();
        // 選択肢リセット
        ResetSelect();
        // 表示を無効にする
        Disable();
    }

    /// <summary>
    /// 選択肢メッセージをリセット
    /// </summary>
    public void ResetSelectMessage()
    {
        ResetSelect();
    }

    /// <summary>
    /// 左から右へ表示する(デフォルト)
    /// </summary>
    public void LeftToRight()
    {
        messageText.isRightToLeftText = false;
        quesstionText.isRightToLeftText = false;
    }

    /// <summary>
    /// 右から左へ表示する(主にアラビア語用)
    /// </summary>
    public void RightToLeft()
    {
        messageText.isRightToLeftText = true;
        quesstionText.isRightToLeftText = true;
    }

    private async UniTask SetText(string text, bool isAnim, bool isSelect)
    {
        if (isAnim)
        {
            var target = (isSelect ? quesstionText : messageText);
            await StartTextGradationAnimation(target, text);
            //await StartTextAnimation(messageText, text);
        }
        else
        {
            quesstionText.text = text;
        }
    }

    // グラデーション表示対応テキストアニメーション
    private async UniTask StartTextGradationAnimation(TextMeshProUGUI target, string text)
    {
        if (target == null) return;

        if (string.IsNullOrEmpty(text) || TextAnimationDelayTimeMs <= 0)
        {
            target.text = text;
            return;
        }

        var step = 5;
        var wait = TextAnimationDelayTimeMs / 5;

        var stepResolution = TextAnimationDelayTimeMs / step;
        DateTime startDt = DateTime.Now;
        while (true)
        {
            //経過時間から表示文字数を算出
            TimeSpan ts = DateTime.Now - startDt;
            var putCount = (int)ts.TotalMilliseconds / TextAnimationDelayTimeMs - step;

            if (putCount >= text.Length)
            {
                target.text = text;
                break;
            }

            string sb = "";
            if (putCount > 0)
                sb = "<alpha=#FF>" + text.Substring(0, putCount);

            //アルファ文字追記
            var rem = ((int)ts.TotalMilliseconds % TextAnimationDelayTimeMs) + stepResolution * (step - 1);
            rem = TextAnimationDelayTimeMs < rem ? TextAnimationDelayTimeMs : rem;

            for (int i = 0; i < step; i++)
            {

                if (rem > 0 && putCount + i >= 0 && putCount + i < text.Length)
                    sb += String.Format("<alpha=#{0}>{1}", (rem * 255 / TextAnimationDelayTimeMs).ToString("X2"), text.Substring(putCount + i, 1));

                rem -= stepResolution;
            }

            target.text = sb.ToString();

            await UniTask.Delay(wait);
        }
    }

    string test_load_path;
    private async UniTask SetImage(ImageAccessTypes type, string filePath)
    {
#if false

        var texture2D = await GraphicsHelper.LoadImage(type, filePath);
        if (texture2D == null)
            return;

#else
        var texture2D = Resources.Load(test_load_path) as Texture2D;
#endif

        messageImage.texture = texture2D;
        messageImage.gameObject.SetActive(true);

        // フェードインさせてみる
        messageImage.color = _disableColor;
        messageImage.DOFade(1.0f, 0.5f);
    }

    private void SetMovie(string filePath)
    {
        if (string.IsNullOrEmpty(filePath)) return;

        messageMovie.gameObject.SetActive(true);
        messageMovie.url = FileHelper.GetCurrentDirectory() + filePath;
        messageMovie.prepareCompleted += OnPrepareCompleted;
        messageMovie.Prepare();
    }

    private void OnPrepareCompleted(VideoPlayer vp)
    {
        Debug.Log("Message movie has prepared.");

        var rawImage = messageMovie.gameObject.GetComponent<RawImage>();
        rawImage.texture = messageMovie.texture;

        // RawImage だと Content Size Filter で動的にサイズ変更されないので暫定対処
        //var verticalLayoutGroup = GetComponent<VerticalLayoutGroup>();
        //verticalLayoutGroup.childControlHeight = false;
        //var contentSizeFilter = GetComponent<ContentSizeFitter>();
        //contentSizeFilter.verticalFit = ContentSizeFitter.FitMode.Unconstrained;
        var rect = GetComponent<RectTransform>();
        rect.sizeDelta = new Vector2(rect.sizeDelta.x, rect.sizeDelta.y + messageMovie.texture.height);
        messageMovie.gameObject.GetComponent<RectTransform>().sizeDelta = new Vector2(messageMovie.texture.width, messageMovie.texture.height);

        messageMovie.prepareCompleted -= OnPrepareCompleted;
        messageMovie.Play();
    }

    private void ResetPosition()
    {
        _rectTransform.localPosition = _defaultPos;
    }

    private void ResetText()
    {
        messageText.text = "";
    }

    private void ResetImage()
    {
        if (messageImage != null)
        {
            messageImage.color = _disableColor;
            messageImage.gameObject.SetActive(false);
            messageImage.texture = null;
        }
    }

    private void ResetMovie()
    {
        if (messageMovie != null)
        {
            messageMovie.gameObject.SetActive(false);
            if (messageMovie.isPlaying)
            {
                messageMovie.Stop();
                messageMovie.url = null;
            }
        }
    }

    private void ResetSelect()
    {
        selectWindow.Play("main_palet_out");
        infoWindow.Play("main_palet_out");

        _pageIndex.Value = -1;
        _selectObjectsInPage.Clear();
        _rightButton.gameObject.SetActive(false);
        _leftButton.gameObject.SetActive(false);
    }

    /// <summary>
    /// ボタン情報の初期化（構造対応)
    /// </summary>
    /// <param name="selectObjects"></param>
    /// <param name="lang">使用中の言語</param>
    private void InitializeSelectButton(List<BotResponseSelect> selectObjects, string type, Language lang)
    {
        var select_type = (type == "top" ? eSlectType.ICON : eSlectType.CHAR);

        // 選択肢リスト作成
        int select_max = (select_type == eSlectType.ICON ? 6 : 3);
        int no1;
        for (no1 = 0; no1 < selectObjects.Count; no1++)
        {
            var page = no1 / select_max;
            if (!_selectObjectsInPage.ContainsKey(page))
            {
                _selectObjectsInPage[page] = new List<BotResponseSelect>();
            }
            _selectObjectsInPage[page].Add(selectObjects[no1]);
        }

        int page_max = _selectObjectsInPage.Count;
        for (no1 = 0; no1 < dotList.Length; no1++)
        {
            bool disp = (page_max > 1 && no1 < page_max);
            dotList[no1].gameObject.SetActive(disp);
            if (disp)
            {
                dotList[no1].Apply(0);
            }
        }

        _pageIndex.Value = 0;
        _viewLanguage = lang;
        _nowType = select_type;

        if (_nowType == eSlectType.ICON)
        {
            _iconSelect.SetActive(true);
            _charSelect.SetActive(false);
        }
        else
        {
            _iconSelect.SetActive(false);
            _charSelect.SetActive(true);
        }

        var buttons = (_nowType == eSlectType.ICON ? _iconSelectButtons : _charSelectButtons);
        foreach (var button in buttons)
        {
            button.transform.parent.gameObject.SetActive(false);
        }

        // ボタン設定
        SetSelectObjectButton();
    }

    /// <summary>
    /// 選択肢をセット
    /// </summary>
    private void SetSelectObjectButton()
    {
        var buttons = (_nowType == eSlectType.ICON ? _iconSelectButtons : _charSelectButtons);
        foreach (var button in buttons)
        {
            button.gameObject.SetActive(false);
        }

        // ボタンテキストをセット
        for (var i = 0; i < _selectObjectsInPage[_pageIndex.Value].Count; i++)
        {
            int index = i;
            if (_pageIndex.Value == 0)
            {
                buttons[i].transform.parent.gameObject.SetActive(true);
            }
            buttons[i].gameObject.SetActive(true);

            var text = BotResponseSelect.GetTargetText(_selectObjectsInPage[_pageIndex.Value][i], _viewLanguage);
            buttons[i].ChangeSlice(i + _pageIndex.Value * buttons.Length);
            buttons[i].SetText(text);
            buttons[i].onClick.AddListener(() => SelectWord(index, text));
            buttons[i].SetEnable(true);

        }
    }

    private void ResetButton()
    {
        var buttons = (_nowType == eSlectType.ICON ? _iconSelectButtons : _charSelectButtons);
        for (var i = 0; i < _selectObjectsInPage[_pageIndex.Value].Count; i++)
        {
            buttons[i].SetText("");
            buttons[i].onClick.RemoveAllListeners();
            buttons[i].gameObject.SetActive(false);
        }
    }

    private void RemoveButtonEvent()
    {
        var buttons = (_nowType == eSlectType.ICON ? _iconSelectButtons : _charSelectButtons);
        foreach(var button in buttons)
        {
            button.SetEnable(false);
        }
        _leftButton.SetEnable(false);
        _rightButton.SetEnable(false);
    }

    private void NextPage()
    {
        // SE 再生
        AudioManager.Instance.PlaySE(AudioManager.SEType.NextPage).Forget();

        // ボタンリセット
        ResetButton();

        // ページ数インクリメント
        _pageIndex.Value++;

        // ボタン設定
        SetSelectObjectButton();
    }

    private void PrevPage()
    {
        // SE 再生
        AudioManager.Instance.PlaySE(AudioManager.SEType.NextPage).Forget();

        // ボタンリセット
        ResetButton();

        // ページ数デクリメント
        _pageIndex.Value--;

        // ボタン設定
        SetSelectObjectButton();
    }


    // ページ変化時コールバック (page: 0, 1, 2, ...)
    private void PageChanged(int page)
    {
        if (page == -1) return;

        var pageNum = _selectObjectsInPage.Keys.Count;
        if (pageNum == 1) return;

        foreach (var dot in dotList)
        {
            dot.Apply(0);
        }
        dotList[page].Apply(1);

        // 次ページ
        if (page + 1 < pageNum)
        {
            _rightButton.gameObject.SetActive(true);
            _rightButton.SetEnable(true);
        }
        else
        {
            _rightButton.gameObject.SetActive(false);
        }

        // 前ページ
        if (page == 0)
        {
            _leftButton.gameObject.SetActive(false);
        }
        else
        {
            _leftButton.gameObject.SetActive(true);
            _leftButton.SetEnable(true);
        }
    }

    private async void SelectWord(int index, string text)
    {
        if (_nowType == eSlectType.ICON)
        {
            test_load_path = string.Format("conttents_img_{0:000}", (index + 1) + _pageIndex.Value * 6);
        }

        // ボタン連打防止
        RemoveButtonEvent();

        // SE 再生
        AudioManager.Instance.PlaySE(AudioManager.SEType.SelectSentence).Forget();

        // アニメーション終了待機
        await UniTask.Delay(200);

        // サウンド的に少し間を空けておく(?)
        await UniTask.Delay(100);

        // イベント発行
        OnClickSelectWord?.Invoke(text);
    }

    /// <summary>
    /// パネルイベント有効化
    /// </summary>
    /// <param name="swt"></param>
    public void EnablePanelEvent(bool swt)
    {
        //messageImage.raycastTarget = swt;
    }

    public void PushPanel()
    {
        OnImageClick?.Invoke();
    }
}
