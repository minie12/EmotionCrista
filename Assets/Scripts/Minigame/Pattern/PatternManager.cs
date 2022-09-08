using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternManager : MonoBehaviour
{
    // YELLOW Pattern
    [HideInInspector] public GameObject UI_canvas;
    [HideInInspector] public Text UI_text;
    [HideInInspector] public MiniManager mini; 
    [HideInInspector] public int gimmick = 0;

    // chat UI 
    public string[] UI_text_info;
    public int UI_text_idx = 0;

    void Awake(){
        UI_canvas = GameObject.Find("PatternCanvas");
        UI_text = GameObject.Find("UIText").GetComponent<Text>();
        mini = GameObject.Find("MiniManager").GetComponent<MiniManager>();
    }

    public PatternManager SpawnPattern(int pattern_idx){
        if(pattern_idx == (int)PatternType.YELLOW){ // YELLOW'
            Debug.Log("Returned Pattern Yellow");
            return GetComponent<PatternYellow>();
        }
        else if(pattern_idx == (int)PatternType.BLUE){ // BLUE
            Debug.Log("Returned Pattern Blue");
            return GetComponent<PatternBlue>();
        }

        Debug.Log("No Pattern Found");
        return GetComponent<PatternYellow>();
    }

    virtual public void StartPattern(int gimmick_){}
    virtual public void StopPattern(){}
    virtual public void RestartPattern(){}

    public void OrganizeCharacterChat(){
        UI_text_idx = 0;
        foreach(CharacterChat myChat in GameManager.Instance.myChatList.characterChat){
            if(myChat.name == mini.fungus_message){
                UI_text_info = myChat.chat.Split('\r');
                break;
            }
        }

        if(UI_text_info.GetLength(0)!=0) SetUIText();
    }

    // 텍스트 한 글자씩 타이핑 효과
    IEnumerator TypingAnimation(Text typingText, string message, float speed)
    {
        for (int i = 0; i < message.Length; i++)
        {
            typingText.text = message.Substring(0, i + 1);
            yield return new WaitForSeconds(speed);
        }
    }

    public void SetUIText(){
        string[] contents = UI_text_info[UI_text_idx].Split('/');

        StartCoroutine(SetUIText_(contents));

        UI_text_idx++;
        if(UI_text_idx >= UI_text_info.GetLength(0)) UI_text_idx--;
    }

    IEnumerator SetUIText_(string[] contents){
        foreach(string content in contents){
            // UI_text.text = content;
            StartCoroutine(TypingAnimation(UI_text, content, 0.07f));
            yield return new WaitForSeconds(4f);
        }
    }

    public void ClearPattern(){
        CancelInvoke(); // stop spawning 

        // de-activate all pattern objects
        foreach (Transform child in UI_canvas.transform)
            child.gameObject.SetActive(false);
    }
}
