using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class UICanvasManager : MonoBehaviour
{
    [SerializeField]
    private Text locationText;
    [SerializeField]
    private Text dayText;

    public GameObject GO_reportButton;
    public GameObject GO_mapButton;

    public Toggle fullScreenToggle;
    public Toggle windowToggle;

    static public string GetLocationName(string inSceneName)
    {
        string locationName = inSceneName;

        switch (inSceneName)
        {
            case "Lobby":
                locationName = "로비";
                break;
            case "LabCorridor":
                locationName = "상담실 복도";
                break;
            case "CounselRoom":
                locationName = "상담실";
                break;
            case "Dormitory":
                locationName = "기숙사";
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
    }

    public void AlterActive(GameObject inObject)
    {
        inObject.SetActive(!inObject.activeSelf);
    }

    public void ChangeScreenSetting(bool bFullscreen)
    {
        if (true == bFullscreen)
        {
            Debug.Log("Fullscreen");
        }
        else
        {
            Debug.Log("Window");
        }
    }

    public void TransferScene(string inSceneName)
    {
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetStringVariable("NextScene", inSceneName);

                flowchart.SendFungusMessage("ToNextScene");
            }
        }
    }

    private bool IsMapMenuNeeded(string inSceneName, bool bInHaveReport)
    {
        switch (inSceneName)
        {
            case "LabCorridor":
                if (bInHaveReport) return false;
                break;

            case "CounselRoom":
                return false;

            case "PatrickLab":
                return false;

            case "AndroidLab":
                return false;

            case "DayEnd":
                return false;
        }

        return true;
    }
}
