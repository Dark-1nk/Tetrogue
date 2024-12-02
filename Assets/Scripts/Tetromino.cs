using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    public float fallSpeed = 1;
    public bool allowRotation = true;
    public bool limitRotation = false;

    [SerializeField]
    public LayerMask blockLayer;
    public LayerMask playerLayer;

    private AudioManager audioManager;

    public bool isLocked;

    void Start()
    {
        audioManager = GameObject.FindGameObjectWithTag("Audio").GetComponent<AudioManager>();
        // Adjust Tetromino position if spawn location is blocked
        AdjustSpawnPosition();
    }

    void Update()
    {
        // If the Tetromino is locked, allow toggling the lock state
        if (isLocked)
        {
            if (Input.GetKeyDown(KeyCode.C)) // Unlock with 'C'
            {
                isLocked = false;
                Debug.Log("Piece unlocked.");
            }
            return; // Skip all other input while locked
        }

        // If not locked, allow all movement and rotation
        if (!isLocked)
        {
            if (Input.GetKeyDown(KeyCode.C)) // Lock with 'C'
            {
                isLocked = true;
                Debug.Log("Piece locked.");
                return; // Ensure no movement happens during the locking frame
            }

            CheckUserInput();
        }
    }

    void CheckUserInput()
    {
        if (Input.GetKeyDown(KeyCode.RightArrow))
        {
            transform.position += new Vector3(1, 0, 0);

            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(-1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.LeftArrow))
        {
            transform.position += new Vector3(-1, 0, 0);

            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(1, 0, 0);
            }
        }
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            RotateTetromino();
        }
        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            MoveDown();
        }
        else if (Input.GetKeyDown(KeyCode.Space)) // Hard drop
        {
            audioManager.PlaySFX(audioManager.hardDrop);
            HardDrop();
        }
    }

    void RotateTetromino()
    {
        if (allowRotation)
        {
            if (limitRotation)
            {
                if (transform.rotation.eulerAngles.z >= 90)
                {
                    transform.Rotate(0, 0, -90);
                }
                else
                {
                    transform.Rotate(0, 0, 90);
                }
            }
            else
            {
                transform.Rotate(0, 0, 90);
            }

            // Push Tetromino inside the grid if necessary
            if (!CheckIsValidPosition())
            {
                AdjustPositionForRotation();
            }

            FindObjectOfType<Game>().UpdateGrid(this);
        }
    }

    void AdjustPositionForRotation()
    {
        for (int i = 0; i < 2; i++) // Check for left and right adjustments
        {
            transform.position += new Vector3(i == 0 ? 1 : -2, 0, 0);
            if (CheckIsValidPosition())
            {
                return;
            }
            transform.position += new Vector3(i == 0 ? -1 : 2, 0, 0);
        }
        // If still invalid, undo rotation
        transform.Rotate(0, 0, -90);
    }

    void MoveDown()
    {
        transform.position += new Vector3(0, -1, 0);

        if (CheckIsValidPosition())
        {
            FindObjectOfType<Game>().UpdateGrid(this);
        }
        else
        {
            audioManager.PlaySFX(audioManager.placeBlock);
            transform.position += new Vector3(0, 1, 0);
            FindObjectOfType<Game>().DeleteRow();
            enabled = false;
            FindObjectOfType<Game>().CheckGameOver();
            FindObjectOfType<Game>().SpawnNextTetromino();
        }

        fall = Time.time;
    }

    void HardDrop()
    {
        while (CheckIsValidPosition())
        {
            transform.position += new Vector3(0, -1, 0);
        }

        // Step back up to the last valid position
        transform.position += new Vector3(0, 1, 0);

        // Update the grid and lock the Tetromino in place
        FindObjectOfType<Game>().UpdateGrid(this);
        FindObjectOfType<Game>().DeleteRow();
        enabled = false;
        FindObjectOfType<Game>().CheckGameOver();
        FindObjectOfType<Game>().SpawnNextTetromino();
    }

    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round(mino.position);
            if (!FindObjectOfType<Game>().CheckIsInsideGrid(pos))
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
            // Check if the position is occupied by a pre-placed block
            Transform blockAtPos = FindObjectOfType<Game>().GetTransformAtGridPosition(pos);
            if (blockAtPos != null && blockAtPos.CompareTag("PreplacedBlock")) // Check the tag
            {
                return false; // Invalid position due to overlap with pre-placed block
            }
        }
        return true;
    }

    void AdjustSpawnPosition()
    {
        while (!CheckIsValidPosition())
        {
            transform.position += new Vector3(0, 1, 0); // Move upward until valid
        }
    }
}
