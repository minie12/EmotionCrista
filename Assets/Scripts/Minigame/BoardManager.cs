using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class BoardManager : MonoBehaviour
{
    // common
    public MiniManager mini;
    private readonly int totalGemTypeCnt = System.Enum.GetNames(typeof(PatternType)).Length - 1;

    // gem
    public GameObject gemPF;
    public Vector3[] dropPos;
    public GameObject clickEffect;
    private GemInfo[,] gems;
    private Vector3[,] boardTiles;
    private bool[,] isLockRotate; // pattern related
    private int[,] isClickedGem; // gem click

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

    // goal state
    public GoalInfo goalInfo;

    // around gems direction vector (odd/even standard: column)
    // up&left, up, up&right, down&right, down, down&left
    private readonly int[,] aroundGem_o = new int[6, 2] { { -1, 0 }, { 0, 1 }, { 1, 0 }, { 1, -1 }, { 0, -1 }, { -1, -1 } };
    private readonly int[,] aroundGem_e = new int[6, 2] { { -1, 1 }, { 0, 1 }, { 1, 1 }, { 1, 0 }, { 0, -1 }, { -1, 0 } };

    private void Start()
    {
        InitBoard();
    }

    private void Update()
    {
        if (bGemClicked && bGemMovable && !isLockRotate[column, row])
        {
            if (Input.GetKeyDown(KeyCode.A))
            {
                SoundEffectManager.Instance.Play((int)SoundEffectName.MiniRotateLeft);
                RotateGem('a');
            }
            else if (Input.GetKeyDown(KeyCode.D))
            {
                SoundEffectManager.Instance.Play((int)SoundEffectName.MiniRotateRight);
                RotateGem('d');
            }
        }

        // locked gem + gem clicked
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (j == 5 && i % 2 == 0) continue;
                if(gems[i,j] != null)
                {
                    gems[i, j].SetOutline("undo");
                }

                if (i == 0 || i == 10 || j == 0 || j == 5 || (j == 4 && i % 2 == 0))
                {
                    isLockRotate[i, j] = true;
                }

                if (gems[i, j] == null)
                {
                    continue;
                }

                if (isLockRotate[i, j] && gems[i,j].GetChainCnt() == 0)
                {
                    gems[i, j].SetFilm();
                }
                else
                {
                    gems[i, j].DeleteFilm();
                }

                if(isClickedGem[i,j] == 1)
                {
                    gems[i, j].SetOutline("side");
                }
                else if(isClickedGem[i,j] == 2)
                {
                    gems[i, j].SetOutline("click");
                }
            }
        }
    }

    // initialize board
    public void InitBoard()
    {
        boardTiles = new Vector3[11, 6];
        gems = new GemInfo[11, 6];
        dropPos = new Vector3[11];
        isLockRotate = new bool[11, 6];
        isClickedGem = new int[11, 6];

        // create 66 gems on correct location
        float diffX = 0.75f;
        float diffY = 0.9f;
        float transY = 3.35f;

        // set board tile (hexagon tile)
        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (j == 5 && i % 2 == 0) continue;

                boardTiles[i, j] = new Vector3(-1.2f + i * diffX, -1.7f + (i % 2) * (-0.45f) + j * diffY, 0);

                int color = Random.Range(0, totalGemTypeCnt);
                GameObject gemTemp = Instantiate(gemPF, boardTiles[i, j], Quaternion.identity, this.transform);
                gemTemp.GetComponent<GemInfo>().InitGem(i, j, color);
                gems[i, j] = gemTemp.GetComponent<GemInfo>();
            }
            dropPos[i] = new Vector2(-1.1f + i * diffX, transY);
        }
        bGemMovable = true;
    }

    // used to change outline of gem that is previously clicked
    private void SaveGemCooridnate(int column_, int row_)
    {
        prevRow = row; prevColumn = column;
        row = row_; column = column_;
    }

    // rotate move gems & update info (direction: -1 (cw), 1 (ccw))
    private void RotateMoveUpdateGem(int[,] d, int direction)
    {
        int idx = 0;
        GemInfo gTemp = gems[column + d[idx, 0], row + d[idx, 1]];
        while (true)
        {
            int prevIdx = idx;
            idx = (idx + direction + 6) % 6;
            if (idx == 0)
            {
                gTemp.MoveGem(column + d[prevIdx, 0], row + d[prevIdx, 1], rotateTime);
                gems[column + d[prevIdx, 0], row + d[prevIdx, 1]] = gTemp;
                break;
            }
            gems[column + d[idx, 0], row + d[idx, 1]].MoveGem(column + d[prevIdx, 0], row + d[prevIdx, 1], rotateTime);
            gems[column + d[prevIdx, 0], row + d[prevIdx, 1]] = gems[column + d[idx, 0], row + d[idx, 1]];
        }
    }

    private void RotateGem(char key)
    {
        bGemMovable = false;
        Invoke(nameof(EnableGemMovable), rotateTime);

        int[,] d = (column % 2 == 0) ? aroundGem_e : aroundGem_o;
        // turn CCW
        if (key == 'a')
        {
            // rotate gem & update info
            RotateMoveUpdateGem(d, 1);
        }
        // turn CW
        else
        {
            // rotate gem & update info
            RotateMoveUpdateGem(d, -1);
        }
    }

    // return around gems
    public List<GemInfo> GetAroundGems(int column_, int row_)
    {
        int[,] direction = (column_ % 2 == 0) ? aroundGem_e : aroundGem_o;
        List<GemInfo> result = new List<GemInfo>();
        for (int i = 0; i < 6; i++)
        {
            int newC = column_ + direction[i, 0];
            int newR = row_ + direction[i, 1];

            if (CheckGemExist(newC, newR))
            {
                result.Add(gems[newC, newR]);
            }
        }
        return result;
    }

    public void StartRefilBoardFever()
    {
        StartCoroutine(nameof(RefillBoardFever));
    }

    public void GemClick(int column_, int row_)
    {
        // current clicked gem color
        int currentGemColor = gems[column_, row_].GetColor();

        // check if goal is met
        if (goalInfo.CheckGoal(column_, row_))
        {
            SaveGemCooridnate(column_, row_);

            EraseGemOutline();
            bGemClicked = false;
            clickEffect.SetActive(false);

            // add score if gem color is the goal color
            if (currentGemColor == mini.patternIdx) mini.AddScore(mini.GetGoalUnit());
            else mini.AddFever(mini.GetGoalUnit());

            // Delete gems
            SoundEffectManager.Instance.Play((int)SoundEffectName.MiniGemCrush);
            goalInfo.EraseGems(column, row, true);

            // related gimmick
            mini.SetTotalCrushedGem(mini.GetGoalUnit());
            mini.OnCrushedGemTrigger(currentGemColor);

            StartCoroutine(nameof(RefillBoard));
        }
        else
        {
            // do not enable click when user clicks the boundary of board
            if (column_ == 10 || column_ == 0 || row_ == 5 || row_ == 0) return;
            if (row_ == 4 && column_ % 2 == 0) return;
            if (isLockRotate[column_, row_]) return;

            SaveGemCooridnate(column_, row_);

            // show that gem has been clicked
            ChangeGemOutline();
        }
    }

    // used in ChangeGemOutline()
    // erase outline of gem if other gem is clicked
    private void EraseGemOutline()
    {
        if (bGemClicked)
        {
            Debug.Log("prev coordinate" + prevColumn + prevRow);
            int eo = (prevColumn % 2 == 0) ? 1 : 0;

            gems[prevColumn, prevRow].SetOutline("undo");
            gems[prevColumn - 1, prevRow + eo].SetOutline("undo");
            gems[prevColumn - 1, prevRow - 1 + eo].SetOutline("undo");
            gems[prevColumn, prevRow - 1].SetOutline("undo");
            gems[prevColumn + 1, prevRow - 1 + eo].SetOutline("undo");
            gems[prevColumn + 1, prevRow + eo].SetOutline("undo");
            gems[prevColumn, prevRow + 1].SetOutline("undo");

            isClickedGem[prevColumn, prevRow] = 0;
            isClickedGem[prevColumn - 1, prevRow + eo] = 0;
            isClickedGem[prevColumn - 1, prevRow - 1 + eo] = 0;
            isClickedGem[prevColumn, prevRow - 1] = 0;
            isClickedGem[prevColumn + 1, prevRow - 1 + eo] = 0;
            isClickedGem[prevColumn + 1, prevRow + eo] = 0;
            isClickedGem[prevColumn, prevRow + 1] = 0;

            clickEffect.SetActive(false);
        }
    }

    private void ChangeGemOutline()
    {
        EraseGemOutline(); // disable previous gems

        int eo;


        eo = (column % 2 == 0) ? 1 : 0;

        gems[column, row].SetOutline("click");

        gems[column - 1, row + eo].SetOutline("side");
        gems[column - 1, row - 1 + eo].SetOutline("side");
        gems[column, row - 1].SetOutline("side");
        gems[column + 1, row - 1 + eo].SetOutline("side");
        gems[column + 1, row + eo].SetOutline("side");
        gems[column, row + 1].SetOutline("side");

        isClickedGem[column, row] = 2;

        isClickedGem[column - 1, row + eo] = 1;
        isClickedGem[column - 1, row - 1 + eo] = 1;
        isClickedGem[column, row - 1] = 1;
        isClickedGem[column + 1, row - 1 + eo] = 1;
        isClickedGem[column + 1, row + eo] = 1;
        isClickedGem[column, row + 1] = 1;

        bGemClicked = true;

        clickEffect.transform.position = boardTiles[column, row];
        clickEffect.SetActive(true);
    }

    private bool CheckGemExist(int column_, int row_)
    {
        if (column_ < 0 || column_ > 10 || row_ > 5 || row_ < 0) return false;
        if (column_ % 2 == 0 && row_ > 4) return false;
        if (gems[column_, row_] == null) return false;
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

        while (crushedGems.Count > 0)
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

            if (j == 0 && gems[i, j].isCryGem == true)
            {
                gems[i, j].DestroyGem();
                crushedGems.Insert(0, new List<int> { i, j });
            }
        }

        // wait for gems to fall down then allow clicks
        yield return new WaitForSeconds(dropTime);
        bGemMovable = true;
    }

    // used to communicate with other classes ---------------------------------------------------
    public int GetGemTypeCnt() { return totalGemTypeCnt; }

    public void DelGem(int column_, int row_)
    {
        gems[column_, row_].DestroyGem();
        gems[column_, row_] = null;
    }
    public void ExplosionGem(int column_, int row_)
    {
        gems[column_, row_].ExplosionGem();
        gems[column_, row_] = null;
    }
    public Vector3 GetGemPosition(int column_, int row_)
    {
        return boardTiles[column_, row_];
    }
    public Vector3 GetDropPosition(int column_)
    {
        return dropPos[column_];
    }
    public int GetGemColor(int column_, int row_)
    {
        if (!CheckGemExist(column_, row_) || gems[column_, row_].bLocationFixed) return -1;
        return gems[column_, row_].GetColor();
    }

    public void SetGem(int column_, int row_, GemInfo gem)
    {
        gems[column_, row_] = gem;
    }

    public GemInfo GetGem(int column_, int row_)
    {
        if (!CheckGemExist(column_, row_)) return null;
        return gems[column_, row_];
    }

    public void SetRotate(int column_, int row_, bool value)
    {
        isLockRotate[column_, row_] = value;
    }

    private void EnableGemMovable() { bGemMovable = true; }
    public bool GetGemMovable() { return bGemMovable; }
    public void SetGemMovable(bool movable) { bGemMovable = movable; }

    public void SetGemClicked(bool clicked)
    {
        if (!clicked)
        {
            SaveGemCooridnate(column, row);
            EraseGemOutline();
        }
        bGemClicked = clicked;
    }

    public bool CheckFever()
    {
        return mini.bFeverOn;
    }

    private void ClearBoardInit()
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

    public void ClearBoard()
    {
        ClearBoardInit();
        foreach (GemInfo gem in gems)
        {
            if (gem != null) gem.DestroyGem();
        }
    }

    // FEVER ---------------------------------------------------------------------------
    public void StartFever()
    {
        feverCnt = 0;
        EraseGemOutline();
        clickEffect.SetActive(false);
        bGemClicked = false;
    }

    public void RefillBoardOut()
    {
        StartCoroutine(RefillBoardFever());
    }

    IEnumerator RefillBoardFever()
    {
        bGemMovable = false;

        yield return new WaitForSeconds(0.3f); // wait for gem crush

        for (int i = 0; i < 11; i++)
        {
            for (int j = 0; j < 6; j++)
            {
                if (i % 2 == 0 && j == 5) break;

                if (!CheckGemExist(i, j))
                {
                    bool filled = false;
                    // check if there is gem on top
                    for (int k = j; k < 6; k++)
                    {
                        if (i % 2 == 0 && k == 5) break;

                        if (CheckGemExist(i, k))
                        {
                            // drop the gem on top to bottom
                            gems[i, j] = gems[i, k];
                            gems[i, k] = null;
                            gems[i, j].MoveGem(i, j, dropTime);
                            filled = true;
                            break;
                        }
                    }

                    // if there was no gem on top
                    if (!filled)
                    {
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

    public void EndFever()
    {
        StartCoroutine("RefillBoardFever");
        // Invoke("RefillBoardFever",0.5f);
    }

    public void FeverClick(int column_, int row_)
    {
        SoundEffectManager.Instance.Play((int)SoundEffectName.MiniGemCrush);
        if (GetGemColor(column_, row_) == mini.patternIdx) mini.AddScore(1);
        else mini.AddScore(0.5f);
        DelGem(column_, row_);

        // in case player clicks all gem before Fever ends
        feverCnt++;
        if (feverCnt > 59) mini.EndFever();
    }

    // PATTERN RELATED
    public GemInfo GetRandomGem()
    {
        // TODO: Does not check whether the gem is already filled with water

        int column_ = Random.Range(0, 11);
        int row_ = Random.Range(0, 6);
        GemInfo gem = GetGem(column_, row_);
        while (gem == null)
        {
            column_ = Random.Range(0, 11);
            row_ = Random.Range(0, 6);
            gem = GetGem(column_, row_);
        }
        return gem;
    }

    public GemInfo GetRandomGemArea()
    {
        // TODO: Does not check whether the gem is already filled with water

        int column_ = Random.Range(2, 9);
        int row_ = Random.Range(2, 4);
        GemInfo gem = GetGem(column_, row_);
        while (gem == null)
        {
            column_ = Random.Range(2, 9);
            row_ = Random.Range(2, 4);
            gem = GetGem(column_, row_);
        }
        return gem;
    }

    public List<List<GemInfo>> GetPatternGems()
    {
        List<List<GemInfo>> gems = new List<List<GemInfo>>(5) { new List<GemInfo>(), new List<GemInfo>(), new List<GemInfo>(), new List<GemInfo>(), new List<GemInfo>() };

        for(int i = 0; i < 11; i++)
        {
            for(int j = 0; j < 6; j++)
            {
                GemInfo gem = GetGem(i, j);
                if(gem != null)
                {
                    gems[gem.GetColor()].Add(gem);
                }
            }
        }

        return gems;
    }

    public GemInfo GetPatternGemRandom()
    {
        List<List<GemInfo>> gems = GetPatternGems();

        if(gems[mini.patternIdx].Count == 0)
        {
            return null;
        }
        int idx = (int)Random.Range(0, gems[mini.patternIdx].Count);
        return gems[mini.patternIdx][idx];
    }

    public List<GemInfo> GetPatternGemManyRandom(int cnt)
    {
        List<List<GemInfo>> gems = GetPatternGems();
        Debug.Log("패턴 젬 " + cnt + ", " + gems[mini.patternIdx].Count);
        cnt = gems[mini.patternIdx].Count < cnt ? gems[mini.patternIdx].Count : cnt;


        List<GemInfo> result = new List<GemInfo>();
        for (int i = 0; i < cnt; )
        {
            GemInfo gem = GetPatternGemRandom();
            if (!result.Contains(gem))
            {
                result.Add(gem);
                i++;
            }
        }

        return result;
    }

    public GemInfo GetRandomGemOnWay(int current_c, int current_r)
    {
        GemInfo gem = null;
        while (gem == null)
        {
            // 0: up, 1: up&right, 2: down&right, 3:down, 4: down&left, 5: up&left
            int direction = Random.Range(0, 6);
            int distance = Random.Range(0, 10);
            int odd = current_c % 2;
            int column_ = current_c, row_ = current_r;
            switch (direction)
            {
                case 0:
                    row_ += distance;
                    gem = GetGem(column_, row_);
                    break;
                case 1:
                    column_ += distance; row_ += ((odd == 1) ? distance / 2 : (distance + 1) / 2);
                    gem = GetGem(column_, row_);
                    break;
                case 2:
                    column_ += distance; row_ -= ((odd == 0) ? distance / 2 : (distance + 1) / 2);
                    gem = GetGem(column_, row_);
                    break;
                case 3: // down
                    row_ -= distance;
                    gem = GetGem(column_, row_);
                    break;
                case 4:
                    column_ -= distance; row_ -= ((odd == 0) ? distance / 2 : (distance + 1) / 2);
                    gem = GetGem(column_, row_);
                    break;
                case 5:
                    column_ -= distance; row_ += ((odd == 1) ? distance / 2 : (distance + 1) / 2);
                    gem = GetGem(column_, row_);
                    break;
                default:
                    break;
            }
        }
        return gem;
    }

    // get 6 direction around gem list
    public List<List<GemInfo>> GetAroundGemList(int current_c, int current_r)
    {
        List<List<GemInfo>> aroundGemList = new List<List<GemInfo>>();
        int odd = current_c % 2;

        // init 6 direction
        // 0: up, 1: up&right, 2: down&right, 3:down, 4: down&left, 5: up&left
        for (int i = 0; i < 6; i++) // direction
        {
            List<GemInfo> temp = new List<GemInfo>();
            for (int j = 1; j < 10; j++) // distance
            {
                int column_ = current_c, row_ = current_r;
                GemInfo gem = null;
                switch (i)
                {
                    case 0:
                        row_ += j;
                        gem = GetGem(column_, row_);
                        break;
                    case 1:
                        column_ += j; row_ += ((odd == 1) ? j / 2 : (j + 1) / 2);
                        gem = GetGem(column_, row_);
                        break;
                    case 2:
                        column_ += j; row_ -= ((odd == 0) ? j / 2 : (j + 1) / 2);
                        gem = GetGem(column_, row_);
                        break;
                    case 3: // down
                        row_ -= j;
                        gem = GetGem(column_, row_);
                        break;
                    case 4:
                        column_ -= j; row_ -= ((odd == 0) ? j / 2 : (j + 1) / 2);
                        gem = GetGem(column_, row_);
                        break;
                    case 5:
                        column_ -= j; row_ += ((odd == 1) ? j / 2 : (j + 1) / 2);
                        gem = GetGem(column_, row_);
                        break;
                    default:
                        break;
                }
                if (gem == null)
                {
                    break;
                }
                temp.Add(gem);
            }
            aroundGemList.Add(temp);
        }
        return aroundGemList;
    }


    public GemInfo[] GetRandomGems(int cnt)
    {
        // TODO: Does not check whether the gem is already filled with water
        GemInfo[] gems = new GemInfo[cnt];
        int[,] pickedCoordinates = new int[cnt, 2];

        for (int i = 0; i < cnt; i++)
        {
            GemInfo gem; int column_, row_;
            bool bPicked;
            do
            {
                bPicked = false;
                column_ = Random.Range(0, 11);
                row_ = Random.Range(0, 6);
                gem = GetGem(column_, row_);

                // check if this gem is already picked
                for (int j = 0; j < i; j++)
                {
                    if (pickedCoordinates[j, 0] == column_ && pickedCoordinates[j, 1] == row_)
                    {
                        bPicked = true;
                        break;
                    }
                }
            } while (gem == null || gem.bPatternApplied || bPicked);

            pickedCoordinates[i, 0] = column_;
            pickedCoordinates[i, 1] = row_;

            gems[i] = gem;
        }

        return gems;
    }

    // 특정 행에 있는 광물 모두 얻어오기
    // 가장 아래 행부터 0, 1, 2, 3, 4 (행이 짝수면 그대로, 홀수면 +1)
    public List<GemInfo> GetGemRows(List<int> rows)
    {
        List<GemInfo> gems = new List<GemInfo>();

        foreach (int r in rows)
        {
            for (int i = 0; i < 11; i++)
            {
                int nr = r;
                if (i % 2 == 1)
                {
                    nr++;
                }
                GemInfo gem = GetGem(i, nr);
                if (gem != null)
                {
                    gems.Add(gem);
                }
            }
        }

        return gems;
    }

    // 특정 열에 있는 광물 모두 얻어오기
    public List<GemInfo> GetGemColumns(List<int> columns)
    {
        List<GemInfo> gems = new List<GemInfo>();

        foreach (int c in columns)
        {
            for(int i = 0; i < 6; i++)
            {
                GemInfo gem = GetGem(c, i);
                if (gem != null)
                {
                    gems.Add(gem);
                }
            }
        }

        return gems;
    }

    // 오른쪽 아래 대각선
    public List<GemInfo> GetGemDiagonalRight(List<int> diagonals)
    {
        List<GemInfo> gems = new List<GemInfo>();
        Dictionary<int, List<int>> diagonalDict = new Dictionary<int, List<int>>()
        {
            {0, new List<int>(){9,5 } },
            {1, new List<int>(){7,5 } },
            {2, new List<int>(){5,5 } },
            {3, new List<int>(){3,5 } },
            {4, new List<int>(){1,5 } },
            {5, new List<int>(){0,4 } },
            {6, new List<int>(){0,3 } },
            {7, new List<int>(){0,2 } },
            {8, new List<int>(){0,1 } },
            {9, new List<int>(){0,0 } },
        };
        foreach (int d in diagonals)
        {
            List<int> gemInfo = diagonalDict[d];
            gems.Add(GetGem(gemInfo[0], gemInfo[1]));
            List<List<GemInfo>> aroundGems = GetAroundGemList(gemInfo[0], gemInfo[1]);
            for (int i = 0; i < aroundGems[2].Count; i++)
            {
                gems.Add(aroundGems[2][i]);
            }
        }

        return gems;
    }

    // 왼쪽 아래 대각선
    public List<GemInfo> GetGemDiagonalLeft(List<int> diagonals)
    {
        List<GemInfo> gems = new List<GemInfo>();
        Dictionary<int, List<int>> diagonalDict = new Dictionary<int, List<int>>()
        {
            {0, new List<int>(){1,5 } },
            {1, new List<int>(){3,5 } },
            {2, new List<int>(){5,5 } },
            {3, new List<int>(){7,5 } },
            {4, new List<int>(){9,5 } },
            {5, new List<int>(){10,4 } },
            {6, new List<int>(){10,3 } },
            {7, new List<int>(){10,2 } },
            {8, new List<int>(){10,1 } },
            {9, new List<int>(){10,0 } },
        };
        foreach (int d in diagonals)
        {
            List<int> gemInfo = diagonalDict[d];
            gems.Add(GetGem(gemInfo[0], gemInfo[1]));
            List<List<GemInfo>> aroundGems = GetAroundGemList(gemInfo[0], gemInfo[1]);
            for (int i = 0; i < aroundGems[4].Count; i++)
            {
                gems.Add(aroundGems[4][i]);
            }
        }

        return gems;
    }
}

