using System;
using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using TMPro;
using UI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class ButtonWrapper : MonoBehaviour
{
    [SerializeField] Animator buttonAnim;
    [SerializeField] Graphic hitTarget;
    [SerializeField] TextMeshProUGUI textComponent;
    [SerializeField] UIAtlasImageSpriteList imageList;

    public string down;
    public string cancel;
    public string click;

    public UnityEvent onClick;

    private EventTrigger trigger;

    private bool isPush = false;


    public void Initialise()
    {
        trigger = hitTarget.gameObject.AddComponent<EventTrigger>();
        hitTarget.GetComponent<Graphic>().raycastTarget = true;

        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerDown;
            entry.callback.AddListener(OnPointerDown);
            trigger.triggers.Add(entry);
        }
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerUp;
            entry.callback.AddListener(OnPointerUp);
            trigger.triggers.Add(entry);
        }
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerEnter;
            entry.callback.AddListener(OnPointerEnter);
            trigger.triggers.Add(entry);
        }
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.PointerExit;
            entry.callback.AddListener(OnPointerExit);
            trigger.triggers.Add(entry);
        }
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.BeginDrag;
            entry.callback.AddListener(OnBeginDrag);
            trigger.triggers.Add(entry);
        }
        {
            EventTrigger.Entry entry = new EventTrigger.Entry();
            entry.eventID = EventTriggerType.Drag;
            entry.callback.AddListener(OnDrag);
            trigger.triggers.Add(entry);
        }

        if (imageList != null)
        {
            imageList.CreateSpriteList();
        }
    }

    public void SetEnable(bool enable)
    {
        hitTarget.raycastTarget = enable;
    }

    private void OnPointerDown(BaseEventData e)
    {
        isPush = true;
        if (buttonAnim != null)
        {
            buttonAnim.Play(down);
        }
    }
    private void OnPointerUp(BaseEventData e)
    {
        if (isPush)
        {
            isPush = false;
            if (buttonAnim != null)
            {
                buttonAnim.Play(click);
            }
            onClick.Invoke();
        }
    }
    private void OnPointerEnter(BaseEventData e)
    {
    }
    private void OnPointerExit(BaseEventData e)
    {
        if (isPush)
        {
            Cancel();
        }
    }


    private const float DRAG_LENGTH = 20.0f;
    private Vector3 dragPos;
    private void OnBeginDrag(BaseEventData e)
    {
        var point_event = e as PointerEventData;
        dragPos = point_event.position;
    }
    private void OnDrag(BaseEventData e)
    {
        var point_event = e as PointerEventData;
        if (isPush && Vector3.Distance(dragPos, point_event.position) > DRAG_LENGTH)
        {
            Cancel();
        }
    }

    private void Cancel()
    {
        isPush = false;
        if (buttonAnim != null)
        {
            buttonAnim.Play(cancel);
        }
    }

    public void SetText(string text)
    {
        if (textComponent != null)
        {
            textComponent.text = text;
        }
    }

    public void ChangeSlice(int index)
    {
        if (imageList != null)
        {
            imageList.Apply(index);
        }
    }

    public void SetAnime(string label)
    {
        if (buttonAnim != null)
        {
            buttonAnim.Play(label);
        }
    }
}
