using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseManager : MonoBehaviour
{
    [SerializeField]
    private Texture2D defaultState;
    [SerializeField]
    private Texture2D clickedState;

    [SerializeField]
    private Vector2 hotspot;
    [SerializeField]
    private CursorMode cursorMode = CursorMode.Auto;

    public void Update()
    { 
        if(Input.GetMouseButtonUp(0))
            {
                Cursor.SetCursor(defaultState, hotspot, cursorMode);
            }
        
        if (Input.GetMouseButtonDown(0))
            {
                Cursor.SetCursor(clickedState, hotspot, cursorMode);
            }
        
    }
}
