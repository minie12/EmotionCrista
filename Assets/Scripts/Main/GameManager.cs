using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//public enum GameState
//{
//    StartMenu,
//    StoryMode,
//    MiniGameMode
//}

public enum StoryRound
{
    None,
    First,
    Second,
    Error
}
public enum CharacterName
{
    None = 0,
    Naria,
    Lulian,
    Russel,
    Nish,
    Ilrak,
    Max
}

public class GameManager : MonoBehaviour
{
    public static GameManager instance;
    //public GameState state;

    //private StoryRound currentRound = StoryRound.First;
    //private CharacterName currentCharacter = CharacterName.Naria;

    //private GameObject fungusManager;
    
    // sound
    public float soundVolumeBGM;
    public float soundVolumeSFX;

    // SaveData
    private EmoSaveData loadEmoSaveData;

    // Start is called before the first frame update
    void Awake()
    {
        Screen.SetResolution(1920, 1080, true);
        if(instance == null) // If there is no instance already
        {
            Debug.Log("inst");
            DontDestroyOnLoad(gameObject); // Keep the GameObject, this component is attached to, across different scenes
            instance = this;  
            //fungusManager = GameObject.Find("FungusManager");

        } else if(instance != this) // If there is already an instance and it's not `this` instance
        {
            Destroy(gameObject); // Destroy the GameObject, this component is attached to
        }
    }

    #region StoryIndex
    public int CreateStoryIndex(int inRound, int inPatient)
    {
        return inRound * 100 + inPatient;
    }
    public int CreateStoryIndex(int inRound, string inCharacter)
    {
        CharacterName characterIndex = CharacterName.Naria;
        for (; characterIndex < CharacterName.Max; characterIndex++)
        {
            if (inCharacter == characterIndex.ToString())
            {
                break;
            }
        }

        Debug.Assert(0 < inRound && inRound < 3 && characterIndex != CharacterName.Max, "Wrong story round or character name in json file");

        return CreateStoryIndex(inRound, (int)characterIndex);
    }
    #endregion

    public virtual EmoSaveData LoadEmoSaveData { set { loadEmoSaveData = value; } }

    public bool TryLoadData(Fungus.Flowchart inFlowchart)
    {
        if (null == loadEmoSaveData)
        {
            return true;
        }

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
                if (0 < dialogueNameText.Length)
                {
                    activeSayDialog.NameText = dialogueNameText;
                    activeSayDialog.StoryText = loadEmoSaveData.dialogueStoryText;

                }
            }
        }

        // Fungus Variable
        {
            inFlowchart.SetStringVariable("PlayerName", loadEmoSaveData.playerName);
            inFlowchart.SetIntegerVariable("StoryRound", loadEmoSaveData.storyRound);
            inFlowchart.SetIntegerVariable("CharacterIndex", loadEmoSaveData.characterIndex);
            inFlowchart.SetBooleanVariable("AfterCounsel", loadEmoSaveData.afterCounsel);
        }

        // Fungus Command
        {
            string blockName = loadEmoSaveData.blockName;
            int commandId = loadEmoSaveData.commandId;

            if (blockName.Length <= 0 || commandId == -1)
            {
                Debug.LogError("Try Load Data: Invalid block id / command id");
            }
            else
            {
                Fungus.Block executeBlock = inFlowchart.FindBlock(blockName);
                if (null != executeBlock)
                {
                    if (true == executeBlock.IsExecuting())
                    {
                        executeBlock.Stop();
                    }

                    inFlowchart.ExecuteBlock(executeBlock, commandId);
                }
            }
        }

        // Delete Load Info so that it won't load again
        loadEmoSaveData = null;

        return true;
    }

    static public GameManager Get()
    {
        return instance;
    }

    static public string GetCharacterName(int characterIndex)
    {
        return ((CharacterName)characterIndex).ToString();
    }
}