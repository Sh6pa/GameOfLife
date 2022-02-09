using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    [SerializeField] private GridManager _gridManager;
    // Update is called once per frame

    private void Awake()
    {
        // Ancienne manière de recherche
        //Object obj = FindObjectOfType(typeof(GridManager));
        //_gridManager = (GridManager)obj;

        // Pour les valeurs nullables
        // _gridManager = obj as GridManager;

        // Nouvelle façon
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
            Vector3 pos = Input.mousePosition;
            Camera mainCamera = Camera.main;
            Vector3 mouseWorldPosition = mainCamera.ScreenToWorldPoint(pos);
            // Mathf.Clamp();



        }
        if (Input.GetMouseButton(1))
        {

        } else if (Input.GetMouseButton(2))
        {

        }
    }
}
