using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    [SerializeField] public float _stepDelay = 1;
    void Update()
    {
        _counter += Time.deltaTime;
        float step = 1 / (_stepDelay * 0.1f);
        if (_counter > step)
        {
            SimulationStep();
            _counter = 0;
        }
    }

    public void ChangeSpeed(float delay)
    {
        _stepDelay = (int)delay;
    }

    private void SimulationStep()
    {
        for(int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
        {
            for(int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
            {
                int n = CountNeighbors(i, j);
                var cell = GridManager.Instance.m_grid[i, j];
                cell.m_Neighbors = n;
                              
            }
        }
        for (int i = 0; i < GridManager.Instance.m_grid.GetLength(0); i++)
        {
            for (int j = 0; j < GridManager.Instance.m_grid.GetLength(1); j++)
            {
                var cell = GridManager.Instance.m_grid[i, j];
                changeMesh(cell);
            }
        }
    }

    private int CountNeighbors(int col, int row)
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
