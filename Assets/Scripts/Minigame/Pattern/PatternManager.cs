using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternManager : MonoBehaviour
{
    // Pattern
    protected GameObject UICanvas;
    protected Text chatTXT;
    protected MiniManager mini;
    protected BoardManager board;
    protected int gimmick = 0;
    protected int level = (int)LevelType.EASY1; // difficulty

    // chat UI 
    protected string[] chatTextInfo;
    protected int chatTextIdx = 0;

    protected virtual void Awake(){
        UICanvas = GameObject.Find("PatternCanvas");
        chatTXT = GameObject.Find("UIText").GetComponent<Text>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    virtual public void StartPattern(int gimmick_, int level_){}
    virtual public void StopPattern(){}
    virtual public void RestartPattern(){}

    public void OrganizeCharacterChat(){
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

    public void ClearPattern(){
        CancelInvoke(); // stop spawning 

        // de-activate all pattern objects
        foreach (Transform child in UICanvas.transform)
            child.gameObject.SetActive(false);
    }
}
