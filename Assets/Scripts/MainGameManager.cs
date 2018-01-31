using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    #region Values
    public static MainGameManager Instance { get; private set; }

    private readonly Vector2 startPos = new Vector2(-4.5f, 3.5f);
    private const int xColumn = 9, yColumn = 8;
    public const float fillTime = 0.25f;

    [SerializeField]
    private List<SweetsPrefabStruct> sweetsPrefabList;
    [SerializeField]
    private List<SweetsColorStruct> sweetsColorList;
    [SerializeField]
    private GameObject gridPrefab;

    public MainUIManager MainUIManager { get; private set; }
    public MainAudioManager MainAudioManager { get; private set; }
    public Dictionary<SweetsType, SweetsPrefabStruct> SweetsPrefabDic { get; private set; }
    public Dictionary<SweetsColorType, SweetsColorStruct> SweetsColorDic { get; private set; }

    private Transform itemRoot;
    private SweetInfo[,] sweetsArray;

    private Queue<KeyValuePair<bool, int>> saveClearLine;// bool == isrow

    private SweetInfo baseSweet;//按下的甜品
    private SweetInfo changeSweet;//要交换甜品

    private float gameTime;
    private int score;
    private float addScore;
    private bool isGameOver;

    #endregion

    private void Awake()
    {
        Init();
    }

    private void Update()
    {
        RunTime();
        UpdateScore();
    }

    #region Init
    private void Init()
    {
        Instance = this;
        InitObject();
        InitSpawnGrid();
        InitSweetsDic();
        InitSweetsArray();

        gameTime = 60f;
    }

    private void InitObject()
    {
        MainUIManager = GameObject.Find("MainUIRoot").GetComponent<MainUIManager>().Init();
        MainAudioManager = GameObject.Find("MainAudioManager").GetComponent<MainAudioManager>().Init();
        itemRoot = GameObject.Find("ItemRoot").transform;
    }

    private void InitSpawnGrid()
    {
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yColumn; j++)
            {
                GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(i, j)
                    , Quaternion.identity, itemRoot);
                chocolate.isStatic = true;
            }
        }
    }

    private void InitSweetsDic()
    {
        saveClearLine = new Queue<KeyValuePair<bool, int>>();

        SweetsPrefabDic = new Dictionary<SweetsType, SweetsPrefabStruct>();
        foreach (var item in sweetsPrefabList)
        {
            if (!SweetsPrefabDic.ContainsKey(item.sweetType))
            {
                SweetsPrefabDic.Add(item.sweetType, item);
            }
        }
        sweetsPrefabList.Clear();
        sweetsPrefabList = null;

        SweetsColorDic = new Dictionary<SweetsColorType, SweetsColorStruct>();
        foreach (var item in sweetsColorList)
        {
            if (!SweetsColorDic.ContainsKey(item.sweetColorType))
            {
                SweetsColorDic.Add(item.sweetColorType, item);
            }
        }
        sweetsColorList.Clear();
        sweetsColorList = null;
    }

    private void InitSweetsArray()
    {
        sweetsArray = new SweetInfo[xColumn, yColumn];
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yColumn; j++)
            {
                sweetsArray[i, j] = CreateNewSweet(SweetsType.Empty, i, j);
            }
        }
        Destroy(sweetsArray[0, 4].gameObject);
        sweetsArray[0, 4] = CreateNewSweet(SweetsType.Barrier, 0, 4);
        Destroy(sweetsArray[3, 4].gameObject);
        sweetsArray[3, 4] = CreateNewSweet(SweetsType.Barrier, 3, 4);
        Destroy(sweetsArray[6, 4].gameObject);
        sweetsArray[6, 4] = CreateNewSweet(SweetsType.Barrier, 6, 4);
        Destroy(sweetsArray[8, 4].gameObject);
        sweetsArray[8, 4] = CreateNewSweet(SweetsType.Barrier, 8, 4);
        AllFill();
    }
    #endregion

    #region Create And Fill
    public SweetInfo CreateNewSweet(SweetsType _sweetsType, int _x, int _y, SweetsSpawnPos posEnum = SweetsSpawnPos.Current, Transform _itemRoot = null)
    {
        Vector2 spawnPos = Vector2.zero; ;
        switch (posEnum)
        {
            case SweetsSpawnPos.Zero:
                spawnPos = Vector2.zero;
                break;
            case SweetsSpawnPos.Up:
                spawnPos = CorrectPosition(_x, _y - 1);
                break;
            case SweetsSpawnPos.Current:
                spawnPos = CorrectPosition(_x, _y);
                break;
            default:
                break;
        }
        return Instantiate(SweetsPrefabDic[_sweetsType].prefab
                    , spawnPos, Quaternion.identity, _itemRoot ? _itemRoot : itemRoot)
                    .Init(_sweetsType, itemRoot, _x, _y, fillTime);
    }

    public SweetInfo CreateSpecialSweet(int count, SweetsColorType color, int _x, int _y)
    {
        SweetInfo info = null;
        if (count == 4)
        {
            Destroy(sweetsArray[_x, _y].gameObject);
            var type = UnityEngine.Random.Range(0, 1f) < 0.5f ? SweetsType.Row_Clear : SweetsType.Column_Clear;
            info = sweetsArray[_x, _y] = CreateNewSweet(type, _x, _y);
            info.SetColor(color);
        }
        else if (count >= 5)
        {
            Destroy(sweetsArray[_x, _y].gameObject);
            sweetsArray[_x, _y] = CreateNewSweet(SweetsType.Rainbowcandy, _x, _y);
        }

        return info;
    }

    public Vector2 CorrectPosition(int newX, int newY)
    {
        return startPos + new Vector2(newX, -newY);
    }

    public void AllFill()
    {
        StartCoroutine(_AllFill());
    }

    /// <summary>
    /// 全部填充的方法
    /// </summary>
    public IEnumerator _AllFill()
    {

        do
        {
            bool isClear;
            do
            {
                yield return new WaitForSeconds(fillTime);


                while (Fill())
                {
                    yield return new WaitForSeconds(fillTime);
                }
                isClear = ClearAllMatchSweet();

            } while (isClear);
        }
        while (ClearSaveLine());
    }

    /// <summary>
    /// 检查有是否为空的
    /// </summary>
    /// <returns></returns>
    public bool CheckAll()
    {
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yColumn; y++)
            {
                if (sweetsArray[x, y].SweetType == SweetsType.Empty)
                {
                    return true;
                }
            }
        }
        return false;
    }

    /// <summary>
    /// 部分填充的办法
    /// </summary>
    public bool Fill()
    {
        bool filledNotFinished = false;//判断本次填充是否完成

        for (int y = yColumn - 2; y >= 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                var sweet = sweetsArray[x, y];//得到当前元素位置

                if (sweet.CanMove())//如果无法移动则无法往下填充
                {
                    var sweetDown = sweetsArray[x, y + 1];

                    if (sweetDown.SweetType == SweetsType.Empty)//垂直填充
                    {
                        Destroy(sweetDown.gameObject);
                        sweet.Move(x, y + 1, fillTime);
                        sweetsArray[x, y + 1] = sweet;
                        sweetsArray[x, y] = CreateNewSweet(SweetsType.Empty, x, y);
                        filledNotFinished = true;
                    }
                    else//斜向填充
                    {
                        for (int _downX = -1; _downX <= 1; _downX++)
                        {
                            if (_downX != 0)
                            {
                                int downX = x + _downX;
                                if (downX >= 0 && downX < xColumn)
                                {
                                    SweetInfo downSweet = sweetsArray[downX, y + 1];
                                    if (downSweet.SweetType == SweetsType.Empty)
                                    {
                                        bool canfill = true;//用来判断垂直填充是否可以满足填充要求
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            SweetInfo sweetAbove = sweetsArray[downX, aboveY];
                                            if (sweetAbove.CanMove())
                                            {
                                                break;
                                            }
                                            else if (sweetAbove.SweetType != SweetsType.Empty)
                                            {
                                                canfill = false;
                                                break;
                                            }
                                        }
                                        if (!canfill)
                                        {
                                            Destroy(downSweet.gameObject);
                                            sweet.Move(downX, y + 1, fillTime);
                                            sweetsArray[downX, y + 1] = sweet;
                                            sweetsArray[x, y] = CreateNewSweet(SweetsType.Empty, x, y);
                                            break;
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        for (int x = 0; x < xColumn; x++)
        {
            var sweet = sweetsArray[x, 0];//得到当前元素位置

            if (sweet.SweetType == SweetsType.Empty)
            {
                Destroy(sweet.gameObject);
                sweetsArray[x, 0] = CreateNewSweet(SweetsType.Normal, x, 0, SweetsSpawnPos.Up);
                filledNotFinished = true;
            }
        }

        //filledNotFinished = !filledNotFinished? CheckAll():true;

        return filledNotFinished;
    }

    #endregion

    #region Swap
    public void SetBaseSweet(SweetInfo sweet)
    {
        if (!isGameOver && sweet.CanClear())
        {
            baseSweet = sweet;
        }
    }

    public void SetChangeSweet(SweetInfo sweet)
    {
        if (!isGameOver && sweet.CanClear())
        {
            changeSweet = sweet;
        }
    }

    public void ReleaseSweet()
    {
        if (baseSweet && changeSweet)
        {
            baseSweet.ExchangeSweets(changeSweet);
        }
        baseSweet = changeSweet = null;
    }


    public bool ExchangeSweets(SweetInfo info1, SweetInfo info2)
    {
        sweetsArray[info1.X, info1.Y] = info2;
        sweetsArray[info2.X, info2.Y] = info1;
        if (info1.SweetType != SweetsType.Rainbowcandy
            && info2.SweetType != SweetsType.Rainbowcandy)
        {
            var list1 = MatchSweets(info1, info2.X, info2.Y);
            var list2 = MatchSweets(info2, info1.X, info1.Y);
            if (list1 != null || list2 != null)
            {
                int tempX = info1.X, tempY = info1.Y;
                info1.Move(info2.X, info2.Y, fillTime);
                info2.Move(tempX, tempY, fillTime);
                ClearSweets(list1);
                ClearSweets(list2);

                return true;
            }
            else
            {
                sweetsArray[info1.X, info1.Y] = info1;
                sweetsArray[info2.X, info2.Y] = info2;
                return false;
            }
        }
        else if (info1 != info2)
        {
            if (info1.SweetType == SweetsType.Rainbowcandy
            && info2.SweetType == SweetsType.Rainbowcandy)
            {
                ClearColor(SweetsColorType.Any, info1, info2);
                return true;
            }
            else if (info1.SweetType == SweetsType.Rainbowcandy
                && info2.SweetType != SweetsType.Empty)
            {

                ClearColor(info2.ColorComponent.SweetColorType, info1);
                return true;
            }
            else if (info2.SweetType == SweetsType.Rainbowcandy
                && info1.SweetType != SweetsType.Empty)
            {

                ClearColor(info1.ColorComponent.SweetColorType, info2);
                return true;
            }
        }
        return false;
    }

    public List<SweetInfo> MatchSweets(SweetInfo sweet, int newX, int newY)
    {
        if (sweet.CanChangeColor())
        {
            SweetsColorType color = sweet.ColorComponent.SweetColorType;
            List<SweetInfo> matchRowSweets = new List<SweetInfo>();
            List<SweetInfo> matchLineSweets = new List<SweetInfo>();
            List<SweetInfo> finisedMatchSweets = new List<SweetInfo>();

            for (int xDir = -1; xDir <= 1; xDir++)
            {
                if (xDir == 0)
                {
                    continue;
                }
                for (int xDistance = 1; xDistance < xColumn; xDistance++)
                {
                    int posX = newX + (xDir * xDistance);
                    if (posX < 0 || posX >= xColumn)
                    {
                        break;
                    }
                    if (sweetsArray[posX, newY].CanChangeColor() && sweetsArray[posX, newY].ColorComponent.SweetColorType == color)
                    {
                        matchRowSweets.Add(sweetsArray[posX, newY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }
            for (int yDir = -1; yDir <= 1; yDir++)
            {
                if (yDir == 0)
                {
                    continue;
                }
                for (int yDistance = 1; yDistance < yColumn; yDistance++)
                {
                    int posY = newY + (yDir * yDistance);
                    if (posY < 0 || posY >= yColumn)
                    {
                        break;
                    }
                    if (sweetsArray[newX, posY].CanChangeColor() && sweetsArray[newX, posY].ColorComponent.SweetColorType == color)
                    {
                        matchLineSweets.Add(sweetsArray[newX, posY]);
                    }
                    else
                    {
                        break;
                    }
                }
            }

            if (matchRowSweets.Count >= 2)
            {
                finisedMatchSweets.AddRange(matchRowSweets);
            }

            if (matchLineSweets.Count >= 2)
            {
                finisedMatchSweets.AddRange(matchLineSweets);
            }

            finisedMatchSweets.Add(sweet);

            if (finisedMatchSweets.Count >= 3)
            {
                return finisedMatchSweets;
            }
        }
        return null;
    }
    #endregion

    #region Clear
    public bool ClearSweets(List<SweetInfo> list, bool isSpecial = false)
    {
        if (list != null && list.Count > 0)
        {
            int realNumber = 0;
            foreach (var item in list)
            {
                if (ClearSweet(item))
                {
                    realNumber++;
                }
            }
            GetScore(realNumber);
            if (!isSpecial)
            {
                var endSweet = list[list.Count - 1];
                CreateSpecialSweet(realNumber, endSweet.ColorComponent.SweetColorType, endSweet.X, endSweet.Y);
            }
            AllFill();
            return true;
        }
        AllFill();
        return false;
    }

    public bool ClearSweet(int x, int y)
    {
        return ClearSweet(sweetsArray[x, y]);
    }

    public bool ClearSweet(SweetInfo item)
    {
        if (item.CanClear() && !item.ClearComponent.IsClear)
        {
            sweetsArray[item.X, item.Y] = CreateNewSweet(SweetsType.Empty, item.X, item.Y);
            ClearBiscuit(item);
            item.ClearComponent.Clear();
            return true;
        }
        return false;
    }

    public bool ClearSaveLine()
    {
        if (saveClearLine.Count > 0)
        {
            var v = saveClearLine.Dequeue();
            if (v.Key)
            {
                _ClearRow(v.Value);
            }
            else
            {
                _ClearColumn(v.Value);
            }
            return true;
        }
        return false;
    }

    public void ClearRow(int y)
    {
        saveClearLine.Enqueue(new KeyValuePair<bool, int>(true, y));
    }

    public bool _ClearRow(int y)
    {
        if (y < 0 || y >= yColumn)
        {
            return false;
        }
        List<SweetInfo> list = new List<SweetInfo>();
        for (int i = 0; i < xColumn; i++)
        {
            if (sweetsArray[i, y].SweetType != SweetsType.Rainbowcandy)
            {
                list.Add(sweetsArray[i, y]);
            }
        }
        ClearSweets(list, true);
        return true;
    }

    public void ClearColumn(int x)
    {
        saveClearLine.Enqueue(new KeyValuePair<bool, int>(false, x));
    }

    public bool _ClearColumn(int x)
    {
        if (x < 0 || x >= xColumn)
        {
            return false;
        }
        List<SweetInfo> list = new List<SweetInfo>();
        for (int i = 0; i < yColumn; i++)
        {
            if (sweetsArray[x, i].SweetType != SweetsType.Rainbowcandy)
            {
                list.Add(sweetsArray[x, i]);
            }

        }
        ClearSweets(list, true);
        return true;
    }

    public void ClearColor(SweetsColorType type, SweetInfo boom1 = null, SweetInfo boom2 = null)
    {
        List<SweetInfo> list = new List<SweetInfo>();
        SweetInfo item;
        for (int x = 0; x < xColumn; x++)
        {
            for (int y = 0; y < yColumn; y++)
            {
                item = sweetsArray[x, y];
                if (item.CanChangeColor()
                    && (item.ColorComponent.SweetColorType == type
                    || type == SweetsColorType.Any))
                {
                    list.Add(item);
                }
            }
        }
        if (boom1 != null)
        {
            list.Add(boom1);
        }
        if (boom2 != null)
        {
            list.Add(boom2);
        }
        ClearSweets(list, true);
    }

    public bool ClearBiscuit(int x, int y)
    {
        return ClearBiscuit(sweetsArray[x, y]);
    }

    public bool ClearBiscuit(SweetInfo item)
    {
        bool haveClear = false;
        for (int friendX = item.X - 1; friendX <= item.X + 1; friendX++)
        {
            if (friendX == item.X || friendX < 0 || friendX >= xColumn)
            {
                continue;
            }
            var newItem = sweetsArray[friendX, item.Y];
            if (newItem.SweetType == SweetsType.Barrier && newItem.CanClear())
            {
                sweetsArray[friendX, item.Y] = CreateNewSweet(SweetsType.Empty, friendX, item.Y);
                newItem.ClearComponent.Clear();
                haveClear = true;
            }
        }

        for (int friendY = item.Y - 1; friendY <= item.Y + 1; friendY++)
        {
            if (friendY == item.Y || friendY < 0 || friendY >= yColumn)
            {
                continue;
            }
            var newItem = sweetsArray[item.X, friendY];
            if (newItem.SweetType == SweetsType.Barrier && newItem.CanClear())
            {
                sweetsArray[item.X, friendY] = CreateNewSweet(SweetsType.Empty, item.X, friendY);
                newItem.ClearComponent.Clear();
                haveClear = true;
            }
        }
        return haveClear;
    }

    private bool ClearAllMatchSweet()
    {
        bool needRefill = false;
        for (int y = 0; y < yColumn; y++)
        {
            for (int x = 0; x < xColumn; x++)
            {
                if (sweetsArray[x, y].CanClear())
                {
                    needRefill = ExchangeSweets(sweetsArray[x, y], sweetsArray[x, y]);
                }
            }
        }

        return needRefill;
    }
    #endregion

    #region Score And Time
    public void GetScore(int number)
    {
        score += number * number;
        if (MainAudioManager)
        {
            MainAudioManager.PlayClearSweetAudio();
        }
    }

    public void UpdateScore()
    {
        var _score = Mathf.Clamp((score - addScore) * Time.deltaTime, 0.15f, (score - addScore) / 10);
        addScore = Mathf.Clamp((addScore + _score), 0, score);
        if (MainUIManager)
        {
            MainUIManager.RefreshScore((int)addScore);
        }
    }

    private void RunTime()
    {
        if (isGameOver)
        {
            return;
        }
        gameTime -= Time.deltaTime;
        if (gameTime <= 0)
        {
            TimeOver();
        }
        else if (MainUIManager)
        {
            MainUIManager.RefreshTime(gameTime);
        }
    }

    private void TimeOver()
    {
        gameTime = 0;
        isGameOver = true;
        baseSweet = changeSweet = null;
        if (MainUIManager)
        {
            MainUIManager.RefreshTime(gameTime);
            MainUIManager.TimeOver(score);
        }
    }
    #endregion

}
