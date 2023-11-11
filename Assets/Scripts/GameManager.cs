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
    public bool bMultiRound;
    public int dayCount;
    public int characterIndex;
    public bool bAfterCounsel;

    public void Initialize()
    {
        playerName = "NoName";
        bMultiRound = true;
        dayCount = 1;
        characterIndex = (int)CharacterName.Naria;
        bAfterCounsel = false;
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
        Screen.SetResolution(1920, 1080, true);
        if (instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;

            currentPlayInfo.Initialize();

            loadManager = this.GetComponent<LoadManager>();
            if (null == loadManager)
            {
                Debug.LogError("[GameManager] Add 'LoadManager' Script to 'GameManager' gameobject");
            }

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

        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetStringVariable("PlayerName", currentPlayInfo.playerName);
                flowchart.SetBooleanVariable("MultiRound", currentPlayInfo.bMultiRound);
                flowchart.SetIntegerVariable("DayCount", currentPlayInfo.dayCount);
                flowchart.SetIntegerVariable("CharacterIndex", currentPlayInfo.characterIndex);
                flowchart.SetBooleanVariable("AfterCounsel", currentPlayInfo.bAfterCounsel);
            }
        }

        GameObject GO_playerCharacter = GameObject.Find("Player");
        if (null != GO_playerCharacter)
        {
            Fungus.Character playerCharacter = GO_playerCharacter.GetComponent<Fungus.Character>();
            if (null != playerCharacter)
            {
                playerCharacter.SetStandardText(currentPlayInfo.playerName);
            }
        }

        GameObject GO_UICanvas = GameObject.Find("GameUICanvas");
        if (null != GO_UICanvas)
        {
            UICanvasManager UICanvasManager = GO_UICanvas.GetComponent<UICanvasManager>();
            if (null != UICanvasManager)
            {
                UICanvasManager.OnSceneLoaded(Scene.name, currentPlayInfo.dayCount, currentPlayInfo.bAfterCounsel);
            }
        }
    }

    #region PlayInfoFunctions
    public void GetPlayInfo(ref PlayInfo refPlayInfo)
    {
        refPlayInfo.playerName = currentPlayInfo.playerName;
        refPlayInfo.bMultiRound = currentPlayInfo.bMultiRound;
        refPlayInfo.dayCount = currentPlayInfo.dayCount;
        refPlayInfo.characterIndex = currentPlayInfo.characterIndex;
        refPlayInfo.bAfterCounsel = currentPlayInfo.bAfterCounsel;
    }
    public void SetPlayInfo(PlayInfo inPlayInfo)
    {
        currentPlayInfo.playerName = inPlayInfo.playerName;
        currentPlayInfo.bMultiRound = inPlayInfo.bMultiRound;
        currentPlayInfo.dayCount = inPlayInfo.dayCount;
        currentPlayInfo.characterIndex = inPlayInfo.characterIndex;
        currentPlayInfo.bAfterCounsel = inPlayInfo.bAfterCounsel;
    }
    public void ProceedNextDay()
    {
        currentPlayInfo.dayCount += 1;
        currentPlayInfo.characterIndex += 1;
        currentPlayInfo.bAfterCounsel = false;
    }
    public void SetFirstRoundPlayInfo()
    {
        currentPlayInfo.bMultiRound = false;
        currentPlayInfo.dayCount = 1;
        currentPlayInfo.characterIndex = 0;
        currentPlayInfo.bAfterCounsel = false;
    }
    #endregion

    #region GetterSetter
    public bool IsMultiRound() { return currentPlayInfo.bMultiRound; }
    public int GetDayCount() { return currentPlayInfo.dayCount; }
    public int GetCharacterIndex() { return currentPlayInfo.characterIndex; }
    public void SetAfterCounsel(bool bInAfterCounsel) { currentPlayInfo.bAfterCounsel = bInAfterCounsel; }
    #endregion
}