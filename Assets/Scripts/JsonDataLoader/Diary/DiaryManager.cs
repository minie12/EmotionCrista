using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class DiaryManager : MonoBehaviour
{
    [SerializeField]
    private Text diaryText;

    public void SetDiaryText()
    {
        bool bMultiRound = SystemManager.Get().IsMultiRound();
        int characterIndex = GameManager.Get().GetCharacterIndex();
        bool bRedButtonPressed = GameManager.Get().IsRedButtonPressed();

        diaryText.text = DiaryDialogReader.GetDialogData(bMultiRound, characterIndex, bRedButtonPressed);
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
