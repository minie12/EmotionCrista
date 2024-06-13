using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using static ScreenObjectInfo;

[System.Serializable]
public class SerializablePortraitState
{
    public bool bDimmed;
    public string positionName;
    public string portraitName;
}

[System.Serializable]
public class EmoSaveData
{
    #region Public Variables
    // Screen Info
    public string sceneName;

    // Fungus Command
    public string blockName;
    public int commandId;

    // Fungus Variable
    public PlayInfo playInfo;

    // Dialogue Text
    public string dialogueNameText;
    public string dialogueStoryText;

    // Characters on Diaglogue Screen
    public Dictionary<string, SerializablePortraitState> portraitStates = new Dictionary<string, SerializablePortraitState>();

    // Point&Click Objects 
    //public ClickableID[] locatedClickableObjects;
    //public byte objectClickedFlag;
    #endregion

    public bool ValidateData()
    {
        if ((false == string.IsNullOrEmpty(sceneName)) && (false == string.IsNullOrEmpty(playInfo.playerName)))
        {
            return true;
        }

        return false;
    }

    static public EmoSaveData CreateSaveData()
    {
        EmoSaveData gameData = new EmoSaveData();

        // Scene Name
        Scene currentScene = SceneManager.GetActiveScene();
        if (null != currentScene)
        {
            gameData.sceneName = currentScene.name;
        }
        else
        {
            return null;
        }

        // Screen Info
        Fungus.Stage activeStage = Fungus.Stage.GetActiveStage();
        if (null != activeStage)
        {
            List<Fungus.Character> activeCharacters = activeStage.CharactersOnStage;

            for (int charIdx = 0; charIdx < activeCharacters.Count; ++charIdx)
            {
                Fungus.Character character = activeCharacters[charIdx];
                if (null != character)
                {
                    Fungus.PortraitState currentPortraitState = character.State;
                    if (null != currentPortraitState)
                    {
                        if (true == currentPortraitState.onScreen)
                        {
                            string portraitCharacterName = character.gameObject.name;

                            if (0 < portraitCharacterName.Length)
                            {
                                SerializablePortraitState serializablePortraitState = new SerializablePortraitState();

                                serializablePortraitState.bDimmed = currentPortraitState.dimmed;

                                foreach (var stageRect in activeStage.Positions)
                                {
                                    if (stageRect == currentPortraitState.position)
                                    {
                                        serializablePortraitState.positionName = currentPortraitState.position.gameObject.name;
                                        break;
                                    }
                                }

                                serializablePortraitState.portraitName = currentPortraitState.portraitImage.gameObject.name;

                                gameData.portraitStates.Add(portraitCharacterName, serializablePortraitState);
                            }
                        }
                    }

                }
            }
        }

        // Dialogue
        Fungus.SayDialog activeSayDialog = Fungus.SayDialog.GetSayDialog();
        if (null != activeSayDialog)
        {
            string activeNameText = activeSayDialog.NameText;
            if (0 < activeNameText.Length)
            {
                string activeStoryText = activeSayDialog.StoryText;
                if (0 < activeStoryText.Length)
                {
                    gameData.dialogueNameText = activeNameText;
                    gameData.dialogueStoryText = activeStoryText;
                }
            }
        }

        // Fungus Command
        GameObject flowchartObj = GameObject.Find("Flowchart");
        if (null != flowchartObj)
        {
            Fungus.Flowchart flowchart = flowchartObj.GetComponent<Fungus.Flowchart>();

            if (null != flowchart)
            {
                // Fungus Command
                List<Fungus.Block> executingBlocks = flowchart.GetExecutingBlocks();
                for (int blockIdx = 0; blockIdx < executingBlocks.Count; ++blockIdx)
                {
                    Fungus.Block executingBlock = executingBlocks[blockIdx];
                    if (null != executingBlock)
                    {
                        Fungus.Command activeCommand = executingBlock.ActiveCommand;
                        if (null != activeCommand)
                        {
                            if (-1 != activeCommand.ItemId)
                            {
                                gameData.blockName = executingBlock.BlockName;
                                gameData.commandId = activeCommand.CommandIndex;
                            }
                        }
                    }
                }
            }
        }

        // Set Current SceneInfo to PlayInfo before saving
        {
            // Clickable Object Info
            GameObject GO_ClickableLocation = GameObject.Find("ClickableLocation");
            if (null != GO_ClickableLocation)
            {
                ScreenObjectInfo screenObjectInfo = GO_ClickableLocation.GetComponent<ScreenObjectInfo>();
                if (null != screenObjectInfo)
                {
                    screenObjectInfo.SaveSceneScreenInfo();
                }
            }
        }

        // PlayInfo
        GameManager.Get().GetPlayInfo(ref gameData.playInfo);

        if (true == gameData.ValidateData())
        {
            return gameData;
        }

        return null;
    }

    // Getter Setter
    public string SceneName { get { return sceneName; } set { sceneName = value; } }
}
