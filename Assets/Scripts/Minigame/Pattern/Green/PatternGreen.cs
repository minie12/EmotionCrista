using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternGreen : PatternManager
{
    public GameObject gemPF;
    private bool[,] check = new bool[11, 6];

    private float bugSpeed = 3f;
    private float rotateSpeed = 0.5f;

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

    GemInfo GetNotGreenGemRandom(int standard_c, int standard_r)
    {
        while (true)
        {
            GemInfo randGem = mini.GetRandomGemOnWay(standard_c, standard_r);
            int column_ = randGem.GetColumn();
            int row_ = randGem.GetRow();
            if (check[column_, row_])
            {
                continue;
            }
            if (randGem.GetColor() != (int)PatternType.GREEN)
            {
                check[column_, row_] = true;
                return randGem;
            }
        }
    }

    List<GemInfo> GetHistoryGemRandom(int cnt, int start_c, int start_r)
    {
        List<GemInfo> result = new List<GemInfo>();
        int prev_c = start_c, prev_r = start_r;
        for (int i = 0; i < cnt;)
        {
            GemInfo randGem = mini.GetRandomGemOnWay(prev_c, prev_r);
            int new_c = randGem.GetColumn();
            int new_r = randGem.GetRow();

            if (check[new_c, new_r])
            {
                continue;
            }
            result.Add(randGem);
            check[new_c, new_r] = true;
            prev_c = new_c; prev_r = new_r;
            i++;
        }
        return result;
    }

    GameObject CreateBug()
    {
        // get green gem random
        GemInfo greenGem = mini.GetPatternGemRandom();
        int prevColumn = greenGem.GetColumn();
        int prevRow = greenGem.GetRow();
        check[prevColumn, prevRow] = true;
       
        greenGem.FadeOut();

        // just create image
        GameObject specialGem = Instantiate(gemPF, greenGem.GetComponent<Transform>().position, Quaternion.identity, this.transform);
        specialGem.transform.GetChild(0).gameObject.SetActive(false);
        specialGem.GetComponent<SpriteRenderer>().sortingOrder = 5;
        GemInfo specialGemInfo = specialGem.GetComponent<GemInfo>();
        specialGemInfo.InitGem(prevColumn, prevRow, (int)PatternType.GREEN);
        specialGemInfo.ChangeSpecialGem();

        specialGemInfo.FadeIn();

        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(prevColumn, prevRow);

        // block rotate gems
        greenGem.bRotateAble = false;
        for (int i = 0; i < aroundGems.Count; i++)
        {
            aroundGems[i].bRotateAble = false;
        }
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



        // turn on rotate true
        StartCoroutine(TurnOnRotateTrue(previousGem));
    }


    IEnumerator TurnOnRotateTrue(GemInfo previousGem)
    {
        yield return new WaitForSeconds(1f);

        previousGem.bRotateAble = true;

        // get around gems
        List<GemInfo> aroundGems = GameObject.Find("Board").GetComponent<BoardManager>().GetAroundGems(previousGem.GetColumn(), previousGem.GetRow());
        for (int i = 0; i < aroundGems.Count; i++)
        {
            aroundGems[i].bRotateAble = true;
        }
    }

    IEnumerator MoveStartToTarget(GameObject start, GameObject target, float endTime)
    {
        float currentTime = 0.0f;
        Vector3 current = start.transform.position;

        WaitForEndOfFrame waitForEndOfFrame = new WaitForEndOfFrame();

        while (currentTime < endTime)
        {
            currentTime += Time.deltaTime * bugSpeed;
            start.transform.position = Vector3.Lerp(current, target.transform.position, currentTime / endTime);
            yield return waitForEndOfFrame;
        }
        start.transform.position = target.transform.position;
    }

    IEnumerator ChangeDirectionTowards(GameObject start, GameObject target)
    {
        // set angle
        Vector2 direction = new Vector2(target.transform.position.x - start.transform.position.x, target.transform.position.y - start.transform.position.y);
        float angle = Mathf.Atan2(direction.y, direction.x) * Mathf.Rad2Deg;
        Quaternion angleAxis = Quaternion.AngleAxis(angle - 90f, Vector3.forward);

        float currentTime = 0.0f;
        Quaternion current = start.transform.rotation;
        while (currentTime < rotateSpeed)
        {
            currentTime += (Time.deltaTime);
            start.transform.rotation = Quaternion.Slerp(current, angleAxis, currentTime / rotateSpeed);
            yield return null;
        }
        start.transform.rotation = angleAxis;
    }

    IEnumerator MoveBug(List<GemInfo> history, GameObject start)
    {
        yield return new WaitForSeconds(bugSpeed);
        for (int i = 0; i < history.Count; i++)
        {
            // get target object
            GameObject target = history[i].gameObject;

            // bug head towards target
            StartCoroutine(ChangeDirectionTowards(start, target));
            // hold time
            yield return new WaitForSeconds(rotateSpeed);

            // get direction start to target
            float endTime = Mathf.Sqrt(Mathf.Pow(start.transform.position.x - target.transform.position.x, 2) + Mathf.Pow(start.transform.position.y - target.transform.position.y, 2));

            // move bug
            StartCoroutine(MoveStartToTarget(start, target, endTime));
            // hold time
            yield return new WaitForSeconds(endTime / bugSpeed);

            yield return new WaitForSeconds(Random.Range(0.0f, 1.5f));
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

        // get history
        GemInfo bugInfo = bug.GetComponent<GemInfo>();
        List<GemInfo> history = GetHistoryGemRandom(2, bugInfo.GetColumn(), bugInfo.GetRow());

        // get target
        int idx = history.Count - 1;
        GemInfo target = GetNotGreenGemRandom(history[idx].GetColumn(), history[idx].GetRow());
        history.Add(target);

        // move bug
        StartCoroutine(MoveBug(history, bug));
    }
}
