using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
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

    private void SimulationStep()
    {
        countNeighbours();
        updateCellMesh();
    }


    private void countNeighbours()
    {
        for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
        {
            for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
            {
                int n = 0;
                if (m_ruleOfNeighbour == RuleOfNeighbour.Closed)
                {
                    n = CountNeighborsCloseGrid(i, j);
                } else if (m_ruleOfNeighbour == RuleOfNeighbour.Symetrical)
                {
                    n = CountNeighborsSymetricalGrid(i, j);
                } else if (m_ruleOfNeighbour == RuleOfNeighbour.Infinite)
                {
                    n = CountNeighborsCloseGrid(i, j);
                }
                
                var cell = GridManager.Instance.m_grid[i, j];
                cell.m_Neighbors = n;

            }
        }
    }

    private void updateCellMesh()
    {
        for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
        {
            for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
            {
                var cell = GridManager.Instance.m_grid[i, j];
                changeMesh(cell);
            }
        }
    }

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

    private int CountNeighborsSymetrical(int col, int row)
    {
        int n = 0;
        for (int i = col - 1; i <= col + 1; i++)
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

    private void changeMesh(Cell cell)
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

    private float _counter;
}
