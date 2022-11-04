using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGreen : PatternManager
{
    public GameObject gemPF;
    private bool[,] check = new bool[11, 6];

    private float bugSpeed = 1f;

    override public void StartPattern(int gimmick_)
    {
        gimmick = gimmick_;
        OrganizeCharacterChat();

        // give term before choose gem because board init
        if (gimmick == 0) 
        { 
            Invoke("GreenGimmick0", 1f); 
        }
    }

    override public void StopPattern() { CancelInvoke(); }
    override public void RestartPattern()
    {
        if (gimmick == 0) 
        {
            Invoke("GreenGimmick0", 1f);
        }

    }

    GemInfo GetGreenGemRandom()
    {
        while (true)
        {
            GemInfo randGem = mini.GetRandomGem();
            if (randGem.GetColor() == (int)PatternType.GREEN)
            {
                return randGem;
            }
        }   
    }

    GemInfo GetNotGreenGemRandom()
    {
        while (true)
        {
            GemInfo randGem = mini.GetRandomGem();
            if (randGem.GetColor() != (int)PatternType.GREEN)
            {
                return randGem;
            }
        }
    }

    List<GemInfo> GetHistoryGemRandom(int cnt)
    {
        List<GemInfo> result = new List<GemInfo>();
        for (int i = 0; i < cnt;)
        {
            GemInfo randGem = mini.GetRandomGem();
            int column_ = randGem.GetColumn();
            int row_ = randGem.GetRow();

            if (check[column_,row_])
            {
                continue;
            }
            result.Add(randGem);
            check[column_, row_] = true;
            i++;
        }
        return result;
    }

    GameObject CreateBug()
    {
        // get green gem random
        GemInfo greenGem = GetGreenGemRandom();
        int prevColumn = greenGem.GetColumn();
        int prevRow = greenGem.GetRow();
        check[prevColumn, prevRow] = true;
       
        greenGem.FadeOut();

        GameObject specialGem = Instantiate(gemPF, greenGem.GetComponent<Transform>().position, Quaternion.identity, this.transform);
        specialGem.transform.GetChild(0).gameObject.SetActive(false);
        specialGem.GetComponent<SpriteRenderer>().sortingOrder = 5;
        GemInfo specialGemInfo = specialGem.GetComponent<GemInfo>();
        specialGemInfo.InitGem(prevColumn, prevRow, (int)PatternType.GREEN);
        specialGemInfo.ChangeSpecialGem();

        specialGemInfo.FadeIn();

        return specialGem;
    }

    void ChangeGemLast(GameObject bug, GemInfo target)
    {
        int targetColor = target.GetColor();
        int startColumn = bug.GetComponent<GemInfo>().GetColumn();
        int startRow = bug.GetComponent<GemInfo>().GetRow();

        // target gem change to green gem
        bug.GetComponent<GemInfo>().FadeOut();
        StartCoroutine(DestroyObject(bug));
        target.ChangeGemColor((int)PatternType.GREEN);
        target.FadeIn();

        // previous gem change to target color
        GemInfo previousGem = GameObject.Find("Board").GetComponent<BoardManager>().GetGem(startColumn, startRow);
        previousGem.ChangeGemColor(targetColor);
        previousGem.FadeIn();
    }

    IEnumerator MoveStartToTarget(GameObject start, GameObject target)
    {
        float currentTime = 0.0f;
        Vector3 current = start.transform.position;
        while (currentTime < bugSpeed)
        {
            currentTime += (Time.deltaTime);
            start.transform.position = Vector3.Lerp(current, target.transform.position, currentTime / bugSpeed);
            yield return null;
        }
        start.transform.position = target.transform.position;
        yield return null;

    }

    IEnumerator MoveBug(List<GemInfo> history, GameObject start)
    {
        yield return new WaitForSeconds(bugSpeed);
        for (int i = 0; i < history.Count; i++)
        {
            StartCoroutine(MoveStartToTarget(start, history[i].gameObject));
            yield return new WaitForSeconds(bugSpeed);
        }
        ChangeGemLast(start, history[history.Count - 1]);
    }

    IEnumerator DestroyObject(GameObject obj)
    {
        yield return new WaitForSeconds(1f);
        Destroy(obj);
    }

    void GreenGimmick0()
    {
        // init
        check = new bool[11, 6];

        // create bug
        GameObject bug = CreateBug();

        // get target
        GemInfo target = GetNotGreenGemRandom();
        int targetColumn = target.GetColumn();
        int targetRow = target.GetRow();

        check[targetColumn, targetRow] = true;

        // get history
        List<GemInfo> history = GetHistoryGemRandom(2);
        history.Add(target);

        // move bug
        StartCoroutine(MoveBug(history, bug));
    }
}
