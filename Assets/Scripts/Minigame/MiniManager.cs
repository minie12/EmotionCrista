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

public class MiniManager : MonoBehaviour
{    
    // UI
    public Image timer_fill;
    public Image score_fill;
    public Text score_txt;
    public GameObject UI_score;
    public GameObject UI_gameover;

    // timer
    [SerializeField] private const float full_time = 50f;
    private float time;
    [SerializeField] private const float full_fever_time = 10f;
    private float fever_time; // 10f

    // board related
    private BoardManager board;
    public SpriteRenderer board_img;
    public Sprite[] puzzle_board_sp;
    private int goal_unit = 3;

    // score
    public float full_score;
    private int score;

    // fever
    [SerializeField] private const int full_fever = 20;
    private int fever;
    public Image fever_fill_img;
    public Image fever_img;
    public Button fever_btn;
    [HideInInspector]public bool bFever_on = false;
    public Sprite[] fever_sp;
    public Sprite[] fever_fill_sp;
    public Animator animator;

    // pattern
    public PatternManager pattern;
    int pattern_idx = (int)PatternType.RED;

    // game mode
    private bool bPuzzle_mode = true;
    public string fungus_message = "D01_NariaGame"; // used to get chat texts in pattern manager

    void Start(){
        time = full_time; score = 0; fever = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        //board.goal_color = 0;

        // ui
        score_fill.fillAmount = 0;
        timer_fill.fillAmount = 0;
        fever_fill_img.fillAmount = 0;

        if(PlayerPrefs.HasKey("goalUnit")) goal_unit = PlayerPrefs.GetInt("goalUnit");
        board.SetGoal(goal_unit);

        // pattern
        pattern = pattern.SpawnPattern(pattern_idx);
        pattern.StartPattern(0);
    }

    void Update(){
        if(bPuzzle_mode){
            // timer_fill.fillAmount = Mathf.InverseLerp(0, full_time, time);
            timer_fill.fillAmount = time/full_time;
            time -= Time.deltaTime;

            if(time <= 0){
                GameOver();
            }

            if(bFever_on){
                fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever_time, fever_time);
                fever_time -= Time.deltaTime;
                if(fever_time <= 0) EndFever();
            }
        }
    }

    public float TimeLeft(){
        return full_time - time;
    }

    public void AddScore(int n){
        score += n;
        SetScoreUI();
        int score_pt = (int)((score/full_score)*100);
        if(score_pt > 100) score_pt = 100;
        // score_txt.text = (score_pt).ToString() + " %";
        // score_fill.fillAmount = Mathf.InverseLerp(0, full_score, score);

        if(!bFever_on) {
            time += 0.5f;
            AddFever(n);
        }
        
        // setting UI chatbox of character
        int prev_score_pt = (int)((score-n)/full_score*100);
        if(prev_score_pt < 30 && score_pt >= 30) pattern.SetUIText();
        else if(prev_score_pt < 35 && score_pt >= 35) pattern.SetUIText();
        else if(prev_score_pt < 40 && score_pt >= 40) pattern.SetUIText();
        else if(prev_score_pt < 60 && score_pt >= 60) pattern.SetUIText();
        else if(prev_score_pt < 65 && score_pt >= 65) pattern.SetUIText();
        else if(prev_score_pt < 80 && score_pt >= 80) pattern.SetUIText();

        if(score_pt >= 100 && bPuzzle_mode) {
            bPuzzle_mode = false; // disable time count
            board.SetGemMovable(false);
            Invoke("StartStoryMode",0.6f);
        }
    }

    void SetScoreUI(){
        int score_pt = (int)((score/full_score)*100);
        if(score_pt > 100) score_pt = 100;
        score_txt.text = (score_pt).ToString() + " %";
        score_fill.fillAmount = Mathf.InverseLerp(0, full_score, score);
    }

    public void AddFever(int n){
        time += 0.5f;
        fever += n;
        fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        if(fever > full_fever && !bFever_on && !animator.GetBool("bFever_on")){
            animator.SetBool("bFever_on", true);
            fever_fill_img.sprite = fever_fill_sp[1];
            fever_img.sprite = fever_sp[1];
            fever_btn.enabled = true;
        }
    }

    public void StartFever(){
        // reset
        bFever_on = true;
        fever_time = full_fever_time;
        board_img.sprite = puzzle_board_sp[1];

        // stop pattern
        pattern.ClearPattern();

        board.StartFever();
        fever_btn.enabled = false;
    }

    public void EndFever(){
        animator.SetBool("bFever_on", false);

        fever_btn.enabled = false;
        board_img.sprite = puzzle_board_sp[0];
        fever_fill_img.sprite = fever_fill_sp[0];
        fever_img.sprite = fever_sp[0];

        fever = 0; fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        bFever_on = false;
        if(bPuzzle_mode && score < full_score) board.EndFever();

        // restart pattern
        if(bPuzzle_mode && score < full_score) pattern.RestartPattern();
    }

    void GameOver(){
        bPuzzle_mode = false;

        board.ClearBoard();

        pattern.ClearPattern();
        Invoke("GameOver_", 0.6f);
    }
    void GameOver_(){ UI_gameover.SetActive(true);}

    void StartStoryMode(){
        bPuzzle_mode = false;
        // if game ended in fever mode
        if(bFever_on) EndFever();

        // clear board sprites
        pattern.StopPattern();
        if(fever != 0) AddFever(-fever);
        UI_score.SetActive(false);
        board.ClearBoard();

        Fungus.Flowchart.BroadcastFungusMessage(fungus_message);
    }

    public void RestartGame(string color, int gimmick_, string message){
        fungus_message = message;
        
        score = 0; SetScoreUI();
        EndFever();
        time = full_time;
        UI_score.SetActive(true);
        board.InitBoard();
        bPuzzle_mode = true;

        pattern.StartPattern(gimmick_);
    }

    public void RestartGameOver(){
        UI_gameover.SetActive(false);

        score = 0; SetScoreUI();
        EndFever();
        time = full_time;
        UI_score.SetActive(true);
        if(bFever_on){
            bFever_on = false;
            EndFever();
        }
        board.InitBoard();
        bPuzzle_mode = true;

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

    public GemInfo[] GetRandomGems(int cnt)
    {
        // TODO: Does not check whether the gem is already filled with water
        GemInfo[] gems = new GemInfo[cnt];
        int[,] picked_coordinate = new int[cnt, 2];

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
                    if (picked_coordinate[j,0] == column_ && picked_coordinate[j, 1] == row_)
                    {
                        bPicked = true;
                        break;
                    }
                }
            } while (gem == null || gem.bPattern_applied || bPicked);

            picked_coordinate[i, 0] = column_;
            picked_coordinate[i, 1] = row_;

            gems[i] = gem;
        }
        
        return gems;
    }
}
