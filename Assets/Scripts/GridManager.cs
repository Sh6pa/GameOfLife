using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GridManager : MonoBehaviour
{
    
    
    
    #region Public

    public GameObject[,] m_grid;

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

    [SerializeField] // inspector modification
    [Range(1, 100)]
    private int _numCol = 10;

    [SerializeField]
    [Range(1, 100)]
    private int _numRow = 10;

    [SerializeField]
    private GameObject _cellPrefab;
    #endregion

    private void Init()
    {
        GenerateGrid();
    }

    private void GenerateGrid()
    {
        m_grid = new GameObject[_numCol, _numRow];
        for (int row = 0; row < _numRow; row++)
        {
            for (int col = 0; col < _numCol; col++)
            {
                Vector3Int pos = new Vector3Int(col, row, 0);
                
                // useless
                //var go = new GameObject();
                //go.name = $"{col},{row}";
                //var mf = go.AddComponent<MeshFilter>();
                //var mr = go.AddComponent<MeshRenderer>();
                

                // create clone #V2
                GameObject clone = Instantiate(_cellPrefab, pos, Quaternion.identity);
                m_grid[col, row] = clone;
            }
        }
    }

    // Start is called before the first frame update
    private void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    #region Private


    #endregion
}
