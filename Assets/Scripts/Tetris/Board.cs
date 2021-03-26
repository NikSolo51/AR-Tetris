using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Random = UnityEngine.Random;
using UnityEngine.EventSystems;
using UnityEngine.Serialization;

public enum CellStates
{
    EMPTY,
    FILL,
    ORIGIN
}

public struct Piece
{
    public CellStates[,] matrixPiece;
    public int x, y;
    public int pieceType;
    public Color _color;


    public Piece(CellStates[,] matrix, int PieceType, Color color, int x, int y)
    {
        this.x = x;
        this.y = y;
        pieceType = PieceType;
        _color = color;
        matrixPiece = matrix;
    }
}

public class Board : MonoBehaviour
{
    public int currentRotation = 0;
    public int speed;
    public CellStates[,] boardMatrix = new CellStates[BOARD_HEIGHT, BOARD_WIDTH];
    public Piece piece;
    public Board3D board3D;
    
    public delegate void Over();
    public static event Over OnGameOver;
    
    public delegate void LineClear();
    public static event LineClear OnClearLine;


    private const int BOARD_WIDTH = 10;
    private const int BOARD_HEIGHT = 20;
    private bool userControl;
    private List<GameObject> notEmptyCells = new List<GameObject>();
    private CellStates[,] boardSave = new CellStates[BOARD_HEIGHT, BOARD_WIDTH];

    private void Start()
    {
        for (int i = 0; i < BOARD_HEIGHT; i++)
        {
            for (int j = 0; j < BOARD_WIDTH; j++)
            {
                boardMatrix[i, j] = CellStates.EMPTY;
            }
        }
        Color color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        PieceInit(Random.Range(0, 2), 0,color, 3, 3);
        SpawnPiece(true);
    }

    private void Update()
    {
        if (userControl)
        {
            if (Input.GetKeyDown(KeyCode.A))
                Shift(-1);
            if (Input.GetKeyDown(KeyCode.D))
                Shift(1);
            
        }

        DrawMatrix();
    }


