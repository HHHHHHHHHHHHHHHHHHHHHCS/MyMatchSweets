using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    private readonly Vector2 startPos = new Vector2(-4.5f, 4.5f);
    private const int xColumn = 10, yColumn = 10;
    private const float fillTime = 0.25f;

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
    private SweetInfo[,] sweets;


    private void Awake()
    {
        Init();
    }

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
        sweets = new SweetInfo[xColumn, yColumn];
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yColumn; j++)
            {
                sweets[i, j] = CreateNewSweet(SweetsType.Empty, itemRoot, i, j);
            }
        }
        Destroy(sweets[4, 4].gameObject);
        sweets[4, 4] = CreateNewSweet(SweetsType.Barrier, itemRoot, 4, 4);
        StartCoroutine(AllFill());
    }

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
        while (Fill())
        {
            yield return new WaitForSeconds(fillTime);
        }
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
                var sweet = sweets[x, y];//得到当前元素位置

                if (sweet.CanMove())//如果无法移动则无法往下填充
                {
                    var sweetDown = sweets[x, y + 1];

                    if (sweetDown.SweetType == SweetsType.Empty)//垂直填充
                    {
                        Destroy(sweetDown.gameObject);
                        sweet.Move(x, y + 1, fillTime);
                        sweets[x, y + 1] = sweet;
                        sweets[x, y] = CreateNewSweet(SweetsType.Empty, itemRoot, x, y);
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
                                    SweetInfo downSweet = sweets[downX, y + 1];
                                    if (downSweet.SweetType == SweetsType.Empty)
                                    {
                                        bool canfill = true;//用来判断垂直填充是否可以满足填充要求
                                        for (int aboveY = y; aboveY >= 0; aboveY--)
                                        {
                                            SweetInfo sweetAbove = sweets[downX, aboveY];
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
                                            sweets[downX, y + 1] = sweet;
                                            sweets[x, y] = CreateNewSweet(SweetsType.Empty, itemRoot, x, y);
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
            var sweet = sweets[x, 0];//得到当前元素位置

            if (sweet.SweetType == SweetsType.Empty)
            {
                Destroy(sweet.gameObject);
                sweets[x, 0] = CreateNewSweet(SweetsType.Normal, itemRoot, x, 0, SweetsSpawnPos.Up);
                filledNotFinished = true;
            }
        }


        return filledNotFinished;
    }
}
