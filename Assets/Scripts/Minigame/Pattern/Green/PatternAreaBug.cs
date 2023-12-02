using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class PatternAreaBug : MonoBehaviour
{
    private Animator animator;

    private readonly int bugTotalCnt = 5;
    private readonly float time = 0.5f; // using falling bug
    private readonly float bugScale = 0.35f;

    private Vector3 bugPos;
    private int typeNum = 0; // 벌레 번호
    private bool isReady = false;

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        typeNum = Random.Range(0, bugTotalCnt);
        animator.Play($"bug_{typeNum}_anim", 0, 0.0f);
    }

    private void Update()
    {
        if (isReady)
        {
            transform.localScale = new Vector3(bugScale, bugScale, 1);
        }
    }

    public void SetBugPos(Vector3 pos)
    {
        bugPos = pos;
    }

    public void SizeDown()
    {
        StartCoroutine(SizeDown_(new Vector3(bugScale, bugScale, 1)));
    }

    IEnumerator SizeDown_(Vector3 size)
    {
        this.GetComponent<Transform>().localScale = new Vector3(size.x + 2f, size.y + 2f, 1);
        this.gameObject.SetActive(true);
        this.transform.DOScale(new Vector3(size.x - 0.15f, size.y - 0.15f), 0.25f);
        yield return new WaitForSeconds(0.2f);
        this.transform.DOScale(new Vector3(size.x, size.y), 0.1f);
        yield return new WaitForSeconds(0.1f);
        isReady = true;
    }

    public void FallBug()
    {
        StartCoroutine(MoveBug(time));
    }

    IEnumerator MoveBug(float time)
    {
        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(0.01f); // used to avoid error but why?
        Vector3 endPos = bugPos;

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        transform.position = endPos;
    }

    // Fade in (fadeTime = time while fade)
    public void FadeIn(float fadeTime = 1f)
    {
        SpriteRenderer temp = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeEffect(0f, fadeTime, temp));
    }

    private IEnumerator FadeEffect(float start, float time, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, start);
        float target = 0.3f - start;
        while ((target == 0f && spriteRenderer.color.a >= target) || (target == 0.3f && target >= spriteRenderer.color.a))
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + Time.deltaTime / time);
            yield return null;
        }
    }
}
