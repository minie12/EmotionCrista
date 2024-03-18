using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ObjectShake : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 0.5f)] float shakeAmount = 0.05f;
    [SerializeField] [Range(0.1f, 5f)] float duration = 1f;

    public void Shake()
    {
        StartCoroutine(ShakeRoutine(0f, shakeAmount, duration, false));
    }

    private IEnumerator ShakeRoutine(float prevTime, float amount, float time, bool keepAmount)
    {
        yield return new WaitForSeconds(prevTime);

        Vector3 originPosition = transform.position;
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            Vector3 rand = new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0) * (keepAmount ? amount : Mathf.Lerp(amount, 0, 1 - t / time));
            transform.position = originPosition + rand;
            yield return null;
        }
        transform.position = originPosition;
    }
}