    void PieceInit(int type, int rotate,Color color, int x, int y)
    {
        CellStates[,] matrix = new CellStates[5, 5];

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                matrix[i, j] = CellStates.EMPTY;
            }
        }
        
        piece = new Piece(matrix, type, color, x, y);


        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                piece.matrixPiece[i, j] = (CellStates) Const.mPieces[type, rotate, i, j];
            }
        }

        piece.pieceType = type;
    }

    void SpawnPiece(bool respawn)
    {
        if (respawn)
        {
            CancelInvoke();
            InvokeRepeating("MovePiece", 1, 1f / speed);
        }

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (piece.matrixPiece[i, j] != CellStates.EMPTY)
                {
                    boardMatrix[piece.x - 2 + i, piece.y - 2 + j] = piece.matrixPiece[i, j];
                }
            }
        }

        userControl = true;
        if (respawn)
            board3D.SpawnPiece();
    }


    void MovePiece()
    {
        if (CheckCollision(1, 0, false))
        {
            StopPiece();
            return;
        }


        ClearPiece();

        piece.x++;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (piece.matrixPiece[i, j] != CellStates.EMPTY)
                {
                    boardMatrix[piece.x - 2 + i, piece.y - 2 + j] = piece.matrixPiece[i, j];
                }
            }
        }

        
    }

    void StopPiece()
    {
        userControl = false;
        for (int i = 0; i < 5; i++)
        {
            if (((int) piece.matrixPiece[i, 0] + (int) piece.matrixPiece[i, 1] + (int) piece.matrixPiece[i, 2]
                 + (int) piece.matrixPiece[i, 3] + (int) piece.matrixPiece[i, 4]) > 0)
            {
                if ((i + piece.x - 2 <= 4))
                {
                    GameOver();
                    return;
                }

                if (LineIsFilled(i + piece.x - 2))
                {
                    ClearLine(i + piece.x - 2);
                    ShiftBoard(i + piece.x - 2);
                }
            }
        }

        currentRotation = 0;
        LoadNewPiece();
    }

    public void Shift(int magnitude)
    {
        if (CheckCollision(0, magnitude, false))
            return;

        ClearPiece();

        if (piece.y + magnitude >= 0)
            piece.y += magnitude;

        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (piece.matrixPiece[i, j] != CellStates.EMPTY)
                {
                    boardMatrix[piece.x - 2 + i, piece.y - 2 + j] = piece.matrixPiece[i, j];
                }
            }
        }

        board3D.Shift3D(magnitude * 1.5f);
    }


    void ClearPiece()
    {
        for (int i = 0; i < 5; i++)
        {
            for (int j = 0; j < 5; j++)
            {
                if (piece.matrixPiece[i, j] != CellStates.EMPTY)
                {
                    boardMatrix[piece.x - 2 + i, piece.y - 2 + j] = CellStates.EMPTY;
                }
            }
        }
    }

    public void RotatePiece()
    {
        if (CheckCollision(0, 0, true))
        {
            return;
        }

        currentRotation = (currentRotation + 1) % 4;
        ClearPiece();

        PieceInit(piece.pieceType, currentRotation,piece._color, piece.x, piece.y);

        SpawnPiece(false);

        board3D.SyncDynaimc();
    }


    void DrawMatrix()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                if (boardMatrix[i, j] != CellStates.EMPTY)
                {
                    DrawBoard.images[i, j].color = Color.blue;
                }
                else
                {
                    DrawBoard.images[i, j].color = Color.white;
                }
            }
        }
    }

    bool CheckCollision(int x, int y, bool rot)
    {
        for (int i = 0; i < 20; i++)
        for (int j = 0; j < 10; j++)
        {
            if (boardMatrix[i, j] != CellStates.EMPTY)
            {
                boardSave = boardMatrix;
            }
        }

        CellStates[,] pieceRotate = new CellStates[5, 5];
        for (int i = 0; i < 5; i++)
        for (int j = 0; j < 5; j++)
            pieceRotate[i, j] = (CellStates) Const.mPieces[piece.pieceType, (currentRotation + 1) % 4, i, j];

        if (rot)
        {
            for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
            {
                if (pieceRotate[i, j] != CellStates.EMPTY)
                {
                    if (piece.x + i - 2 >= 20 || piece.y + j - 2 >= 10 || piece.y + j - 2 < 0)
                    {
                        return true;
                    }

                    if (boardMatrix[piece.x + i - 2, piece.y + j - 2] != CellStates.EMPTY)
                        if (piece.matrixPiece[i, j] == CellStates.EMPTY)
                            return true;
                }
            }
        }
        else
        {
            for (int i = 0; i < 5; i++)
            for (int j = 0; j < 5; j++)
            {
                Vector2 world = new Vector2(piece.x + x - 2 + i, piece.y + y - 2 + j);
                Vector2 local = new Vector2(i + x, j + y);

                if (piece.matrixPiece[i, j] != CellStates.EMPTY)
                {
                    if ((world.x >= 20 || world.y >= 10 || world.y < 0))
                    {
                        boardMatrix = boardSave;
                        return true;
                    }

                    if (boardMatrix[(int) world.x, (int) world.y] != CellStates.EMPTY)
                    {
                        if (local.x < 5 && local.y < 5)
                        {
                            if (piece.matrixPiece[(int) local.x, (int) local.y] == CellStates.EMPTY)
                            {
                                boardMatrix = boardSave;
                                return true;
                            }
                        }
                        else
                        {
                            boardMatrix = boardSave;
                            return true;
                        }
                    }
                }
            }
        }

        for (int b = 0; b < 20; b++)
        for (int n = 0; n < 10; n++)
            if (boardSave[b, n] != CellStates.EMPTY)
            {
                boardMatrix = boardSave;
            }

        return false;
    }

    void ClearLine(int line)
    {
        for (int j = 0; j < 10; j++)
        {
            boardMatrix[line, j] = CellStates.EMPTY;
        }
        OnClearLine?.Invoke();
        board3D.ClearLine3D(line);
    }

    bool LineIsFilled(int line)
    {
        for (int i = 0; i < 10; i++)
        {
            if (boardMatrix[line, i] == CellStates.EMPTY)
                return false;
        }

        return true;
    }

    void LoadNewPiece()
    {
        Color color = UnityEngine.Random.ColorHSV(0f, 1f, 1f, 1f, 0.5f, 1f);
        PieceInit(Random.Range(0, 6), 0,color, 3, 3);
        SpawnPiece(true);
    }

    void ShiftBoard(int line)
    {
        for (int i = line - 1; i >= 0; i--)
        for (int j = 0; j < 10; j++)
        {
            boardMatrix[i + 1, j] = boardMatrix[i, j];
        }
    }

    void GameOver()
    {
        for (int i = 0; i < 20; i++)
        for (int j = 0; j < 10; j++)
            if (boardMatrix[i, j] != CellStates.EMPTY)
                notEmptyCells.Add(board3D.cubes[i, j]);
        
        
        ExplosionBoard();
        
        OnGameOver?.Invoke();
        
        Destroy(board3D.currentPiece);
        Destroy(this.gameObject);
    }

    public void ExplosionBoard()
    {
        Rigidbody rb;

        for (int j = 0; j < notEmptyCells.Count; j++)
        {
            float randomForce = Random.Range(10, 100);
            randomForce = randomForce * board3D.origin.transform.localScale.x;
            int direction = Random.Range(0, 2);

            rb = notEmptyCells[j].GetComponent<Rigidbody>();
            rb.isKinematic = false;
            rb.useGravity = true;

            if (direction == 0)
                rb.AddForce(
                    Vector3.left / 4 * randomForce + -Vector3.forward * randomForce + Vector3.up * randomForce / 3,
                    ForceMode.Impulse);
            else
                rb.AddForce(
                    Vector3.right / 4 * randomForce + -Vector3.forward * randomForce + Vector3.up * randomForce / 3,
                    ForceMode.Impulse);
        }
    }
}