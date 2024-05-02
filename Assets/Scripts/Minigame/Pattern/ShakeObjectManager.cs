using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShakeObjectManager : MonoBehaviour
{
    [SerializeField] [Range(0.01f, 0.5f)] float shakeAmount = 0.05f;
    [SerializeField] [Range(0.1f, 5f)] float duration = 1f;

    public List<Transform> puzzleTransform = new List<Transform>(); // 퍼즐판
    public List<Transform> puzzleUiTransform = new List<Transform>(); // 퍼즐판 UI
    public List<Transform> backgroundTransform = new List<Transform>(); // 배경 
    public List<Transform> uiTransform = new List<Transform>(); // 미니게임 UI

    public void ShakePuzzle()
    {
        StartCoroutine(ShakeRoutine(puzzleTransform, 0f, shakeAmount, duration, false));
    }

    public void ShakePuzzleUi()
    {
        List<Transform> objects = new List<Transform>();
        objects.AddRange(puzzleTransform);
        objects.AddRange(puzzleUiTransform);
        StartCoroutine(ShakeRoutine(objects, 0f, shakeAmount, duration, false));
    }

    public void ShakeBackground()
    {
        StartCoroutine(ShakeRoutine(backgroundTransform, 0f, shakeAmount, duration, false));
    }

    public void ShakeUi()
    {
        List<Transform> objects = new List<Transform>();
        objects.AddRange(puzzleUiTransform);
        objects.AddRange(uiTransform);
        StartCoroutine(ShakeRoutine(objects, 0f, shakeAmount, duration, false));
    }

    public void ShakeAll()
    {
        List<Transform> objects = new List<Transform>();
        objects.AddRange(puzzleTransform);
        objects.AddRange(puzzleUiTransform);
        objects.AddRange(backgroundTransform);
        objects.AddRange(uiTransform);
        StartCoroutine(ShakeRoutine(objects, 0f, shakeAmount, duration, false));
    }

    private IEnumerator ShakeRoutine(List<Transform> objects, float prevTime, float amount, float time, bool keepAmount)
    {
        yield return new WaitForSeconds(prevTime);

        // 원래 위치 백업
        List<Vector3> originPosition = new List<Vector3>();
        for (int i = 0; i < objects.Count; i++)
        {
            originPosition.Add(objects[i].position);
        }

        // 진동
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            Vector3 rand = new Vector3(Random.insideUnitCircle.x, Random.insideUnitCircle.y, 0) * (keepAmount ? amount : Mathf.Lerp(amount, 0, 1 - t / time));
            
            // 모든 오브젝트 변경
            for (int i = 0; i < objects.Count; i++)
            {
                objects[i].position = originPosition[i] + rand;
            }
            yield return null;
        }

        // 원래 위치로 되돌리기
        for (int i = 0; i < objects.Count; i++)
        {
            objects[i].position = originPosition[i];
        }
    }
}

