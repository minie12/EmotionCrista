using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogExtra : MonoBehaviour
{
    private int razPoint;

    public void SetCharacterName(string nameText_, Fungus.Character player){        
        player.SetStandardText(nameText_);
    }

    public void RestartPuzzle(string color, int gimmick, string message){
        GameObject.Find("MiniManager").GetComponent<MiniManager>().RestartGame(color, gimmick, message);
    }

    public void StartSaveData(){
        GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>().SetSaveData();
    }

    public void ExitGame(){
        Application.Quit();
    }

    public void AlterActive(GameObject go){
        go.SetActive(!go.activeSelf);
    }

    public void OnTogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    public void LoadPlayerPrefs()
    {
        if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0)
            return;

        PlayerPrefs.SetInt("LoadData", 0);

        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        flowchart.SetStringVariable("PlayerName", PlayerPrefs.GetString("PlayerName"));
        flowchart.SetStringVariable("StoryNumb", PlayerPrefs.GetString("StoryNumb"));
    }

    //------------------ LOVE ENDING --------------------------------------
    public void StartRazEnding()
    {
        razPoint = 0;
    }

    public void AddRazPoint()
    {
        razPoint += 1;
    }

    public void ChangeDialogImage(Sprite changeSP, Color changeColor, Font changeFont)
    {
        GameObject.Find("SayDialog").GetComponentInChildren(typeof(Image)).GetComponent<Image>().sprite = changeSP;
        GameObject.Find("NameText").GetComponent<Outline>().effectColor = changeColor;
        GameObject.Find("StoryText").GetComponent<Text>().font = changeFont;
        GameObject.Find("StoryText").GetComponent<RectTransform>().localPosition = new Vector3(200, 200, 0);
    }

    public void ChangeUIImage(Sprite changeSP)
    {
        GameObject.Find("Setting").GetComponent<Image>().sprite = changeSP;
    }
    // --------------------------------------------------------------------
}

