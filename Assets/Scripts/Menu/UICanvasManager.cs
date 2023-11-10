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


    public void OnSceneLoaded(string inSceneName, int inDayCount, bool bAfterCounsel)
    {
        ChangeLocationName(inSceneName);

        dayText.text = "Day " + inDayCount;

        GO_reportButton.SetActive(bAfterCounsel);
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
            case "Dormitory":
                locationName = "±â¼÷»ç";
                break;
            default:
                Debug.LogError("[OnSceneLoaded()] No case found. Add " + inSceneName);
                break;
        }

        locationText.text = locationName;
    }
}
