using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UI;

public class LanguageSelecter : MonoBehaviour
{
    [SerializeField]
    Animator WindowAnim = null;
    [SerializeField]
    ButtonWrapper OpenButton = null;
    [SerializeField]
    Animator CurrentLanguageObj = null;
    [SerializeField]
    UIAtlasImageSpriteList CurrentLanguageImage = null;
    [SerializeField]
    List<ButtonWrapper> LanguageButtonList = null;
    [SerializeField]
    AudioClip SpreadSE = null;
    [SerializeField]
    AudioClip NarrowSE = null;
    [SerializeField]
    AudioClip SelectSE = null;
    [SerializeField]
    AudioSource SESpeaker = null;

    /// <summary>
    /// 言語選択時コールバック
    /// </summary>
    public Action<SignageSettings.Language> OnSelectLanguage = null;

    /// <summary>
    /// 対応言語数
    /// </summary>
    public int LanguageCount { get; private set; } = 0;

    private bool isOpen = false;

    void Awake()
    {
        // 対応言語数取得
        LanguageCount = SignageSettings.Settings.LanguageVoiceMap.Keys.Count;

        OpenButton.Initialise();
        OpenButton.onClick.AddListener(()=> { SelectButton(OpenButton, null); });

        CurrentLanguageImage.CreateSpriteList();
        CurrentLanguageImage.Apply(GetLanguageIndex(SignageSettings.CurrentLanguage.Value));

        // 選択言語オブジェクト
        InitLanguageButton();
    }

    private void InitLanguageButton()
    {
        // 選択言語オブジェクト
        var index = 0;
        foreach (var lang in SignageSettings.Settings.LanguageVoiceMap.Keys)
        {
            if (index == LanguageButtonList.Count)
            {
                break;
            }

            if (lang == SignageSettings.CurrentLanguage.Value)
            {
                continue;
            }

            // プレハブからゲームオブジェクト生成
            var button = LanguageButtonList[index]; //!< 0は選択中言語でボタン無しなので、1つずつずらす
            button.Initialise();
            button.onClick.RemoveAllListeners();
            button.onClick.AddListener(() => { SelectButton(button, lang); });

            // 画像をセット
            button.ChangeSlice((GetLanguageIndex(lang)));

            // アクティブにしておく
            button.gameObject.SetActive(true);
            index++;
        }

        for (int no1 = index; no1 < LanguageButtonList.Count; no1++)
        {
            LanguageButtonList[no1].gameObject.SetActive(false);
        }
    }

    public void SetDefaultAnim()
    {
        isOpen = false;
        WindowAnim.Play("language_sel_base");
        CurrentLanguageObj.Play("btn_main_001_hold");

        foreach (var button in LanguageButtonList)
        {
            button.SetAnime("btn_main_001_base");
        }
    }

    void OnEnable()
    {
        ResetComponent();
    }

    void OnDisable()
    {
        // コルーチン全停止
        StopAllCoroutines();
    }

    /// <summary>
    /// リセット
    /// </summary>
    public void ResetComponent()
    {
    }

    /// <summary>
    /// 外部から言語をセットする
    /// </summary>
    /// <param name="language"></param>
    public void SetLanguage(SignageSettings.Language lang)
    {
    }

    /// <summary>
    /// 現在有効になっている言語タイプの取得
    /// </summary>
    /// <returns></returns>
    public SignageSettings.Language GetActiveLanguage()
    {
        return SignageSettings.CurrentLanguage.Value;
    }

    public void SelectButton(ButtonWrapper sender, SignageSettings.Language? param)
    {
        if (isOpen)
        {
            isOpen = false;
            if (param != null)
            {
                // 言語ボタンが選択された
                OnSelectLanguage?.Invoke(param.Value);
                sender.SetAnime("btn_main_001_hold");
                SESpeaker.PlayOneShot(SelectSE);
                WindowAnim.Play("language_sel_off");
                CurrentLanguageImage.Apply(GetLanguageIndex(param.Value));
                InitLanguageButton();
            }
            else
            {
                // 言語選択メニュークローズ
                SESpeaker.PlayOneShot(NarrowSE);
                WindowAnim.Play("language_sel_off");
            }
        }
        else
        {
            isOpen = true;
            WindowAnim.Play("language_sel_on");
        }
    }

    private Sprite GetLanguageSprite(SignageSettings.Language lang)
    {
        Sprite ret = null;

        // TODO: 画像ファイル名を json に含める
        switch (lang)
        {
            case SignageSettings.Language.Japanese:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_ja");
                break;
            case SignageSettings.Language.English:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_en");
                break;
            case SignageSettings.Language.Russian:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_ru");
                break;
            case SignageSettings.Language.Arabic:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_ar");
                break;
            case SignageSettings.Language.Chinese:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_cn");
                break;
            case SignageSettings.Language.Vietnamese:
                ret = Resources.Load<Sprite>("Images/Icon/icon_lang_vi");
                break;
            default:
                Debug.LogError("Not implemented language.");
                break;
        }

        return ret;
    }

    private int GetLanguageIndex(SignageSettings.Language lang)
    {
        int ret = 0;

        var langCount = 0;
        foreach (var i in SignageSettings.Settings.LanguageVoiceMap.Keys)
        {
            if (lang == i)
            {
                ret = langCount;
                break;
            }
            langCount++;
        }

        return ret;
    }
}
