using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SurveyButton : MonoBehaviour
{
    private TabletManager tabletManager;

    void Awake(){
        tabletManager = GameObject.Find("Tablet").GetComponent<TabletManager>();
    }

    public void ChangeButtonColor(GameObject btn){
        bool prev_clicked = false;
        foreach(Transform button in transform){
            if(!button.gameObject.GetComponent<Button>().interactable){
                button.gameObject.GetComponent<Image>().color = new Color32(0XFF, 0XFF, 0XFF, 0XFF);
                button.gameObject.GetComponent<Button>().interactable = true;
                prev_clicked = true;
            }
        }
        
        btn.GetComponent<Image>().color = new Color32(0xBE, 0xBF, 0xBE, 0xFF);
        btn.GetComponent<Button>().interactable = false;
        if(!prev_clicked) tabletManager.survey_cnt++;
    }
}
