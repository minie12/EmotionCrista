using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static ScreenObjectInfo;
using System;
using Fungus;

public class DialogExtra : MonoBehaviour
{
    private int razPoint;

    #region StoryCondition
    public void IsMenuDialogActive()
    {
        bool bCondition = false;

        GameObject menuDialogObj = GameObject.Find("MenuDialog");
        if (null != menuDialogObj)
        {
            bCondition = menuDialogObj.activeSelf;
        }

        SetBoolCondition(bCondition);
    }

    public void IsObjectClicked(ClickableID inObjectFlag)
    {
        bool bCondition = GameManager.Get().IsObjectClicked(inObjectFlag);

        SetBoolCondition(bCondition);
    }

    public void IsObjectClicked(string inSceneName, ClickableID inObjectFlag)
    {
        bool bCondition = GameManager.Get().IsObjectClicked(inSceneName, inObjectFlag);

        SetBoolCondition(bCondition);
    }

    public void IsMinigameHistoryEqual(int inHistory)
    {
        bool bCondition = (inHistory == GameManager.Get().GetMinigameHistory());

        SetBoolCondition(bCondition);
    }

    public void HasStoryConditionState(StoryConditionState inState)
    {
        bool bCondition = (GameManager.Get().HasStoryConditionState(inState));

        SetBoolCondition(bCondition);
    }

    private void SetBoolCondition(bool bInCondition)
    {
        // Set Fungus variables to PlayInfo
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetBooleanVariable("BoolCondition", bInCondition);
            }
        }
    }
    #endregion

    #region Area
    public void ShowAllClickables()
    {
        Area currentArea = GameManager.Get().GetArea();

        if (null != currentArea)
        {
            currentArea.ChangeAllClickablesVisibility(true);
        }
    }

    public void HideAllClickables()
    {
        Area currentArea = GameManager.Get().GetArea();

        if (null != currentArea)
        {
            currentArea.ChangeAllClickablesVisibility(false);
        }
    }

    public void SetClickableObject(ClickableID inObjectID, int inLocationIndex)
    {
        Area currentArea = GameManager.Get().GetArea();

        if (null != currentArea)
        {
            currentArea.SetClickableObject(inObjectID, inLocationIndex);
        }
    }

    public void CleanClickableObject(ClickableID inObjectID)
    {
        Area currentArea = GameManager.Get().GetArea();

        if (null != currentArea)
        {
            currentArea.CleanClickableObject(inObjectID);
        }
    }

    public void OnObjectClicked(ClickableID inClickableID)
    {
        Area currentArea = GameManager.Get().GetArea();

        if (null != currentArea)
        {
            currentArea.OnObjectClicked(inClickableID);
        }
    }
    #endregion

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

    public void SetCharacterName(Text nameText, Fungus.Character player) 
    {
        GameManager.Get().SetPlayerName(nameText.text);
        player.SetStandardText(nameText.text);
    }

    public void ToggleMapButton()
    {
        GameObject GO_UICanvas = GameObject.Find("GameUICanvas");
        if (null != GO_UICanvas)
        {
            UICanvasManager UICanvasManager = GO_UICanvas.GetComponent<UICanvasManager>();
            if (null != UICanvasManager)
            {
                UICanvasManager.ToggleMapActive();
            }
        }
    }

    public void ExitGame() {
        Application.Quit();
    }

    public void AlterActive(GameObject go) {
        go.SetActive(!go.activeSelf);
    }

    public void SetMiniGameLevel(int inGameLevel)
    {
        GameManager.Get().SetGameLevel(inGameLevel);
    }

    public void SetStartBlockCompletion(bool bInCompletion)
    {
        GameManager.Get().SetStartBlockCompletion(bInCompletion);
    }

    public void ResetAfterMinigame()
    {
        GameManager.Get().ResetAfterMinigame();
    }

    public void SetStoryConditionState(StoryConditionState inState)
    {
        GameManager.Get().SetStoryConditionState(inState);
    }

    #region Minigame
    public void RestartPuzzle()
    {
        GameObject MiniManagerObj = GameObject.Find("MiniManager");

        if (null != MiniManagerObj)
        {
            MiniManager MiniManager = MiniManagerObj.GetComponent<MiniManager>();

            if (null != MiniManager)
            {
                MiniManager.RestartGame();
            }
        }
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

        //int storyIndex = GameManager.instance.CreateStoryIndex(varStoryRound, varCharacterIndex);
        //diaryText.text = DiaryDialogReader.GetDialogData(storyIndex);
    }
    #endregion
}

