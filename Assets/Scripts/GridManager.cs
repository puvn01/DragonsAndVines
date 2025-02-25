using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    public int rows, cols;
    public TileController tile;
    public Transform cameraPos;
    public DragonController dragon;



    private int tileIndexer = 0;
    [HideInInspector]
    public List<TileController> _tilesList = new List<TileController>();
    private SpriteRenderer board;
    [HideInInspector]
    public Vector2 TileScaleFactor;
    private Dictionary<int, int> dragonDict;

    private void Awake()
    {
        board = GetComponentInChildren<SpriteRenderer>();
  
    }
    private void Start()
    {

        GenerateGrid();
        spawnDragon();
        spawnVine();
    }

    private void GenerateGrid()
    {
        TileScaleFactor.x = board.size.x / rows;
        TileScaleFactor.y = board.size.y / cols;


        for (int y = 0; y < rows; y++)
        {

            if (y % 2 == 0)
            {
                for (int x = 0; x < cols; x++)
                {
                    spawnTile(x, y);
                }
            }
            else
            {
                for (int x = cols - 1; x >= 0; x--)
                {
                    spawnTile(x, y);
                }
            }


        }
        cameraPos.transform.position = new Vector3((rows / 2 - 0.5f) * TileScaleFactor.x, (cols / 2 - 0.5f) * TileScaleFactor.y, cameraPos.transform.position.z);
        board.transform.position = new Vector3(cameraPos.transform.position.x, cameraPos.transform.position.y, board.transform.position.z);
    }

    private void spawnTile(int x, int y)
    {

        // var spawnedTile = Instantiate(tile, new Vector3(x * TileScaleFactor.x, y * TileScaleFactor.y), Quaternion.identity);
        var spawnedTile = Instantiate(tile, this.transform);
        spawnedTile.transform.position = new Vector3(x * TileScaleFactor.x, y * TileScaleFactor.y);

        spawnedTile.name = $"Tile {x} {y}";
        spawnedTile.TileNum = tileIndexer;
        tileIndexer += 1;
        _tilesList.Add(spawnedTile);
        var isOffset = (x % 2 == 0 && y % 2 != 0) || (x % 2 != 0 && y % 2 == 0);
        spawnedTile.Init(isOffset);
        spawnedTile.Scale(TileScaleFactor);
        
    }

    public TileController GetTileAtIndex(int index)
    {
        return _tilesList[index];

    }

    public void spawnDragon()
    {
        Dictionary<int, int> dragonDict = new Dictionary<int, int>();
        dragonDict.Add(24, -21);
        dragonDict.Add(52, -25);
        dragonDict.Add(82, -24);

        modifyTile(dragonDict);


        //var spawnDragon = Instantiate(dragon, this.transform);

        //spawnDragon.transform.position = new Vector3(t.transform.position.x , t.transform.position.y, 0);

        //spawnDragon.name = "Dragon 1";
        //spawnDragon.Scale(TileScaleFactor);
        //spawnDragon.moveModifier = -20;


    }

    public void spawnVine()
    {

        Dictionary<int, int> vineDict = new Dictionary<int, int>();
        vineDict.Add(11, 19);
        vineDict.Add(22, 34);
        vineDict.Add(51, 22);

        modifyTile(vineDict);


    }

    public void modifyTile(Dictionary<int,int> dragonVineDict)
    {
        foreach (var coords in dragonVineDict)
        {
            TileController t = GetTileAtIndex(coords.Key);
            t.moveModifier = coords.Value;


        }
    }

}
