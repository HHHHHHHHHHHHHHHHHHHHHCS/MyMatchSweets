using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MainGameManager : MonoBehaviour
{
    public static MainGameManager Instance { get; private set; }

    public const int xColumn = 10;
    public const int yColumn = 10;

    [SerializeField]
    private GameObject gridPrefab;
    private Transform itemRoot;

    private void Awake()
    {
        Instance = this;
        itemRoot = GameObject.Find("ItemRoot").transform;
    }

    private void Start()
    {
        SpawnGrid();
    }

    private void SpawnGrid()
    {
        for (int i = 0; i < xColumn; i++)
        {
            for (int j = 0; j < yColumn; j++)
            {
                GameObject chocolate = Instantiate(gridPrefab, new Vector3(i, j, 0)
                    ,Quaternion.identity, itemRoot);
            }
        }
    }
}
