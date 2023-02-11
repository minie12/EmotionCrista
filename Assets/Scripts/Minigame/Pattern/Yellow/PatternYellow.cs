using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternYellow : PatternManager
{
    private GameObject chatBoxPF;
    private GameObject[] chatBoxes;
    private int patternIdx; // is exist minimanager
    private int chatBoxCnt = 6;

    private GameObject chatFlowPF;
    private GameObject[] chatFlows;
    private int chatFlowCnt = 12;
    private int currentChatIdx;

    private float fullSpawnTime = 2;


    protected override void Awake()
    {
        base.Awake();
        chatBoxPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatBox");
        chatFlowPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatFlow");
    }


    override public void StartPattern(int gimmick_, int level_){
        patternIdx = 0;
        chatTextIdx = 0;
        gimmick = gimmick_; 
        level = level_;
        OrganizeCharacterChat();

        if(gimmick == 0){
            if(UICanvas.transform.childCount == 0){
                chatBoxes = new GameObject[chatBoxCnt];
                for(int i = 0; i < chatBoxCnt; i++){
                    GameObject temp = Instantiate(chatBoxPF, new Vector3(0,0,0), Quaternion.identity, UICanvas.transform);
                    temp.SetActive(false);
                    chatBoxes[i] = temp;
                }
            }

            Invoke("Y_SpawnChatBox", fullSpawnTime);
        }
        else if(gimmick == 1){
            if(UICanvas.transform.childCount == 0){
                chatFlows = new GameObject[chatFlowCnt];
                for(int i = 0; i < chatFlowCnt; i++){
                    GameObject temp = Instantiate(chatFlowPF, new Vector3(0,0,0), Quaternion.identity, UICanvas.transform);
                    temp.SetActive(false);
                    chatFlows[i] = temp;
                }
            }

            Invoke("Y_SpawnChatFlow", fullSpawnTime);
        }
    }


    
    override public void StopPattern(){ CancelInvoke(); }

    override public void RestartPattern(){
        patternIdx = 0; 

        if(gimmick == 0) Invoke("Y_SpawnChatBox", fullSpawnTime);
        else if(gimmick == 1) Invoke("Y_SpawnChatFlow", fullSpawnTime);
    }

    float CalcSpawnTime(){
        float decreaseTime = mini.TimeLeft()/15f;
        if(decreaseTime > 1.3f) decreaseTime = 1.3f;

        return fullSpawnTime - decreaseTime;
    }
    // Y1 --------------------------------------------------------------------------------------------------------
    void Y_SpawnChatBox(){
        // set position
        Vector3 randPos = new Vector3(Random.Range(800.0f, 1600.0f), Random.Range(220.0f, 850.0f), 5);
        chatBoxes[patternIdx].transform.position = Camera.main.ScreenToWorldPoint(randPos);
        float size = Random.Range(0.45f, 1.2f);

        chatBoxes[patternIdx].transform.SetSiblingIndex(chatBoxCnt-1);
        StartCoroutine(Y_ChatBoxAnim(size));
        patternIdx = (patternIdx+1)%chatBoxCnt;

        Invoke("Y_SpawnChatBox", CalcSpawnTime());
    }

    IEnumerator Y_ChatBoxAnim(float size){
        GameObject go = chatBoxes[patternIdx];
        go.GetComponent<RectTransform>().localScale = new Vector3(size+1f, size+1f, 1);
        go.SetActive(true);
        go.transform.DOScale(new Vector3(size-0.15f, size-0.15f), 0.25f);
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<AudioSource>().Play();
        go.transform.DOScale(new Vector3(size, size), 0.1f);
    }

    // Y2 --------------------------------------------------------------------------------------------------------
    void Y_SpawnChatFlow(){
        int indexTemp = currentChatIdx;
        while(indexTemp == currentChatIdx){
            // so that chatFlow gets spawned at diff locations 
            indexTemp = Random.Range(0, 7); // 7 is length of spawn_positions (at PatternChatFlow)
        }
        currentChatIdx = indexTemp;
        chatFlows[patternIdx].GetComponent<PatternChatFlow>().index = indexTemp;
        chatFlows[patternIdx].SetActive(true);

        patternIdx = (patternIdx+1)%chatFlowCnt;

        Invoke("Y_SpawnChatFlow", CalcSpawnTime()+0.4f);
    }
}
