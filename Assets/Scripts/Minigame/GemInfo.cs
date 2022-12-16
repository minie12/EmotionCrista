using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GemInfo : MonoBehaviour
{
    private BoardManager board;
    private int column, row;

    // purple gimmick
    private int chainCnt;

    private PatternType color;
    public Sprite[] gemSprites;
    public Sprite[] special_gem_sprites;

    public SpriteRenderer gemOutline;
    // public SpriteRenderer gem_pattern;
    public Sprite gemClickSP;
    public Sprite gemSideSP;

    public Animator sparkleANIM; 
    public Animator patternANIM; 
    public Animator gemANIM;
    public Animator explosionANIM;
    public GameObject chainAnimObj;

    public bool bPatternApplied;

    // change gem effect
    private SpriteRenderer spriteRenderer;

    // manage rotate
    [HideInInspector] public bool bRotateAble = true;

    // manage location fixed
    [HideInInspector] public bool bLocationFixed = false; 

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void Start(){
        board = GameObject.Find("Board").GetComponent<BoardManager>();
    }

    public void InitGem(int column_, int row_, int color_){
        bPatternApplied = false;
        bRotateAble = true;
        bLocationFixed = false;
        column = column_; row = row_; 
        SetColor_(color_);
    }

    /// <summary> Set outline sprite of gem </summary>
    /// <param name="type"> click, side, undo. </param>
    public void SetOutline(string type){
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

    public void PrintInfo(){
        Debug.Log(column+ " " + row + " " + color);
    }

    // Start is called before the first frame update
    void OnMouseUp(){
        if(board.CheckFever()){
            board.FeverClick(column,row);
        }
        else if(board.GetGemMovable()){
            board.GemClick(column, row);
        }
    }

    public int GetColor(){
        return (int)color;
    }

    public void SetColor(){
        int now;
        int prev = now = (int)color;

        while(prev == now){
            now = Random.Range(0, board.GetGemTypeCnt());
        }

        SetColor_(now);
    }
    
    void SetColor_(int color_){
        if(color_ == (int)PatternType.YELLOW) color = PatternType.YELLOW;
        else if(color_ == (int)PatternType.BLUE) color = PatternType.BLUE;
        else if(color_ == (int)PatternType.RED) color = PatternType.RED;
        else if(color_ == (int)PatternType.GREEN) color = PatternType.GREEN;
        else color = PatternType.PURPLE;

        spriteRenderer.sprite = gemSprites[(int)color];
    }

    public int GetColumn()
    {
        return column;
    }

    public int GetRow()
    {
        return row;
    }

    // purple gimmick chain
    public int MinusChainCnt()
    {
        if(chainCnt > 0)
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
        yield return new WaitForSeconds(1f);
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
        spriteRenderer.sprite = gemSprites[color_];
    }

    // Fade in (fadeTime = time while fade, target = object to fade)
    public void FadeIn(float fadeTime = 1f, int target = -1)
    {
        SpriteRenderer temp = spriteRenderer;
        if(target >= 0)
        {
            temp = gameObject.transform.GetChild(target).GetComponent<SpriteRenderer>();
        }
        StartCoroutine(FadeInCorutine(fadeTime, temp));
    }

    // Fade out
    public void FadeOut(float fadeTime = 1f, int target = -1)
    {
        SpriteRenderer temp = spriteRenderer;
        if (target >= 0)
        {
            temp = gameObject.transform.GetChild(target).GetComponent<SpriteRenderer>();
        }
        StartCoroutine(FadeOutCorutine(fadeTime, temp));
    }

    private IEnumerator FadeInCorutine(float fadeTime, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 0);
        while (spriteRenderer.color.a < 1.0f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a + Time.deltaTime * fadeTime);
            yield return null;
        }
    }

    private IEnumerator FadeOutCorutine(float fadeTime, SpriteRenderer spriteRenderer)
    {
        spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, 1);
        while (spriteRenderer.color.a > 0.0f)
        {
            spriteRenderer.color = new Color(spriteRenderer.color.r, spriteRenderer.color.g, spriteRenderer.color.b, spriteRenderer.color.a - Time.deltaTime * fadeTime);
            yield return null;
        }
    }

    public void DestroyGem(){
        gameObject.GetComponent<Collider2D>().enabled = false;
        StartCoroutine("DestroyGemC");
    }
    IEnumerator DestroyGemC(){
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
        for(int i = 0; i < cnt; i++)
        {
            chainAnimObj.transform.GetChild(i).gameObject.SetActive(true);
        }
    }

    public void FillWaterInHex(){
        bPatternApplied = true;  // so that this gem does not get selected at GetRandomGem()
        patternANIM.Play("gem_fill_water", 0, 0.0f);
        StartCoroutine("FillWaterInHexC");
    }

    IEnumerator FillWaterInHexC(){
        yield return new WaitForSeconds(0.1f);

        // check whether gem_fill_water animation is done or not
        while(!patternANIM.GetCurrentAnimatorStateInfo(0).IsTag("changeColor")){
            yield return new WaitForSeconds(0.1f);
        }

        SetColor(); // change color of gem
        bPatternApplied = false;
        patternANIM.SetBool("bWaterFilled", true);
    }

    // just change row, column. Actually move action in BoardManager's RotateGem func.
    public void MoveGem(int column_, int row_, float time){
        row = row_; column = column_;

        StartCoroutine(MoveGemC(time));
    }
    IEnumerator MoveGemC(float time){    
        gameObject.GetComponent<Collider2D>().enabled = false;

        Vector3 startPos = transform.position;
        yield return new WaitForSeconds(0.01f); // used to avoid error but why?
        Vector3 endPos = board.GetGemPosition(column, row);

        for(float t = 0; t <= 1 * time; t += Time.deltaTime){
            transform.position = Vector3.Lerp(startPos, endPos, t / time);
            yield return 0;
        }

        transform.position = endPos;
        
        gameObject.GetComponent<Collider2D>().enabled = true;
    }
}
