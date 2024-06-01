using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LogSpeech : MonoBehaviour
{
    private static float additionalMinHeight = 32;
    private static int minLineCount = 2;

    public RectTransform speechBoxGO;

    public Text nameText;
    public Text speechBoxText;

    public void SetSpeech(string inSpeechName, string inSpeechText)
    {
        if (inSpeechText.Length <= 0 || null == speechBoxText)
        {
            return;
        }

        if (0 < inSpeechName.Length)
        {
            // System Log Speech does not have nameText
            if (null != nameText)
            {
                nameText.text = inSpeechName;
            }
        }

        speechBoxText.text = inSpeechText;

        AdjustBoxHeight();
    }

    public float GetBoxHeight()
    {
        return speechBoxText.rectTransform.sizeDelta.y;
    }

    private void AdjustBoxHeight()
    {
        TextGenerator textGen = new TextGenerator();
        TextGenerationSettings generationSettings = speechBoxText.GetGenerationSettings(speechBoxText.rectTransform.rect.size);

        float textHeight = textGen.GetPreferredHeight(speechBoxText.text, generationSettings);

        // height of text
        speechBoxText.rectTransform.sizeDelta = new Vector2(speechBoxText.rectTransform.rect.width, textHeight);

        int lineCount = speechBoxText.cachedTextGenerator.lines.Count;

        if (minLineCount < lineCount)
        {
            float additionalBoxHeight = additionalMinHeight * ((lineCount - 1) / minLineCount);

            // height of speech box
            speechBoxGO.sizeDelta = new Vector2(speechBoxGO.rect.width, speechBoxGO.rect.height + additionalBoxHeight);

            // height of prefab
            this.GetComponent<RectTransform>().sizeDelta = speechBoxGO.sizeDelta = new Vector2(this.GetComponent<RectTransform>().rect.width, this.GetComponent<RectTransform>().rect.height + additionalBoxHeight);
        }
    }
}
