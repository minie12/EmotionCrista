using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
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

    private Dictionary<string, Area> areaMap = new Dictionary<string, Area>();

    private PlayInfo currentPlayInfo;

    private Area currentArea;

#if UNITY_EDITOR
    bool bFirstLoad = true;
#endif

    // Start is called before the first frame update
    void Awake()
    {
        Debug.Log("GameManager Instance Created");
        if (instance == null) // If there is no instance already
        {
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;

            currentPlayInfo.Reset();

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

    public Area GetArea()
    {
        return currentArea;
    }

    protected void OnSceneLoaded(Scene Scene, LoadSceneMode mode)
    {
        if (Scene.name == "Start")
        {
            return;
        }

        if (null != loadManager)
        {
            loadManager.LoadGameData(); // load game data from saved file
        }

        if (false == areaMap.ContainsKey(Scene.name))
        {
            Area newArea = new Area();
            newArea.sceneName = Scene.name;

            areaMap.Add(Scene.name, newArea);
        }

        currentArea = areaMap[Scene.name];

        if (null != currentArea)
        {
            currentArea.OnSceneLoaded();
        }

        // Set Fungus variables to PlayInfo
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
#if UNITY_EDITOR
                bool bDebugging = false;
                if (true == bFirstLoad)
                {
                    bDebugging = flowchart.GetBooleanVariable("Debugging");
                }

                if (true == bDebugging)
                {
                    PlayInfo DebugPlayInfo = new PlayInfo();
                    DebugPlayInfo.Reset();

                    bool bMultiRound = flowchart.GetBooleanVariable("MultiRound");
                    SystemManager.Get().SetMultiRound(bMultiRound);

                    DebugPlayInfo.playerName = flowchart.GetStringVariable("PlayerName");
                    DebugPlayInfo.characterIndex = flowchart.GetIntegerVariable("CharacterIndex");
                    DebugPlayInfo.dayCount = DebugPlayInfo.characterIndex + 1;

                    DebugPlayInfo.bHaveReport = flowchart.GetBooleanVariable("HaveReport");

                    DebugPlayInfo.endingMode = flowchart.GetIntegerVariable("EndingMode");

                    bool bCompletedStart = flowchart.GetBooleanVariable("CompletedStart");
                    if (true == bCompletedStart)
                    {
                        SetStartBlockCompletion(true);
                    }
                    
                    SetPlayInfo(DebugPlayInfo);
                }
                else
#endif
                {
                    flowchart.SetBooleanVariable("MultiRound", SystemManager.Get().IsMultiRound());

                    flowchart.SetStringVariable("PlayerName", currentPlayInfo.playerName);
                    flowchart.SetIntegerVariable("CharacterIndex", currentPlayInfo.characterIndex);

                    flowchart.SetBooleanVariable("HaveReport", currentPlayInfo.bHaveReport);

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

            if (null != currentArea)
            {
                flowchart.SetBooleanVariable("CompletedStart", currentArea.bSceneStartBlockCompleted);
            }
        }

        // Reset Log 
        LogCache.ClearAllLog();

#if UNITY_EDITOR
        bFirstLoad = false;
#endif
    }

    public bool IsObjectClicked(ClickableID inObjectFlag)
    {
        bool bClickedObject = false;

        if (null != currentArea)
        {
            bClickedObject = currentArea.IsObjectClicked(inObjectFlag);
        }

        return bClickedObject;
    }

    public bool IsObjectClicked(string inSceneName, ClickableID inObjectFlag)
    {
        bool bClickedObject = false;

        if (true == areaMap.ContainsKey(inSceneName))
        {
            if (null != areaMap[inSceneName])
            {
                bClickedObject = areaMap[inSceneName].IsObjectClicked(inObjectFlag);
            }
        }

        return bClickedObject;
    }

    public void GetAreaMap(ref Dictionary<string, Area> refAreaMap)
    {
        refAreaMap = areaMap;
    }

    public void SetAreaMap(Dictionary<string, Area> inAreaMap)
    {
        areaMap = inAreaMap;
    }

    #region PlayInfoFunctions
    public string GetPlayerName()
    {
        return currentPlayInfo.playerName;
    }
    public void SetPlayerName(string inPlayerName)
    {
        currentPlayInfo.playerName = inPlayerName;
    }

    public void GetPlayInfo(ref PlayInfo refPlayInfo)
    {
        refPlayInfo.playerName = currentPlayInfo.playerName;
        refPlayInfo.dayCount = currentPlayInfo.dayCount;
        refPlayInfo.characterIndex = currentPlayInfo.characterIndex;

        refPlayInfo.bHaveReport = currentPlayInfo.bHaveReport;
        refPlayInfo.minigameLevel = currentPlayInfo.minigameLevel;

        refPlayInfo.storyConditionState = currentPlayInfo.storyConditionState;

        refPlayInfo.endingMode = currentPlayInfo.endingMode;
    }
    public void SetPlayInfo(PlayInfo inPlayInfo)
    {
        currentPlayInfo.playerName = inPlayInfo.playerName;
        currentPlayInfo.dayCount = inPlayInfo.dayCount;
        currentPlayInfo.characterIndex = inPlayInfo.characterIndex;

        currentPlayInfo.bHaveReport = inPlayInfo.bHaveReport;
        currentPlayInfo.minigameLevel = inPlayInfo.minigameLevel;

        currentPlayInfo.storyConditionState = inPlayInfo.storyConditionState;

        currentPlayInfo.endingMode = inPlayInfo.endingMode;
    }
    public void ProceedNextDay()
    {
        currentPlayInfo.dayCount += 1;
        currentPlayInfo.characterIndex = GetPatientIndex(currentPlayInfo.dayCount);

        currentPlayInfo.bHaveReport = false;
        currentPlayInfo.storyConditionState = StoryConditionState.None;

        currentArea = null;
        areaMap.Clear();
    }

    public void ResetPlayInfo()
    {
        currentPlayInfo.Reset();
        areaMap.Clear();
    }
    public void ResetAfterMinigame()
    {
        if (null != currentArea)
        {
            currentArea.bSceneStartBlockCompleted = true;
        }

        currentPlayInfo.ResetAfterMinigame();
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
    public bool IsAfterCounsel() { return currentPlayInfo.bHaveReport; }
    public int GetDayCount() { return currentPlayInfo.dayCount; }
    public int GetCharacterIndex() { return currentPlayInfo.characterIndex; }
    public int GetMinigameLevel() { return currentPlayInfo.minigameLevel; }
    public int GetMinigameHistory() { return currentPlayInfo.minigameHistory; }
    public bool HasStoryConditionState(StoryConditionState inState) { return currentPlayInfo.storyConditionState.HasFlag(inState); }
    public void SetHaveReport(bool bInHaveReport) { currentPlayInfo.bHaveReport = bInHaveReport; }
    public void SetGameLevel(int inGameLevel) 
    { 
        currentPlayInfo.minigameLevel = inGameLevel;
        currentPlayInfo.minigameHistory = currentPlayInfo.minigameHistory * 10 + inGameLevel;
    }
    public void SetStoryConditionState(StoryConditionState inState)
    {
        currentPlayInfo.storyConditionState |= inState;
    }
    public void SetLoadData(EmoSaveData inLoadData) 
    {
        if (null != loadManager)
        {
            loadManager.LoadEmoSaveData = inLoadData;
        }
    }
    public void SetStartBlockCompletion(bool bCompletion)
    {
        if (null != currentArea)
        {
            currentArea.bSceneStartBlockCompleted = bCompletion;
        }
    }

    #endregion
}