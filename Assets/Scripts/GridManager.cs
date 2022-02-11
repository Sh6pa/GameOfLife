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
    private Cell _cellPrefab;
    #endregion

    private void Init()
    {
        GenerateGrid();
    }

    public void GenerateGrid()
    {
        if(m_grid != null)
        {
            foreach(var cell in m_grid)
            {
                Destroy(cell.gameObject);
            }
        }
        m_grid = new Cell[m_numCol, m_numRow];
        for (int row = 0; row < m_numRow; row++)
        {
            for (int col = 0; col < m_numCol; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);

                // useless
                //var go = new GameObject();
                //go.name = $"{col},{row}";
                //var mf = go.AddComponent<MeshFilter>();
                //var mr = go.AddComponent<MeshRenderer>();


                // create clone #V2
                Cell clone = Instantiate(_cellPrefab, pos, Quaternion.identity);
                m_grid[col, row] = clone;
            }
        }
    }

    public void ChangeRows(float rows)
    {
        m_numRow = (int) rows;
    }

    public void ChangeCols(float cols)
    {
        m_numCol = (int)cols;
    }

    #region Private


    #endregion
}
