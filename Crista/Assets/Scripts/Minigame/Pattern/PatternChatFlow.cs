using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine;

public class PatternChatFlow : MonoBehaviour
{
    private string[] chats = new string[3] {"정말 웃기지 않나요?",
                                            "그 우는 얼굴마저도 잘생겼어요!",
                                            "탐내지 마세요, 제꺼니까요."};
    private float[] spawn_positions = new float[7] {330,200,80,-30,-140,-231,-355};

    private Text text;
    private RectTransform r_transform;
    private float half_width;
    [HideInInspector]
    public int index;

    void Awake(){
        text = GetComponent<Text>();
        r_transform = GetComponent<RectTransform>();
    }

    void OnEnable(){
        text.text = chats[Random.Range(0, chats.GetLength(0))];
        // index set before activating the object (at MiniManager)
        r_transform.anchoredPosition = new Vector3(1350, spawn_positions[index], 0);
        half_width = LayoutUtility.GetPreferredWidth(r_transform)/2;
    }
    void FixedUpdate(){
        Vector3 org_pos = transform.position;
        transform.position =  new Vector3(org_pos.x - 0.06f, org_pos.y, org_pos.z);

        if(r_transform.anchoredPosition.x+half_width < -1200) this.gameObject.SetActive(false);
    }
}
