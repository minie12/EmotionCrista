using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternEye : MonoBehaviour
{
    private List<GameObject> eyeChildrens = new List<GameObject>();
    private Animator animator;
    private bool check = false;

    private void Start()
    {
        animator = this.GetComponent<Animator>();
        GetEyeChildren(this.gameObject.transform);

        // all children non - activation
        SetObjectActive(false);
    }

    private void Update()
    {
        // if animator done
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("purple_eye") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !check)
        {
            check = true;
            animator.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 1f, 1f, 0f);
            SetObjectActive(true);
        }
    }

    void SetObjectActive(bool state)
    {
        for (int i = 0; i < eyeChildrens.Count; i++)
        {
            eyeChildrens[i].SetActive(state);
        }
    }

    void GetEyeChildren(Transform curr)
    {
        if (curr.childCount == 0)
        {
            return;
        }
        for (int i = 0; i < curr.childCount; i++)
        {
            eyeChildrens.Add(curr.GetChild(i).gameObject);
            GetEyeChildren(curr.GetChild(i));
        }
    }

    public void AllChildrenFadeOut()
    {
        for (int i = 0; i < eyeChildrens.Count; i++)
        {
            if (!eyeChildrens[i].GetComponent<SpriteRenderer>())
            {
                continue;
            }
            StartCoroutine(FadeOut(eyeChildrens[i].GetComponent<SpriteRenderer>(), 1f));
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sr, float time)
    {
        while (sr.color.a >= 0f)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime / time);
            yield return null;
        }
    }

}
