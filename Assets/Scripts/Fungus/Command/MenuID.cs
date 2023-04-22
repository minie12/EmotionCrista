using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Serialization;

namespace Fungus
{
    /// <summary>
    /// Displays a button in a multiple choice menu.
    /// </summary>
    [CommandInfo("Narrative",
                 "MenuID",
                 "Displays a button in a multiple choice menu")]
    [AddComponentMenu("")]
    public class MenuID : Command, ILocalizable, IBlockCaller
    {
        [SerializeField] protected string textID = "";

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [TextArea()]
        [SerializeField] protected string description = "";

        [FormerlySerializedAs("targetSequence")]
        [Tooltip("Block to execute when this option is selected")]
        [SerializeField] protected Block targetBlock;

        [Tooltip("Hide this option if the target block has been executed previously")]
        [SerializeField] protected bool hideIfVisited;

        [Tooltip("If false, the menu option will be displayed but will not be selectable")]
        [SerializeField] protected BooleanData interactable = new BooleanData(true);

        [Tooltip("A custom Menu Dialog to use to display this menu. All subsequent Menu commands will use this dialog.")]
        [SerializeField] protected MenuDialog setMenuDialog;

        [Tooltip("If true, this option will be passed to the Menu Dialogue but marked as hidden, this can be used to hide options while maintaining a Menu Shuffle.")]
        [SerializeField] protected BooleanData hideThisOption = new BooleanData(false);

        protected string storyText = "";

        #region Public members

        public MenuDialog SetMenuDialog { get { return setMenuDialog; } set { setMenuDialog = value; } }

        public override void OnEnter()
        {
            if (setMenuDialog != null)
            {
                // Override the active menu dialog
                MenuDialog.ActiveMenuDialog = setMenuDialog;
            }

            bool hideOption = (hideIfVisited && targetBlock != null && targetBlock.GetExecutionCount() > 0) || hideThisOption.Value;

            var menuDialog = MenuDialog.GetMenuDialog();
            if (menuDialog != null)
            {
                menuDialog.SetActive(true);

                var flowchart = GetFlowchart();

                DialogData data = StoryDialogReader.GetDialogData(textID);
                storyText = data.dialog;
                string displayText = flowchart.SubstituteVariables(storyText);

                menuDialog.AddOption(displayText, interactable, hideOption, targetBlock);
            }

            Continue();
        }

        public override void GetConnectedBlocks(ref List<Block> connectedBlocks)
        {
            if (targetBlock != null)
            {
                connectedBlocks.Add(targetBlock);
            }
        }

        public override string GetSummary()
        {
            if (targetBlock == null)
            {
                return "Error: No target block selected";
            }

            return textID + "( " + targetBlock.BlockName + "): " + description;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override bool HasReference(Variable variable)
        {
            return interactable.booleanRef == variable || hideThisOption.booleanRef == variable ||
                base.HasReference(variable);
        }

        public bool MayCallBlock(Block block)
        {
            return block == targetBlock;
        }

        #endregion

        #region ILocalizable implementation

        public virtual string GetStandardText()
        {
            return storyText;
        }

        public virtual void SetStandardText(string standardText)
        {
            storyText = standardText;
        }

        public virtual string GetDescription()
        {
            return description;
        }

        public virtual string GetStringId()
        {
            // String id for Menu commands is MENU.<Localization Id>.<Command id>
            return "MENUID." + GetFlowchartLocalizationId() + "." + itemId;
        }

        #endregion

        #region Editor caches
#if UNITY_EDITOR
        protected override void RefreshVariableCache()
        {
            base.RefreshVariableCache();

            var f = GetFlowchart();

            f.DetermineSubstituteVariables(storyText, referencedVariables);
        }
#endif
        #endregion Editor caches
    }
}
