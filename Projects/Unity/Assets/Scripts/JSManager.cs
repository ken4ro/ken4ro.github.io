using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using static GlobalState;

public class JSManager : SingletonMonoBehaviour<JSManager>
{
    // �{�b�g�����J�n
    public void StartBotProcess()
    {
        GlobalState.Instance.CurrentState.Value = State.Starting;
    }

    // �{�b�g������~
    public void StopBotProcess()
    {
        //SceneManager.LoadScene(0);
        GlobalState.Instance.CurrentState.Value = State.Waiting;
    }

    // ���[�U�[���b�Z�[�W���Z�b�g
    public void SetUserMessage(string text)
    {
        // �����F���̍ŏI������Ƃ��ăZ�b�g
        StreamingSpeechToText.Instance.RecognitionCompleteText = text;

        GlobalState.Instance.CurrentState.Value = State.SpeakingComplete;
    }

    // ���b����������Z�b�g
    public void SetSpeakingText(string text)
    {
        // ���b���������\��
        UIManager.Instance.SetSpeakingText(text);

        GlobalState.Instance.CurrentState.Value = State.Speaking;
    }

    public class AudioVolumeJson
    {
        public float Volume;
    }

    public void SetVoiceVolume(float volume)
    {
        Debug.Log($"SetVoiceVolume: volume = {volume}");
        //var volume = JsonUtility.FromJson<AudioVolumeJson>(json).Volume;
        CharacterManager.Instance.SetMouseOpenYParameter(volume);
    }

    public void WebRTCDisconnect()
    {
        GlobalState.Instance.CurrentState.Value = GlobalState.State.Starting;
    }

    private bool _isCharacterAnimation = true;
    public void SetFaceInfo(string faceInfoJson)
    {
        if (_isCharacterAnimation)
        {
            _isCharacterAnimation = false;
            // �L�����N�^�[�A�j���[�V����������
            CharacterManager.Instance.DisableAnimation();
            // �A�C�h�����[�V�������̃L�����N�^�[�g�����X�t�H�[�����Z�b�g
            CharacterManager.Instance.SetTransformsForIdle();
            // ��F���L��
            FaceInfoManager.Instance.Enable();
        }

        FaceInfoManager.Instance.FaceInfoReceived(faceInfoJson);
    }

    #region for Signaling

    public void SignalingServerOnConnect()
    {

    }

    public void SignalingServerOnDisconnect()
    {

    }

    public void SignalingServerOnNotification(string json)
    {
        SignalingManager.Instance.OnNotification(json);
    }

    public void SignalingServerOnResponse(string json)
    {
        SignalingManager.Instance.OnResponse(json);
    }

    public void SignalingServerOnClose()
    {
        SignalingManager.Instance.OnClose();
    }

    public void SignalingServerOnError()
    {
        SignalingManager.Instance.OnError();
    }

    #endregion for Signaling
}
