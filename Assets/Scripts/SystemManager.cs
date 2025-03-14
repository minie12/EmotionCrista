using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Net;
using Fungus;
using UnityEngine.Rendering;

[System.Serializable]
struct GameSettingData
{
    public bool bFullScreen;

    public bool bMultiRound;

    public bool bMuteSFX;
    public bool bMuteBGM;

    public float SFXVolume;
    public float BGMVolume;

    public void Initialize()
    {
        bFullScreen = true; 
        bMultiRound = false;
        bMuteSFX    = false;
        bMuteBGM    = false;
        SFXVolume   = 1.0f;
        BGMVolume   = 1.0f;
    }
}

public class SystemManager  : MonoBehaviour
{
    public static SystemManager instance;
    private string gameSettingDataPath;

    private GameSettingData gameSetting;

    void Awake()
    {
        gameSettingDataPath = string.Format("{0}/{1}.bin", Application.persistentDataPath, "GS1127");

        if (instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(this.gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;

            gameSetting.Initialize();

            LoadGameSetting();

            Screen.SetResolution(1920, 1080, gameSetting.bFullScreen);
        }
        else if (instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    static public SystemManager Get()
    {
        return instance;
    }

    public void SetMultiRound(bool bInRound) { gameSetting.bMultiRound = bInRound; }
    public bool IsMultiRound() { return gameSetting.bMultiRound; }

    public void SetFullScreenMode(bool bInFullScreenMode) 
    {
        gameSetting.bFullScreen = bInFullScreenMode;
        Screen.fullScreen = gameSetting.bFullScreen;
    }
    public bool IsFullScreenMode() { return gameSetting.bFullScreen; }

    public void ToggleMuteSFX() 
    { 
        gameSetting.bMuteSFX = !gameSetting.bMuteSFX;

        if ((false == gameSetting.bMuteSFX) && (Mathf.Approximately(gameSetting.SFXVolume, 0f)))
        {
            gameSetting.SFXVolume = 0.1f;
        }
    }
    public bool IsSFXMuted() { return gameSetting.bMuteSFX; }
    public float GetSFXVolume() 
    {
        if (gameSetting.bMuteSFX)
        {
            return 0.0f;
        }
        else 
        { 
            return gameSetting.SFXVolume;
        }
    }
    public void SetSFXVolume(float inVolume) 
    { 
        gameSetting.SFXVolume = inVolume;

        gameSetting.bMuteSFX = (Mathf.Approximately(gameSetting.SFXVolume, 0f));
    }

    public void ToggleMuteBGM() 
    { 
        gameSetting.bMuteBGM = !gameSetting.bMuteBGM;

        if ((false == gameSetting.bMuteBGM) && (Mathf.Approximately(gameSetting.BGMVolume, 0f)))
        {
            gameSetting.BGMVolume = 0.1f;
        }

        OnBGMVolumeChanged();
    }
    public bool IsBGMMuted() { return gameSetting.bMuteBGM; }
    public float GetBGMVolume() 
    {
        if (gameSetting.bMuteBGM)
        {
            return 0.0f;
        }
        else
        {
            return gameSetting.BGMVolume;
        }
    }
    public void SetBGMVolume(float inVolume) 
    { 
        gameSetting.BGMVolume = inVolume;

        gameSetting.bMuteBGM = (Mathf.Approximately(gameSetting.BGMVolume, 0f));

        OnBGMVolumeChanged();
    }

    private void OnBGMVolumeChanged()
    {
        float audioVolume = (gameSetting.bMuteBGM) ? 0f : gameSetting.BGMVolume;

        var musicManager = FungusManager.Instance.MusicManager;

        musicManager.SetAudioVolume(audioVolume, 0.0f, null);
    }

    public void SaveGameSetting()
    {
        // Save To File
        FileStream stream = new FileStream(gameSettingDataPath, FileMode.Create);

        BinaryFormatter formatter = new BinaryFormatter();
        formatter.Serialize(stream, gameSetting);

        stream.Close();
    }

    void LoadGameSetting()
    {
        if (File.Exists(gameSettingDataPath))
        {
            FileStream stream = new FileStream(gameSettingDataPath, FileMode.Open);

            BinaryFormatter formatter = new BinaryFormatter();
            gameSetting = (GameSettingData)formatter.Deserialize(stream);

            stream.Close();
        }
    }

    // DEBUG
    public void EraseData()
    {
        if (File.Exists(gameSettingDataPath))
        {
            File.Delete(gameSettingDataPath);
        }

        gameSetting.Initialize();
    }
}
