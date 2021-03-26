using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Board3D : MonoBehaviour
{
    public GameObject[,] cubes = new GameObject[20, 10];
    public GameObject prefab;
    public GameObject origin;
    [HideInInspector] public GameObject currentPiece;
    public Board board;
    public List<GameObject> tetrinoList = new List<GameObject>();
    public Explosion explosion;

    private Renderer _renderer;


    void Awake()
    {
        for (int i = 0; i < 20; i++)
        {
            for (int j = 0; j < 10; j++)
            {
                GameObject a = Instantiate(prefab, origin.transform);
                a.transform.localScale = new Vector3(1, 1, 1);
                a.transform.localPosition += new Vector3(j, -i, 0) * 1.5f;
                cubes[i, j] = a;
            }
        }
    }

    private void FixedUpdate()
    {
        if (currentPiece)
            currentPiece.transform.Translate((-Vector3.up * 1.5f / 50) * board.speed * origin.transform.localScale.x,
                Space.World);
    }

    private void Update()
    {
        RenderStatic();
    }

    void RenderStatic()
    {
        Color color = new Color(0, 0, 0, 0);
        for (int i = 0; i < 20; i++)
        for (int j = 0; j < 10; j++)
        {
            _renderer = cubes[i, j].GetComponent<Renderer>();
            if (board.boardMatrix[i, j] != CellStates.EMPTY)
            {
                _renderer.material.color = Color.blue;

                if (i + 2 - board.piece.x < 5 && i + 2 - board.piece.x >= 0)
                    if (j + 2 - board.piece.y < 5 && j + 2 - board.piece.y >= 0)
                        if (board.piece.matrixPiece[i + 2 - board.piece.x, j + 2 - board.piece.y] != CellStates.EMPTY)
                            _renderer.material.color = color;
            }
            else
                _renderer.material.color = color;
        }
            
    }

    public void SyncDynaimc()
    {
        if (board.piece.pieceType == 0)
            return;
        if (board.piece.pieceType == 1)
            currentPiece.transform.localRotation = Quaternion.Euler(0, 0, 90 * board.currentRotation);
        else
            currentPiece.transform.localRotation = Quaternion.Euler(0, 0, -90 * board.currentRotation);
    }

    public void SpawnPiece()
    {
        Vector3 offset = new Vector3(0, 3f, 0);
        Destroy(currentPiece);
        currentPiece = Instantiate(tetrinoList[board.piece.pieceType], origin.transform);
        currentPiece.transform.localScale = new Vector3(1, 1, 1);
        currentPiece.transform.localPosition = cubes[board.piece.x, board.piece.y].transform.localPosition + offset;
        
        RecolorTetrino();
        SyncDynaimc();
    }

    void RecolorTetrino()
    {
        Renderer r;
        for (int i = 0; i < currentPiece.transform.childCount; i++)
        {
            r = currentPiece.transform.GetChild(i).GetComponent<Renderer>();
            r.material.color = board.piece._color;
        }
    }

    public void Shift3D(float stride)
    {
        currentPiece.transform.localPosition += new Vector3(stride, 0, 0);
    }

    public void ClearLine3D(int line)
    {
        List<GameObject> listExp = new List<GameObject>();
        for (int i = 0; i < 10; i++)
        {
            listExp.Add(cubes[line, i]);
        }

        explosion.ExplodeObjects(listExp);
    }
}