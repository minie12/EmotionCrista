using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LoadManager : MonoBehaviour
{
    private EmoSaveData loadEmoSaveData;
    public EmoSaveData LoadEmoSaveData { set { loadEmoSaveData = value; } }

    public bool HaveLoadData()
    {
        return null != loadEmoSaveData;
    }

    public bool LoadGameData()
    {
        if (null == loadEmoSaveData)
        {
            Debug.Log("SceneLoaded : No game data to be loaded");

            // No data to be loaded.
            return true;
        }

        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null == GO_flowchart) return false;

        Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
        if (null == flowchart) return false;

        flowchart.StopAllBlocks();

        // Screen Info
        {
            Fungus.Stage activeStage = Fungus.Stage.GetActiveStage();
            if (null != activeStage)
            {
                foreach (KeyValuePair<string, SerializablePortraitState> portraitState in loadEmoSaveData.portraitStates)
                {
                    string activeCharacterName = portraitState.Key;

                    GameObject activeCharacterObj = GameObject.Find(activeCharacterName);
                    if (null != activeCharacterObj)
                    {
                        Fungus.Character activeCharacter = activeCharacterObj.GetComponent<Fungus.Character>();

                        if (null != activeCharacter)
                        {
                            Sprite activeSprite = null;
                            RectTransform activeTransform = null;

                            foreach (var characterSprite in activeCharacter.Portraits)
                            {
                                if (portraitState.Value.portraitName == characterSprite.name)
                                {
                                    activeSprite = characterSprite;
                                    break;
                                }
                            }

                            foreach (var stageRect in activeStage.Positions)
                            {
                                if (portraitState.Value.positionName == stageRect.gameObject.name)
                                {
                                    activeTransform = stageRect;
                                    break;
                                }
                            }

                            if (null != activeSprite && null != activeTransform)
                            {
                                Fungus.PortraitOptions options = new Fungus.PortraitOptions();

                                options.character = activeCharacter;
                                options.portrait = activeSprite;
                                options.toPosition = activeTransform;
                                options.dim = portraitState.Value.bDimmed;

                                activeStage.Show(options);
                            }
                        }
                    }
                }
            }
        }

        // Dialogue
        {
            Fungus.SayDialog activeSayDialog = Fungus.SayDialog.GetSayDialog();
            if (null != activeSayDialog)
            {
                string dialogueNameText = loadEmoSaveData.dialogueNameText;
                if (false == string.IsNullOrEmpty(dialogueNameText))
                {
                    activeSayDialog.NameText = dialogueNameText;
                    activeSayDialog.StoryText = loadEmoSaveData.dialogueStoryText;
                }
            }
        }

        // Fungus Variable
        {
            GameManager.Get().SetPlayInfo(loadEmoSaveData.playInfo);
        }

        // Fungus Command
        {
            string blockName = loadEmoSaveData.blockName;
            int commandId = loadEmoSaveData.commandId;

            if (false == string.IsNullOrEmpty(blockName) && commandId != -1)
            {
                Fungus.Block executeBlock = flowchart.FindBlock(blockName);
                if (null != executeBlock)
                {
                    if (true == executeBlock.IsExecuting())
                    {
                        executeBlock.Stop();
                    }

                    flowchart.ExecuteBlock(executeBlock, commandId);
                }
            }
        }

        // Delete Load Info so that it won't load again
        loadEmoSaveData = null;

        return true;
    }


}
