using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{

    public static int gridWidth = 10;
    public static int gridHeight = 20;
    // Start is called before the first frame update
    void Start()
    {
        SpawnNextTetromino();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnNextTetromino()
    {
        GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 18.0f), Quaternion.identity) as GameObject;
    }
    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth &&(int)pos.y >= 0);
    }
    public Vector2 Round(Vector2 pos)
    {
        return new Vector2 (Mathf.Round(pos.x), Mathf.Round(pos.y));
    }

    string GetRandomTetromino()
    {
        int randomTetromino = Random.Range(1, 8);
        string randomTetrominoName = "Prefabs/Tetromino T";

        switch (randomTetromino)
        {
            case 1:
                randomTetrominoName = "Prefabs/Tetromino T";
                break;
            case 2:
                randomTetrominoName = "Prefabs/Tetromino L";
                break;
            case 3:
                randomTetrominoName = "Prefabs/Tetromino J";
                break;
            case 4:
                randomTetrominoName = "Prefabs/Tetromino O";
                break;
            case 5:
                randomTetrominoName = "Prefabs/Tetromino I";
                break;
            case 6:
                randomTetrominoName = "Prefabs/Tetromino S";
                break;
            case 7:
                randomTetrominoName = "Prefabs/Tetromino Z";
                break;
        }

        return randomTetrominoName;
    }
}
