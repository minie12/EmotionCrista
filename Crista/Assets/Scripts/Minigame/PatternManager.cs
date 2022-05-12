using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternManager : MonoBehaviour
{
    // YELLOW Pattern
    public GameObject UI_Canvas;
    public GameObject bubblePF;
    private GameObject[] bubbles;


    private int y_index;
    private int bubble_numb;

    // Start is called before the first frame update
    void Y_Start()
    {
        // Pattern yellow
        y_index = 0; bubble_numb = 5;

        bubbles = new GameObject[bubble_numb];
        for(int i = 0; i < bubble_numb; i++){
            GameObject gem_temp = Instantiate(bubblePF, new Vector3(0,0,0), Quaternion.identity, UI_Canvas.transform);
            gem_temp.SetActive(false);
            bubbles[i] = gem_temp;
        }
        InvokeRepeating ("Y_SpawnBubble", 2, 2);
    }

    // PATTERN -- YELLOW
    void Y_SpawnBubble(){
        Vector3 rand_pos = new Vector3(Random.Range(750.0f, 1600.0f), Random.Range(120.0f, 850.0f), 5);
        bubbles[y_index].transform.position = Camera.main.ScreenToWorldPoint(rand_pos);
        bubbles[y_index].SetActive(true);
        y_index = (y_index+1)%bubble_numb;
    }
}
