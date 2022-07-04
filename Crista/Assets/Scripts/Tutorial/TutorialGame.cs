using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TutorialGame : MonoBehaviour
{
    private bool clickable = false;
    public Sprite SPmiddle_gem;
    public Sprite SPside_gem;
    public GameObject click_effect;

    public GameObject parent_gem;
    public SpriteRenderer[] gem_outline;



    // position
    private float[,] positions = new float[6,2]{{0.755f, 0.444f}, {0f, 0.894f}, {-0.749f, 0.444f},
                                                {-0.75f, -0.456f}, {0f, -0.904f}, {0.759f, -0.458f}}; 

        // Start is called before the first frame update
    void OnMouseUp(){
        if(clickable){
            clickable = false;
            gem_outline[0].sprite = SPmiddle_gem;

            for(int i = 0; i < gem_outline.GetLength(0); i++){
                gem_outline[i].sprite = SPside_gem;
            }

            click_effect.SetActive(true);
            Fungus.Flowchart.BroadcastFungusMessage("ClickGem");
        }
    }
    public void ActivateClick(){
        clickable = true;
    }
    public void DeactivateGems(){
        for(int i = 0; i < gem_outline.GetLength(0); i++){
            gem_outline[i].sprite = null;
        }
    }
}
