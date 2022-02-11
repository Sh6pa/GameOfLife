using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameOfLife : MonoBehaviour
{
    public float _stepDelay;
    void Update()
    {
        _counter += Time.deltaTime;

        if (_counter > _stepDelay)
        {
            SimulationStep();
            _counter = 0;
        }
    }

    private void SimulationStep()
    {
        for(int i = 0; i < GridManager.Instance.m_numCol; i++)
        {
            for(int j = 0; j < GridManager.Instance.m_numRow; j++)
            {
                int n = CountNeighbors(i, j);
                //Debug.Log(n);
                var cell = GridManager.Instance.m_grid[i, j];
                changeMesh(cell, n);               
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
                
                if (i < GridManager.Instance.m_numCol && i >= 0 && j < GridManager.Instance.m_numRow && j >= 0 && (i != col && j != row))
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

    private void changeMesh(Cell cell, int n)
    {
        Debug.Log($"n={n}");
        var meshRenderer = cell.GetComponentInChildren<MeshRenderer>();
        if (n == 3)
        {
            Debug.Log("222222");
            meshRenderer.sharedMaterial = InputManager.IM.m_aliveMaterial;
            cell.m_IsAlive = true;
            Debug.Log("3");
        }
        else if (n < 2 || n > 3)
        {
            Debug.Log("333333");
            meshRenderer.sharedMaterial = InputManager.IM.m_deadMaterial;
            cell.m_IsAlive = false;
            Debug.Log("2");
        }
    } 

    private float _counter;
}
