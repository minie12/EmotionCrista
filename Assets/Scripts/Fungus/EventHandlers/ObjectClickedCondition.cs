using System.Collections;
using UnityEngine;

namespace Fungus
{
    /// <summary>
    /// The block will execute when the user clicks or taps on the clickable object and meets the condition specified.
    /// </summary>
    [EventHandlerInfo("Sprite",
                      "Object Clicked And Condition",
                      "The block will execute when the user clicks or taps on the clickable object and meets the condition specified.")]
    [AddComponentMenu("")]
    public class ObjectClickedCondition : ObjectClicked
    {
        [Tooltip("Conditions to execute the block.")]
        [SerializeField] protected int storyRound = 1;
        [SerializeField] protected int characterIndex = 1;
        [SerializeField] protected bool afterCounsel = false;

        #region Public members

        /// <summary>
        /// Called by the Clickable2D object when it is clicked.
        /// </summary>
        public override void OnObjectClicked(Clickable2D clickableObject)
        {
            Flowchart flowchart = ParentBlock.GetFlowchart();

            int varStoryRound = flowchart.GetVariable<IntegerVariable>("StoryRound").Value;
            int varCharacterIndex = flowchart.GetVariable<IntegerVariable>("CharacterIndex").Value;
            bool varAfterCounsel= flowchart.GetVariable<BooleanVariable>("AfterCounsel").Value;
            if ((clickableObject == this.clickableObject) &&
                (varStoryRound == storyRound) && (varCharacterIndex == characterIndex) && (varAfterCounsel == afterCounsel))
            {
                StartCoroutine(DoExecuteBlock(waitFrames));
            }
        }

        public override string GetSummary()
        {
            if (clickableObject != null)
            {
                return clickableObject.name + "Conditioned";
            }

            return "None";
        }
        #endregion
    }
}