using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class AsyncLoadingManager : MonoBehaviour
{
    private int[] PosX = { 0, 0, 185, 375, 565, 750, 935 }; // first 0 is None. This is because day starts with 1
    [SerializeField]
    private Sprite [] times;
    [SerializeField]
    private Sprite[] colors;

    [SerializeField]
    private GameObject boxObj;
    [SerializeField]
    private GameObject timeObj;
    [SerializeField]
    private GameObject colorObj;

    private float twinklingTime = 0.4f;
    private float swipeTime = 0.3f;

    private bool activeBool = false;

    void Start()
    {
        int dayCount = PosX.Length;

        GameManager gameManager = GameManager.Get();
        if (gameManager)
        {
            dayCount = gameManager.GetDayCount();
            gameManager.ProceedNextDay();
        }

        if (dayCount < 1 || PosX.Length <= dayCount)
        {
            Debug.LogError("Loading Scene: Invalid day count " + dayCount);
            dayCount = 1;
        }

        boxObj.transform.position = new Vector3(PosX[dayCount] * 0.01f, 0, 0);
        StartCoroutine(AsyncLoadScene(dayCount+1));
    }

    void Blink()
    {
        timeObj.SetActive(activeBool);
        activeBool = !activeBool;
    }

    IEnumerator AsyncLoadScene(int inNextDayCount)
    {
        // Remove Fade Screen 
        var cameraManager = Fungus.FungusManager.Instance.CameraManager;
        if (null != cameraManager)
        {
            cameraManager.Fade(0, 1, null);
        }

        // Start Async Load
        AsyncOperation oper = SceneManager.LoadSceneAsync("Dormitory");
        oper.allowSceneActivation = false;

        // Load Animation
        yield return new WaitForSeconds(twinklingTime);
        timeObj.SetActive(false);

        int blinkCount = 0;
        float lerpTimer = 0.0f;
        float swipeCoefficient = 1 / swipeTime;

        Vector3 startPos = boxObj.transform.position;
        Vector3 endPos = new Vector3(PosX[inNextDayCount] * 0.01f, 0, 0);

        bool bDayChanged = false;
        while (false == bDayChanged)
        {
            if (0.9f <= oper.progress && 1 < blinkCount)
            {
                lerpTimer += Time.deltaTime;

                if (1.0f < (lerpTimer * swipeCoefficient))
                {
                    boxObj.transform.position = endPos;
                    bDayChanged = true;
                }
                else
                {
                    boxObj.transform.position = Vector3.Lerp(startPos, endPos, lerpTimer * swipeCoefficient);
                    yield return 0;
                }
            }
            else
            {
                // Blink
                ++blinkCount;
                yield return new WaitForSeconds(twinklingTime);
                timeObj.SetActive(true);
                yield return new WaitForSeconds(twinklingTime);
                timeObj.SetActive(false);
            }
        }

        timeObj.GetComponent<SpriteRenderer>().sprite = times[1];
        timeObj.SetActive(true);

        // Fade Screen 
        yield return new WaitForSeconds(0.5f);
        if (null != cameraManager)
        {
            cameraManager.Fade(1, 1, null);
        }
        yield return new WaitForSeconds(1f);

        oper.allowSceneActivation = true;
    }

}
