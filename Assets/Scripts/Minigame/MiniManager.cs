using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public enum PatternType{
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

    // timer
    [SerializeField] private const float fullPlayTime = 50f;
    private float playTime;
    [SerializeField] private const float fullFeverTime = 10f;
    private float feverTime; // 10f

    // board related
    private BoardManager board;
    public SpriteRenderer boardImg;
    public Sprite[] puzzleBoardSP;
    private int goalGemCnt = 3;

    // score
    public float fullScore;
    private int score;

    // fever
    [SerializeField] private const int fullFever = 20;
    private int fever;
    public Image feverFillIMG;
    public Image feverIMG;
    public Button feverBTN;
    [HideInInspector]public bool bFeverOn = false;
    public Sprite[] feverSP;
    public Sprite[] feverFillSP;
    public Animator animator;

    // pattern
    public PatternManager pattern;
    int patternIdx = (int)PatternType.YELLOW;
    int patternGimmick = 2;
    int patternLevel = (int)LevelType.EASY1;

    // game mode
    private bool bPuzzleMode = true;
    public string fungusMessage = "D01_NariaGame"; // used to get chat texts in pattern manager

    // Get & Set -----------------------------------------------------
    public string GetFungusMessage() { return fungusMessage; }
    // ---------------------------------------------------------------

    void Start(){
        playTime = fullPlayTime; score = 0; fever = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        // temp
        board.goalColor = patternIdx;

        // ui
        scoreFill.fillAmount = 0;
        timerFill.fillAmount = 0;
        feverFillIMG.fillAmount = 0;

        if(PlayerPrefs.HasKey("goalUnit")) goalGemCnt = PlayerPrefs.GetInt("goalUnit");
        board.SetGoal(goalGemCnt);

        // pattern
        pattern = SpawnPattern(patternIdx);
        pattern.StartPattern(patternGimmick, patternLevel);
    }

    public PatternManager SpawnPattern(int patternIdx)
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


    void Update(){
        if(bPuzzleMode){
            // timerFill.fillAmount = Mathf.InverseLerp(0, fullPlayTime, playTime);
            timerFill.fillAmount = playTime/fullPlayTime;
            playTime -= Time.deltaTime;

            if(playTime <= 0){
                GameOver();
            }

            if(bFeverOn){
                feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFeverTime, feverTime);
                feverTime -= Time.deltaTime;
                if(feverTime <= 0) EndFever();
            }
        }
    }

    public float TimeLeft(){
        return fullPlayTime - playTime;
    }

    public void AddScore(int n){
        score += n;
        SetScoreUI();
        int scorePercent = (int)((score/fullScore)*100);
        if(scorePercent > 100) scorePercent = 100;
        // scoreTXT.text = (scorePercent).ToString() + " %";
        // scoreFill.fillAmount = Mathf.InverseLerp(0, fullScore, score);

        if(!bFeverOn) {
            playTime += 0.5f;
            AddFever(n);
        }
        
        // setting UI chatbox of character
        int prevScorePercent = (int)((score-n)/fullScore*100);
        if(prevScorePercent < 30 && scorePercent >= 30) pattern.SetUIText();
        else if(prevScorePercent < 35 && scorePercent >= 35) pattern.SetUIText();
        else if(prevScorePercent < 40 && scorePercent >= 40) pattern.SetUIText();
        else if(prevScorePercent < 60 && scorePercent >= 60) pattern.SetUIText();
        else if(prevScorePercent < 65 && scorePercent >= 65) pattern.SetUIText();
        else if(prevScorePercent < 80 && scorePercent >= 80) pattern.SetUIText();

        if(scorePercent >= 100 && bPuzzleMode) {
            bPuzzleMode = false; // disable playTime count
            board.SetGemMovable(false);
            Invoke("StartStoryMode",0.6f);
        }
    }

    void SetScoreUI(){
        int scorePercent = (int)((score/fullScore)*100);
        if(scorePercent > 100) scorePercent = 100;
        scoreTXT.text = (scorePercent).ToString() + " %";
        scoreFill.fillAmount = Mathf.InverseLerp(0, fullScore, score);
    }

    public void AddFever(int n){
        playTime += 0.5f;
        fever += n;
        feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFever, fever);

        if(fever > fullFever && !bFeverOn && !animator.GetBool("bFeverOn")){
            animator.SetBool("bFeverOn", true);
            feverFillIMG.sprite = feverFillSP[1];
            feverIMG.sprite = feverSP[1];
            feverBTN.enabled = true;
        }
    }

    public void StartFever(){
        // reset
        bFeverOn = true;
        feverTime = fullFeverTime;
        boardImg.sprite = puzzleBoardSP[1];

        // stop pattern
        pattern.ClearPattern();

        board.StartFever();
        feverBTN.enabled = false;
    }

    public void EndFever(){
        animator.SetBool("bFeverOn", false);

        feverBTN.enabled = false;
        boardImg.sprite = puzzleBoardSP[0];
        feverFillIMG.sprite = feverFillSP[0];
        feverIMG.sprite = feverSP[0];

        fever = 0; feverFillIMG.fillAmount = Mathf.InverseLerp(0, fullFever, fever);

        bFeverOn = false;
        if(bPuzzleMode && score < fullScore) board.EndFever();

        // restart pattern
        if(bPuzzleMode && score < fullScore) pattern.RestartPattern();
    }

    void GameOver(){
        bPuzzleMode = false;

        board.ClearBoard();

        pattern.ClearPattern();
        Invoke("GameOver_", 0.6f);
    }
    void GameOver_(){ UIGameOver.SetActive(true);}

    void StartStoryMode(){
        bPuzzleMode = false;
        // if game ended in fever mode
        if(bFeverOn) EndFever();

        // clear board sprites
        pattern.StopPattern();
        if(fever != 0) AddFever(-fever);
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
    public void RestartGame(string color, int gimmick_, string message){
        fungusMessage = message;
        
        score = 0; SetScoreUI();
        EndFever();
        playTime = fullPlayTime;
        UIScore.SetActive(true);
        board.InitBoard();
        bPuzzleMode = true;

        pattern.StartPattern(gimmick_, patternLevel);
    }

    // after game over
    public void RestartGameOver(){
        UIGameOver.SetActive(false);

        score = 0; SetScoreUI();
        EndFever();
        playTime = fullPlayTime;
        UIScore.SetActive(true);
        if(bFeverOn){
            bFeverOn = false;
            EndFever();
        }
        board.InitBoard();
        bPuzzleMode = true;

        pattern.RestartPattern();
    }

    // PATTERN RELATED
    public GemInfo GetRandomGem(){
        // TODO: Does not check whether the gem is already filled with water

        int column_ = Random.Range(0, 11);
        int row_ = Random.Range(0, 6);
        GemInfo gem = board.GetGem(column_, row_);
        while(gem == null){
            column_ = Random.Range(0, 11);
            row_ = Random.Range(0, 6);
            gem = board.GetGem(column_, row_);
        }
        return gem;
    }

    public GemInfo GetPatternGemRandom()
    {
        while (true)
        {
            GemInfo randGem = GetRandomGem();
            if (randGem.GetColor() == patternIdx)
            {
                return randGem;
            }
        }
    }

    public GemInfo GetRandomGemOnWay(int current_c, int current_r)
    {
        GemInfo gem = null;
        while(gem == null)
        {
            // 0: up, 1: up&right, 2: down&right, 3:down, 4: down&left, 5: up&left
            int direction = Random.Range(0, 6);
            int distance = Random.Range(0, 10);
            int odd = current_c % 2;
            int column_ = current_c, row_ = current_r;
            switch (direction)
            {
                case 0:
                    row_ += distance;
                    gem = board.GetGem(column_, row_);
                    break;
                case 1:
                    column_ += distance; row_ += ((odd == 1) ? distance/2 : (distance+1)/2);
                    gem = board.GetGem(column_, row_);
                    break;
                case 2:
                    column_ += distance; row_ -= ((odd == 0) ? distance / 2 : (distance + 1) / 2);
                    gem = board.GetGem(column_, row_);
                    break;
                case 3: // down
                    row_ -= distance;
                    gem = board.GetGem(column_, row_);
                    break;
                case 4:
                    column_ -= distance; row_ -= ((odd == 0) ? distance / 2 : (distance + 1) / 2);
                    gem = board.GetGem(column_, row_);
                    break;
                case 5:
                    column_ -= distance; row_ += ((odd == 1) ? distance / 2 : (distance + 1) / 2);
                    gem = board.GetGem(column_, row_);
                    break;
                default:
                    break;
            }
        }
        return gem;
    }

    // get 6 direction around gem list
    public List<List<GemInfo>> GetAroundGemList(int current_c, int current_r)
    {
        List<List<GemInfo>> aroundGemList = new List<List<GemInfo>>();
        int odd = current_c % 2;

        // init 6 direction
        // 0: up, 1: up&right, 2: down&right, 3:down, 4: down&left, 5: up&left
        for (int i = 0; i < 6; i++) // direction
        {
            List<GemInfo> temp = new List<GemInfo>();
            for(int j = 1; j < 10; j++) // distance
            {
                int column_ = current_c, row_ = current_r;
                GemInfo gem = null;
                switch (i)
                {
                    case 0:
                        row_ += j;
                        gem = board.GetGem(column_, row_);
                        break;
                    case 1:
                        column_ += j; row_ += ((odd == 1) ? j / 2 : (j + 1) / 2);
                        gem = board.GetGem(column_, row_);
                        break;
                    case 2:
                        column_ += j; row_ -= ((odd == 0) ? j / 2 : (j + 1) / 2);
                        gem = board.GetGem(column_, row_);
                        break;
                    case 3: // down
                        row_ -= j;
                        gem = board.GetGem(column_, row_);
                        break;
                    case 4:
                        column_ -= j; row_ -= ((odd == 0) ? j / 2 : (j + 1) / 2);
                        gem = board.GetGem(column_, row_);
                        break;
                    case 5:
                        column_ -= j; row_ += ((odd == 1) ? j / 2 : (j + 1) / 2);
                        gem = board.GetGem(column_, row_);
                        break;
                    default:
                        break;
                }
                if (gem == null)
                {
                    break;
                }
                temp.Add(gem);
            }
            aroundGemList.Add(temp);
        }
        return aroundGemList;
    }


    public GemInfo[] GetRandomGems(int cnt)
    {
        // TODO: Does not check whether the gem is already filled with water
        GemInfo[] gems = new GemInfo[cnt];
        int[,] pickedCoordinates = new int[cnt, 2];

        for (int i = 0; i < cnt; i++)
        {
            GemInfo gem; int column_, row_;
            bool bPicked;
            do
            {
                bPicked = false;
                column_ = Random.Range(0, 11);
                row_ = Random.Range(0, 6);
                gem = board.GetGem(column_, row_);

                // check if this gem is already picked
                for (int j = 0; j < i; j++)
                {
                    if (pickedCoordinates[j,0] == column_ && pickedCoordinates[j, 1] == row_)
                    {
                        bPicked = true;
                        break;
                    }
                }
            } while (gem == null || gem.bPatternApplied || bPicked);

            pickedCoordinates[i, 0] = column_;
            pickedCoordinates[i, 1] = row_;

            gems[i] = gem;
        }
        
        return gems;
    }
}
