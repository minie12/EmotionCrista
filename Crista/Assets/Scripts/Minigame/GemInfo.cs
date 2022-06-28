using System.Collections;
using System.Collections.Generic;
using UnityEngine;


enum Color{ Yellow = 0, Blue = 1, Red = 2, Green = 3, Purple = 4}

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    private int column, row;
    Color color;

    public Sprite[] gem_sprites;
    public Animator ANsparkle; 
    public Animator ANgem; 


    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void InitGem(int column_, int row_, int color_){
        column = column_; row = row_; 
        if(color_ == (int)Color.Yellow) color = Color.Yellow;
        else if(color_ == (int)Color.Blue) color = Color.Blue;
        else if(color_ == (int)Color.Red) color = Color.Red;
        else if(color_ == (int)Color.Green) color = Color.Green;
        else color = Color.Purple;
        gameObject.GetComponent<SpriteRenderer>().sprite = gem_sprites[(int)color];
    }

    public void PrintInfo(){
        Debug.Log(column+ " " + row + " " + color);
    }

    // Start is called before the first frame update
    void OnMouseUp(){
        if(board.CheckFever()){
            board.FeverClick(column,row);
        }
        else if(board.gem_movable){
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

        if(color == Color.Red) ANgem.Play("gem_crush_red",0, 0.0f);
        else if(color == Color.Yellow) ANgem.Play("gem_crush_yellow",0, 0.0f);
        else if(color == Color.Green) ANgem.Play("gem_crush_green",0, 0.0f);
        else if(color == Color.Blue) ANgem.Play("gem_crush_blue",0, 0.0f);
        else if(color == Color.Purple) ANgem.Play("gem_crush_purple",0, 0.0f);
        
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
        yield return new WaitForSeconds(0.01f); // used to prevent many accesses to GetGemPosition() at the same time
        Vector3 end_pos = board.GetGemPosition(column, row);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(start_pos, end_pos, t / time);
            yield return 0;
        }

        transform.position = end_pos;
        
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
