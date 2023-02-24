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
    private float dropTime = 0.1f;

    private GameObject gemPF;


    protected override void Awake()
    {
        base.Awake();
        chatBoxPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatBox");
        chatFlowPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatFlow");
        gemPF = Resources.Load<GameObject>("Prefabs/MiniGame/org_gem");
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
        else if(gimmick == 2)
        {
            Invoke("Y_HeartBeat", fullSpawnTime);
        }
    }


    
    override public void StopPattern(){ CancelInvoke(); }

    override public void RestartPattern(){
        patternIdx = 0; 

        if(gimmick == 0) Invoke("Y_SpawnChatBox", fullSpawnTime);
        else if(gimmick == 1) Invoke("Y_SpawnChatFlow", fullSpawnTime);
        else if(gimmick == 2)
        {
            Invoke("Y_HeartBeat", fullSpawnTime);
        }
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

    // Y3 --------------------------------------------------------------------------------------------------------
    IEnumerator GemWavelength(GemInfo startGem, List<List<GemInfo>> aroundGemList)
    {
        yield return new WaitForSeconds(2f);

        
        for(int i = 0; i < 6; i++)
        {
            int originC = -1;
            int originR = -1;
            for (int j = 10; j >= 0; j--)
            {
                if (aroundGemList[i].Count <= j)
                {
                    continue;
                }

                // gem move
                if (originC == -1)
                {
                    originC = aroundGemList[i][j].GetColumn();
                    originR = aroundGemList[i][j].GetRow();
                    aroundGemList[i][j].FadeOut(1f / dropTime);
                    aroundGemList[i][j].OnlyDestroyGem(dropTime);
                }
                if (j - 1 >= 0) // move distance upper 2
                {
                    int newC = aroundGemList[i][j - 1].GetColumn();
                    int newR = aroundGemList[i][j - 1].GetRow();
                    aroundGemList[i][j] = aroundGemList[i][j - 1];
                    aroundGemList[i][j].MoveGem(originC, originR, dropTime);
                    GameObject.Find("Board").GetComponent<BoardManager>().SetGem(originC, originR, aroundGemList[i][j]);
                    originC = newC;
                    originR = newR;
                }
                else // move distance 1
                {
                    // fill with new gem
                    int color = Random.Range(0, System.Enum.GetNames(typeof(PatternType)).Length - 1);
                    GameObject gemTemp = Instantiate(gemPF, startGem.transform.position, Quaternion.identity, this.transform);
                    gemTemp.GetComponent<GemInfo>().InitGem(originC, originR, color);
                    gemTemp.GetComponent<GemInfo>().MoveGem(originC, originR, dropTime);
                    gemTemp.GetComponent<GemInfo>().FadeIn(1f / dropTime);
                    GameObject.Find("Board").GetComponent<BoardManager>().SetGem(originC, originR, gemTemp.GetComponent<GemInfo>());
                }
            }
        }

        yield return new WaitForSeconds(dropTime);
        GameObject.Find("Board").GetComponent<BoardManager>().SetGemMovable(true);
    }

    void Y_HeartBeat()
    {
        // get yellow gem random
        GemInfo gem = mini.GetPatternGemRandom();
        gem.ChangeSpecialGem();
        gem.FadeIn();
        gem.GemShake(1f, 0.01f, 1f);

        // get around gem list
        List<List<GemInfo>> aroundGemList = mini.GetAroundGemList(gem.GetColumn(), gem.GetRow());

        // start gem wavelength
        GameObject.Find("Board").GetComponent<BoardManager>().SetGemMovable(false);
        StartCoroutine(GemWavelength(gem, aroundGemList));
    }
}
