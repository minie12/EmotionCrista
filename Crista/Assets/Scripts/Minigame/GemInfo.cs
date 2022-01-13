using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private MiniMain main;
    public int column, row;
    public int color;

    void Start(){
        main = GameObject.Find("MainManager").GetComponent<MiniMain>();
    }

    // Start is called before the first frame update
    void OnMouseUp(){
        if(main.gem_movable){
            main.GemClick(column, row, this.transform);
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
    private IEnumerator MoveGemC(float time){    
        main.gem_movable = false;

        Vector3 start_pos = transform.position;
        Vector3 end_pos = main.board_tiles[column, row].transform.position;

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(start_pos, end_pos, t / time);
            yield return 0;
        }

        transform.position = end_pos;

        yield return new WaitForSeconds(0.2f); // need to check on this (to prevent user from clicking while moving the GEM)
        main.gem_movable = true;
    }
}
