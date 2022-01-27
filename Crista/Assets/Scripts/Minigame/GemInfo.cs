using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    public GameObject outline;
    public int column, row;
    public int color;

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
        Destroy(gameObject);
    }

    public void MoveGem(float time){
        StartCoroutine(MoveGemC(time));
    }
    public void MoveGem(int column_, int row_, float time){
        row = row_; column = column_;

        StartCoroutine(MoveGemC(time));
    }
    IEnumerator MoveGemC(float time){    
        // board.gem_movable = false;
        Vector3 start_pos = transform.position;
        yield return new WaitForSeconds(0.05f); // used to prevent many accesses to GetPosition() at the same time
        Vector3 end_pos = board.GetPosition(column, row);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(start_pos, end_pos, t / time);
            yield return 0;
        }

        transform.position = end_pos;
        // board.gem_movable = true;
    }
}
