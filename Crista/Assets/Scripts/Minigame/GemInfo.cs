using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    public int column, row;
    public int color;
    // 0: red, 1: yellow, 2: green, 3: blue, 4: purple

    public Animator anim; 

    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void PrintInfo(){
        Debug.Log(column+ " " + row + " " + color);
    }

    // Start is called before the first frame update
    void OnMouseUp(){
        if(board.gem_movable && !board.fever_on){
            board.GemClick(column, row);
        }
        else if(board.fever_on){
            board.FeverClick(column,row);
        }
    }

    public void DestroyGem(){
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("DestroyGemC");
    }

    IEnumerator DestroyGemC(){
        anim.Play("gem_sparkle", 0, 0.0f);
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
