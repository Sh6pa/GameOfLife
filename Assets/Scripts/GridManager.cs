using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    
    
    #region Public

    public Cell[,] m_grid;
    
    [SerializeField] // inspector modification
    [Range(1, 100)]
    public int m_numCol = 10;

    [SerializeField]
    [Range(1, 100)]
    public int m_numRow = 10;

    [SerializeField] public int m_BiggerGridPadding = 15;

    #endregion

    #region Singleton
    public static GridManager Instance;

    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(this);
            Init();
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Show in inspector

    

    [SerializeField]
    public Cell m_cellPrefab;
    #endregion



    public void GenerateGrid()
    {
        if(m_grid != null)
        {
            foreach(var cell in m_grid)
            {
                Destroy(cell.gameObject);
            }
        }
        SetNewGrid();
        UpdateCamera();
    }

    public void ChangeRows(float rows)
    {
        m_numRow = (int) rows;
    }

    public void ChangeCols(float cols)
    {
        m_numCol = (int)cols;
    }

    // sets camera size and position ton fill the screen with current grid
    public void UpdateCamera()
    {
        Camera mainCamera = Camera.main;
        // if ratio width/height is bigger for the grid than the cam,
        // this means width is larger -> size on width calculated by height and ratio
        if (mainCamera.aspect < ((float)m_numCol / (float)m_numRow))
        {
            mainCamera.orthographicSize = m_numCol / mainCamera.aspect * 0.5f;
            // dividing by the ratio width/height to get 
        }
        else // only need on height /2 as it is in the middle
        {
            mainCamera.orthographicSize = (m_numRow * 0.5f);
        }

        mainCamera.transform.position = new Vector3(m_numCol / 2.0f, m_numRow / 2.0f, mainCamera.transform.position.z);
    }

    public void SetNewGridWithSize(int col, int row)
    {
        m_numRow = row;
        m_numCol = col;
        SetNewGrid();
    }

    // creates empty grid with the great size
    public void SetNewGrid()
    {
        m_grid = new Cell[m_numCol, m_numRow];
        GameOfLife.BiggerGrid = new LightCell[m_numCol + (m_BiggerGridPadding*2), m_numRow + (m_BiggerGridPadding*2)];
        FillGrid();
    }



    // make grid appear
    public void ShowGrid(bool show)
    {
        foreach (var cell in m_grid)
        {
            cell.gameObject.SetActive(show);
        }
    }

    // delete previous cell in grid, sets camera & new empty grids (but with good size)
    public void DeleteOldGridAndCreateNewOneWithSize(int col, int row)
    {
        if (m_grid != null)
        {
            foreach (var cell in GridManager.Instance.m_grid)
            {
                Destroy(cell.gameObject);
            }
        }
        SetNewGridWithSize(col, row);
        UIManager.UIM.ChangeCols(col);
        UIManager.UIM.ChangeRows(row);
        UpdateCamera();
    }

    private void Init()
    {
        GenerateGrid();
    }

    // fill visible grid with Cell && bigger grid with LightCell
    private void FillGrid()
    {
        for (int row = 0; row < m_numRow; row++)
        {
            for (int col = 0; col < m_numCol; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);
                // create clone #V2
                Cell clone = Instantiate(m_cellPrefab, pos, Quaternion.identity);
                m_grid[col, row] = clone;
            }
        }
        for (int row = 0; row < m_numRow + (m_BiggerGridPadding * 2); row++)
        {
            for (int col = 0; col < m_numCol + (m_BiggerGridPadding * 2); col++)
            {
                // create clone #light
                LightCell clone = new LightCell();
                GameOfLife.BiggerGrid[col, row] = clone;
            }
        }
    }

    #region Private
    // HEHEHE It's empty!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!!
    // UwU
    #endregion
}
