using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HeadingMouse : MonoBehaviour
{
    private RectTransform objTransform;

    private void Start()
    {
        objTransform = this.GetComponent<RectTransform>();
    }

    private void Update()
    {
        Vector2 mousePos = Input.mousePosition;
        float offsetX = 1920.0f / 2f - 3f;
        float offsetY = 1080.0f / 2f + 5f;
        objTransform.localPosition = new Vector3(mousePos.x - offsetX, mousePos.y - offsetY, 0f);
    }
}
