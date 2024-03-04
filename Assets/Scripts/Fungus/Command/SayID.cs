using UnityEngine;
using System.Collections.Generic;

namespace Fungus
{
    /// <summary>
    /// Writes text in a dialog box.
    /// </summary>
    [CommandInfo("Narrative",
                 "SayID",
                 "Set dialog and character sprite. Writes text in a dialog box.")]
    [AddComponentMenu("")]
    public class SayID : Command, ILocalizable
    {
        [SerializeField] protected string textID = "";

        [Tooltip("Notes about the option text for other authors, localization, etc.")]
        [TextArea()]
        [SerializeField] protected string description = "";

        #region Variable: Say

        protected string storyText = "";
        protected Character character;
        protected Sprite portrait;
        protected AudioClip voiceOverClip;
        protected bool showAlways = true;
        protected int showCount = 1;
        protected bool extendPrevious = false;
        protected bool fadeWhenDone = true;
        protected bool waitForClick = true;
        protected bool stopVoiceover = true;
        protected bool waitForVO = false;

        //add wait for vo that overrides stopvo

        protected SayDialog setSayDialog;

        protected int executionCount;
        #endregion

        #region Public members

        /// <summary>
        /// Character that is speaking.
        /// </summary>
        public virtual Character _Character { get { return character; } }

        /// <summary>
        /// Portrait that represents speaking character.
        /// </summary>
        public virtual Sprite Portrait { get { return portrait; } set { portrait = value; } }

        /// <summary>
        /// Type this text in the previous dialog box.
        /// </summary>
        public virtual bool ExtendPrevious { get { return extendPrevious; } }

        public override void OnEnter()
        {
            DialogData data = StoryDialogReader.GetDialogData(textID);
            
            character = GameObject.Find(data.character).GetComponent<Character>();
            storyText = data.dialog;

            #region Method: Say
            if (!showAlways && executionCount >= showCount)
            {
                Continue();
                return;
            }

            executionCount++;

            // Override the active say dialog if needed
            if (character != null && character.SetSayDialog != null)
            {
                SayDialog.ActiveSayDialog = character.SetSayDialog;
            }

            if (setSayDialog != null)
            {
                SayDialog.ActiveSayDialog = setSayDialog;
            }

            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                Continue();
                return;
            }

            var flowchart = GetFlowchart();

            sayDialog.SetActive(true);

            sayDialog.SetCharacter(character);
            // sayDialog.SetCharacterImage(portrait);

            string displayText = storyText;

            var activeCustomTags = CustomTag.activeCustomTags;
            for (int i = 0; i < activeCustomTags.Count; i++)
            {
                var ct = activeCustomTags[i];
                displayText = displayText.Replace(ct.TagStartSymbol, ct.ReplaceTagStartWith);
                if (ct.TagEndSymbol != "" && ct.ReplaceTagEndWith != "")
                {
                    displayText = displayText.Replace(ct.TagEndSymbol, ct.ReplaceTagEndWith);
                }
            }

            string subbedText = flowchart.SubstituteVariables(displayText);

            sayDialog.Say(subbedText, !extendPrevious, waitForClick, fadeWhenDone, stopVoiceover, waitForVO, voiceOverClip, delegate {
                Continue();
            });
            #endregion
        }

        public override void Continue()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog != null)
            {
                string speaker = sayDialog.NameText;
                string dialogue = sayDialog.StoryText;

                if (0 < dialogue.Length)
                {
                    if (speaker.Length <= 0)
                    {
                        speaker = "system";
                    }

                    LogCache.AddLog(speaker, dialogue);
                }
            }

            // This is a noop if the Block has already been stopped
            if (IsExecuting)
            {
                Continue(CommandIndex + 1);
            }
        }


        public override string GetSummary()
        {
            string namePrefix = "";
            if (textID != null)
            {
                namePrefix += textID + ": ";
            }
            if (description != null)
            {
                namePrefix += description + " ";
            }

            return namePrefix;
        }

        public override Color GetButtonColor()
        {
            return new Color32(184, 210, 235, 255);
        }

        public override void OnReset()
        {
            executionCount = 0;
        }

        public override void OnStopExecuting()
        {
            var sayDialog = SayDialog.GetSayDialog();
            if (sayDialog == null)
            {
                return;
            }

            sayDialog.Stop();
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
            // String id for Say commands is SAY.<Localization Id>.<Command id>.[Character Name]
            string stringId = "SAYID." + GetFlowchartLocalizationId() + "." + itemId + ".";
            if (character != null)
            {
                stringId += character.NameText;
            }

            return stringId;
        }

        #endregion
    }
}