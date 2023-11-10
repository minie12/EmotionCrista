using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

struct PlayInfo
{
    public string playerName;
    public bool bMultiRound;
    public int dayCount;
    public bool bAfterCounsel;

    public void Initialize()
    {
        playerName = "";
        bMultiRound = true;
        dayCount = 1;
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

    PlayInfo currentPlayInfo;

    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        if(instance == null) // If there is no instance already
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
        else if(instance != this) // If there is already an instance and it's not `this` instance
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
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetStringVariable("PlayerName", currentPlayInfo.playerName);
                flowchart.SetBooleanVariable("MultiRound", currentPlayInfo.bMultiRound);
                flowchart.SetIntegerVariable("DayCount", currentPlayInfo.dayCount);
                flowchart.SetBooleanVariable("AfterCounsel", currentPlayInfo.bAfterCounsel);
            }
        }

        if (null != loadManager)
        {
            loadManager.LoadGameData(); // load game data from saved file
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
    public void ProceedNextDay()
    {
        currentPlayInfo.dayCount += 1;
        currentPlayInfo.bAfterCounsel = false;
    }
    public void SetFirstRoundPlayInfo()
    {
        currentPlayInfo.bMultiRound = false;
        currentPlayInfo.dayCount = 1;
        currentPlayInfo.bAfterCounsel = false;
    }
    #endregion

    #region GetterSetter
    public int GetDayCount() { return currentPlayInfo.dayCount; }
    #endregion
}