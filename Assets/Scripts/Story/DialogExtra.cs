using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class DialogExtra : MonoBehaviour
{
    private int razPoint;

    //public void GoToScene(string sceneName)
    //{
    //    SceneManager.LoadScene(sceneName);
    //}    

    private Fungus.Flowchart GetFlowchartOnScene()
    {
        GameObject flowchartObj = GameObject.Find("Flowchart");
        if (null != flowchartObj)
        {
            Fungus.Flowchart flowchart = flowchartObj.GetComponent<Fungus.Flowchart>();
            return flowchart;
        }

        return null;
    }

    public void TransferSceneWithFungus(string SceneName)
    {
        Fungus.Flowchart flowchart = GetFlowchartOnScene();

        if (null != flowchart)
        {
            flowchart.SetStringVariable("NextScene", SceneName);

            flowchart.SendFungusMessage("ToNextScene");
        }
    }

    public void SetCharacterName(string nameText_, Fungus.Character player) {
        player.SetStandardText(nameText_);
    }

    public void InitializeScene(Fungus.Flowchart flowchart)
    {
        if (null != flowchart)
        {
            // Load Save Data if has one
            {
                GameManager gameManager = GameManager.Get();
                gameManager.TryLoadData(flowchart);
            }
        }
    }

    //public void StartSaveData(){
    //    GameObject.Find("SaveLoadManager").GetComponent<SaveLoadManager>().SetSaveData();
    //}

    public void ExitGame() {
        Application.Quit();
    }

    public void AlterActive(GameObject go) {
        go.SetActive(!go.activeSelf);
    }

    public void ChangeParent(Transform parent, GameObject obj)
    {
        //Instantiate(clickableObj, location);
        obj.transform.SetParent(parent, false);
    }
    //public void EraseClickable(GameObject location)
    //{
    //    foreach (Transform children in location.transform)
    //    {
    //        GameObject.Destroy(children.gameObject);
    //    }
    //}

    //public void LoadPlayerPrefs()
    //{
    //    if (!PlayerPrefs.HasKey("LoadData") || PlayerPrefs.GetInt("LoadData") == 0)
    //        return;

    //    PlayerPrefs.SetInt("LoadData", 0);

    //    Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

    //    flowchart.SetStringVariable("PlayerName", PlayerPrefs.GetString("PlayerName"));
    //    flowchart.SetStringVariable("StoryNumb", PlayerPrefs.GetString("StoryNumb"));
    //}

    #region Minigame
    public void RestartPuzzle(string color, int gimmick)
    {
        GameObject.Find("MiniManager").GetComponent<MiniManager>().RestartGame(color, gimmick);
    }

    // minigame option (pause)
    public void OnTogglePause()
    {
        Time.timeScale = Time.timeScale == 0 ? 1 : 0;
    }

    // minigame option (restart)
    public void RestartPuzzleOption()
    {
        GameObject.Find("MiniManager").GetComponent<MiniManager>().RestartGamePause();
        GameObject.Find("PauseOptions").SetActive(false);
        Time.timeScale = 1;
    }
    #endregion

    #region LoveEnding
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
    #endregion

    #region Diary
    public void SetDiaryText(Text diaryText)
    {
        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();

        int varStoryRound = flowchart.GetVariable<Fungus.IntegerVariable>("StoryRound").Value;
        int varCharacterIndex = flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex").Value;

        int storyIndex = GameManager.instance.CreateStoryIndex(varStoryRound, varCharacterIndex);
        diaryText.text = DiaryDialogReader.GetDialogData(storyIndex);
    }
    #endregion
}

