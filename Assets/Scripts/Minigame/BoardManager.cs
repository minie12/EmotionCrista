using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Linq;

public class BoardManager : MonoBehaviour
{
    public MiniManager mini;
    public int goalColor; // 0 yellow, 1 blue, 2 red, 3 green, 4 purple
    private int totalGemTypeCnt = System.Enum.GetNames(typeof(PatternType)).Length-1;

    public GameObject gemPF;
    public Vector3[] dropPos;
    public GameObject clickEffect;
    private GemInfo[,] gems;
    private Vector3[,] boardTiles;

    // wait for rotate time
    private bool bGemMovable = false;
    private bool bGemClicked;
    private int prevRow, prevColumn;
    private int row, column;

    // fever related
    private int feverCnt = 0;

    // animation and audio
    public float rotateTime = 0.1f;
    public float dropTime = 0.1f;
    public AudioSource boardAUD;
    
    // goal state
    public GoalInfo goalInfo;
    public int goalGemCnt = 3;

    // around gems direction vector (odd/even standard: col)
    // up&left, up, up&right, down&right, down, down&left
    private int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    void Start(){   
        InitBoard();
        // for testing purpose ERASE BELOW
        goalInfo.SetGoal(goalGemCnt);
    }

    void Update(){
        if (bGemClicked && bGemMovable && gems[column, row].bRotateAble)
        {
            Debug.Log("column row : " + column + " " + row + " " + gems[column, row].bRotateAble);
            if (Input.GetKeyDown(KeyCode.A)) RotateGem('a');
            else if (Input.GetKeyDown(KeyCode.D)) RotateGem('d');
        }
    }

    public void InitBoard(){
        boardTiles = new Vector3[11, 6];
        gems = new GemInfo[11, 6];
        dropPos = new Vector3[11];

        // create 66 gems on correct location
        float diffX = 0.75f;
        float diffY = 0.9f;
        float transY = 3.35f;

        // set board tile (hexagon tile)
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(j == 5 && i % 2 == 0) continue; 

                boardTiles[i, j] = new Vector3(-1.2f + i * diffX, -1.7f + (i%2)*(-0.45f) + j * diffY, 0);

                int color = Random.Range(0, totalGemTypeCnt);
                GameObject gemTemp = Instantiate(gemPF, boardTiles[i,j], Quaternion.identity, this.transform);
                gemTemp.GetComponent<GemInfo>().InitGem(i, j, color);
                gems[i, j] = gemTemp.GetComponent<GemInfo>();

            }

