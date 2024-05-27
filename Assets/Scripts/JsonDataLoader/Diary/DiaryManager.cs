using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    [SerializeField]
    private Text diaryText1P;
    [SerializeField]
    private Text diaryText2P;

    public void SetDiaryText()
    {
        bool bMultiRound = SystemManager.Get().IsMultiRound();
        int characterIndex = GameManager.Get().GetCharacterIndex();
        bool bRedButtonPressed = GameManager.Get().IsRedButtonPressed();

        string DiaryTextData = DiaryDialogReader.GetDialogData(bMultiRound, characterIndex, bRedButtonPressed);
        string[] DiaryTexts = DiaryTextData.Split('/');

        diaryText1P.text = DiaryTexts[0];

        if (1 < DiaryTexts.Length)
        {
            diaryText2P.text = DiaryTexts[1];
        }
    }

    public void LoadLoadingScene()
    {
        GameObject GO_flowchart = GameObject.Find("Flowchart");
        if (null != GO_flowchart)
        {
            Fungus.Flowchart flowchart = GO_flowchart.GetComponent<Fungus.Flowchart>();
            if (null != flowchart)
            {
                flowchart.SetStringVariable("NextScene", "LoadingScene");

                flowchart.SendFungusMessage("ToNextScene");
            }
        }
    }
}
