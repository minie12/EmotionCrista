using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SettingUICanvasManager : MonoBehaviour
{
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
}
