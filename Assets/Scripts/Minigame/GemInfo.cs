using DG.Tweening;
using System.Collections;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    private int column, row;

    private PatternType color;
    public Sprite[] gemSprites;
    public Sprite[] special_gem_sprites;

    public SpriteRenderer gemOutline;
    public Sprite gemClickSP;
    public Sprite gemSideSP;

    public Animator sparkleANIM;
    public Animator patternANIM;
    public Animator gemANIM;
    public Animator explosionANIM;
    public GameObject chainAnimObj;

    public bool bPatternApplied;

    // purple gimmick
    private int chainCnt;

    // change gem effect
    private SpriteRenderer spriteRenderer;

    // manage rotate
    [HideInInspector] public bool bRotateAble = true;

    // manage location fixed
    [HideInInspector] public bool bLocationFixed = false;

    // check fire road
    [HideInInspector] public bool isFired = false;

    void Awake()
    {
        // get gem sprite renderer
        spriteRenderer = transform.GetChild(0).GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void InitGem(int column_, int row_, int color_)
    {
        bPatternApplied = false;
        bRotateAble = true;
        bLocationFixed = false;
        column = column_; row = row_;
        SetColor_(color_);
    }

    /// <summary> Set outline sprite of gem </summary>
    /// <param name="type"> click, side, undo. </param>
    public void SetOutline(string type)
    {
        switch (type)
        {
            case "click":
                gemOutline.sprite = gemClickSP; // middle of hex when clicked
                break;
            case "side":
                gemOutline.sprite = gemSideSP; // sides of hex when clicked
                break;
            case "undo":
                gemOutline.sprite = null; // undo the click
                break;
        }
    }

    public void PrintInfo()
    {
        Debug.Log(column + " " + row + " " + color);
    }

    // Start is called before the first frame update
    void OnMouseUp()
    {
        if (board.CheckFever())
        {
            board.FeverClick(column, row);
        }
        else if (board.GetGemMovable())
        {
            board.GemClick(column, row);
        }
    }

    public int GetColor()
    {
        return (int)color;
    }

    public void SetColor()
    {
        int now;
        int prev = now = (int)color;

        while (prev == now)
        {
            now = Random.Range(0, board.GetGemTypeCnt());
        }

        SetColor_(now);
    }

    void SetColor_(int color_)
    {
        if (color_ == (int)PatternType.YELLOW) color = PatternType.YELLOW;
        else if (color_ == (int)PatternType.BLUE) color = PatternType.BLUE;
        else if (color_ == (int)PatternType.RED) color = PatternType.RED;
        else if (color_ == (int)PatternType.GREEN) color = PatternType.GREEN;
        else color = PatternType.PURPLE;

        spriteRenderer.sprite = gemSprites[(int)color];
    }

    // set gem sprite renderer color
    public void SetSpriteColor(float r, float g, float b, float a)
    { 
        spriteRenderer.color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    // set background sprite renderer color
    public void SetBackgroundColor(float r, float g, float b, float a)
    {
        transform.GetComponent<SpriteRenderer>().color = new Color(r / 255f, g / 255f, b / 255f, a / 255f);
    }

    // set gem transform scale
    public void SetTransformScale(float coefficient)
    {
        Vector3 originScale = spriteRenderer.gameObject.transform.localScale;
        spriteRenderer.gameObject.transform.localScale = new Vector3(originScale.x * coefficient, originScale.y * coefficient, originScale.z * coefficient);
    }

    public void SetColumn(int col)
    {
        column = col;
    }

    public void SetRow(int row_)
    {
        row = row_;
    }

    public int GetColumn()
    {
        return column;
    }

    public int GetRow()
    {
        return row;
    }

    public int GetChainCnt()
    {
        return chainCnt;
    }

    // purple gimmick chain
    public int MinusChainCnt()
    {
        if (chainCnt > 0)
        {
            chainCnt--;

            // play animation
            StartCoroutine(ChainGemC(chainCnt));
        }
        return chainCnt;
    }

    IEnumerator ChainGemC(int cnt)
    {
        GameObject temp = chainAnimObj.transform.GetChild(cnt).gameObject;
        temp.GetComponent<Animator>().Play("gem_chain_purple", 0, 0.0f);
        yield return new WaitForSeconds(0.9f);
        temp.SetActive(false);
    }

    // change special gem
    public void ChangeSpecialGem()
    {
        spriteRenderer.sprite = special_gem_sprites[(int)color];
    }

    // change gem color
    public void ChangeGemColor(int color_)
    {
        SetColor_(color_);
    }

    // Fade in (fadeTime = time while fade, target = object to fade (default = child 0 (gem)))
    public void FadeIn(float fadeTime = 1f, int target = 0)
    {
        SpriteRenderer temp = GetComponent<SpriteRenderer>();
        if (target >= 0)
        {
            temp = gameObject.transform.GetChild(target).GetComponent<SpriteRenderer>();
        }
        StartCoroutine(FadeEffect(0f, fadeTime, temp));
    }

    // Fade out
    public void FadeOut(float fadeTime = 1f, int target = 0)
    {
        SpriteRenderer temp = GetComponent<SpriteRenderer>();
        if (target >= 0)
        {
            temp = gameObject.transform.GetChild(target).GetComponent<SpriteRenderer>();
        }
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

    // hearbeat gem (use in yellow gimmick 2)
    public void GemHeartBeat(float prevTime, float amount, float time)
    {
        StartCoroutine(GemBeatRoutine(prevTime, amount, time));
    }

    private IEnumerator GemBeatRoutine(float prevTime, float amount, float time)
    {
        yield return new WaitForSeconds(prevTime);

        float originX = transform.GetChild(0).localScale.x;

        float maxScale = originX * amount;
        float midScale = originX * amount * 0.9f;
        float minScale = originX * 0.9f;
        float[] scaleList = { minScale, midScale, originX, maxScale, originX };
        float[] timeList = { time * 0.025f, time * 0.175f, time * 0.11f, time * 0.11f, time * 0.34f };
        float[] intervalList = { time * 0.025f, time * 0.175f, time * 0.11f, time * 0.35f, time * 0.34f };

        for (int i = 0; i < scaleList.Length; i++)
        {
            transform.GetChild(0).DOScale(new Vector3(scaleList[i], scaleList[i]), timeList[i]);
            yield return new WaitForSeconds(intervalList[i]);
        }
    }

    // shake gem effect
    public void GemShake(float prevTime, float amount, float time, bool keepAmount = true)
    {
        StartCoroutine(GemShakeRoutine(prevTime, amount, time, keepAmount));
    }

    private IEnumerator GemShakeRoutine(float prevTime, float amount, float time, bool keepAmount)
    {
        yield return new WaitForSeconds(prevTime);

        Vector3 originPosition = transform.position;
        for (float t = time; t >= 0; t -= Time.deltaTime)
        {
            Vector3 rand = new Vector3(0, Random.insideUnitCircle.y, 0) * (keepAmount ? amount : Mathf.Lerp(amount, 0, 1 - t / time));
            transform.position = originPosition + rand;
            yield return null;
        }
        transform.position = originPosition;
    }

    // gem fire
    public void FireGem()
    {
        isFired = true;
        explosionANIM.gameObject.SetActive(true);
        explosionANIM.Play("gem_fire_red", 0, 0.0f);
    }

    public void StopFireGem()
    {
        isFired = false;
        explosionANIM.gameObject.SetActive(false);
    }

    public void OnlyDestroyGem(float prevTime = 0f)
    {
        StartCoroutine(OnlyDestroyGemC(prevTime));
    }

    IEnumerator OnlyDestroyGemC(float prevTime)
    {
        yield return new WaitForSeconds(prevTime);
        Destroy(gameObject);
    }

    public void DestroyGem()
    {
        GameObject.Find("MiniManager").GetComponent<PatternRed>().SetFireCheckFalse(column, row);
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("DestroyGemC");
    }
    IEnumerator DestroyGemC()
    {
        gemANIM.enabled = true;

        if (color == PatternType.RED) gemANIM.Play("gem_crush_red", 0, 0.0f);
        else if (color == PatternType.YELLOW) gemANIM.Play("gem_crush_yellow", 0, 0.0f);
        else if (color == PatternType.GREEN) gemANIM.Play("gem_crush_green", 0, 0.0f);
        else if (color == PatternType.BLUE) gemANIM.Play("gem_crush_blue", 0, 0.0f);
        else if (color == PatternType.PURPLE) gemANIM.Play("gem_crush_purple", 0, 0.0f);

        sparkleANIM.Play("gem_sparkle", 0, 0.0f);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    public void ExplosionGem()
    {
        GameObject.Find("MiniManager").GetComponent<PatternRed>().SetFireCheckFalse(column, row);
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("ExplosionGemC");
    }

    IEnumerator ExplosionGemC()
    {
        gemANIM.enabled = true;

        explosionANIM.Play("gem_explosion_red", 0, 0.0f);
        yield return new WaitForSeconds(0.3f);
        Destroy(gameObject);
    }

    public void SetChainGem(int cnt)
    {
        chainCnt = cnt;

        // turn chain obj active true
        chainAnimObj.SetActive(true);
        for (int i = 0; i < cnt; i++)
        {
            chainAnimObj.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void FillWaterInHex()
    {
        bPatternApplied = true;  // so that this gem does not get selected at GetRandomGem()
        patternANIM.Play("gem_fill_water", 0, 0.0f);
        StartCoroutine("FillWaterInHexC");
    }

    IEnumerator FillWaterInHexC()
    {
        yield return new WaitForSeconds(0.1f);

        // check whether gem_fill_water animation is done or not
        while (!patternANIM.GetCurrentAnimatorStateInfo(0).IsTag("changeColor"))
        {
            yield return new WaitForSeconds(0.1f);
        }

        SetColor(); // change color of gem
        bPatternApplied = false;
        patternANIM.SetBool("bWaterFilled", true);
    }

    // just change row, column. Actually move action in BoardManager's RotateGem func.
    public void MoveGem(int column_, int row_, float time)
    {
        row = row_; column = column_;

        StartCoroutine(MoveGemC(time));
    }
    IEnumerator MoveGemC(float time)
    {
        gameObject.GetComponent<Collider2D>().enabled = false;

        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(0.01f); // used to avoid error but why?
        Vector3 endPos = board.GetGemPosition(column, row);

        for (float t = 0; t <= 1 * time; t += Time.deltaTime)
        {
            transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        transform.position = endPos;

        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
