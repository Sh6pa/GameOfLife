using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    #region Singleton
    public static InputManager IM;
   
    #endregion

    // Update is called once per frame
    [SerializeField] public Material m_deadMaterial;
    [SerializeField] public Material m_aliveMaterial;

    private void Awake()
    {
        if (IM == null)
        {
            IM = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateCell();
        }
    }
    // Updates the cell when left click pressed, changing mesh and isAlive boolean
    private void UpdateCell()
    {
        if (GridManager.Instance != null)
        {
            Vector3 pos = Input.mousePosition;
            Camera mainCamera = Camera.main;
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(pos);
            var col = (int)mouseWorldPosition.x;
            var row = Mathf.FloorToInt(mouseWorldPosition.y);
            if (col >= 0 && col < GridManager.Instance.m_numCol &&
                row >= 0 && row < GridManager.Instance.m_numRow)
            {
                var cell = GridManager.Instance.m_grid[col, row];
                ChangeCell(cell);
            }
        }
    }

    private void ChangeCell(Cell cell)
    {
        var meshRenderer = cell.GetComponentInChildren<MeshRenderer>();
        if (cell.m_IsAlive)
        {
            meshRenderer.sharedMaterial = m_deadMaterial;
            cell.m_IsAlive = false;
        }
        else
        {
            meshRenderer.sharedMaterial = m_aliveMaterial;
            cell.m_IsAlive = true;
        }
    }
}
