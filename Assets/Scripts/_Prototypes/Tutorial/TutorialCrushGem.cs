using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialCrushGem : MonoBehaviour
{
    
    private bool crushable = false;

    public GameObject gem;

    // anim
    public Animator[] ANsparkle; 
    public Animator[] ANgem;

    void OnMouseUp(){
        if(crushable){
            // crush gem
            ANgem[0].Play("gem_crush_yellow",0, 0.0f);
            ANsparkle[0].Play("gem_sparkle", 0, 0.0f);
            ANgem[1].Play("gem_crush_yellow",0, 0.0f);
            ANsparkle[1].Play("gem_sparkle", 0, 0.0f);
            Invoke("DestroyGem", 0.3f);

            Fungus.Flowchart.BroadcastFungusMessage("CrushGem");
        }   
    }
    void DestroyGem(){
        Destroy(gem);
        Destroy(gameObject);
    }

    
    public void ActivateCrush(){
        crushable = true;
    }

}
