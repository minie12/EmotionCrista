using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternBlue : PatternManager
{
    private float water_fill_time = 3.8f;
    private string day_and_level = "2hard"; // 1easy: 1, 1normal: 2, 1hard: 3, 2easy: 3, 2normal: 4, 2hard: 5

    override public void StartPattern(int gimmick_){
        gimmick = gimmick_;
        OrganizeCharacterChat();

        if(gimmick == 0) InvokeRepeating("B_StartWaterFill", 0.4f, water_fill_time);
    }

    override public void StopPattern(){ CancelInvoke(); }
    override public void RestartPattern(){
        if (gimmick == 0) InvokeRepeating("B_StartWaterFill", 0.4f, water_fill_time);
    }

    // B1 -----------------------------------------------
    void B_StartWaterFill(){
        int water_gem_cnt = 1;
        float rand = Random.value;
        switch (day_and_level)
        {
            case "1normal":
                if (rand <= 0.6f) water_gem_cnt = 1;
                else water_gem_cnt = 2;
                break;
            case "1hard":
                if (rand <= 0.5f) water_gem_cnt = 1;
                else if (rand <= 0.8f) water_gem_cnt = 2;
                else water_gem_cnt = 3;
                break;
            case "2easy":
                if (rand <= 0.45f) water_gem_cnt = 1;
                else if (rand <= 0.35f) water_gem_cnt = 2;
                else water_gem_cnt = 3;
                break;
            case "2normal":
                if (rand <= 0.45f) water_gem_cnt = 1;
                else if (rand <= 0.8f) water_gem_cnt = 2;
                else if (rand <= 0.95f) water_gem_cnt = 3;
                else water_gem_cnt = 4;
                break;
            case "2hard":
                if (rand <= 0.5f) water_gem_cnt = 1;
                else if (rand <= 0.70f) water_gem_cnt = 2;
                else if (rand <= 0.83f) water_gem_cnt = 3;
                else if (rand <= 0.95f) water_gem_cnt = 4;
                else water_gem_cnt = 5;
                break;
        }

        GemInfo[] gems = mini.GetRandomGems(water_gem_cnt);
        for (int i = 0; i < water_gem_cnt; i++)
        {
            gems[i].FillWaterInHex();
        }
    }
}
