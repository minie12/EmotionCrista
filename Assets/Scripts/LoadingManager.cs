using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class LoadingManager : MonoBehaviour
{
    [SerializeField]
    private Sprite [] times;
    [SerializeField]
    private Sprite[] colors;


    private GameObject boxObj, timeObj, colorObj;
    private float twinklingTime = 1f;

    // Start is called before the first frame update
    void Awake()
    {
        boxObj = GameObject.Find("Box");
        timeObj = GameObject.Find("Time");
        colorObj = GameObject.Find("Color");
    }

    /*  void Start()
      {
          StartCoroutine(TwinklingObject(timeObj));
          Invoke("ChangeTime", twinklingTime);
      }

      void ChangeTime()
      {
          timeObj.GetComponent<Image>().sprite = times[1];
          StartCoroutine(TwinklingObject(timeObj));
      }

      IEnumerator TwinklingObject(GameObject obj)
      {
          obj.SetActive(false);
          yield return new WaitForSeconds(twinklingTime/2);
          obj.SetActive(true);
          yield return new WaitForSeconds(twinklingTime/2);
      }
  */
    void Start()
    {
        StartCoroutine(TwinklingChangeObj(timeObj, times[1]));
    }

    IEnumerator TwinklingChangeObj(GameObject obj, Sprite change)
    {
        yield return new WaitForSeconds(twinklingTime / 2);
        obj.SetActive(false);
        yield return new WaitForSeconds(twinklingTime / 2);
        obj.GetComponent<Image>().sprite = change;
        obj.SetActive(true);
    }

}
