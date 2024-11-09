using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PatternEye : MonoBehaviour
{
    private List<GameObject> eyeChildrens = new List<GameObject>();
    private Animator animator;
    private bool check = false;
    private int flip = 0;

    // eye uuid
    private string eyeId;


    private void Start()
    {
        animator = this.GetComponent<Animator>();
        GetEyeChildren(this.gameObject.transform);

        // all children non - activation
        SetObjectActive(false);

        // random flip
        flip = Random.Range(0, 2);
        if (flip == 1)
        {
            transform.localScale = new Vector3((-1) * transform.localScale.x, transform.localScale.y, 1);
        }
    }

    private void Update()
    {
        // if animator done
        if(animator.GetCurrentAnimatorStateInfo(0).IsName("purple_eye") && animator.GetCurrentAnimatorStateInfo(0).normalizedTime >= 1f && !check)
        {
            check = true;
            StartCoroutine(FadeOut(animator.gameObject.GetComponent<SpriteRenderer>(), 0.5f));
            SetObjectActive(true);
            AllChildrenFadeIn();
        }
    }

    public string GetEyeId()
    {
        return eyeId;
    }

    public void SetEyeId(string id) 
    {
        this.eyeId = id;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        PatternPurple scripts = GameObject.Find("MiniManager").GetComponent<PatternPurple>();
        if (collision.tag == "FlashLight" && scripts.ExistEyeId(this.eyeId))
        {
            Debug.Log("eye delete " + this.eyeId);

            // collision destroy
            scripts.DeleteEye(this.eyeId);

            // collision recreate
            StartCoroutine(scripts.AddEyeAfterTime(5f));

            // 실제로 삭제
            StartCoroutine(this.CollisionDestroy());
        }
    }

    private IEnumerator CollisionDestroy()
    {
        yield return new WaitForSeconds(0.5f);

        // collision fade out
        this.AllChildrenFadeOut();

        yield return new WaitForSeconds(10f);

        Destroy(this.gameObject);
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

    public void AllChildrenFadeIn()
    {
        for (int i = 0; i < eyeChildrens.Count; i++)
        {
            if (!eyeChildrens[i].GetComponent<SpriteRenderer>())
            {
                continue;
            }
            StartCoroutine(FadeIn(eyeChildrens[i].GetComponent<SpriteRenderer>(), 0.5f));
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

    private IEnumerator FadeIn(SpriteRenderer sr, float time)
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 0f);
        while (sr.color.a <= 1f)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a + Time.deltaTime / time);
            yield return null;
        }
    }

    private IEnumerator FadeOut(SpriteRenderer sr, float time)
    {
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, 1f);
        while (sr.color.a >= 0f)
        {
            sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - Time.deltaTime / time);
            yield return null;
        }
    }

}
