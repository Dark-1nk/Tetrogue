using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Tetromino : MonoBehaviour
{
    float fall = 0;
    public float fallSpeed = 1;
    public bool allowRotation = true;
    public bool limitRotation = false;
    public LayerMask blockLayer;

    bool isLocked;
    void Start()
    {

    }
    void Update()
    {
        

        if (isLocked)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isLocked = false;
            }
            return;
        }

        if (!isLocked)
        {
            if (Input.GetKeyDown(KeyCode.C))
            {
                isLocked = true;
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
        //this is a little bit convoluted, but this entire condition determines how a piece will rotate.
        else if (Input.GetKeyDown(KeyCode.UpArrow))
        {
            //if rotation is even allowed (in other words, if it's not an O tetromino) 
            if (allowRotation)
            {
                //if rotation is allowed, but not only with two positions, (like the Z, S, and the I tetromino)
                if (limitRotation)
                {
                    //if the rotation is already 90 degrees
                    if (transform.rotation.eulerAngles.z >= 90)
                    {
                        //rotate the piece backwards instead of forward
                        transform.Rotate(0, 0, -90);
                    }
                    //if the rotation is NOT already 90 degrees
                    else
                    {
                        //rotate the piece forward 90 degrees
                        transform.Rotate(0, 0, 90);
                    }
                }

                //if limit rotation is not active (Like on an L, J, or T tetromino that has 4 different positions)
                else
                {
                    //it will always rotate forward 90 degrees if it has room to do so
                    transform.Rotate(0, 0, 90);
                }

                //if you are in the grid
                if (CheckIsValidPosition())
                {
                    FindObjectOfType<Game>().UpdateGrid(this);
                }
                //if the piece's next movement will put it outside of the grid
                else
                {
                    //if the rotation is limited
                    if (limitRotation)
                    {
                        //if the rotation was already at 90 degrees before inputing the rotation key
                        if (transform.rotation.eulerAngles.x >= 90)
                        {
                            //undo the attempted action 
                            transform.Rotate(0, 0, -90);
                        }
                        //if the rotation was not at 90 degrees, or in other words, going to be rotated -90 degrees
                        else
                        {
                            //undo the attempted action
                            transform.Rotate(0, 0, 90);
                        }
                    }

                    //if the rotation is not limited
                    else
                    {
                        //put the piece back in its previous state, undoing the action. 
                        transform.Rotate(0, 0, -90);
                    }
                }
            }
        }

        else if (Input.GetKeyDown(KeyCode.DownArrow) || Time.time - fall >= fallSpeed)
        {
            transform.position += new Vector3(0, -1, 0);

            if (CheckIsValidPosition())
            {
                FindObjectOfType<Game>().UpdateGrid(this);
            }
            else
            {
                transform.position += new Vector3(0, 1, 0);
                FindObjectOfType<Game>().DeleteRow();

                enabled = false;

                FindObjectOfType<Game>().SpawnNextTetromino();
            }

            fall = Time.time;
        }
    }


    /// <summary>
    /// This function essentially makes it so that the "game" script, which contains the grid dimensions, is asked whether or not the minos in the tetromino are all inside of the grid. This is used in the movement of the tetrominos to check if the desired movement is possible. 
    /// In other words, its an inverted box collider for the grid.
    /// </summary>
    /// <returns></returns>
    bool CheckIsValidPosition()
    {
        foreach (Transform mino in transform)
        {
            Vector2 pos = FindObjectOfType<Game>().Round (mino.position);
            if (FindObjectOfType<Game>().CheckIsInsideGrid(pos) == false)
            {
                return false;
            }

            if (FindObjectOfType<Game>().GetTransformAtGridPosition(pos) != null && FindObjectOfType<Game>().GetTransformAtGridPosition(pos).parent != transform)
            {
                return false;
            }
        }
        return true;
    }
}
