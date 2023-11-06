using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternPupilController : MonoBehaviour
{
    public Transform MidLocation;
    public EdgeCollider2D BoundaryCollider;

    private int targetLayerMask;

    private void Start()
    {
        // get PurpleEye layer !!!! 
        targetLayerMask = LayerMask.GetMask("PurpleEye");
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        Vector3 EyeballPosition = this.transform.position;

        Vector2 MouseLocation = Input.mousePosition;
        MouseLocation = Camera.main.ScreenToWorldPoint(MouseLocation);

        Vector2 MidLocation2D = new Vector2(MidLocation.position.x, MidLocation.position.y);
        Vector2 MouseToMid = MidLocation2D - MouseLocation;
        float MouseDistance = MouseToMid.magnitude;
        MouseToMid.Normalize();

        //Debug.Log("Mouse Pos: (" + MouseLocation.x + ", " + MouseLocation.y + ")");
        RaycastHit2D hit = Physics2D.Raycast(MouseLocation, MouseToMid, Mathf.Infinity, targetLayerMask);
        //Debug.DrawRay(MouseLocation, MouseToMid, Color.red, 0f, false);
        int count = 0;
        while (null != hit.collider && count < 10)
        {
            if (hit.collider == BoundaryCollider)
            {
                //Debug.Log("Collision Pos: (" + hit.point.x + ", " + hit.point.y + ")");
                Vector2 CollisionToMouse = MouseLocation - hit.point;
                if (CollisionToMouse.magnitude < MouseDistance)
                {
                    EyeballPosition = hit.point;
                }
                else
                {
                    EyeballPosition = MouseLocation;
                }

                //Debug.Log("Mouse: " + MouseToMid.magnitude + ", Collision: " + CollisionToMid.magnitude);
                break;
            }

            // for smoothing (mouse pointer in pupil)
            Vector2 NewLocation = hit.point + (MouseToMid * 0.1f);
            hit = Physics2D.Raycast(NewLocation, MouseToMid);
            count++;
        }

        this.transform.position = EyeballPosition;
    }
}
