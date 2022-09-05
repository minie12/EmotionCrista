using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogExtra : MonoBehaviour
{
    public void SetCharacterName(string nameText_, Fungus.Character player){        
        player.SetStandardText(nameText_);
    }

    public void RestartPuzzle(string color, int gimmick, string message){
        GameObject.Find("MiniManager").GetComponent<MiniManager>().RestartGame(color, gimmick, message);
    }

    public void StartSaveData(Fungus.Flowchart flowchart){
        GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>().SetSaveData(flowchart.Variables);
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void AlterActive(GameObject go){
        go.SetActive(!go.activeSelf);
    }
}

