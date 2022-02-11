using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    // Update is called once per frame
    [SerializeField] private Material _deadMaterial;
    [SerializeField] private Material _aliveMaterial;

    private void Awake()
    {
        // Ancienne mani�re de recherche
        //Object obj = FindObjectOfType(typeof(GridManager));
        //_gridManager = (GridManager)obj;

        // Pour les valeurs nullables
        // _gridManager = obj as GridManager;

        // Nouvelle fa�on
        _gridManager = FindObjectOfType< GridManager >();

        // Tous les composants
        // FindObjectOfType < GridManager >();

        //tag 
        // GameObject toto = GameObject.FindGameObjectWithTag("toto");
        // GameObject.FindGameObjectWithTag("toto");
        // Transform tr = toto.getComponent<Transform>();

    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            UpdateCell();
           
        }
        if (Input.GetMouseButton(1))
        {

        } else if (Input.GetMouseButton(2))
        {

        }
    }

    private void UpdateCell()
    {
        Vector3 pos = Input.mousePosition;
        Camera mainCamera = Camera.main;
        Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(pos);
        // Mathf.Clamp();
        var col = (int)mouseWorldPosition.x;
        var row = Mathf.FloorToInt(mouseWorldPosition.y);
        if (col >= 0 && col < GridManager.Instance.m_numCol &&
            row >= 0 && row < GridManager.Instance.m_numRow)
        {
            var cell = GridManager.Instance.m_grid[col, row];
            //var cell = cellGO.GetComponent<Cell>();
            //var meshRenderer = cellGO.GetComponentInChildren<MeshRenderer>();

            ChangeCell(cell);
        }
    }
    private void ChangeCell(Cell cell)
    {
        var meshRenderer = cell.GetComponentInChildren<MeshRenderer>();
        if (cell.m_IsAlive)
        {
            meshRenderer.sharedMaterial = _deadMaterial;
            cell.m_IsAlive = false;
        }
        else
        {
            meshRenderer.sharedMaterial = _aliveMaterial;
            cell.m_IsAlive = true;
        }
    }
}
