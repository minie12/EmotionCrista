using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    public int surveyCnt;

    // contents
    public GameObject[] contents;

    public void IncreaseSurveyCnt(int n) { surveyCnt += n; }
    
    public void AlterContent(Toggle toggle)
    {
        Fungus.Flowchart.BroadcastFungusMessage("SurveyAnim");
    }

    public void FinishSurvey(){
        if(surveyCnt >= 5) {
            surveyCnt = 0;
            GameManager.Get().SetHaveReport(false);
            Fungus.Flowchart.BroadcastFungusMessage("SurveyEnded");
        }
    }


}
