using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    private int column, row;

    private PatternType color;
    public Sprite[] gemSprites;

    public SpriteRenderer gemOutline;
    // public SpriteRenderer gem_pattern;
    public Sprite gemClickSP;
    public Sprite gemSideSP;

    public Animator sparkleANIM; 
    public Animator patternANIM; 
    public Animator gemANIM;

    public bool bPatternApplied;

    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void InitGem(int column_, int row_, int color_){
        bPatternApplied = false;
        column = column_; row = row_; 
        SetColor_(color_);
    }

    /// <summary> Set outline sprite of gem </summary>
    /// <param name="type"> click, side, undo. </param>
    public void SetOutline(string type){
        switch (type)
        {
            case "click":
                gemOutline.sprite = gemClickSP; // middle of hex when clicked
                break;
            case "side":
                gemOutline.sprite = gemSideSP; // sides of hex when clicked
                break;
            case "undo":
                gemOutline.sprite = null; // undo the click
                break;
        }
    }

    public void PrintInfo(){
        Debug.Log(column+ " " + row + " " + color);
    }

    // Start is called before the first frame update
    void OnMouseUp(){
        if(board.CheckFever()){
            board.FeverClick(column,row);
        }
        else if(board.GetGemMovable()){
            board.GemClick(column, row);
        }
    }

    public int GetColor(){
        return (int)color;
    }

    public void SetColor(){
        int now;
        int prev = now = (int)color;

        while(prev == now){
            now = Random.Range(0, board.GetGemTypeCnt());
        }

        SetColor_(now);
    }
    
    void SetColor_(int color_){
        if(color_ == (int)PatternType.YELLOW) color = PatternType.YELLOW;
        else if(color_ == (int)PatternType.BLUE) color = PatternType.BLUE;
        else if(color_ == (int)PatternType.RED) color = PatternType.RED;
        else if(color_ == (int)PatternType.GREEN) color = PatternType.GREEN;
        else color = PatternType.PURPLE;

        gameObject.GetComponent<SpriteRenderer>().sprite = gemSprites[(int)color];
    }

    public void DestroyGem(){
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("DestroyGemC");
    }
    IEnumerator DestroyGemC(){
        gemANIM.enabled = true;

        if(color == PatternType.RED) gemANIM.Play("gem_crush_red",0, 0.0f);
        else if(color == PatternType.YELLOW) gemANIM.Play("gem_crush_yellow",0, 0.0f);
        else if(color == PatternType.GREEN) gemANIM.Play("gem_crush_green",0, 0.0f);
        else if(color == PatternType.BLUE) gemANIM.Play("gem_crush_blue",0, 0.0f);
        else if(color == PatternType.PURPLE) gemANIM.Play("gem_crush_purple",0, 0.0f);
        
        sparkleANIM.Play("gem_sparkle", 0, 0.0f);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    public void FillWaterInHex(){
        bPatternApplied = true;  // so that this gem does not get selected at GetRandomGem()
        patternANIM.Play("gem_fill_water", 0, 0.0f);
        StartCoroutine("FillWaterInHexC");
    }

    IEnumerator FillWaterInHexC(){
        yield return new WaitForSeconds(0.1f);

        // check whether gem_fill_water animation is done or not
        while(!patternANIM.GetCurrentAnimatorStateInfo(0).IsTag("changeColor")){
            yield return new WaitForSeconds(0.1f);
        }

        SetColor(); // change color of gem
        bPatternApplied = false;
        patternANIM.SetBool("bWaterFilled", true);
    }

    public void MoveGem(int column_, int row_, float time){
        row = row_; column = column_;

        StartCoroutine(MoveGemC(time));
    }
    IEnumerator MoveGemC(float time){    
        gameObject.GetComponent<Collider2D>().enabled = false;

        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(0.01f); // used to avoid error but why?
        Vector3 endPos = board.GetGemPosition(column, row);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        transform.position = endPos;
        
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
