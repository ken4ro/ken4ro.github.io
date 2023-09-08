using Cysharp.Threading.Tasks;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Waiting : IState
{
    public async void OnEnter()
    {
        // �^�C�}�[���������Z�b�g
        ActionManager.Instance.TimerTask?.Dispose();
        // UI���Z�b�g
        await UIManager.Instance.Reset();
        // �L�����N�^�[���Z�b�g
        CharacterManager.Instance.DisableAnimation();
        CharacterManager.Instance.SetTransformsForIdle();
        CharacterManager.Instance.EnableAnimation();
        CharacterManager.Instance.ResetAnimation();
        CharacterManager.Instance.ResetPosition();
        //CharacterManager.Instance.Disable();
        // �T�E���h���Z�b�g
        AudioManager.Instance.ResetCharacterVoice();
        // �f�t�H���g����ɖ߂��Ă���
        SignageSettings.InitializeCurrentLanguage();
    }

    public void OnUpdate()
    {

    }

    public void OnExit()
    {

    }
}
