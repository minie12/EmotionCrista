using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    private int column, row;

    private PatternType color;
    public Sprite[] gem_sprites;

    public SpriteRenderer gem_outline;
    public Sprite SPgem_click;
    public Sprite SPgem_side;

    public Animator ANsparkle; 
    public Animator ANgem; 


    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void InitGem(int column_, int row_, int color_){
        column = column_; row = row_; 
        if(color_ == (int)PatternType.YELLOW) color = PatternType.YELLOW;
        else if(color_ == (int)PatternType.BLUE) color = PatternType.BLUE;
        else if(color_ == (int)PatternType.RED) color = PatternType.RED;
        else if(color_ == (int)PatternType.GREEN) color = PatternType.GREEN;
        else color = PatternType.PURPLE;
        gameObject.GetComponent<SpriteRenderer>().sprite = gem_sprites[(int)color];
    }

    /// <summary> Set outline sprite of gem </summary>
    /// <param name="type"> click, side, undo. </param>
    public void SetOutline(string type){
        switch (type)
        {
            case "click":
                gem_outline.sprite = SPgem_click;
                break;
            case "side":
                gem_outline.sprite = SPgem_side;
                break;
            case "undo":
                gem_outline.sprite = null;
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

    public void DestroyGem(){
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("DestroyGemC");
    }

    IEnumerator DestroyGemC(){
        ANgem.enabled = true;

        if(color == PatternType.RED) ANgem.Play("gem_crush_red",0, 0.0f);
        else if(color == PatternType.YELLOW) ANgem.Play("gem_crush_yellow",0, 0.0f);
        else if(color == PatternType.GREEN) ANgem.Play("gem_crush_green",0, 0.0f);
        else if(color == PatternType.BLUE) ANgem.Play("gem_crush_blue",0, 0.0f);
        else if(color == PatternType.PURPLE) ANgem.Play("gem_crush_purple",0, 0.0f);
        
        ANsparkle.Play("gem_sparkle", 0, 0.0f);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    public void MoveGem(int column_, int row_, float time){
        row = row_; column = column_;

        StartCoroutine(MoveGemC(time));
    }
    IEnumerator MoveGemC(float time){    
        gameObject.GetComponent<Collider2D>().enabled = false;

        Vector3 start_pos = transform.position;
        yield return new WaitForSeconds(0.01f); // used to avoid error but why?
        Vector3 end_pos = board.GetGemPosition(column, row);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(start_pos, end_pos, t / time);
            yield return 0;
        }

        transform.position = end_pos;
        
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
