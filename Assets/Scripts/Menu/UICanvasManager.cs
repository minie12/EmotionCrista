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

    [SerializeField]
    private GameObject GO_reportButton;
    [SerializeField]
    private GameObject GO_mapButton;


    public void OnSceneLoaded(string inSceneName, int inDayCount, bool bAfterCounsel)
    {
        ChangeLocationName(inSceneName);

        dayText.text = "Day " + inDayCount;

        if (bAfterCounsel != GO_reportButton.activeSelf)
        {
            GO_reportButton.SetActive(bAfterCounsel);
        }

        bool bNeedMapMenu = IsMapMenuNeeded(inSceneName);
        if (bNeedMapMenu != GO_mapButton.activeSelf)
        {
            GO_mapButton.SetActive(bNeedMapMenu);
        }
    }

    public void AlterActive(GameObject inObject)
    {
        inObject.SetActive(!inObject.activeSelf);
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

    private void ChangeLocationName(string inSceneName)
    {
        string locationName = "";

        switch (inSceneName)
        {
            case "LabCorridor":
                locationName = "상담실 복도";
                break;
            case "CounselRoom":
                locationName = "상담실";
                break;
            case "Dormitory":
                locationName = "기숙사";
                break;
            default:
                Debug.LogError("[OnSceneLoaded()] No case found. Add " + inSceneName);
                break;
        }

        locationText.text = locationName;
    }

    private bool IsMapMenuNeeded(string inSceneName)
    {
        switch (inSceneName)
        {
            case "CounselRoom":
                return false;
                
            case "Dormitory":
                return false;
        }

        return true;
    }
}
