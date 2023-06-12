using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PatternBubble : MonoBehaviour
{
    private RectTransform r_transform;
    private float half_height;
    private float speed = 0.06f;

    void Awake()
    {
        r_transform = GetComponent<RectTransform>();
    }

    void OnEnable()
    {
        half_height = LayoutUtility.GetPreferredHeight(r_transform) / 2;
    }
    void FixedUpdate()
    {
        transform.Translate(new Vector3(0, speed, 0));

        if (r_transform.anchoredPosition.y + half_height > 1080) this.gameObject.SetActive(false);
    }

    public void OnClickBubble()
    {
        Destroy(this.gameObject);
    }
}