            dropPos[i] = new Vector2(-1.1f + i * diffX, transY);
        }

        bGemMovable = true;
    }

    // used to change outline of gem that is previously clicked
    void SaveGemCooridnate(int column_, int row_){      
        prevRow = row; prevColumn = column;
        row = row_; column = column_;
    }

    // rotate move gems (direction: 1 (cw), -1 (ccw))
    void RotateMoveGem(int[,] d, int direction)
    {
        int idx = 0;
        while(true)
        {
            int prevIdx = idx;
            idx = (idx + direction + 6) % 6;
            gems[column + d[prevIdx, 0], row + d[prevIdx, 1]].MoveGem(column + d[idx, 0], row + d[idx, 1], rotateTime);
            if (idx == 0)
            {
                break;
            }
        }
    }

    // rotate gems info (direction: -1 (cw), 1 (ccw))
    void RotateUpdateGemInfo(int[,] d, int direction)
    {
        int idx = 0;
        GemInfo gTemp = gems[column + d[idx, 0], row + d[idx, 1]];
        while (true)
        {
            int prevIdx = idx;
            idx = (idx + direction + 6) % 6;
            if (idx == 0)
            {
                gems[column + d[prevIdx, 0], row + d[prevIdx, 1]] = gTemp;
                break;
            }
            gems[column + d[prevIdx, 0], row + d[prevIdx, 1]] = gems[column + d[idx, 0], row + d[idx, 1]];
        }
    }

    // rotate gem's rotateAble value (opposite direction with update gem array)
    void RotateGemRotateAble(int[,] d, int direction)
    {
        int idx = 0;
        bool temp = gems[column + d[idx, 0], row + d[idx, 1]].bRotateAble;
        while (true)
        {
            int prevIdx = idx;
            idx = (idx + direction + 6) % 6;
            if (idx == 0)
            {
                gems[column + d[prevIdx, 0], row + d[prevIdx, 1]].bRotateAble = temp;
                break;
            }
            gems[column + d[prevIdx, 0], row + d[prevIdx, 1]].bRotateAble = gems[column + d[idx, 0], row + d[idx, 1]].bRotateAble;
        }
    }

    void RotateGem(char key){
        bGemMovable = false;

        int[,] d = (column % 2 == 0) ? aroundGem_e : aroundGem_o;

        Invoke("EnableGemMovable", rotateTime);

        // turn CCW
        if(key == 'a'){
            // rotate gameobjects
            RotateMoveGem(d, -1);

            // update gems array
            RotateUpdateGemInfo(d, 1);

            // return bRotateAble value
            RotateGemRotateAble(d, -1);
        } 
        
        // turn CW
        else{
            // rotate gameobjects
            RotateMoveGem(d, 1);

            // update gems array
            RotateUpdateGemInfo(d, -1);

            // return bRotateAble value
            RotateGemRotateAble(d, 1);
        }
    }

    // return around gems
    public List<GemInfo> GetAroundGems(int column_, int row_)
    {
        int[,] direction = new int[6,2];
        if(column_ % 2 == 0)
        {
            direction = aroundGem_e;
        }
        else
        {
            direction = aroundGem_o;
        }

        List<GemInfo> result = new List<GemInfo>();
        for(int i = 0; i < 6; i++)
        {
            int newC = column_ + direction[i,0];
            int newR = row_ + direction[i,1];

            if(CheckGemExist(newC, newR))
            {
                result.Add(gems[newC, newR]);
            }
        }
        return result;
    }

    public void StartRefilBoardFever()
    {
        StartCoroutine("RefillBoardFever");
    }

    public void GemClick(int column_, int row_){
        // current clicked gem color
        int currentGemColor = gems[column_, row_].GetColor();
        
        // check if goal is met
        if (goalInfo.CheckGoal(column_, row_)){
            SaveGemCooridnate(column_, row_);

            EraseGemOutline();
            bGemClicked = false;
            clickEffect.SetActive(false);

            Debug.Log(goalColor);

            // add score if gem color is the goal color
            if (currentGemColor == goalColor) mini.AddScore(goalGemCnt);
            else mini.AddFever(goalGemCnt);

            // Delete gems
            boardAUD.Play();
            goalInfo.EraseGems(column, row, true);

            // red gimmick
            if (goalColor == 2 && currentGemColor == 2 && GameObject.Find("MiniManager").GetComponent<PatternRed>().GetIsPlaying())
            {
                Debug.Log("red gimmick");
                GameObject.Find("MiniManager").GetComponent<PatternRed>().InvokeExplosion();
                return;
            }

            // purple gimmick
            if (goalColor == (int)PatternType.PURPLE && GameObject.Find("MiniManager").GetComponent<PatternPurple>().GetIsPlaying())
            {
                List<GemInfo> aroundChainGems = CheckExitChainAround();
                
                for(int i = 0; i < aroundChainGems.Count; i++)
                {
                    Debug.Log(aroundChainGems[i]);
                    int extraChain = aroundChainGems[i].MinusChainCnt();

                    // end chain
                    if(extraChain == 0)
                    {
                        float fadeTime = 1f;
                        aroundChainGems[i].FadeOut(fadeTime, 5);
                        StartCoroutine(DeleteChain(fadeTime, aroundChainGems[i]));
                    }

                }
            }

            StartCoroutine("RefillBoard");
        }
        else{
            // do not enable click when user clicks the boundary of board
            if(column_ == 10 || column_ == 0 || row_ == 5 || row_ == 0) return;
            if(row_ == 4 && column_ % 2 == 0) return;

            SaveGemCooridnate(column_, row_);

            // show that gem has been clicked
            ChangeGemOutline();
            //gems[column, row].SetOutline("click");
            clickEffect.transform.position = boardTiles[column, row];
            clickEffect.SetActive(true);
        }
    }

    IEnumerator DeleteChain(float fadeTime, GemInfo gem)
    {
        yield return new WaitForSeconds(fadeTime); // term fade out 

        gem.chainAnimObj.SetActive(false);
        gem.bLocationFixed = false;


        List<GemInfo> aroundGems = GetAroundGems(gem.GetColumn(), gem.GetRow());
        aroundGems.Add(gem);

        for(int i = 0; i < aroundGems.Count; i++)
        {
            if (IsExitChainAround(aroundGems[i].GetColumn(), aroundGems[i].GetRow()))
            {
                gem.bRotateAble = false;
            }
            else
            {
                gem.bRotateAble = true;
            }
        }
        
    }


    // check exit chain around gem
    List<GemInfo> CheckExitChainAround()
    {
        List<GemInfo> result = new List<GemInfo>();
        bool[,] check = new bool[11, 6];

        List<List<int>> crushedGems = GameObject.Find("Board").GetComponent<GoalInfo>().crushedGems;
        Debug.Log("크러쉬된 광물 개수: "+ crushedGems.Count);

        for(int i = 0; i < crushedGems.Count; i++)
        {
            List<GemInfo> aroundGems = GetAroundGems(crushedGems[i][0], crushedGems[i][1]);

            for (int j = 0; j < aroundGems.Count; j++)
            {
                // exit chain
                int column_ = aroundGems[j].GetColumn();
                int row_ = aroundGems[j].GetRow();
                if (aroundGems[j].GetChainCnt() > 0 && !check[column_,row_])
                {
                    result.Add(aroundGems[j]);
                    check[column_, row_] = true;
                }
            }
        }
        
        return result;
    }


    bool IsExitChainAround(int col, int r)
    {
        List<GemInfo> aroundGems = GetAroundGems(col, r);

        for (int j = 0; j < aroundGems.Count; j++)
        {
            // exit chain
            int column_ = aroundGems[j].GetColumn();
            int row_ = aroundGems[j].GetRow();
            if(aroundGems[j].GetChainCnt() > 0)
            {
                return true;
            }
        }
        return false;
    }


    // used in ChangeGemOutline()
    // erase outline of gem if other gem is clicked
    void EraseGemOutline(){
        if(bGemClicked){
            int eo = (prevColumn%2 == 0)?1:0;

            gems[prevColumn, prevRow].SetOutline("undo");

            gems[prevColumn-1, prevRow+eo].SetOutline("undo");
            gems[prevColumn-1, prevRow-1+eo].SetOutline("undo");
            gems[prevColumn, prevRow-1].SetOutline("undo");
            gems[prevColumn+1, prevRow-1+eo].SetOutline("undo");
            gems[prevColumn+1, prevRow+eo].SetOutline("undo");
            gems[prevColumn, prevRow+1].SetOutline("undo");
        }
    }

    void ChangeGemOutline(){
        int eo;
        
        EraseGemOutline(); // disable previous gems
      
        eo = (column%2 == 0)?1:0;

        gems[column, row].SetOutline("click");

        gems[column-1, row+eo].SetOutline("side");
        gems[column-1, row-1+eo].SetOutline("side");
        gems[column, row-1].SetOutline("side");
        gems[column+1, row-1+eo].SetOutline("side");
        gems[column+1, row+eo].SetOutline("side");
        gems[column, row+1].SetOutline("side");

        bGemClicked = true;
    }

    bool CheckGemExist(int column_, int row_){
        if(column_ < 0 || column_ > 10 || row_ > 5 || row_ < 0) return false;
        if(column_ % 2 == 0 && row_ > 4) return false;
        if(gems[column_, row_] == null) return false;
        return true;
    }


    IEnumerator RefillBoard()
    {
        bGemMovable = false;
        yield return new WaitForSeconds(0.3f); // wait for gem crush

        // sort row increasing -> column increasing
        List<List<int>> crushedGems = GameObject.Find("Board").GetComponent<GoalInfo>().crushedGems;
        Debug.Log("보드 채우기 크러쉬된 광물 개수: " + crushedGems.Count);
        
        crushedGems = crushedGems.OrderBy(x => x[1]).ThenBy(y => y[0]).ToList();

        Debug.Log("정렬 후 " + crushedGems.Count);

        while(crushedGems.Count > 0)
        {
            List<int> cur = crushedGems[0];
            crushedGems.RemoveAt(0);

            bool filled = false;
            int i = cur[0]; // column
            int j = cur[1]; // row
            // check if there is gem on top
            for (int k = j + 1; k < 6; k++)
            {
                if (i % 2 == 0 && k == 5) break;

                if (!CheckGemExist(i, k))
                {
                    continue;
                }

                int newColumn = i;
                int newRow = k;

                if (gems[i, k].bLocationFixed)
                {
                    List<GemInfo> aroundGems = GetAroundGems(i, k);
                    if (aroundGems.Count == 0)
                    {
                        continue;
                    }
                    bool checkAround = false;
                    for (int l = aroundGems.Count - 1; l >= 0; l--)
                    {
                        if (aroundGems[l].bLocationFixed)
                        {
                            continue;
                        }
                        // drop the gem on top to bottom
                        checkAround = true;
                        newColumn = aroundGems[l].GetColumn();
                        newRow = aroundGems[l].GetRow();
                        break;
                    }
                    if (!checkAround)
                    {
                        continue;
                    }

                }

                Debug.Log("채운 광물 : " + newColumn + ", " + newRow);

                // drop the gem on top to bottom
                gems[i, j] = gems[newColumn, newRow];
                gems[newColumn, newRow] = null;
                gems[i, j].MoveGem(i, j, dropTime);
                crushedGems.Add(new List<int> { newColumn, newRow });
                filled = true;
                break;
            }

            // if there was no gem on top
            if (!filled)
            {
                Debug.Log("탑 비어있음 " + i);
                // fill with new gem
                int color = Random.Range(0, totalGemTypeCnt);
                GameObject gemTemp = Instantiate(gemPF, dropPos[i], Quaternion.identity, this.transform);
                gemTemp.GetComponent<GemInfo>().InitGem(i, j, color);
                // yield return new WaitForSeconds(0.1f);
                gems[i, j] = gemTemp.GetComponent<GemInfo>();
                gems[i, j].MoveGem(i, j, dropTime);
            }

            // 주변에 사슬 있다면 회전 막기
            if (IsExitChainAround(i, j))
            {
                gems[i, j].bRotateAble = false;
            }
            else
            {
                gems[i, j].bRotateAble = true;
            }
        }


        // wait for gems to fall down then allow clicks
        yield return new WaitForSeconds(dropTime);
        bGemMovable = true;
    }

    // used to communicate with other classes ---------------------------------------------------
    public int GetGemTypeCnt() { return totalGemTypeCnt; }
    public void SetGoal(int unit){
        goalGemCnt = unit;
        goalInfo.SetGoal(unit);
    }
    public void DelGem(int column_, int row_){
        gems[column_, row_].DestroyGem();
        gems[column_, row_] = null;
    }
    public void ExplosionGem(int column_, int row_)
    {
        gems[column_, row_].ExplosionGem();
        gems[column_, row_] = null;
    }
    public Vector3 GetGemPosition(int column_, int row_){
        return boardTiles[column_, row_];
    }
    public int GetGemColor(int column_, int row_){
        if(!CheckGemExist(column_, row_) || gems[column_, row_].bLocationFixed) return -1;
        return gems[column_, row_].GetColor();
    }

    public GemInfo GetGem(int column_, int row_){
        if(!CheckGemExist(column_, row_)) return null;
        return gems[column_, row_];
    }

    void EnableGemMovable() { bGemMovable = true; }
    public bool GetGemMovable() { return bGemMovable; }
    public void SetGemMovable(bool movable) { bGemMovable = movable; }

    public bool CheckFever(){
        return mini.bFeverOn;
    }

    void ClearBoardInit()
    {
        bGemMovable = false;
        clickEffect.SetActive(false);
        bGemClicked = false;

        StopAllCoroutines();
    }

    public void ClearBoardWithoutAnim()
    {
        ClearBoardInit();
        foreach (GemInfo gem in gems)
        {
            if (gem != null)
            {
                Destroy(gem.gameObject);
            }
        }
    }

    public void ClearBoard(){
        ClearBoardInit();
        foreach (GemInfo gem in gems){
            if(gem != null) gem.DestroyGem();
        }
    }

