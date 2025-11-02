using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using Fungus;
using UnityEngine.Rendering;

public class UICanvasManager : MonoBehaviour
{
    [SerializeField]
    private Text locationText;
    [SerializeField]
    private Text dayText;

    public GameObject GO_reportButton;
    public GameObject GO_mapButton;
    public GameObject GO_mapContainer;

    static public string GetLocationName(string inSceneName)
    {
        string locationName = inSceneName;

        switch (inSceneName)
        {
            case "Lobby":
                locationName = "로비";
                break;
            case "LabCorridor":
            case "LabCorridor_AC":
                locationName = "상담실 복도";
                break;
            case "DormCorridor":
                locationName = "관사 복도";
                break;
            case "CounselRoom":
                locationName = "상담실";
                break;
            case "FirstDayDormitory":
            case "Dormitory":
                locationName = "기숙사";
                break;
            case "Storage":
                locationName = "창고";
                break;
            case "PatrickLab":
                locationName = "패트릭 연구실";
                break;
            case "DayEnd":
                locationName = "기숙사";
                break;
            case "AndroidLab":
                locationName = "실험실";
                break;
            default:
                Debug.LogError("[OnSceneLoaded()] No case found. Add " + inSceneName);
                break;
        }

        return locationName;
    }

    public void OnSceneLoaded(string inSceneName, int inDayCount, bool bHaveReport)
    {
        locationText.text = GetLocationName(inSceneName);

        dayText.text = "Day " + inDayCount;

        { 
            if (inSceneName == "PatrickLab")
            {
                GO_reportButton.SetActive(false);
            }
            else
            {
                if (bHaveReport != GO_reportButton.activeSelf)
                {
                    GO_reportButton.SetActive(bHaveReport);
                }
            }
        }

        bool bNeedMapMenu = IsMapMenuNeeded(inSceneName, bHaveReport);
        if (bNeedMapMenu != GO_mapButton.activeSelf)
        {
            GO_mapButton.SetActive(bNeedMapMenu);
        }

        MapManager mapManager = GO_mapContainer.GetComponent<MapManager>();
        if (mapManager != null) 
        {
            // mapManager.OnSceneLoaded(inSceneName);  
        }
    }

    public void AlterActive(GameObject inObject)
    {
        inObject.SetActive(!inObject.activeSelf);
    }

    public void ToggleMapActive()
    {
        AlterActive(GO_mapButton);
    }

    public void TransferScene(string SceneName)
    {
        if (true == GameManager.Get().IsAfterCounsel())
        {
            string sceneName_AC = SceneName + "_AC";

            Scene scene = SceneManager.GetSceneByName(sceneName_AC);
            if (scene != null)
            {
                if (-1 != scene.buildIndex)
                    SceneName = sceneName_AC;
            }
        }

        // NEED EDIT! need to incorporate with MapManager.
        Scene currentScene = SceneManager.GetActiveScene();
        if (null != currentScene)
        {
            if ((currentScene.name == SceneName) ||
                (SceneName.Contains("Dormitory") && currentScene.name.Contains("Dormitory")) )
            {
                return;
            }
        }

        GO_mapContainer.SetActive(false);

        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetStringVariable("NextScene", SceneName);

                flowchart.SendFungusMessage("ToNextScene");
            }
        }
    }

    private bool IsMapMenuNeeded(string inSceneName, bool bInHaveReport)
    {
        switch (inSceneName)
        {
            case "LabCorridor_AC":
            case "CounselRoom":
            case "PatrickLab":
            case "AndroidLab":
            case "FirstDayDormitory":
            case "DayEnd":
                return false;
        }

        return true;
    }
}
