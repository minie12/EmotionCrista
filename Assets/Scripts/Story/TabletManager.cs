using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    public int survey_cnt;
    private string fungus_message = "D01_NariaSurvey";

    // contents
    public GameObject[] contents;

    public void FinishSurvey(){
        if(survey_cnt >= 4) {
            survey_cnt = 0;
            Fungus.Flowchart.BroadcastFungusMessage(fungus_message);
        }
    }

    public void AlterContent(Toggle toggle){
        Fungus.Flowchart.BroadcastFungusMessage("SurveyAnim");
    }
}
