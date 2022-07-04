using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternYellow : PatternManager
{
    public GameObject chatBoxPF;
    private GameObject[] chatBoxes;
    private int y_index;
    private int chatBox_numb = 6;

    public GameObject chatFlowPF;
    private GameObject[] chatFlows;
    private int chatFlow_numb = 12;
    private int chat_index;

    private float full_spawn_time = 2;

    override public void StartPattern(int gimmick_){
        y_index = 0; gimmick = gimmick_; UI_text_idx = 0;
        OrganizeCharacterChat();

        if(gimmick == 0){
            if(UI_canvas.transform.childCount == 0){
                chatBoxes = new GameObject[chatBox_numb];
                for(int i = 0; i < chatBox_numb; i++){
                    GameObject temp = Instantiate(chatBoxPF, new Vector3(0,0,0), Quaternion.identity, UI_canvas.transform);
                    temp.SetActive(false);
                    chatBoxes[i] = temp;
                }
            }

            Invoke("Y_SpawnChatBox", full_spawn_time);
        }
        else if(gimmick == 1){
            if(UI_canvas.transform.childCount == 0){
                chatFlows = new GameObject[chatFlow_numb];
                for(int i = 0; i < chatFlow_numb; i++){
                    GameObject temp = Instantiate(chatFlowPF, new Vector3(0,0,0), Quaternion.identity, UI_canvas.transform);
                    temp.SetActive(false);
                    chatFlows[i] = temp;
                }
            }

            Invoke("Y_SpawnChatFlow", full_spawn_time);
        }
    }


    
    override public void StopPattern(){ CancelInvoke(); }

    override public void RestartPattern(){
        y_index = 0; 

        if(gimmick == 0) Invoke("Y_SpawnChatBox", full_spawn_time);
        else if(gimmick == 1) Invoke("Y_SpawnChatFlow", full_spawn_time);
    }

    float CalcSpawnTime(){
        float decrease_time = mini_manager.TimeLeft()/15f;
        if(decrease_time > 1.3f) decrease_time = 1.3f;

        return full_spawn_time - decrease_time;
    }
    // Y1 --------------------------------------------------------------------------------------------------------
    void Y_SpawnChatBox(){
        // set position
        Vector3 rand_pos = new Vector3(Random.Range(800.0f, 1600.0f), Random.Range(220.0f, 850.0f), 5);
        chatBoxes[y_index].transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        float size = Random.Range(0.45f, 1.2f);

        chatBoxes[y_index].transform.SetSiblingIndex(chatBox_numb-1);
        StartCoroutine(Y_ChatBoxAnim(size));
        y_index = (y_index+1)%chatBox_numb;

        Invoke("Y_SpawnChatBox", CalcSpawnTime());
    }

    IEnumerator Y_ChatBoxAnim(float size){
        GameObject go = chatBoxes[y_index];
        go.GetComponent<RectTransform>().localScale = new Vector3(size+1f, size+1f, 1);
        go.SetActive(true);
        go.transform.DOScale(new Vector3(size-0.15f, size-0.15f), 0.25f);
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<AudioSource>().Play();
        go.transform.DOScale(new Vector3(size, size), 0.1f);
    }

    // Y2 --------------------------------------------------------------------------------------------------------
    void Y_SpawnChatFlow(){
        int index_temp = chat_index;
        while(index_temp == chat_index){
            // so that chatFlow gets spawned at diff locations 
            index_temp = Random.Range(0, 7); // 7 is length of spawn_positions (at PatternChatFlow)
        }
        chat_index = index_temp;
        chatFlows[y_index].GetComponent<PatternChatFlow>().index = index_temp;
        chatFlows[y_index].SetActive(true);

        y_index = (y_index+1)%chatFlow_numb;

        Invoke("Y_SpawnChatFlow", CalcSpawnTime()+0.4f);
    }
}
