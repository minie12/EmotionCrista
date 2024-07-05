using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum PatternType
{
    YELLOW,
    BLUE,
    RED,
    GREEN,
    PURPLE,
    NOTHING
}

public enum LevelType
{
    EASY1,
    NORMAL1,
    HARD1,
    EASY2,
    NORMAL2,
    HARD2
}

public class MiniManager : MonoBehaviour
{
    // UI
    public Image timerFill;
    public Image scoreFill;
    public Text scoreTXT;
    public GameObject UIScore;
    public GameObject UIGameOver;
    public GameObject timer;
    public Sprite timerOrigin, timerRed;
    public Sprite[] Characters;

    // Counselee Color
    public Color[] counseleeColor;

    // timer
    private float fullPlayTime = 50f;
    private float playTimeSpeed = 1f;
    private float playTime = 0f;
    private float fullFeverTime = 10f;
    private float feverTime = 0f;
    private float crushedGaugeTime = 1f;
    private bool isTwinkle = false;

    // board related
    private BoardManager board;
    public SpriteRenderer boardImg;
    public Sprite[] puzzleBoardSP;
    public GoalInfo goalInfo;
    private int goalUnit = 3; // goal gem count

    // score
    private float fullScore = 100f;
    private float score = 0f;
    private float scoreSpeed = 1f;

    // fever
    private float fullFever = 50;
    private float fever = 0;
    public Image feverFillIMG;
    public Image feverIMG;
    public Button feverBTN;
    [HideInInspector] public bool bFeverOn = false;
    public Sprite[] feverSP;
    public Sprite[] feverFillSP;
    public Animator animator;

    // pattern
    public PatternManager pattern;
    [HideInInspector] public int patternIdx;
    [HideInInspector] public int patternLevel;
    private int totalCrushedGem = 0;

    private int storyRound = 0;
    private int miniGameLevel = 0;

    // game mode
    private bool bPuzzleMode = true;

    // Get & Set -----------------------------------------------------
    public string GetFungusMessage()
    {
        // D0회차_내담자이름난이도
        string characterName = Enum.GetName(typeof(CharacterName), patternIdx);
        string[] levelName = { "Easy", "Normal", "Hard" };
        string message = "D" + string.Format("{0:D2}", storyRound + 1) + "_" + characterName + levelName[miniGameLevel];

        return message;
    }

    public int GetTotalCrushedGem()
    {
        return this.totalCrushedGem;
    }

    public int GetGoalUnit()
    {
        return this.goalUnit;
    }

    public int GetClearGauge()
    {
        return (int)this.score;
    }

    // set game option by pattern
    public void SetGameTimeInit(float fullPlayTime, float playTimeSpeed, float crushedGaugeTime, float fullScore, float scoreSpeed, int goalUnit)
    {
        this.fullPlayTime = fullPlayTime;
        this.playTimeSpeed = playTimeSpeed;
        this.crushedGaugeTime = crushedGaugeTime;
        this.fullScore = fullScore;
        this.scoreSpeed = scoreSpeed;
        this.goalUnit = goalUnit;
    }

    public void SetPlayTime(float offset)
    {
        playTime += offset;
        Debug.Log("guage up! " + offset);
    }

    public void SetTotalCrushedGem(int cnt)
    {
        totalCrushedGem += cnt;
    }
    // ---------------------------------------------------------------

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<BoardManager>();

        // get variable about story from Fungus
        storyRound = SystemManager.Get().IsMultiRound() ? 1 : 0;
        patternIdx = GameManager.Get().GetCharacterIndex();

        UpdatePatternLevel();

        // for test
        //patternLevel = TestLoadMini.patternLevel;
        //patternIdx = TestLoadMini.patternIdx;

        // pattern
        pattern = SpawnPattern(patternIdx);
        pattern.StartPattern(patternLevel);
        GameObject.Find("Counselee").GetComponent<SpriteRenderer>().sprite = Characters[patternIdx];
        scoreFill.GetComponent<Image>().color = counseleeColor[patternIdx];
        scoreFill.transform.GetChild(0).GetComponent<Image>().color = counseleeColor[patternIdx];

        Debug.Log("mini game pattern " + patternIdx);
        Debug.Log("mini game level " + patternLevel);

