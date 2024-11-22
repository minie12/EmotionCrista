using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SettingMenuManager : MonoBehaviour
{
    public Sprite PlaySFXSprite;
    public Sprite MuteSFXSprite;
    public Sprite PlayBGMSprite;
    public Sprite MuteBGMSprite;

    public Image SFXIcon;
    public Image BGMIcon;

    public Slider SFXSlider;
    public Slider BGMSlider;

    public Toggle fullScreenToggle;
    public Toggle windowToggle;

    public void Awake()
    {
        SystemManager systemManager = SystemManager.Get();
        if (null != systemManager)
        {
            if (systemManager.IsFullScreenMode())
            {
                fullScreenToggle.isOn = true;
                windowToggle.isOn = false;
            }
            else
            {
                fullScreenToggle.isOn = false;
                windowToggle.isOn = true;
            }

            if (systemManager.IsSFXMuted())
            {
                SFXIcon.sprite = MuteSFXSprite;
                SFXSlider.value = 0;
            }
            else
            {
                SFXIcon.sprite = PlaySFXSprite;
                SFXSlider.value = systemManager.GetSFXVolume();
            }

            if (systemManager.IsBGMMuted())
            {
                BGMIcon.sprite = MuteBGMSprite;
                BGMSlider.value = 0;
            }
            else
            {
                BGMIcon.sprite = PlayBGMSprite;
                BGMSlider.value = systemManager.GetBGMVolume();
            }
        }
    }

    public void ChangeScreenSetting()
    {
        SystemManager systemManager = SystemManager.Get();
        if (null == systemManager)
            return;

        if (!systemManager.IsFullScreenMode() && fullScreenToggle.isOn)
        {
            systemManager.SetFullScreenMode(true);
        }
        else if (systemManager.IsFullScreenMode() && windowToggle.isOn)
        {
            systemManager.SetFullScreenMode(false);
        }
    }
    public void ToggleSFX()
    {
        SystemManager systemManager = SystemManager.Get();
        if (null != systemManager)
        {
            systemManager.ToggleMuteSFX();
            if (systemManager.IsSFXMuted())
            {
                SFXIcon.sprite = MuteSFXSprite;
                SFXSlider.value = 0.0f;
            }
            else
            {
                SFXIcon.sprite = PlaySFXSprite;
                SFXSlider.value = systemManager.GetSFXVolume();
            }
        }
    }

    public void SetSFXVolume(float inVolume)
    {
        SystemManager systemManager = SystemManager.Get();
        if ((null != systemManager) && (false == systemManager.IsSFXMuted()))
        {
            systemManager.SetSFXVolume(inVolume);
        }

        if (Mathf.Approximately(inVolume, 0f))
        {
            SFXIcon.sprite = MuteSFXSprite;
        }
        else
        {
            SFXIcon.sprite = PlaySFXSprite;
        }
    }

    public void ToggleBGM()
    {
        SystemManager systemManager = SystemManager.Get();
        if (null != systemManager)
        {
            systemManager.ToggleMuteBGM();
            if (systemManager.IsBGMMuted())
            {
                BGMIcon.sprite = MuteBGMSprite;
                BGMSlider.value = 0.0f;
            }
            else
            {
                BGMIcon.sprite = PlayBGMSprite;
                BGMSlider.value = systemManager.GetBGMVolume();
            }
        }
    }

    public void SetBGMVolume(float inVolume)
    {
        SystemManager systemManager = SystemManager.Get();
        if ((null != systemManager) && (false == systemManager.IsBGMMuted()))
        {
            systemManager.SetBGMVolume(inVolume);
        }

        if (Mathf.Approximately(inVolume, 0f))
        {
            BGMIcon.sprite = MuteBGMSprite;
        }
        else
        {
            BGMIcon.sprite = PlayBGMSprite;
        }
    }
}
