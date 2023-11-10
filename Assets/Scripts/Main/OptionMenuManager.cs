using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;

public class OptionMenuManager : MonoBehaviour
{
    private Vector2 centerPosition;
    private float mouseAngle;
    private float anglePerSlot;

    private int previousIdx;

    private bool bInMenuBox;

    public GameObject[] menuSlots;

    // Start is called before the first frame update
    void Start()
    {
        mouseAngle = 0.0f;
        //centerPosition = new Vector2(1920*0.5f, 1080*0.5f);
        centerPosition = new Vector2(940.0f, 485.0f);
        anglePerSlot = 360 * 0.2f;
        previousIdx = -1;
        bInMenuBox = false;
    }

    // Update is called once per frame
    void Update()
    {
        float diffX = Input.mousePosition.x - centerPosition.x;
        float diffY = Input.mousePosition.y - centerPosition.y;

        if (42000.0f < diffX * diffX + diffY * diffY)
        {
            if (0 <= previousIdx && previousIdx < menuSlots.Length)
            {
                menuSlots[previousIdx].SetActive(false);
                previousIdx = -1;
            }

            return;
        }

        if (diffX * diffX + diffY * diffY < 38000.0f)
        {
            mouseAngle = -(Mathf.Atan2(diffY, diffX) * Mathf.Rad2Deg - 90.0f);
            if (mouseAngle < 0.0f)
            {
                mouseAngle = 180.0f + (180.0f + mouseAngle);
            }

            int activateIdx = 0;

            if (0.0f < mouseAngle && mouseAngle <= anglePerSlot)
            {
                activateIdx = 0;
            }
            else if (anglePerSlot < mouseAngle && mouseAngle < anglePerSlot * 2)
            {
                activateIdx = 1;
            }
            else if (anglePerSlot * 2 < mouseAngle && mouseAngle < anglePerSlot * 3)
            {
                activateIdx = 2;
            }
            else if (anglePerSlot * 3 < mouseAngle && mouseAngle < anglePerSlot * 4)
            {
                activateIdx = 3;
            }
            else if (anglePerSlot * 4 < mouseAngle && mouseAngle < anglePerSlot * 5)
            {
                activateIdx = 4;
            }

            if (0 <= activateIdx && activateIdx < menuSlots.Length)
            {
                if (previousIdx != activateIdx)
                {
                    if (0 <= previousIdx && previousIdx < menuSlots.Length)
                    {
                        menuSlots[previousIdx].SetActive(false);
                    }
                    previousIdx = activateIdx;
                    menuSlots[activateIdx].SetActive(true);
                }
            }
        }
    }
}
 