using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class PatternManager : MonoBehaviour
{

    // YELLOW Pattern
    [HideInInspector]
    public GameObject UI_canvas;

    void Start(){
        UI_canvas = GameObject.Find("PatternCanvas");
    }

    public PatternManager SpawnPattern(int pattern_idx){
        if(pattern_idx == 0){ // YELLOW
            return GetComponent<PatternYellow>();
        }

        return GetComponent<PatternYellow>();
    }

    virtual public void StartPattern(){}
    virtual public void StartFever(){}
    virtual public void EndFever(){}
}
