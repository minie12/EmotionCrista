using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternYellow : PatternManager
{
    private GameObject chatBoxPF;
    private GameObject[] chatBoxes;
    private int chatBoxIdx = 0;
    private int chatBoxCnt = 6;

    private GameObject chatFlowPF;
    private GameObject[] chatFlows;
    private int chatFlowIdx = 0;
    private int chatFlowCnt = 12;
    private int currentChatIdx;

    private float fadeTime = 1f;
    private float heartbeatTime = 1f;
    private float heartSizeOffset = 0.88f;
    private float fullSpawnTime = 5f;
    private float dropTime = 0.2f;

    private GameObject gemPF;


    protected override void Awake()
    {
        base.Awake();
        chatBoxPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatBox");
        chatFlowPF = Resources.Load<GameObject>("Prefabs/MiniGame/chatFlow");
        gemPF = Resources.Load<GameObject>("Prefabs/MiniGame/org_gem");
    }


    public override void StartPattern(int level_){
        base.StartPattern(level_);

        // [TODO] ��ȹ
        switch (level_)
        {
            case 0:
                StartGimmick(0);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
            case 1:
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
            case 2:
                StartGimmick(0);
                StartGimmick(1);
                StartGimmick(2);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
            case 3:
                StartGimmick(0);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
            case 4:
                StartGimmick(1);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
            case 5:
                StartGimmick(0);
                StartGimmick(1);
                StartGimmick(2);
                mini.SetGameTimeInit(200f, 2f, 1f, 100f, 3.6f, 3);
                break;
        }
    }

    public override void StopPattern()
    {
        base.StopPattern();
    }

    public override void StartGimmick(int gimmick_)
    {
        base.StartGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                chatBoxIdx = 0;
                chatBoxes = GetChatInitArray(0);
                Invoke("Y_SpawnChatBox", Random.Range(1f, fullSpawnTime));
                break;
            case 1:
                chatFlowIdx = 0;
                chatFlows = GetChatInitArray(1);
                Invoke("Y_SpawnChatFlow", Random.Range(1f, fullSpawnTime));
                break;
            case 2:
                InvokeRepeating("Y_HeartBeat", Random.Range(1f, fullSpawnTime), 60f);
                break;
        }
    }

    public override void StopGimmick(int gimmick_)
    {
        base.StopGimmick(gimmick_);

        switch (gimmick_)
        {
            case 0:
                CancelInvoke("Y_SpawnChatBox");
                break;
            case 1:
                CancelInvoke("Y_SpawnChatFlow");
                break;
            case 2:
                CancelInvoke("Y_HeartBeat");
                break;
        }
    }

    GameObject[] GetChatInitArray(int gimmick_)
    {
        int cnt = gimmick_ == 0 ? chatBoxCnt : chatFlowCnt;
        GameObject[] result = new GameObject[cnt];

        for (int i = 0; i < cnt; i++)
        {
            GameObject prefab = gimmick_ == 0 ? chatBoxPF : chatFlowPF;
            GameObject temp = Instantiate(prefab, new Vector3(0, 0, 0), Quaternion.identity, UICanvas.transform);
            temp.SetActive(false);
            result[i] = temp;
        }
        return result;
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
        chatBoxes[chatBoxIdx].transform.position = Camera.main.ScreenToWorldPoint(randPos);
        float size = Random.Range(0.45f, 1.2f);

        chatBoxes[chatBoxIdx].transform.SetSiblingIndex(chatBoxCnt-1);
        StartCoroutine(Y_ChatBoxAnim(size));
        chatBoxIdx = (chatBoxIdx+1)%chatBoxCnt;

        if (gimmick[0])
            Invoke("Y_SpawnChatBox", CalcSpawnTime());
    }

    IEnumerator Y_ChatBoxAnim(float size){
        GameObject go = chatBoxes[chatBoxIdx];
        go.GetComponent<RectTransform>().localScale = new Vector3(size+1f, size+1f, 1);
        go.SetActive(true);
        go.transform.DOScale(new Vector3(size-0.15f, size-0.15f), 0.25f);
        yield return new WaitForSeconds(0.2f);
        go.GetComponent<AudioSource>()?.Play();
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
        chatFlows[chatFlowIdx].GetComponent<PatternChatFlow>().index = indexTemp;
        chatFlows[chatFlowIdx].SetActive(true);

        chatFlowIdx = (chatFlowIdx+1)%chatFlowCnt;

        if (gimmick[1])
            Invoke("Y_SpawnChatFlow", CalcSpawnTime()+0.4f);
    }

    // Y3 --------------------------------------------------------------------------------------------------------
    IEnumerator GemWavelength(GemInfo startGem, GemInfo temp, List<List<GemInfo>> aroundGemList)
    {
        yield return new WaitForSeconds(fadeTime + heartbeatTime * 0.8f);

        
        for(int i = 0; i < 6; i++)
        {
            for (int j = 0; j <= 10; j++)
            {
                if (aroundGemList[i].Count <= j)
                {
                    break;
                }

                int column_ = aroundGemList[i][j].GetColumn();
                int row_ = aroundGemList[i][j].GetRow();

                // gem move
                if (j == aroundGemList[i].Count - 1) // last gem
                {
                    aroundGemList[i][j].FadeOut(dropTime);
                    aroundGemList[i][j].OnlyDestroyGem(dropTime);
                }
                if (j - 1 >= 0) // move distance upper 2 from startGem
                {
                    aroundGemList[i][j - 1].MoveGem(column_, row_, dropTime);
                    GameObject.Find("Board").GetComponent<BoardManager>().SetGem(column_, row_, aroundGemList[i][j - 1]);
                }
                else // move distance 1 from startGem
                {
                    // fill with new gem
                    int color = Random.Range(0, System.Enum.GetNames(typeof(PatternType)).Length - 1);
                    GameObject gemTemp = Instantiate(gemPF, startGem.transform.position, Quaternion.identity, this.transform);
                    gemTemp.GetComponent<GemInfo>().InitGem(column_, row_, color);
                    gemTemp.GetComponent<GemInfo>().MoveGem(column_, row_, dropTime);
                    gemTemp.GetComponent<GemInfo>().FadeIn(dropTime);
                    GameObject.Find("Board").GetComponent<BoardManager>().SetGem(column_, row_, gemTemp.GetComponent<GemInfo>());
                }
            }
        }

        yield return new WaitForSeconds(dropTime + 0.1f);
        GameObject.Find("Board").GetComponent<BoardManager>().SetGemMovable(true);

        // gem origin
        temp.SetSpriteColor(255f, 255, 255f, 255f);
        temp.FadeOut(fadeTime);
        temp.OnlyDestroyGem(fadeTime);
        startGem.ChangeGemColor((int)PatternType.YELLOW);
        startGem.SetTransformScale(1f / heartSizeOffset);
        startGem.FadeIn(fadeTime);
    }

    void AroundGemShake(List<List<GemInfo>> aroundGemList)
    {
        for (int i = 0; i < aroundGemList.Count; i++)
        {
            for (int j = 0; j < aroundGemList[i].Count; j++)
            {
                aroundGemList[i][j].GemShake(fadeTime, 0.02f, heartbeatTime);
            }
        }
    }

    void SetGemFeature(GemInfo gem, float prevTime, float amount)
    {
        gem.ChangeSpecialGem();
        gem.SetTransformScale(heartSizeOffset);
        gem.GemHeartBeat(prevTime, amount, heartbeatTime);
    }

    void Y_HeartBeat()
    {
        // get yellow gem random
        GemInfo gem = board.GetPatternGemRandom();
        SetGemFeature(gem, fadeTime, 1.5f);
        gem.FadeIn(fadeTime);

        // ghost effect
        GemInfo temp = Instantiate(gemPF, gem.transform.position, Quaternion.identity, UICanvas.transform).GetComponent<GemInfo>();
        SetGemFeature(temp, fadeTime + 0.1f, 1.6f);
        temp.SetBackgroundColor(255f, 255f, 255f, 0f); // background transparency
        temp.SetSpriteColor(188f, 188f, 188f, 110f);

        // get around gem list
        List<List<GemInfo>> aroundGemList = board.GetAroundGemList(gem.GetColumn(), gem.GetRow());

        // around gems vibration
        AroundGemShake(aroundGemList);

        // start gem wavelength
        GameObject.Find("Board").GetComponent<BoardManager>().SetGemMovable(false);
        StartCoroutine(GemWavelength(gem, temp, aroundGemList));
    }
}
