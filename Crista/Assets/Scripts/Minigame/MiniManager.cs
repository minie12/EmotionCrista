using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniManager : MonoBehaviour
{
    public enum patternType{
        YELLOW,
        BLUE,
        RED,
        GREEN,
        PURPLE
    }

    // UI
    public Image timer_fill;
    public Image score_fill;
    public Text score_txt;

    // timer
    public static float full_time;
    public static float time;
    public float full_fever_time = 10f;
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
    public int full_fever;
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
    

    // move to GameManager (after adding main)
    void Awake(){
        Screen.SetResolution(1920, 1080, true);
    }


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
        pattern.StartPattern();
    }

    void Update(){
        // timer_fill.fillAmount = Mathf.InverseLerp(0, full_time, time);
        timer_fill.fillAmount = time/full_time;
        time -= Time.deltaTime;

        if(fever_on){
            fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever_time, fever_time);
            fever_time -= Time.deltaTime;
            if(fever_time <= 0) EndFever();
        }
    }

    public void AddScore(int n){
        score += n;
        int score_pt = (int)((score/full_score)*100);
        if(score_pt > 100) score_pt = 100;
        score_txt.text = (score_pt).ToString() + " %";
        score_fill.fillAmount = Mathf.InverseLerp(0, full_score, score);

        if(!fever_on) AddFever(n);
    }

    public void AddFever(int n){
        fever += n;
        fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        if(fever > full_fever && !fever_on) {
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
        fever = 0;
        board_img.sprite = puzzle_board_sp[1];


        //reset fever gauge
        // fever_fill_img.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        // stop pattern
        pattern.StartFever();

        board.StartFever();
        fever_btn.enabled = false;
    }

    public void EndFever(){
        animator.SetBool("fever_on", false);

        board_img.sprite = puzzle_board_sp[0];
        fever_fill_img.sprite = fever_fill_sp[0];
        fever_img.sprite = fever_sp[0];
        fever_on = false;
        board.EndFever();

        // restart pattern
        pattern.EndFever();
    }


    // used to move to another scene
    public void SceneTransfer()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
