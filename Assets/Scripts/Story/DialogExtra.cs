using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DialogExtra : MonoBehaviour
{
    private int raz_point;

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
        raz_point = 0;
    }

    public void AddRazPoint()
    {
        raz_point += 1;
    }

    public void ChangeDialogImage(Sprite change_sp, Color change_color, Font change_font)
    {
        GameObject.Find("SayDialog").GetComponentInChildren(typeof(Image)).GetComponent<Image>().sprite = change_sp;
        GameObject.Find("NameText").GetComponent<Outline>().effectColor = change_color;
        GameObject.Find("StoryText").GetComponent<Text>().font = change_font;
        GameObject.Find("StoryText").GetComponent<RectTransform>().localPosition = new Vector3(200, 200, 0);
    }

    public void ChangeUIImage(Sprite change_sp)
    {
        GameObject.Find("Setting").GetComponent<Image>().sprite = change_sp;
    }
    // --------------------------------------------------------------------
}

