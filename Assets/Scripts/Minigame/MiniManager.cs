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
    [SerializeField] private const float fullPlayTime = 50f;
    private float playTime;
    [SerializeField] private const float fullFeverTime = 10f;
    private float feverTime;
    private bool isTwinkle;

    // board related
    private BoardManager board;
    public SpriteRenderer boardImg;
    public Sprite[] puzzleBoardSP;
    public GoalInfo goalInfo;
    public int goalUnit = 3; // goal gem count

    // score
    public float fullScore;
    private int score;

    // fever
    [SerializeField] private const int fullFever = 20;
    private int fever;
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
    [HideInInspector] public int patternGimmick;
    [HideInInspector] public int patternLevel;

    // game mode
    private bool bPuzzleMode = true;
    public string fungusMessage = "D01_NariaGame"; // used to get chat texts in pattern manager

    // Get & Set -----------------------------------------------------
    public string GetFungusMessage() { return fungusMessage; }
    // ---------------------------------------------------------------

    private void Start()
    {
        playTime = 0;
        score = 0;
        fever = 0;
        isTwinkle = false;
        board = GameObject.Find("Board").GetComponent<BoardManager>();

        // ui
        scoreFill.fillAmount = 0;
        timerFill.fillAmount = 0;
        feverFillIMG.fillAmount = 0;

        // set goal
        if (PlayerPrefs.HasKey("goalUnit")) goalUnit = PlayerPrefs.GetInt("goalUnit");
        goalInfo.SetGoal(goalUnit);

        // set pattern
        //patternIdx = TestLoadMini.patternIdx;
        //patternGimmick = TestLoadMini.patternGimmick;
        //patternLevel = TestLoadMini.patternLevel;
        Fungus.Flowchart flowchart = GameObject.Find("Flowchart").GetComponent<Fungus.Flowchart>();
        int storyRound = 0;
        int level = 0;
        if (flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex") != null)
        {
            patternIdx = flowchart.GetVariable<Fungus.IntegerVariable>("CharacterIndex").Value;
        }
        if (flowchart.GetVariable<Fungus.IntegerVariable>("StoryRound") != null)
        {
            storyRound = flowchart.GetVariable<Fungus.IntegerVariable>("StoryRound").Value;
        }
        if (flowchart.GetVariable<Fungus.IntegerVariable>("Level") != null)
        {
            level = flowchart.GetVariable<Fungus.IntegerVariable>("Level").Value;
        }
        patternLevel = storyRound * level;
        patternGimmick = 0;


        // pattern
        pattern = SpawnPattern(patternIdx);
        pattern.StartPattern(patternGimmick, patternLevel);
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
            timerFill.fillAmount = playTime / fullPlayTime;
            playTime += Time.deltaTime;

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
                StopCoroutine("TwinkleTimer");
            }

            if (bFeverOn)
            {
                feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFeverTime, feverTime);
                feverTime -= Time.deltaTime;
                if (feverTime <= 0) EndFever();
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
        score += n;
        SetScoreUI();
        int scorePercent = (int)((score / fullScore) * 100);
        if (scorePercent > 100) scorePercent = 100;
        // scoreTXT.text = (scorePercent).ToString() + " %";
        // scoreFill.fillAmount = Mathf.InverseLerp(0, fullScore, score);

        if (!bFeverOn)
        {
            playTime -= 0.5f;
            AddFever(n);
        }

        // setting UI chatbox of character
        int prevScorePercent = (int)((score - n) / fullScore * 100);
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
        scoreTXT.text = (scorePercent).ToString() + " %";
        scoreFill.fillAmount = Mathf.InverseLerp(0, fullScore, score);
    }

    public void AddFever(int n)
    {
        playTime -= 0.5f;
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
        pattern.ClearPattern();

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

        fever = 0; feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFever, fever);

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

        board.ClearBoard();
        pattern.ClearPattern();
        Invoke(nameof(GameOver_), 2.0f);
    }
    private void GameOver_() { UIGameOver.SetActive(true); }

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

        Fungus.Flowchart.BroadcastFungusMessage(fungusMessage);
    }

    public void RestartGamePause()
    {
        bPuzzleMode = false;
        board.ClearBoardWithoutAnim();
        pattern.ClearPattern();
        RestartGameOver();
    }

    // after dialogue with client
    public void RestartGame(string color, int gimmick_, string message)
    {
        fungusMessage = message;

        score = 0; SetScoreUI();
        EndFever();
        playTime = 0;
        UIScore.SetActive(true);
        board.InitBoard();
        bPuzzleMode = true;

        pattern.StartPattern(gimmick_, patternLevel);
    }

    // after game over
    public void RestartGameOver()
    {
        UIGameOver.SetActive(false);

        score = 0; SetScoreUI();
        EndFever();
        playTime = 0;
        UIScore.SetActive(true);
        if (bFeverOn)
        {
            bFeverOn = false;
            EndFever();
        }
        board.InitBoard();
        bPuzzleMode = true;

        pattern.RestartPattern();
    }
}
