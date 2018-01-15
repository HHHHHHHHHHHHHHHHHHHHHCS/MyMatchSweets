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
    private List<SweetStruct> sweetPrefabList;



    [SerializeField]
    private GameObject gridPrefab;

    private Transform itemRoot;
    private Dictionary<SweetsType, SweetStruct> sweetPrefabDic;
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
        InitSweetPrefab();
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
                GameObject chocolate = Instantiate(gridPrefab, CorrectPosition(i,j)
                    , Quaternion.identity, itemRoot);
            }
        }
    }

    private void InitSweetPrefab()
    {
        sweetPrefabDic = new Dictionary<SweetsType, SweetStruct>();
        foreach(var item in sweetPrefabList)
        {
            if(!sweetPrefabDic.ContainsKey(item.sweetType))
            {
                sweetPrefabDic.Add(item.sweetType, item);
            }
        }
        sweetPrefabList.Clear();
        sweetPrefabList = null;
    }

    private void InitSweetsArray()
    {
        sweets = new SweetInfo[xColumn, yColumn];
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yColumn; j++)
            {
                var item = Instantiate(sweetPrefabDic[SweetsType.Normal].prefab
                    , CorrectPosition(i, j), Quaternion.identity, itemRoot)
                    .Init(SweetsType.Normal,i,j);
                item.transform.SetParent(itemRoot);
                sweets[i, j] = item;
            }
        }
    }

    public Vector2 CorrectPosition(int newX, int newY)
    {
        return startPos + new Vector2(newX, -newY);
    }
}
