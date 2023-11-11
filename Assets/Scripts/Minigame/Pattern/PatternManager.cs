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

    // chat UI 
    protected string[] chatTextInfo;
    protected int chatTextIdx = 0;

    private readonly int[] gimmickCnt = new int[5] { 3, 3, 2, 2, 2 };
    private readonly List<List<int>> failGaugeMount = new List<List<int>> { new List<int> { 3, 0, 10 },
                                                                            new List<int> { 0, 0, 0 },
                                                                            new List<int> { 0, 0 },
                                                                            new List<int> { 0, 0 },
                                                                            new List<int> { 0, 0 }};

    protected virtual void Awake(){
        UICanvas = GameObject.Find("PatternCanvas");
        chatTXT = GameObject.Find("UIText").GetComponent<Text>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    virtual public void OnCrushedGem(bool isMatchColor) { }

    virtual public void StartPattern(int level_) 
    {
        mini.patternLevel = level_;
        gimmick = new bool[gimmickCnt[mini.patternIdx]];
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
        for (int i = 0; i < gimmickCnt[mini.patternIdx]; i++)
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
    virtual public void RestartPattern() { }

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
        mini.SetPlayTime((float)failGaugeMount[mini.patternIdx][gimmick_]);
    }
}
