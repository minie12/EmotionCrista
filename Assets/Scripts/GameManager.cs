using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum CharacterName
{
    Naria = 0,
    Lulian,
    Russel,
    Nish,
    Ilrak,
    Max
}

[System.Serializable]
public struct PlayInfo
{
    public string playerName;
    public int dayCount;
    public int characterIndex;

    public bool bHaveReport;
    public bool bRedButtonPressed;

    public int endingMode;

    public void Initialize()
    {
        playerName = "NoName";
        dayCount = 1;
        characterIndex = (int)CharacterName.Naria;

        bHaveReport = false;
        bRedButtonPressed = false;

        endingMode = (int)EndingMode.None;
    }
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;

    private LoadManager loadManager;

    #region MouseVariables
    [SerializeField]
    private Texture2D defaultState;
    [SerializeField]
    private Texture2D clickedState;

    [SerializeField]
    private Vector2 hotspot;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;
    #endregion

    private PlayInfo currentPlayInfo;

    // Start is called before the first frame update
    void Awake()
    {
        if (instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;

            currentPlayInfo.Initialize();

            loadManager = this.GetComponent<LoadManager>();

            Debug.Assert(loadManager != null, "[GameManager] Add 'LoadManager' Script to 'GameManager' gameobject");

            SceneManager.sceneLoaded += OnSceneLoaded;
        }
        else if (instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }
    public void Update()
    {
        if (Input.GetMouseButtonUp(0))
        {
            Cursor.SetCursor(defaultState, hotspot, cursorMode);
        }

        if (Input.GetMouseButtonDown(0))
        {
            Cursor.SetCursor(clickedState, hotspot, cursorMode);
        }

    }

    static public GameManager Get()
    {
        return instance;
    }

    protected void OnSceneLoaded(Scene Scene, LoadSceneMode mode)
    {
        if (null != loadManager)
        {
            loadManager.LoadGameData(); // load game data from saved file
        }

        // Set Fungus variables to PlayInfo
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetBooleanVariable("MultiRound", SystemManager.Get().IsMultiRound());

                flowchart.SetStringVariable("PlayerName", currentPlayInfo.playerName);
                flowchart.SetIntegerVariable("CharacterIndex", currentPlayInfo.characterIndex);

                flowchart.SetBooleanVariable("HaveReport", currentPlayInfo.bHaveReport);
                // RedButton

                flowchart.SetIntegerVariable("EndingMode", currentPlayInfo.endingMode);
            }
        }

        // Set PlayerName
        GameObject GO_playerCharacter = GameObject.Find("Player");
        if (null != GO_playerCharacter)
        {
            Fungus.Character playerCharacter = GO_playerCharacter.GetComponent<Fungus.Character>();
            if (null != playerCharacter)
            {
                playerCharacter.SetStandardText(currentPlayInfo.playerName);
            }
        }

        // Set UI Info
        GameObject GO_UICanvas = GameObject.Find("GameUICanvas");
        if (null != GO_UICanvas)
        {
            UICanvasManager UICanvasManager = GO_UICanvas.GetComponent<UICanvasManager>();
            if (null != UICanvasManager)
            {
                UICanvasManager.OnSceneLoaded(Scene.name, currentPlayInfo.dayCount, currentPlayInfo.bHaveReport);
            }
        }
    }

    #region PlayInfoFunctions
    public string GetPlayerName()
    {
        return currentPlayInfo.playerName;
    }

    public void GetPlayInfo(ref PlayInfo refPlayInfo)
    {
        refPlayInfo.playerName = currentPlayInfo.playerName;
        refPlayInfo.dayCount = currentPlayInfo.dayCount;
        refPlayInfo.characterIndex = currentPlayInfo.characterIndex;

        refPlayInfo.bHaveReport = currentPlayInfo.bHaveReport;
        refPlayInfo.bRedButtonPressed = currentPlayInfo.bRedButtonPressed;

        refPlayInfo.endingMode = currentPlayInfo.endingMode;
    }
    public void SetPlayInfo(PlayInfo inPlayInfo)
    {
        currentPlayInfo.playerName = inPlayInfo.playerName;
        currentPlayInfo.dayCount = inPlayInfo.dayCount;
        currentPlayInfo.characterIndex = inPlayInfo.characterIndex;

        currentPlayInfo.bHaveReport = inPlayInfo.bHaveReport;
        currentPlayInfo.bRedButtonPressed = inPlayInfo.bRedButtonPressed;

        currentPlayInfo.endingMode = inPlayInfo.endingMode;
    }
    public void ProceedNextDay()
    {
        currentPlayInfo.dayCount += 1;
        currentPlayInfo.characterIndex = GetPatientIndex(currentPlayInfo.dayCount);

        currentPlayInfo.bHaveReport = false;
        currentPlayInfo.bRedButtonPressed = false;
    }

    public void ResetPlayInfo()
    {
        currentPlayInfo.playerName = "NoName";
        currentPlayInfo.dayCount = 1;
        currentPlayInfo.characterIndex = 0;

        currentPlayInfo.bHaveReport = false;
        currentPlayInfo.bRedButtonPressed = false;

        currentPlayInfo.endingMode = (int)EndingMode.None;
    }

    public void ResetPlayInfoDebug()
    {
        ResetPlayInfo();

        currentPlayInfo.dayCount = 3;
        currentPlayInfo.characterIndex = (int)CharacterName.Russel;
    }

    public void SetEndingMode(EndingMode inMode) { currentPlayInfo.endingMode = (int)inMode; }
    #endregion

    int GetPatientIndex(int inDayCount)
    {
        switch (inDayCount)
        {
            case 1:
                return (int)CharacterName.Naria;
            case 2:
                return (int)CharacterName.Lulian;
            case 3:
                return (int)CharacterName.Russel;
            case 4:
                return (int)CharacterName.Nish;
            case 5:
                return (int)CharacterName.Ilrak;
        }
        return (int)CharacterName.Max;
    }

    #region GetterSetter
    public bool IsRedButtonPressed() { return currentPlayInfo.bRedButtonPressed; }
    public int GetDayCount() { return currentPlayInfo.dayCount; }
    public int GetCharacterIndex() { return currentPlayInfo.characterIndex; }
    public void SetHaveReport(bool bInHaveReport) { currentPlayInfo.bHaveReport = bInHaveReport; }
    public void SetLoadData(EmoSaveData inLoadData) 
    {
        if (null != loadManager)
        {
            loadManager.LoadEmoSaveData = inLoadData;
        }
    }
    #endregion
}