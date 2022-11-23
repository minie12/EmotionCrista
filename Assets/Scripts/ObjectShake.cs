using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    Vector3 originPos;
    [SerializeField] [Range(0.01f, 0.1f)] float shakeRange = 0.05f;
    [SerializeField] [Range(0.1f, 5f)] float duration = 0.5f;

    // Start is called before the first frame update
    void Start()
    {
        originPos = gameObject.transform.position;
        InvokeRepeating("StartShake", 0f, 0.005f);
        Invoke("StopShake", duration);
    }

    void StartShake()
    {
        float newX = Random.value * shakeRange * 2 - shakeRange;
        float newY = Random.value * shakeRange * 2 - shakeRange;
        Vector3 newPosition = gameObject.transform.position;
        newPosition.x += newX;
        newPosition.y += newY;
        gameObject.transform.position = newPosition;
    }

    void StopShake()
    {
        CancelInvoke("StartShake");
        gameObject.transform.position = originPos;
    }
}

