using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PatternBubble : MonoBehaviour
{
    private RectTransform r_transform;
    private float half_height;
    private float speed = 0.06f;
    public Sprite[] bubbles;
    private int bubbleId;
    private bool clicked;

    void Awake()
    {
        r_transform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        clicked = false;
        half_height = LayoutUtility.GetPreferredHeight(r_transform) / 2;
        bubbleId = Random.Range(0, bubbles.Length);
        this.GetComponent<SpriteRenderer>().sprite = bubbles[bubbleId];
    }

    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, speed, 0));

        if (r_transform.anchoredPosition.y + half_height > 10.0f) Destroy(this.gameObject);
    }

    private void OnMouseUp()
    {
        if (clicked == false)
        {
            clicked = true;
            this.GetComponent<Animator>().enabled = true;
            this.GetComponent<Animator>().Play($"blue_bubble_{bubbleId + 1}", 0, 0.0f);
            Invoke(nameof(DestroyObject), 0.9f);
        }
    }

    private void DestroyObject()
    {
        Destroy(this.gameObject);
    }
}

