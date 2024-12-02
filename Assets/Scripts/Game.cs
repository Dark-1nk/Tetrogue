using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Game : MonoBehaviour
{
    public static int gridWidth = 10;
    public static int gridHeight = 20;

    public Canvas gameOverCanvas; // Reference to the Game Over Canvas
    AudioManager audioManager;

    public static Transform[,] grid = new Transform[gridWidth, gridHeight];

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();

        // Ensure the Game Over Canvas is hidden at the start
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(false);
        }
        InitializePreplacedBlocks();

        SpawnNextTetromino();

    }

    void InitializePreplacedBlocks()
    {
        // Find all preplaced blocks in the scene
        GameObject[] preplacedBlocks = GameObject.FindGameObjectsWithTag("PreplacedBlock");

        foreach (GameObject block in preplacedBlocks) // Iterate over GameObjects
        {
            Transform blockTransform = block.transform; // Access the Transform of each GameObject
            Vector2 pos = Round(blockTransform.position);

            // Check if the position is inside the grid bounds
            if (CheckIsInsideGrid(pos))
            {
                grid[(int)pos.x, (int)pos.y] = blockTransform; // Add block to grid
            }
            else
            {
                Debug.LogWarning($"Preplaced block at {pos} is out of bounds!");
            }
        }
    }

    public bool IsRowFullAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] == null)
            {
                return false;
            }
        }

        return true;
    }

    public void DeleteMinoAt(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            Destroy(grid[x, y].gameObject);

            grid[x, y] = null;
        }
    }

    public void MoveRowDown(int y)
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, y] != null)
            {
                grid[x, y - 1] = grid[x, y];
                grid[x, y] = null;
                grid[x, y - 1].position += new Vector3(0, -1, 0);
            }
        }
    }

    public void MoveAllRowsDown(int y)
    {
        for (int i = y; i < gridHeight; ++i)
        {
            MoveRowDown(i);
        }
    }

    public void DeleteRow()
    {
        for (int y = 0; y < gridHeight; y++)
        {
            if (IsRowFullAt(y))
            {
                audioManager.PlaySFX(audioManager.clearLine);
                DeleteMinoAt(y);
                MoveAllRowsDown(y + 1);
                --y;
            }
        }
    }

    public void UpdateGrid(Tetromino tetromino)
    {
        for (int y = 0; y < gridHeight; ++y)
        {
            for (int x = 0; x < gridWidth; ++x)
            {
                if (grid[x, y] != null)
                {
                    if (grid[x, y].parent == tetromino.transform)
                    {
                        grid[x, y] = null;
                    }
                }
            }
        }

        foreach (Transform mino in tetromino.transform)
        {
            Vector2 pos = Round(mino.position);
            if (pos.y < gridHeight)
            {
                grid[(int)pos.x, (int)pos.y] = mino;
            }
        }
    }

    public Transform GetTransformAtGridPosition(Vector2 pos)
    {
        if (pos.y > gridHeight - 1)
        {
            return null;
        }

        else
        {
            return grid[(int)pos.x, (int)pos.y];
        }
    }

    public void SpawnNextTetromino()
    {
        GameObject nextTetromino = (GameObject)Instantiate(Resources.Load(GetRandomTetromino(), typeof(GameObject)), new Vector2(5.0f, 20.0f), Quaternion.identity);
        nextTetromino.GetComponent<Tetromino>().isLocked = false;
    }

    public bool CheckIsInsideGrid(Vector2 pos)
    {
        return ((int)pos.x >= 0 && (int)pos.x < gridWidth && (int)pos.y >= 0);
    }

    public Vector2 Round(Vector2 pos)
    {
        return new Vector2(Mathf.Round(pos.x), Mathf.Round(pos.y));
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

    public void CheckGameOver()
    {
        for (int x = 0; x < gridWidth; ++x)
        {
            if (grid[x, gridHeight - 1] != null) // Check if any block is above the grid
            {
                audioManager.StopMusic();
                audioManager.PlaySFX(audioManager.death);
                Debug.Log("Game Over!");
                TriggerGameOver();
                break;
            }
        }
    }

    void TriggerGameOver()
    {
        // Show the Game Over Canvas
        if (gameOverCanvas != null)
        {
            gameOverCanvas.gameObject.SetActive(true);
            // Stop the game
            Time.timeScale = 0;
        }
        // Stop the game
        Time.timeScale = 0;
    }
}
