using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using UnityEngine;

public class PatternBubble : MonoBehaviour
{
    private RectTransform r_transform;
    private float half_height;

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
        Vector3 orgPos = transform.position;
        transform.position = new Vector3(orgPos.x, orgPos.y + 0.06f, orgPos.z);

        if (r_transform.anchoredPosition.y + half_height > 1200) this.gameObject.SetActive(false);
    }

    public void onClickBubble()
    {
        Destroy(this.gameObject);
    }
}

