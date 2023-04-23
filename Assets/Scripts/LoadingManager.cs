using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    private int[] PosX = { 0, 185, 375, 565, 750, 935 };
    [SerializeField]
    private Sprite [] times;
    [SerializeField]
    private Sprite[] colors;


    private GameObject boxObj, timeObj, colorObj;
    private float twinklingTime = 0.5f;
    private int minTwinkleCnt = 2;

    private bool activeBool = false;

    // Start is called before the first frame update
    void Awake()
    {
        boxObj = GameObject.Find("Box");
        timeObj = GameObject.Find("Time");
        colorObj = GameObject.Find("Color");
    }

    void Start()
    {
        //LoadScene("LabCorridor");
        //StartCoroutine(TwinklingChangeObj(timeObj, times[1]));
        StartCoroutine(TwinklingChangeObj(timeObj, times[1]));
    }

    void Blink()
    {
        timeObj.SetActive(activeBool);
        activeBool = !activeBool;
    }
    /*public void LoadScene(string sceneName)
    {
        //gameObject.SetActive(true);
        SceneManager.sceneLoaded += LoadSceneEnd;
        loadSceneName = sceneName;
        StartCoroutine(TwinkleWhileLoad(sceneName));
    }

    private void LoadSceneEnd(Scene scene, LoadSceneMode loadSceneMode)
    {
        if (scene.name == loadSceneName)
        {
            StartCoroutine(Fade(false));
            SceneManager.sceneLoaded -= LoadSceneEnd;
        }
    }
    private IEnumerator Fade(bool isFadeIn)
    {
        float timer = 0f;
        while (timer <= 1f)
        {
            yield return null;
            timer += Time.unscaledDeltaTime * 2f;
            sceneLoaderCanvasGroup.alpha = Mathf.Lerp(isFadeIn ? 0 : 1, isFadeIn ? 1 : 0, timer);
        }
        if (!isFadeIn)
        {
            gameObject.SetActive(false);
        }
    }

    IEnumerator TwinkleWhileLoad(string sceneName)
    {
        AsyncOperation op = SceneManager.LoadSceneAsync(sceneName);
        op.allowSceneActivation = false;

        float timer = 0.0f;
        int twinkleCnt = 0;
        while (!op.isDone)
        {
            timer += Time.unscaledDeltaTime;

            if (op.progress < 0.9f || twinkleCnt < minTwinkleCnt)
            {
                StartCoroutine(TwinklingObj(timeObj));
                twinkleCnt++;
                yield return new AsyncOperation;
            }
            else
            {
                StartCoroutine(TwinklingChangeObj(timeObj, times[1]));
                yield break;
            }

            yield return null;
        }
    }*/

    IEnumerator TwinklingObj(GameObject obj)
    {
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(true);
    }
    IEnumerator TwinklingChangeObj(GameObject obj, Sprite change)
    {
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);
        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(true);

        yield return new WaitForSeconds(twinklingTime);
        obj.SetActive(false);

        Vector3 startPos = boxObj.transform.position;
        Vector3 endPos = new Vector3(PosX[1] * 0.01f, 0, 0);

        for (float t = 0; t <= 1 * twinklingTime; t += Time.deltaTime)
        {
            boxObj.transform.position = Vector3.Lerp(startPos, endPos, t / twinklingTime);
            yield return 0;
        }
        boxObj.transform.position = endPos;

        obj.GetComponent<SpriteRenderer>().sprite = change;
        obj.SetActive(true);

        yield return new WaitForSeconds(1f);

        SceneManager.LoadScene("LabCorridor");
    }

}
