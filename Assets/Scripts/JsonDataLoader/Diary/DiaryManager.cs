using System;
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

    public void FirstDayText()
    {
        System.Random rand = new System.Random();
        int colorIdx = rand.Next(0, 4);

        String colorString = "노랑";
        switch(colorIdx)
        {
            case 1:
                colorString = "파랑";
                break;
            case 2:
                colorString = "초록";
                break;
            case 3:
                colorString = "빨강";
                break;
            case 4:
                colorString = "보라";
                break;
        }

        diaryText1P.text = "[회사이름]에 입사하게 되었다. \n\n" + colorString + " 마을에선 오랜만에 나온 인재라고 부모님이 기뻐하셨다. \n\n새로운 마음 가짐으로 새로 펼친 일기장이 낯설지만 이 페이지에 채워질 내용이 기대된다.";
            
        diaryText2P.text = "이사 끝! \n\n나머지 짐은 다음에 마저 풀자. \n\n내가 [회사이름]에 다니게 되다니... 내일부터 출근이니까 힘내보자!";
    }

    public void SetDiaryText()
    {
        bool bMultiRound = SystemManager.Get().IsMultiRound();
        int characterIndex = GameManager.Get().GetCharacterIndex();
        bool bRedButtonPressed = GameManager.Get().HasStoryConditionState(StoryConditionState.PressedRedButton);

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