        // init board option
        InitBoardOption();
    }

    private void InitBoardOption()
    {
        timer.GetComponent<Image>().sprite = timerOrigin;
        timer.transform.GetChild(1).gameObject.SetActive(false);

        playTime = 0;
        score = 0;
        fever = 0;
        isTwinkle = false;
        totalCrushedGem = 0;
        SetScoreUI();

        // ui
        scoreFill.fillAmount = 0;
        timerFill.fillAmount = 0;
        feverFillIMG.fillAmount = 0;

        // set goal
        goalInfo.SetGoal(goalUnit);
    }

    private void UpdatePatternLevel()
    {
        miniGameLevel = GameManager.Get().GetMiniGameLevel();
        patternLevel = (storyRound) * 3 + miniGameLevel;
    }

    private PatternManager SpawnPattern(int patternIdx)
    {
        if (patternIdx == (int)PatternType.YELLOW)
        { // YELLOW
            Debug.Log("Returned Pattern Yellow");
            this.gameObject.AddComponent<PatternYellow>();
            return GetComponent<PatternYellow>();
        }
        else if (patternIdx == (int)PatternType.BLUE)
        { // BLUE
            Debug.Log("Returned Pattern Blue");
            this.gameObject.AddComponent<PatternBlue>();
            return GetComponent<PatternBlue>();
        }
        else if (patternIdx == (int)PatternType.RED)
        { // RED
            Debug.Log("Returned Pattern Red");
            this.gameObject.AddComponent<PatternRed>();
            return GetComponent<PatternRed>();
        }
        else if (patternIdx == (int)PatternType.GREEN)
        { // GREEN
            Debug.Log("Returned Pattern Green");
            this.gameObject.AddComponent<PatternGreen>();
            return GetComponent<PatternGreen>();
        }
        else if (patternIdx == (int)PatternType.PURPLE)
        { // PURPLE
            Debug.Log("Returned Pattern Purple");
            this.gameObject.AddComponent<PatternPurple>();
            return GetComponent<PatternPurple>();
        }

        Debug.Log("No Pattern Found");
        return GetComponent<PatternYellow>();
    }


    private void Update()
    {
        if (bPuzzleMode)
        {
            if (playTime >= fullPlayTime)
            {
                GameOver();
            }

            if (playTime >= fullPlayTime * 0.8f && isTwinkle == false)
            {
                isTwinkle = true;
                StartCoroutine("TwinkleTimer");
            }
            else if (playTime < fullPlayTime * 0.8f && isTwinkle == true)
            {
                isTwinkle = false;
                timer.GetComponent<Image>().sprite = timerOrigin;
                StopCoroutine("TwinkleTimer");
            }

            if (bFeverOn)
            {
                feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFeverTime, feverTime);
                feverTime -= Time.deltaTime;
                if (feverTime <= 0) EndFever();
            }
            else
            {
                timerFill.fillAmount = playTime / fullPlayTime;
                playTime += playTimeSpeed * Time.deltaTime;
            }
        }
    }

    IEnumerator TwinkleTimer()
    {
        while (true)
        {
            timer.GetComponent<Image>().sprite = timerRed;
            yield return new WaitForSeconds(0.5f);
            timer.GetComponent<Image>().sprite = timerOrigin;
            yield return new WaitForSeconds(0.5f);
        }
    }

    public float TimeLeft()
    {
        return playTime;
    }

    public void AddScore(float n)
    {
        score += (float)n * scoreSpeed;
        SetScoreUI();

        if (!bFeverOn)
        {
            AddFever(n);
        }

        // setting UI chatbox of character
        int scorePercent = (int)((score / fullScore) * 100);
        if (scorePercent > 100) scorePercent = 100;
        int prevScorePercent = (int)((score - (float)n * scoreSpeed) / fullScore * 100);
        if (prevScorePercent < 30 && scorePercent >= 30) pattern.SetUIText();
        else if (prevScorePercent < 35 && scorePercent >= 35) pattern.SetUIText();
        else if (prevScorePercent < 40 && scorePercent >= 40) pattern.SetUIText();
        else if (prevScorePercent < 60 && scorePercent >= 60) pattern.SetUIText();
        else if (prevScorePercent < 65 && scorePercent >= 65) pattern.SetUIText();
        else if (prevScorePercent < 80 && scorePercent >= 80) pattern.SetUIText();

        if (scorePercent >= 100 && bPuzzleMode)
        {
            bPuzzleMode = false; // disable playTime count
            board.SetGemMovable(false);
            Invoke(nameof(StartStoryMode), 0.6f);
        }
    }

    private void SetScoreUI()
    {
        int scorePercent = (int)((score / fullScore) * 100);
        if (scorePercent > 100) scorePercent = 100;
        scoreTXT.text = scorePercent.ToString() + " %";
        scoreFill.fillAmount = Mathf.InverseLerp(0, fullScore, score);
    }

    public void AddFever(float n)
    {
        playTime -= n * crushedGaugeTime;
        fever += n;
        feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFever, fever);

        if (fever > fullFever && !bFeverOn && !animator.GetBool("bFeverOn"))
        {
            animator.SetBool("bFeverOn", true);
            feverFillIMG.sprite = feverFillSP[1];
            feverIMG.sprite = feverSP[1];
            feverBTN.enabled = true;
        }
    }

    public void StartFever()
    {
        // reset
        bFeverOn = true;
        feverTime = fullFeverTime;
        boardImg.sprite = puzzleBoardSP[1];

        // stop pattern
        pattern.StopPattern();

        board.StartFever();
        feverBTN.enabled = false;
    }

    public void EndFever()
    {
        animator.SetBool("bFeverOn", false);

        feverBTN.enabled = false;
        boardImg.sprite = puzzleBoardSP[0];
        feverFillIMG.sprite = feverFillSP[0];
        feverIMG.sprite = feverSP[0];

        fever = 0; 
        feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFever, fever);

        bFeverOn = false;
        if (bPuzzleMode && score < fullScore) board.EndFever();

        // restart pattern
        if (bPuzzleMode && score < fullScore) pattern.RestartPattern();
    }

    private void GameOver()
    {
        bPuzzleMode = false;
        StopCoroutine("TwinkleTimer");

        timer.transform.GetChild(1).gameObject.SetActive(true);
        timer.transform.GetChild(1).gameObject.GetComponent<Animator>().Play("mini_game_over");

        board.ClearBoard();
        pattern.StopPattern();
        Invoke(nameof(GameOver_), 2.0f);
    }
    private void GameOver_() 
    { 
        UIGameOver.SetActive(true); 
    }

    private void StartStoryMode()
    {
        bPuzzleMode = false;
        // if game ended in fever mode
        if (bFeverOn) EndFever();

        // clear board sprites
        pattern.StopPattern();
        if (fever != 0) AddFever(-fever);
        UIScore.SetActive(false);
        board.ClearBoard();

        string message = GetFungusMessage();
        Fungus.Flowchart.BroadcastFungusMessage(message);
        Debug.Log(message);
    }

    public void RestartGamePause()
    {
        bPuzzleMode = false;
        board.ClearBoardWithoutAnim();
        pattern.StopPattern();
        RestartGameOver();
    }

    // after dialogue with client
    public void RestartGame()
    {
        EndFever();
        UIScore.SetActive(true);
        board.InitBoard();
        bPuzzleMode = true;

        UpdatePatternLevel();

        pattern.StartPattern(patternLevel);
        InitBoardOption();
    }

    // after game over
    public void RestartGameOver()
    {
        UIGameOver.SetActive(false);

        InitBoardOption(); 
        SetScoreUI();
        EndFever();
        UIScore.SetActive(true);
        if (bFeverOn)
        {
            bFeverOn = false;
            EndFever();
        }
        board.InitBoard();
        bPuzzleMode = true;

        pattern.StartPattern(patternLevel);
    }

    public void SetAfterCounsel(bool bInHaveReport)
    {
        GameManager.Get().SetHaveReport(bInHaveReport);
    }

    public void OnCrushedGemTrigger(int color, List<List<int>> crushedGems)
    {
        this.pattern.OnCrushedGem(color == this.patternIdx, crushedGems);
    }

    public void SkipMinigame()
    {
        AddScore(10000);
    }
}
