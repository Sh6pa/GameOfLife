using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    #region Singleton
    // public static Dictionary<string, bool> BiggerGrid;
    public static LightCell[,] BiggerGrid;
    #endregion

    #region Public
    [SerializeField] public float m_stepDelay = 1;
    [SerializeField] public bool m_play = false;
    

    [System.Serializable]
    public enum RuleOfNeighbour
    {
        Closed,
        Symetrical,
        Infinite
    };

    // Sets up the algorithm of choice
    public RuleOfNeighbour m_ruleOfNeighbour;
    #endregion
    // Handle the game of life simulation
    void Update()
    {
        if (m_play)
        {
            _counter += Time.deltaTime;
            float step = 1 / (m_stepDelay * 0.001f);
            if (_counter > step)
            {
                SimulationStep();
                _counter = 0;
            }
        }
       
    }

    public void ChangeSpeed(float delay)
    {
        m_stepDelay = (int)delay;
    }

    public void Play()
    {
        m_play = true;
    }

    public void Pause()
    {
        m_play = false;
    }

    public void Step()
    {
        m_play = false;
        SimulationStep();

    }
    // always check neighboors of all grid and then update the status if needed
    private void SimulationStep()
    {
        CountNeighbours();
        UpdateCellMesh();
    }

    // synchronises the "visible" cells on the invible bigger grid ton endure in-game click & algorithm change
    public void SynchronizeGridsOnVisibleOne()
    {
        for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
        {
            for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
            {
                BiggerGrid[i+GridManager.Instance.m_BiggerGridPadding, j+GridManager.Instance.m_BiggerGridPadding].m_IsAlive = GridManager.Instance.m_grid[i, j].m_IsAlive;
            }
        }
    }

    // Sets up the differents options of algorithms for ui Dropdown
    public List<string> GetOptions()
    {
        List<string> optionList = new List<string>();
        optionList.Add("Closed");
        optionList.Add("Symetrical");
        optionList.Add("Infinite");
        return optionList;
    }


    public void ChangeAlgo(string algo)
    {
        if (algo == "Closed")
        {
            m_ruleOfNeighbour = RuleOfNeighbour.Closed;
        } else if(algo == "Symetrical")
        {
            m_ruleOfNeighbour = RuleOfNeighbour.Symetrical;
        }
        else
        {
            m_ruleOfNeighbour = RuleOfNeighbour.Infinite;
        }
    }

    public void Quit()
    {
        Application.Quit();
    }

    private void CountNeighbours()
    {
        // syncronize the visible part and then handle the neighboors of the bigger grid to look infinite
        if (m_ruleOfNeighbour == RuleOfNeighbour.Infinite)
        {
            SynchronizeGridsOnVisibleOne();
            for (int i = 0; i < BiggerGrid.GetLength(0); i++)
            {
                for (int j = 0; j < BiggerGrid.GetLength(1); j++)
                {
                    int n = CountNeighborsBiggerGrid(i, j);
                    BiggerGrid[i, j].m_Neighbors = n;
                }
            }
        } else
        {
            for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
            {
                for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
                {
                    int n = 0;
                    // handles similar algorythm change
                    if (m_ruleOfNeighbour == RuleOfNeighbour.Closed)
                    {
                        n = CountNeighborsCloseGrid(i, j);
                    }
                    else if (m_ruleOfNeighbour == RuleOfNeighbour.Symetrical)
                    {
                        n = CountNeighborsSymetricalGrid(i, j);
                    }

                    var cell = GridManager.Instance.m_grid[i, j];
                    cell.m_Neighbors = n;

                }
            }
        }
        
    }

    private void UpdateCellMesh()
    {
       
        if (m_ruleOfNeighbour != RuleOfNeighbour.Infinite)
        {
            for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
            {
                for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
                {
                    var cell = GridManager.Instance.m_grid[i, j];
                    // apply the game of life neighboor rule
                    ChangeMesh(cell);
                }
            }
        } else
        {
            for (int i = 0; i < BiggerGrid.GetLength(0); i++)
            {
                for (int j = 0; j < BiggerGrid.GetLength(1); j++)
                {
                    // apply the game of life neighboor rule to bigger grid
                    UpdateBigGrid(i, j);
                }
            }
            for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
            {
                for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
                {
                    // apply results on visible grid
                    var cell = GridManager.Instance.m_grid[i, j];
                    UpdateCellOnBiggerGrid(i, j, cell);
                }
            }
        }
    }

    // game of life rule for bigger grid (infinite mode)
    private void UpdateBigGrid(int col, int row)
    {
        if (BiggerGrid[col, row].m_Neighbors == 3)
        { 
            BiggerGrid[col, row].m_IsAlive = true;
        }
        else if (BiggerGrid[col, row].m_Neighbors < 2 || BiggerGrid[col, row].m_Neighbors > 3)
        {
            BiggerGrid[col, row].m_IsAlive = false;
        }
    }
    
    // counts the limits of grids as dead but is bigger than visible grid => gives inpression of infinity
    private int CountNeighborsBiggerGrid(int col, int row)
    {
        int n = 0;
        for (int i = col - 1; i <= col + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {

                if (i < BiggerGrid.GetLength(0) && i >= 0 && j < BiggerGrid.GetLength(1) && j >= 0 && !(i == col && j == row)) // (i != col && j != row)
                {
                    var cell = BiggerGrid[i, j];
                    if (cell.m_IsAlive)
                    {
                        n++;
                    }
                }
            }
        }
        return n;
    }

    // counts the limits of grid as dead cell
    private int CountNeighborsCloseGrid(int col, int row)
    {
        int n = 0;
        for(int i = col - 1; i <= col + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {
                
                if (i < GridManager.Instance.m_grid.GetLength(0) && i >= 0 && j < GridManager.Instance.m_grid.GetLength(1) && j >= 0 && !(i == col && j == row)) // (i != col && j != row)
                {
                    var cell = GridManager.Instance.m_grid[i, j];
                    if (cell.m_IsAlive)
                    {
                        n++;
                    }
                }
            }
        }
        return n;
    }

    // joins with modulo opposite borders of grid to make glider run infinitely on screen for example
    private int CountNeighborsSymetricalGrid(int col, int row)
    {
        int n = 0;
        for (int i = col - 1; i <= col + 1; i++)
        {
            for (int j = row - 1; j <= row + 1; j++)
            {

                if (!(i == col && j == row)) // (i != col && j != row)
                {
                    
                    var cell = GridManager.Instance.m_grid[(i + GridManager.Instance.m_grid.GetLength(0)) % GridManager.Instance.m_grid.GetLength(0), (j + GridManager.Instance.m_grid.GetLength(1)) % GridManager.Instance.m_grid.GetLength(1)];
                    if (cell.m_IsAlive)
                    {
                        n++;
                    }
                }
            }
        }
        return n;
    }

    // applies on non infinite algorithm rule of the game & mesh changes if necessary
    private void ChangeMesh(Cell cell)
    {
        var meshRenderer = cell.GetComponentInChildren<MeshRenderer>();
        

        if (cell.m_Neighbors == 3)
        {
            meshRenderer.sharedMaterial = InputManager.IM.m_aliveMaterial;
            cell.m_IsAlive = true;

        }
        else if (cell.m_Neighbors < 2 || cell.m_Neighbors > 3)
        {
            meshRenderer.sharedMaterial = InputManager.IM.m_deadMaterial;
            cell.m_IsAlive = false;
        }
    }
    
    // applis mesh and status change on cell based on bigger grid
    private void UpdateCellOnBiggerGrid(int col, int row, Cell cell)
    {
        var meshRenderer = cell.GetComponentInChildren<MeshRenderer>();
        var bigGridCell = BiggerGrid[col + GridManager.Instance.m_BiggerGridPadding, row + GridManager.Instance.m_BiggerGridPadding];
        if (cell.m_IsAlive != bigGridCell.m_IsAlive)
        {
            cell.m_IsAlive = bigGridCell.m_IsAlive;
            meshRenderer.sharedMaterial = (bigGridCell.m_IsAlive) ? InputManager.IM.m_aliveMaterial : InputManager.IM.m_deadMaterial;
        }
       
        
    }

    #region Private
    private float _counter;
    // feeling quite lonely in this private area
    #endregion
}
