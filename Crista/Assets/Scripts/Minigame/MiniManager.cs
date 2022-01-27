using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class MiniManager : MonoBehaviour
{


    // UI
    public Image timer_fill;
    public Image fever_fill;
    public Image fever_bg;
    public Image score_fill;
    public Text score_txt;

    // timer
    public int full_time;
    private float time;

    // board related
    private BoardManager board;
    private int goal_num = 0;

    // score
    public int full_score;
    private int score;
    public int full_fever;
    private int fever;
    public GameObject fever_btn;
    private bool fever_on = false;

    void Start(){
        time = 30; score = 0; fever = 0;
        board = GameObject.Find("Board").GetComponent<BoardManager>();
        board.goal_color = 0;

        // ui
        score_fill.fillAmount = 0;
        timer_fill.fillAmount = 0;
        fever_fill.fillAmount = 0;

        if(PlayerPrefs.HasKey("goalNum")) goal_num = PlayerPrefs.GetInt("goalNum");
    }

    void Update(){
        // timer_fill.fillAmount = Mathf.InverseLerp(0, full_time, time);
        timer_fill.fillAmount = time/full_time;
        time -= Time.deltaTime;
    }

    public void AddScore(int n){
        score += n;
        score_txt.text = (score/full_score).ToString() + " %";
        score_fill.fillAmount = Mathf.InverseLerp(0, full_score, score);

        if(!fever_on) AddFever(n);
    }

    public void AddFever(int n){
        fever += n;
        fever_fill.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        if(fever > full_fever && !fever_on) fever_btn.SetActive(true);
    }

    public void StartFever(){
        //reset fever gauge
        fever = 0;
        fever_fill.fillAmount = Mathf.InverseLerp(0, full_fever, fever);

        fever_on = true;
        board.StartFever();
        fever_btn.SetActive(false);
    }

    public void EndFever(){
        fever_on = false;
    }

    // used to move to another scene
    public void SceneTransfer()
    {
        string nowbutton = EventSystem.current.currentSelectedGameObject.name;
        SceneManager.LoadScene(nowbutton, LoadSceneMode.Single);
    }
}
