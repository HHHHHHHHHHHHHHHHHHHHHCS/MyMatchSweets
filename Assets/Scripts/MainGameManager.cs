using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    private readonly Vector2 startPos = new Vector2(-4.5f, 4.5f);
    private const int xColumn = 10, yColumn = 10;

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
                sweets[i, j] = CreateNewSweet(SweetsType.Empty, i, j, itemRoot);
            }
        }

        StartCoroutine(AllFill());
    }

    public SweetInfo CreateNewSweet(SweetsType _sweetsType, int _x, int _y, Transform itemRoot)
    {
        return Instantiate(sweetsPrefabDic[_sweetsType].prefab
                    , Vector2.zero, Quaternion.identity, itemRoot)
                    .Init(_sweetsType, _x, _y, itemRoot);
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
            yield return new WaitForSeconds(0.5f);
        }
    }


    /// <summary>
    /// 部分填充的办法
    /// </summary>
    public bool Fill()
    {
        bool filledNotFinished = false;//判断本次填充是否完成

        for (int y = yColumn - 1; y > 0; y--)
        {
            for (int x = 0; x < xColumn; x++)
            {
                var sweet = sweets[x, y];//得到当前元素位置

                if (sweet.SweetType == SweetsType.Empty)//如果该物体无法移动，则无法往下填充
                {
                    var sweetUp = sweets[x, y - 1];
                    if (sweetUp.CanMove())
                    {
                        Destroy(sweet.gameObject);
                        sweetUp.Move(x, y);
                        sweets[x, y] = sweetUp;
                        sweets[x, y - 1] = CreateNewSweet(SweetsType.Empty, x, y, itemRoot);
                        filledNotFinished = true;
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
                sweets[x, 0] = CreateNewSweet(SweetsType.Normal, x, 0, itemRoot);
                filledNotFinished = true;
            }
        }


        return filledNotFinished;
    }
}
