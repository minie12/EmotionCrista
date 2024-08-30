using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;

public class PatternAreaBug : MonoBehaviour, IPointerEnterHandler
{
    private Animator animator;

    private readonly int bugTotalCnt = 5;
    private readonly float time = 0.5f; // using falling bug
    private readonly float bugScaleOrigin = 30f;
    private float bugScale;

    private Vector3 bugPos;
    private int typeNum = 0; // 벌레 번호
    private bool isReady = false;

    private float beformDropMaxTime = 0.5f;

    [HideInInspector]
    public float bugSpeed;

    void Awake()
    {
        bugScale = bugScaleOrigin;
    }

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        typeNum = Random.Range(0, bugTotalCnt);
        animator.Play($"bug_{typeNum}_anim", 0, 0.0f);

        // 벌레 속도 랜덤으로 정하기
        bugSpeed = Random.Range(5f, 15f);
    }

    private void Update()
    {
        if (isReady)
        {
            this.GetComponent<RectTransform>().localScale = new Vector3(bugScale, bugScale, 1);
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
        yield return new WaitForSeconds(Random.Range(0, beformDropMaxTime));
        this.GetComponent<RectTransform>().localScale = new Vector3(size.x + bugScale * 1.5f, size.y + bugScale * 1.5f, 1);
        this.gameObject.SetActive(true);
        this.transform.DOScale(new Vector3(size.x - bugScale * 0.15f, size.y - bugScale * 0.15f), 0.25f);
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

    // Fade in (fadeTime = time while fade)
    public void FadeOut(float fadeTime = 1f)
    {
        SpriteRenderer temp = GetComponent<SpriteRenderer>();
        StartCoroutine(FadeEffect(1f, -fadeTime, temp));
    }

    private IEnumerator FadeEffect(float start, float time, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, start);
        float target = 1f - start;
        while ((target == 0f && spriteRenderer.color.a >= target) || (target == 1f && target >= spriteRenderer.color.a))
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + Time.deltaTime / time);
            yield return null;
        }
    }

    // 벌레 위에 마우스 포인터 올라갔을 때 -> 애니메이션 속도 증가 & 진동
    public void OnPointerEnter(PointerEventData eventData)
    {
        StartCoroutine(BugFlinch());
        BugShake(0.03f, 0.35f);
        Debug.Log("mouse cursor");
    }

    private IEnumerator BugFlinch()
    {
        if (animator == null) yield return null;

        animator.speed = 3.0f * animator.speed;
        yield return new WaitForSeconds(0.35f);
        animator.speed = 1.0f;
    }

    // shake bug
    public void BugShake(float amount, float time, bool keepAmount = true)
    {
        StartCoroutine(BugShakeRoutine(amount, time, keepAmount));
    }

    private IEnumerator BugShakeRoutine(float amount, float time, bool keepAmount)
    {
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
