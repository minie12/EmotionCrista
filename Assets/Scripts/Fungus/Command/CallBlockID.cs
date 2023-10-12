using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
