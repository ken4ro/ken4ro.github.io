using Live2D.Cubism.Core;
using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class ModelViewManager : MonoBehaviour
{
    [SerializeField]
    Camera mainCamera;
    [SerializeField]
    TMP_Dropdown SelectCharacter;

    CharacterManager charaObj;

    void Start()
    {
        SelectCharacter.ClearOptions();

        int no1;
        List<string> name_list = new List<string>();
        for (no1 = 0; no1 < Enum.GetValues(typeof(GlobalState.CharacterModel)).Length; no1++)
        {
            name_list.Add(((GlobalState.CharacterModel)no1).ToString());
        }
        SelectCharacter.AddOptions(name_list);
    }

    void Update()
    {
        
    }

    public async void LoadCharacter()
    {
        if (charaObj != null)
        {
            charaObj.Release();
            GameObject.Destroy(charaObj.gameObject);
        }
        if (AssetBundleManager.Instance.AvatarAssetBundle != null)
        {
            AssetBundleManager.Instance.AvatarAssetBundle.Unload(true);
            AssetBundleManager.Instance.AvatarAssetBundle = null;
        }

        GlobalState.Instance.CurrentCharacterModel.Value = (GlobalState.CharacterModel)SelectCharacter.value;

        await AssetBundleManager.Instance.LoadAvatarAssetBundleFromStreamingAssets();
        var asset = AssetBundleManager.Instance.CreateAvatarAssetBundle();
        var go = GameObject.Instantiate(asset, null);
        charaObj = go.GetComponent<CharacterManager>();

        Vector2 scrernsize = new Vector2(Screen.width, Screen.height);
        charaObj.transform.position = mainCamera.transform.position + mainCamera.transform.forward * 1.5f - mainCamera.transform.up * 0.9f;
        charaObj.transform.eulerAngles = new Vector3(0, 180, 0);
    }

    public void ChangeScreen()
    {
        Screen.fullScreen = !Screen.fullScreen;
    }
}
