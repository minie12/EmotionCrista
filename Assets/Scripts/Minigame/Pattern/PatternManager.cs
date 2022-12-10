using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternManager : MonoBehaviour
{
    // YELLOW Pattern
    [HideInInspector] public GameObject UICanvas;
    [HideInInspector] public Text chatTXT;
    [HideInInspector] public MiniManager mini; 
    [HideInInspector] public int gimmick = 0;

    // chat UI 
    [HideInInspector] public string[] chatTextInfo;
    [HideInInspector] public int chatTextIdx = 0;

    void Awake(){
        UICanvas = GameObject.Find("PatternCanvas");
        chatTXT = GameObject.Find("UIText").GetComponent<Text>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
    }

    public PatternManager SpawnPattern(int patternIdx){
        if(patternIdx == (int)PatternType.YELLOW){ // YELLOW
            Debug.Log("Returned Pattern Yellow");
            return GetComponent<PatternYellow>();
        }
        else if(patternIdx == (int)PatternType.BLUE){ // BLUE
            Debug.Log("Returned Pattern Blue");
            return GetComponent<PatternBlue>();
        }
        else if (patternIdx == (int)PatternType.RED){ // RED
            Debug.Log("Returned Pattern Red");
            return GetComponent<PatternRed>();
        } 
        else if (patternIdx == (int)PatternType.GREEN){ // GREEN
            Debug.Log("Returned Pattern Green");
            return GetComponent<PatternGreen>();
        }
        else if (patternIdx == (int)PatternType.PURPLE){ // PURPLE
            Debug.Log("Returned Pattern Purple");
            return GetComponent<PatternPurple>();
        }

        Debug.Log("No Pattern Found");
        return GetComponent<PatternYellow>();
    }

    virtual public void StartPattern(int gimmick_){}
    virtual public void StopPattern(){}
    virtual public void RestartPattern(){}

    public void OrganizeCharacterChat(){
        chatTextIdx = 0;
        foreach(CharacterChat myChat in GameManager.Instance.myChatList.characterChat){
            if(myChat.name == mini.GetFungusMessage())
            {
                chatTextInfo = myChat.chat.Split('\r');
                break;
            }
        }

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
