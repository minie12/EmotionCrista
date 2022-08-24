using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    private float water_fill_time = 3.8f;
    private float water_gem_cnt = 1; // 1easy: 1, 1normal: 2, 1hard: 3, 2easy: 3, 2normal: 4, 2hard: 5

    override public void StartPattern(int gimmick_){
        gimmick = gimmick_;
        OrganizeCharacterChat();

        if(gimmick == 0)
        {
            InvokeRepeating("B_StartWaterFill", 0.4f, water_fill_time);
        }
    }

    override public void StopPattern(){ CancelInvoke(); }
    override public void RestartPattern(){}

    // B1 -----------------------------------------------
    void B_StartWaterFill(){
        for(int i = 0; i < water_gem_cnt; i++){
            mini.GetRandomGem().FillWaterInHex();
        }
    }
}
