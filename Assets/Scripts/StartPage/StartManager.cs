using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Fungus;
using UnityEngine.Playables;

public class StartManager : MonoBehaviour
{
    public SpriteRenderer background;
    public SpriteRenderer logo;

    // Default is set to Night ver.
    public Sprite dayBG;
    public Sprite dayLogo;
    public Sprite dayButton;
    public Sprite dayButtonHover;

    void Start()
    {
        RefreshStartScene();
    }

    public void GameStart()
    {
        GameManager gameManager = GameManager.Get();
        if (null != gameManager)
        {
            // DEBUG PURPOSE
            SystemManager.Get().SetMultiRound(true);

            gameManager.ResetPlayInfo();
            SceneManager.LoadScene("FirstDayDormitory");
        }
        else
        {
            Debug.LogError("No GameManager Found");
        }
    }

    public void ResetGame()
    {
        SystemManager.Get().EraseData();
        SaveLoadMenuManager.EraseData();
        RefreshStartScene();
    }

    public void QuitGame()
    {
#if UNITY_EDITOR
            UnityEditor.EditorApplication.isPlaying = false;
#else
        Application.Quit();
#endif
    }

    void RefreshStartScene()
    {
        if (false == SystemManager.Get().IsMultiRound())
        {
            background.sprite = dayBG;
            logo.sprite = dayLogo;
        }
    }

    // Debug
    public void OnStartDebug(int CharacterIndex) // Naria : 0
    {
        if (null != GameManager.Get() && null != SystemManager.Get())
        {
            GameManager.Get().ResetPlayInfo();
            SystemManager.Get().SetMultiRound(true);

            PlayInfo DebugPlayInfo = new PlayInfo();
            DebugPlayInfo.Reset();

            DebugPlayInfo.playerName = (CharacterIndex == 0)? " ": "Debugging";
            DebugPlayInfo.characterIndex = CharacterIndex;
            DebugPlayInfo.dayCount = DebugPlayInfo.characterIndex + 1;

            DebugPlayInfo.bHaveReport = false;
            DebugPlayInfo.endingMode = 0;

            GameManager.Get().SetPlayInfo(DebugPlayInfo);
        }
        
        if (CharacterIndex == 0)
        {
            SceneManager.LoadScene("FirstDayDormitory");
        }
        else
        {
            SceneManager.LoadScene("Dormitory");
        }
    }
}
