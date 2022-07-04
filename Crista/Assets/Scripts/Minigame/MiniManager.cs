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
        PURPLE
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
    private int goal_unit = 2;

    // score
    public float full_score;
    private int score;

    // fever
    [SerializeField] private const int full_fever = 20;
    private int fever;
    public Image fever_fill_img;
    public Image fever_img;
    public Button fever_btn;
    [HideInInspector]public bool fever_on = false;
    public Sprite[] fever_sp;
    public Sprite[] fever_fill_sp;
    public Animator animator;

    // pattern
    public PatternManager pattern;
    int pattern_idx = 0;

    // game mode
    private bool puzzle_mode = true;
    public string fungus_message = "D01_NariaGame";

    void Start(){
        time = full_time; score = 0; fever = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        board.goal_color = 0;

        // ui
        score_fill.fillAmount = 0;
        timer_fill.fillAmount = 0;
        fever_fill_img.fillAmount = 0;

        if(PlayerPrefs.HasKey("goalUnit")) goal_unit = PlayerPrefs.GetInt("goalUnit");
        board.SetGoal(goal_unit);

        // pattern
        pattern = pattern.SpawnPattern(pattern_idx);
        pattern.StartPattern(-1);
    }

    void Update(){
        if(puzzle_mode){
            // timer_fill.fillAmount = Mathf.InverseLerp(0, full_time, time);
            timer_fill.fillAmount = time/full_time;
            time -= Time.deltaTime;

            if(time <= 0){
                GameOver();
            }

            if(fever_on){
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

        if(!fever_on) {
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

        if(score_pt >= 100 && puzzle_mode) {
            puzzle_mode = false; // disable time count
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

        if(fever > full_fever && !fever_on && !animator.GetBool("fever_on")){
            animator.SetBool("fever_on", true);
            fever_fill_img.sprite = fever_fill_sp[1];
            fever_img.sprite = fever_sp[1];
            fever_btn.enabled = true;
        }
    }

    public void StartFever(){
        // reset
        fever_on = true;
        fever_time = full_fever_time;
        board_img.sprite = puzzle_board_sp[1];

        // stop pattern
        pattern.ClearPattern();

        board.StartFever();
        fever_btn.enabled = false;
    }

    public void EndFever(){
        animator.SetBool("fever_on", false);

        fever_btn.enabled = false;
        board_img.sprite = puzzle_board_sp[0];
        fever_fill_img.sprite = fever_fill_sp[0];
        fever_img.sprite = fever_sp[0];

        fever = 0; fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        fever_on = false;
        if(puzzle_mode && score < full_score) board.EndFever();

        // restart pattern
        if(puzzle_mode && score < full_score) pattern.RestartPattern();
    }

    void GameOver(){
        puzzle_mode = false;

        board.ClearBoard();

        pattern.ClearPattern();
        Invoke("GameOver_", 0.6f);
    }
    void GameOver_(){ UI_gameover.SetActive(true);}

    void StartStoryMode(){
        puzzle_mode = false;
        // if game ended in fever mode
        if(fever_on) EndFever();

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
        puzzle_mode = true;

        pattern.StartPattern(gimmick_);
    }

    public void RestartGameOver(){
        UI_gameover.SetActive(false);

        score = 0; SetScoreUI();
        EndFever();
        time = full_time;
        UI_score.SetActive(true);
        if(fever_on){
            fever_on = false;
            EndFever();
        }
        board.InitBoard();
        puzzle_mode = true;

        pattern.RestartPattern();
    }
}