// FEVER ---------------------------------------------------------------------------
    public void StartFever(){
        feverCnt = 0;
        clickEffect.SetActive(false);
        bGemClicked = false;
    }

    IEnumerator RefillBoardFever(){
        bGemMovable = false;

        yield return new WaitForSeconds(0.3f); // wait for gem crush
        
        for(int i = 0; i < 11; i++){
            for(int j = 0; j < 6; j++){
                if(i % 2 == 0 && j == 5) break; 
                
                if(!CheckGemExist(i, j)){
                    bool filled = false;
                    // check if there is gem on top
                    for(int k = j; k < 6; k++){
                        if(i % 2 == 0 && k == 5) break; 

                        if(CheckGemExist(i, k)){
                            // drop the gem on top to bottom
                            gems[i, j] = gems[i,k];
                            gems[i, k] = null;
                            gems[i, j].MoveGem(i, j, dropTime);
                            filled = true;
                            break;
                        }
                    }

                    // // if there was no gem on top
                    if(!filled){
                        // fill with new gem
                        int color = Random.Range(0, totalGemTypeCnt);
                        GameObject gemTemp = Instantiate(gemPF, dropPos[i], Quaternion.identity, this.transform);
                        gemTemp.GetComponent<GemInfo>().InitGem(i, j, color);
                        // yield return new WaitForSeconds(0.1f);
                        gems[i, j] = gemTemp.GetComponent<GemInfo>();
                        gems[i, j].MoveGem(i, j, dropTime);
                    }
                }
            }
        }

        // wait for gems to fall down then allow clicks
        yield return new WaitForSeconds(dropTime);
        bGemMovable = true;
    }

    public void EndFever(){
        StartCoroutine("RefillBoardFever");
        // Invoke("RefillBoardFever",0.5f);
    }

    public void FeverClick(int column_, int row_){
        boardAUD.Play();
        mini.AddScore(1);
        DelGem(column_, row_);

        // in case player clicks all gem before Fever ends
        feverCnt++;
        if(feverCnt > 59) mini.EndFever();
    }
}
