using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PatternManager : MonoBehaviour
{
    // Pattern
    protected GameObject UICanvas;
    protected Text chatTXT;
    protected MiniManager mini;
    protected BoardManager board;
    protected bool[] gimmick; // manage current running gimmick
    protected PatternConfig patternConfig; // 패턴 관련 설정들

    // chat UI 
    protected string[] chatTextInfo;
    protected int chatTextIdx = 0;

    // 기믹 실패했을 때, 떨어지는 게이지량
    private List<int> failGaugeMount;

    protected virtual void Awake(){
        UICanvas = GameObject.Find("PatternCanvas");
        chatTXT = GameObject.Find("UIText").GetComponent<Text>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    virtual public void OnCrushedGem(bool isMatchColor, List<List<int>> crushedGems) { }

    virtual public void StartPattern(int level_) 
    {
        mini.patternLevel = level_;
        patternConfig = PatternConfigReader.GetPatternConfig(mini.patternIdx);
        failGaugeMount = patternConfig.gimmick.failGaugeMount;
        gimmick = new bool[patternConfig.gimmick.cnt];
        OrganizeCharacterChat();
    }

    virtual public void StopPattern() 
    {
        // de-activate all pattern objects
        foreach (Transform child in UICanvas.transform)
        {
            Destroy(child.gameObject);
        }

        // stop all gimmick
        for (int i = 0; i < patternConfig.gimmick.cnt; i++)
        {
            StopGimmick(i);
        }
    }
    virtual public void StartGimmick(int gimmick_)
    {
        this.gimmick[gimmick_] = true;
    }
    virtual public void StopGimmick(int gimmick_)
    {
        this.gimmick[gimmick_] = false;
    }
    
    // 이어하기에 가까운 함수 !!
    virtual public void RestartPattern() 
    {
        BoardSettings set = patternConfig.boardSettings;
        int level = mini.patternLevel - 1; // 0 ~ 5
        mini.SetGameTimeInit(set.fullPlayTime[level], set.playTimeSpeed[level], set.crushedGaugeTime[level], set.fullScore[level], set.scoreSpeed[level], set.goalUnit[level]);
        foreach (int gimmick_num in patternConfig.gimmick.type[level])
        {
            this.StartGimmick(gimmick_num);
        }
    }

    private void OrganizeCharacterChat(){
        chatTextIdx = 0;

        string storyIndex = mini.GetFungusMessage();
        chatTextInfo = MinigameDialogReader.GetDialogData(storyIndex);

        if(chatTextInfo.GetLength(0)!=0) SetUIText();
    }
    public void SetUIText(){
        string[] contents = chatTextInfo[chatTextIdx].Split('/');

        StartCoroutine(SetUIText_(contents));

        chatTextIdx++;
        if(chatTextIdx >= chatTextInfo.GetLength(0)) chatTextIdx--;
    }

    IEnumerator SetUIText_(string[] contents){
        foreach(string content in contents){
            chatTXT.text = content;
            yield return new WaitForSeconds(4f);
        }
    }

    public bool IsRunningGimmick(int gimmick_)
    {
        return this.gimmick[gimmick_];
    }

    public void SetFailGaugeMount(int gimmick_)
    {
        mini.SetPlayTime((float)failGaugeMount[gimmick_]);
    }
}
