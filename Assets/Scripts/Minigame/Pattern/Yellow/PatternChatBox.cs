using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;

public class PatternChatBox : MonoBehaviour, IDragHandler
{
    private string[] chats = new string[9] {"저는 작가님에게 아무런 문제가 없다고 알고 있어요!",
                                            "작가님은 반드시 차기작을 내주실거에요! 하하하!!",
                                            "그래서 그 차기작도 대박이나서 전작도 다시 회자되고",
                                            "전작의 주인공의 얘기도 작가님이 조금이라도 더 해주시지 않을까요?",
                                            "그 왜 유명한 마법사 시리즈처럼요!",
                                            "이번에 열심히 일해서 모아둔 월급들 아낌없이 쓸 생각이에요!",
                                            "시리즈 나올때마다 꼬박꼬박 사서",
                                            "완결나면 소장용으로 전권 시리즈 살 생각이에요!",
                                            "어떡해요, 벌써 너무 설랜다, 그쵸!"};
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
