using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatternChatBox : MonoBehaviour, IDragHandler
{
    private string[] chats = new string[3] {"정말 웃기지 않나요?",
                                            "그 우는 얼굴마저도 잘생겼어요!",
                                            "탐내지 마세요, 제꺼니까요."};
    public Sprite[] sprites;
    private Text text;
    private Image image;

    void Awake(){
        text = GetComponentInChildren<Text>();
        image = GetComponent<Image>();
    }

    void OnEnable(){
        image.sprite = sprites[Random.Range(0, sprites.GetLength(0))];
        text.text = chats[Random.Range(0, chats.GetLength(0))];
        // Invoke("BubbleOff",3);
        StartCoroutine("BubbleOff");
    }
    IEnumerator BubbleOff(){
        yield return new WaitForSeconds(3);
        gameObject.SetActive(false);
    }

    public void OnDrag(PointerEventData eventData)
    {
        Vector3 screenPoint = Input.mousePosition;
        screenPoint.z = 5.0f; //distance of the plane from the camera
        transform.position = Camera.main.ScreenToWorldPoint(screenPoint);
    }
}
