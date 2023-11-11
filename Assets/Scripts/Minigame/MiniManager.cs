using System;
using System.Collections;
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
    private int fullFever = 20;
    private int fever = 0;
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
        string characterName = Enum.GetName(typeof(CharacterName), patternIdx + 1);
        string[] levelName = { "Easy", "Normal", "Hard" };
        string message = "D" + string.Format("{0:D2}", storyRound + 1) + "_" + characterName + levelName[miniGameLevel];

        return message;
    }

    // get fungus variable (check null)
    int GetFungusVariable(Fungus.Flowchart flowchart, string variableName)
    {
        if (flowchart.GetVariable<Fungus.IntegerVariable>(variableName) != null)
        {
            return flowchart.GetVariable<Fungus.IntegerVariable>(variableName).Value;
        }
        return 0;
    }

    public int GetTotalCrushedGem()
    {
        return this.totalCrushedGem;
    }

    public int GetGoalUnit()
    {
        return this.goalUnit;
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
    // ---------------------------------------------------------------

    private void Start()
    {
        board = GameObject.Find("Board").GetComponent<BoardManager>();

        // get variable about story from Fungus
        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();
        miniGameLevel = GetFungusVariable(flowchart, "Level"); // 0: easy, 1: normal, 2: hard
        storyRound = GetFungusVariable(flowchart, "StoryRound"); // 0: 1회차, 1: 다회차
        patternIdx = GetFungusVariable(flowchart, "CharacterIndex");
        patternLevel = (storyRound) * 3 + miniGameLevel;

        // pattern
        pattern = SpawnPattern(patternIdx);
        pattern.StartPattern(patternLevel);

        // init board option
        InitBoardOption();
    }

    private void InitBoardOption()
    {
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

    public void AddScore(int n)
    {
        score += (float)n * scoreSpeed;
        totalCrushedGem += n;
        SetScoreUI();

        if (!bFeverOn)
        {
            playTime -= n * crushedGaugeTime;
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

    public void AddFever(int n)
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
        if (bPuzzleMode && score < fullScore) pattern.StartPattern(patternLevel);
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
    public void RestartGame(string color, int gimmick_)
    {
        EndFever();
        UIScore.SetActive(true);
        board.InitBoard();
        bPuzzleMode = true;

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
}
