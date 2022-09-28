using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternRed : PatternManager
{
    // check gimmick 0 is running
    private bool isRunning = false; 

    // Setting gimmick
    override public void StartPattern(int gimmick_)
    {
        gimmick = gimmick_;
        OrganizeCharacterChat();

        if (gimmick == 0)
        {
            isRunning = true;
            StartCoroutine(RedExplosion());
        }
    }

    override public void StopPattern() { isRunning = false; }
    override public void RestartPattern()
    {
        if (gimmick == 0)
        {
            isRunning = true;
            StartCoroutine(RedExplosion());
        }
    }

    // Red gimmick 0
    IEnumerator RedExplosion(){
        while (isRunning)
        {

            yield return null;
        }
    }

    public void RedExplosionTest()
    {

    }

    public void Explosion()
    {

    }
}
