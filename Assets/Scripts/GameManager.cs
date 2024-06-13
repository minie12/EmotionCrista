using Fungus;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Xml;
using UnityEngine;
using UnityEngine.SceneManagement;
using static ScreenObjectInfo;

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
public struct ScreenInfoData
{
    public ClickableID[] locatedObjects;
    public byte objectClickedFlag;

    public ScreenInfoData(ClickableID[] inLocatedObjects, byte inObjectClickedFlag)
    {
        locatedObjects = inLocatedObjects;
        objectClickedFlag = inObjectClickedFlag;
    }
}

[System.Serializable]
public struct PlayInfo
{
    public string playerName;
    public int dayCount;
    public int characterIndex;

    public bool bHaveReport;
    public bool bRedButtonPressed;

    public int miniGameLevel;

    public int endingMode;

    public Dictionary<string, bool> sceneStartBlockCompletedMap;
    public Dictionary<string, ScreenInfoData> sceneScreenInfoMap;

    public void Reset()
    {
        playerName = "NoName";
        dayCount = 1;
        characterIndex = (int)CharacterName.Naria;

        bHaveReport = false;
        bRedButtonPressed = false;

        miniGameLevel = (int)LevelType.EASY1;

        endingMode = (int)EndingMode.None;

        if (null == sceneStartBlockCompletedMap)
        {
            sceneStartBlockCompletedMap = new Dictionary<string, bool>();
        }
        else
        {
            sceneStartBlockCompletedMap.Clear();
        }

        if (null == sceneScreenInfoMap)
        {
            sceneScreenInfoMap = new Dictionary<string, ScreenInfoData>();
        }
        else
        {
            sceneScreenInfoMap.Clear();
        }
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

    public void SetSceneScreenInfo(ClickableID[] locatedObjects, byte objectClickedFlag)
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;

        currentPlayInfo.sceneScreenInfoMap[sceneName] = new ScreenInfoData(locatedObjects, objectClickedFlag);
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
                    // RedButton

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
                    // RedButton

                    flowchart.SetIntegerVariable("EndingMode", currentPlayInfo.endingMode);

                    if (null != currentPlayInfo.sceneStartBlockCompletedMap)
                    {
                        if (currentPlayInfo.sceneStartBlockCompletedMap.ContainsKey(Scene.name))
                        {
                            flowchart.SetBooleanVariable("CompletedStart", currentPlayInfo.sceneStartBlockCompletedMap[Scene.name]);
                        }
                    }
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

        // Reset Log 
        LogCache.ClearAllLog();

        // Set ScreenInfo
        if (null != currentPlayInfo.sceneScreenInfoMap)
        {
            GameObject GO_ClickableLocation = GameObject.Find("ClickableLocation");
            if (null != GO_ClickableLocation)
            {
                ScreenObjectInfo screenObjectInfo = GO_ClickableLocation.GetComponent<ScreenObjectInfo>();
                if (null != screenObjectInfo)
                {
                    if (currentPlayInfo.sceneScreenInfoMap.ContainsKey(Scene.name))
                    {
                        screenObjectInfo.SetLoadData(currentPlayInfo.sceneScreenInfoMap[Scene.name].locatedObjects, currentPlayInfo.sceneScreenInfoMap[Scene.name].objectClickedFlag);
                    }
                }
            }
        }
        

#if UNITY_EDITOR
        bFirstLoad = false;
#endif
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
        refPlayInfo.miniGameLevel = currentPlayInfo.miniGameLevel;

        refPlayInfo.endingMode = currentPlayInfo.endingMode;

        refPlayInfo.sceneStartBlockCompletedMap = currentPlayInfo.sceneStartBlockCompletedMap;
        refPlayInfo.sceneScreenInfoMap = currentPlayInfo.sceneScreenInfoMap;
    }
    public void SetPlayInfo(PlayInfo inPlayInfo)
    {
        currentPlayInfo.playerName = inPlayInfo.playerName;
        currentPlayInfo.dayCount = inPlayInfo.dayCount;
        currentPlayInfo.characterIndex = inPlayInfo.characterIndex;

        currentPlayInfo.bHaveReport = inPlayInfo.bHaveReport;
        currentPlayInfo.bRedButtonPressed = inPlayInfo.bRedButtonPressed;
        currentPlayInfo.miniGameLevel = inPlayInfo.miniGameLevel;

        currentPlayInfo.endingMode = inPlayInfo.endingMode;

        currentPlayInfo.sceneStartBlockCompletedMap = inPlayInfo.sceneStartBlockCompletedMap;
        currentPlayInfo.sceneScreenInfoMap = inPlayInfo.sceneScreenInfoMap;
    }
    public void ProceedNextDay()
    {
        currentPlayInfo.dayCount += 1;
        currentPlayInfo.characterIndex = GetPatientIndex(currentPlayInfo.dayCount);

        currentPlayInfo.bHaveReport = false;
        currentPlayInfo.bRedButtonPressed = false;

        if (null != currentPlayInfo.sceneStartBlockCompletedMap)
        {
            currentPlayInfo.sceneStartBlockCompletedMap.Clear();
        }

        if (null != currentPlayInfo.sceneScreenInfoMap)
        {
            currentPlayInfo.sceneScreenInfoMap.Clear();
        }
    }

    public void ResetPlayInfo()
    {
        currentPlayInfo.Reset();
    }
    public void ResetAfterMinigame()
    {
        if (null != currentPlayInfo.sceneStartBlockCompletedMap)
        {
            currentPlayInfo.sceneStartBlockCompletedMap.Clear();
        }

        if (null != currentPlayInfo.sceneScreenInfoMap)
        {
            currentPlayInfo.sceneScreenInfoMap.Clear();
        }

        currentPlayInfo.miniGameLevel = 0;
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
    public int GetMiniGameLevel() { return currentPlayInfo.miniGameLevel; }
    public void SetHaveReport(bool bInHaveReport) { currentPlayInfo.bHaveReport = bInHaveReport; }
    public void SetGameLevel(int inGameLevel) { currentPlayInfo.miniGameLevel = inGameLevel; }
    public void SetLoadData(EmoSaveData inLoadData) 
    {
        if (null != loadManager)
        {
            loadManager.LoadEmoSaveData = inLoadData;
        }
    }
    public void SetStartBlockCompletion(bool bCompletion)
    {
        Scene scene = SceneManager.GetActiveScene();
        string sceneName = scene.name;

        currentPlayInfo.sceneStartBlockCompletedMap[sceneName] = bCompletion;
    }

    #endregion
}