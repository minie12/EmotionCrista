using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

[System.Serializable]
public class EmoSaveData
{
    // Screen Info
    protected string sceneName;

    // Characters on Screen
    protected List<Fungus.Character> activeCharacters;

    // Fungus Command
    protected string blockName;
    protected int commandId;

    // Fungus Variable
    protected string playerName;
    protected int storyRound;
    protected int characterIndex;
    protected bool afterCounsel;
    protected string nextScene;

    EmoSaveData(string inSceneName, string inBlockName, int inCommandId, 
                string inPlayerName, int inStoryRound, int inCharacterIndex, bool inAfterCounsel, string inNextScene, 
                List<Fungus.Character> inActiveCharacters)
    {
        sceneName = inSceneName;
        blockName = inBlockName;
        commandId = inCommandId;
        playerName = inPlayerName;
        storyRound = inStoryRound;
        characterIndex = inCharacterIndex;
        afterCounsel = inAfterCounsel;
        nextScene = inNextScene;
        activeCharacters = inActiveCharacters;
    }

    public bool ValidateData()
    {
        if ((0 < sceneName.Length) && (0 < blockName.Length) && (-1 < commandId)
            && (0 < playerName.Length) && (-1 < storyRound) && (-1 < characterIndex) && (0 < nextScene.Length))
        {
            return true;
        }

        return false;
    }

    static public EmoSaveData CreateSaveData()
    {
        // Init savedata variables
        string inSceneName, inBlockName, inPlayerName, inNextScene;
        inSceneName = inBlockName = inPlayerName = inNextScene = "";

        int inCommandId, inStoryRound, inCharacterIndex;
        inCommandId = inStoryRound = inCharacterIndex = 0;

        List<Fungus.Character> inActiveCharacters = new List<Fungus.Character>();
        bool inAfterCounsel = false;

        // Scene Name
        Scene currentScene = SceneManager.GetActiveScene();
        if (null != currentScene)
        {
            inSceneName = currentScene.name;
        }
        else
        {
            return null;
        }

        // Screen Info
        Fungus.Stage activeStage = Fungus.Stage.GetActiveStage();
        if (null != activeStage)
        {
            inActiveCharacters = activeStage.CharactersOnStage;
        }
        else
        {
            return null;
        }

        // Fungus Command & Variable
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
                                inBlockName = executingBlock.BlockName;
                                inCommandId = activeCommand.CommandIndex;
                            }
                        }
                    }
                }

                // Fungus Variable
                inPlayerName = flowchart.GetVariable<Fungus.StringVariable>("PlayerName").Value;
                inStoryRound = flowchart.GetVariable<Fungus.IntegerVariable>("StoryRound").Value;
                inCharacterIndex = flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex").Value;
                inAfterCounsel = flowchart.GetVariable<Fungus.BooleanVariable>("AfterCounsel").Value;
                inNextScene = flowchart.GetVariable<Fungus.StringVariable>("NextScene").Value;
            }
        }
        else 
        {
            return null;
        }

        EmoSaveData gameData = new EmoSaveData(inSceneName, inBlockName, inCommandId,
                                    inPlayerName, inStoryRound, inCharacterIndex, inAfterCounsel, inNextScene,
                                    inActiveCharacters);

        if (gameData.ValidateData())
        {
            return gameData;

        }

        return null;
    }

    // Getter Setter
    public string SceneName { get { return sceneName; } set { sceneName = value; } }
    public string PlayerName { get { return playerName; } set { playerName = value; } }
    public int CharacterIndex { get { return characterIndex; } set { characterIndex = value; } }
    public int StoryRound { get { return storyRound; } set { storyRound = value; } }

    /*
    List<Fungus.Character> activeCharacters;
    protected string blockName;
    protected int commandId;
    protected bool afterCounsel;
    protected string nextScene;
    */
}


namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Flow",
                 "CallBlockID",
                 "Call specific command in the block.")]
    [AddComponentMenu("")]
    public class CallBlockID : Command
    {
        [SerializeField] protected string blockId = "";

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [SerializeField] protected int indexId = 0;

        [SerializeField] protected Flowchart flowchart;

        public override void OnEnter()
        {
            #region Load
            /*
            var executeBlock = flowchart.FindBlock(blockId);
            if (null != executeBlock)
            {
                flowchart.ExecuteBlock(executeBlock, indexId);
            }*/
            #endregion

            SayDialog activeDialog = SayDialog.GetSayDialog();
            if (null != activeDialog)
            {
                string storyText = activeDialog.StoryText;

                Character speakingCharacter = activeDialog.SpeakingCharacter;
            }

            #region Save
            if (null == flowchart)
            {
                GameObject flowchartObj = GameObject.Find("Flowchart");
                if (null != flowchartObj)
                {
                    flowchart = flowchartObj.GetComponent<Flowchart>();
                    if (null == flowchart)
                    {
                        Debug.LogError("CallBlockID.cs: No Flowchart found in this Scene");
                    }
                }
            }

            List<Block> executingBlocks = flowchart.GetExecutingBlocks();
            for (int blockIdx = 0; blockIdx < executingBlocks.Count; blockIdx++)
            {
                Block executingBlock = executingBlocks[blockIdx];
                if (null != executingBlock)
                {
                    Command activeCommand = executingBlock.ActiveCommand;
                    if (null != activeCommand)
                    {
                        if (-1 != activeCommand.ItemId)
                        {
                            blockId = executingBlock.BlockName;
                            indexId = activeCommand.CommandIndex;
                        }
                    }
                }
            }

            if (blockId == "" || indexId == -1)
            {
                Debug.Log("ERROR: COULD NOT FIND EXECUTING COMMAND");
            }
            else
            {
                
                Block executeBlock = flowchart.FindBlock(blockId);
                if (null != executeBlock)
                {
                    if (true == executeBlock.IsExecuting())
                    {
                        executeBlock.Stop();
                    }
                        
                    //Scene scene = SceneManager.GetActiveScene(); SceneManager.LoadScene(scene.name);
                    flowchart.ExecuteBlock(executeBlock, indexId-2);
                }
            }
            #endregion
        }
    }
}
