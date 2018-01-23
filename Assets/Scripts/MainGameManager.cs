using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
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

    public Dictionary<SweetsType, SweetsPrefabStruct> SweetsPrefabDic { get { return sweetsPrefabDic; } }
    public Dictionary<SweetsColorType, SweetsColorStruct> SweetsColorDic { get { return sweetsColorDic; } }

    private Transform itemRoot;
    private Dictionary<SweetsType, SweetsPrefabStruct> sweetsPrefabDic;
    private Dictionary<SweetsColorType, SweetsColorStruct> sweetsColorDic;
    private SweetInfo[,] sweetsArray;

    private SweetInfo baseSweet;//按下的甜品
    private SweetInfo changeSweet;//要交换甜品


    private void Awake()
    {
        Init();
    }

    #region Init
    private void Init()
    {
        Instance = this;
        InitRoot();
        InitSpawnGrid();
        InitSweetsDic();
        InitSweetsArray();
    }

    private void InitRoot()
    {
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
        sweetsPrefabDic = new Dictionary<SweetsType, SweetsPrefabStruct>();
        foreach (var item in sweetsPrefabList)
        {
            if (!sweetsPrefabDic.ContainsKey(item.sweetType))
            {
                sweetsPrefabDic.Add(item.sweetType, item);
            }
        }
        sweetsPrefabList.Clear();
        sweetsPrefabList = null;

        sweetsColorDic = new Dictionary<SweetsColorType, SweetsColorStruct>();
        foreach (var item in sweetsColorList)
        {
            if (!sweetsColorDic.ContainsKey(item.sweetColorType))
            {
                sweetsColorDic.Add(item.sweetColorType, item);
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
                sweetsArray[i, j] = CreateNewSweet(SweetsType.Empty, itemRoot, i, j);
            }
        }
        Destroy(sweetsArray[0, 4].gameObject);
        sweetsArray[0, 4] = CreateNewSweet(SweetsType.Barrier, itemRoot, 0, 4);
        Destroy(sweetsArray[3, 4].gameObject);
        sweetsArray[3, 4] = CreateNewSweet(SweetsType.Barrier, itemRoot, 3, 4);
        Destroy(sweetsArray[6, 4].gameObject);
        sweetsArray[6, 4] = CreateNewSweet(SweetsType.Barrier, itemRoot, 6, 4);
        Destroy(sweetsArray[8, 4].gameObject);
        sweetsArray[8, 4] = CreateNewSweet(SweetsType.Barrier, itemRoot, 8, 4);
        StartCoroutine(AllFill());
    }
    #endregion

    #region Create And Fill
    public SweetInfo CreateNewSweet(SweetsType _sweetsType, Transform itemRoot, int _x, int _y, SweetsSpawnPos posEnum = SweetsSpawnPos.Current)
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
        return Instantiate(sweetsPrefabDic[_sweetsType].prefab
                    , spawnPos, Quaternion.identity, itemRoot)
                    .Init(_sweetsType, itemRoot, _x, _y, fillTime);
    }

    public Vector2 CorrectPosition(int newX, int newY)
    {
        return startPos + new Vector2(newX, -newY);
    }

    /// <summary>
    /// 全部填充的方法
    /// </summary>
    public IEnumerator AllFill()
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
                        sweetsArray[x, y] = CreateNewSweet(SweetsType.Empty, itemRoot, x, y);
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
                                            sweetsArray[x, y] = CreateNewSweet(SweetsType.Empty, itemRoot, x, y);
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
                sweetsArray[x, 0] = CreateNewSweet(SweetsType.Normal, itemRoot, x, 0, SweetsSpawnPos.Up);
                filledNotFinished = true;
            }
        }


        return filledNotFinished;
    }

    #endregion

    #region Swap
    public void SetBaseSweet(SweetInfo sweet)
    {
        baseSweet = sweet;
    }

    public void SetChangeSweet(SweetInfo sweet)
    {
        changeSweet = sweet;
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
        var list1 = MatchSweets(info1, info2.X, info2.Y);
        var list2 = MatchSweets(info2, info1.X, info1.Y);
        if (list1 != null || list2 != null)
        {
            int tempX = info1.X, tempY = info1.Y;
            info1.Move(info2.X, info2.Y, fillTime);
            info2.Move(tempX, tempY, fillTime);
            ClearSweets(list1);
            ClearSweets(list2);
            StartCoroutine(AllFill());
            return true;
        }
        else
        {
            sweetsArray[info1.X, info1.Y] = info1;
            sweetsArray[info2.X, info2.Y] = info2;
            return false;
        }
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
    public bool ClearSweets(List<SweetInfo> list)
    {
        if (list != null && list.Count > 0)
        {
            foreach (var item in list)
            {
                ClearSweet(item);
            }
            return true;
        }
        return false;
    }

    public bool ClearSweets(int x, int y)
    {
        return ClearSweet(sweetsArray[x, y]);
    }


    public bool ClearSweet(SweetInfo item)
    {
        if (item.CanClear() && !item.ClearComponent.IsClear)
        {
            sweetsArray[item.X, item.Y] = CreateNewSweet(SweetsType.Empty, itemRoot, item.X, item.Y);
            item.ClearComponent.Clear();
            return true;
        }
        return false;
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
}
