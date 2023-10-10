using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UniRx;
using DG.Tweening;
using Unity.VisualScripting;

public class PushableSimpleIcon : MonoBehaviour
{
    [SerializeField]
    private Animator _animator;

    public string StartLabel;
    public string EndLabel;

    private RectTransform _rectTransform = null;

    int _endAnimHash;
    private Coroutine loopAnim;

    /// <summary>
    /// 表示を有効にする
    /// </summary>
    public void Enable() => gameObject.SetActive(true);

    /// <summary>
    /// 表示を無効にする
    /// </summary>
    public void Disable() => gameObject.SetActive(false);

    /// <summary>
    /// 初期化
    /// </summary>
    public void Initialize()
    {
        _rectTransform = GetComponent<RectTransform>();
        _endAnimHash = Animator.StringToHash(EndLabel);
    }

    public void ResetObject()
    {
        if (_animator != null)
        {
            return;
        }
        _rectTransform.DOComplete();
        _rectTransform.localScale = new Vector3(1.0f, 1.0f);
        _animator.Play(EndLabel, 0, 1.0f);
    }

    /// <summary>
    /// フェードイン
    /// </summary>
    public void FadeIn()
    {
        if (!gameObject.activeSelf) Enable();

        ResetObject();

        if (loopAnim == null)
        {
            loopAnim = UIManager.Instance.StartCoroutine(HandAnime());
        }
    }

    /// <summary>
    /// フェードアウト
    /// </summary>
    public void FadeOut()
    {
        if (loopAnim != null)
        {
            UIManager.Instance.StopCoroutine(loopAnim);
        }
        loopAnim = null;

        Observable.Timer(TimeSpan.FromSeconds(0.3f)).Subscribe(_ =>
        {
            Disable();
        });
    }

    const float WAIT_TIMER = 10.0f;
    IEnumerator HandAnime()
    {
        float timer = WAIT_TIMER;
        while (true)
        {
            if (Input.touches.Length > 0)
            {
                timer = WAIT_TIMER;
            }
            if (Input.GetMouseButton(0))
            {
                timer = WAIT_TIMER;
            }

            timer -= Time.deltaTime;
            if (timer < 0)
            {
                _animator.Play(StartLabel, 0, 0.0f);
                yield return new WaitUntil(() => 
                {
                    var currentState = _animator.GetCurrentAnimatorStateInfo(0);
                    if (currentState.shortNameHash == _endAnimHash)
                    {
                        return currentState.normalizedTime > 1.0f;
                    }
                    return false;
                });
                timer = WAIT_TIMER;
            }
            yield return null;
        }
    }
}