using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TabletManager : MonoBehaviour
{
    public int surveyCnt;
    private string fungusMessage = "D1_NariaSurvey";

    // contents
    public GameObject[] contents;

    public void IncreaseSurveyCnt(int n) { surveyCnt += n; }

    public void FinishSurvey(){
        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        int varStoryRound = flowchart.GetVariable<Fungus.IntegerVariable>("StoryRound").Value;
        int varCharacterIndex = flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex").Value;

        string message = fungusMessage;
        
        if (varCharacterIndex != 1)
        {
            //string characterName = GameManager.GetCharacterName(varCharacterIndex);
            //message = "D" + varStoryRound + "_" + characterName + "Survey";
            message = "D1_LulianSurvey";
        }
        
        if(surveyCnt >= 4) {
            surveyCnt = 0;
            Fungus.Flowchart.BroadcastFungusMessage(message);
        }
    }

    public void AlterContent(Toggle toggle){
        Fungus.Flowchart.BroadcastFungusMessage("SurveyAnim");
    }
}
