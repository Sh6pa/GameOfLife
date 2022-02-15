using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    private string name = "Grid";

    private string SetData()
    {
        Grid getClassData  = new Grid();
        getClassData.m_name = name;
        getClassData.m_cols = GridManager.Instance.m_numCol;
        getClassData.m_rows = GridManager.Instance.m_numRow;
        getClassData.m_Cells = new string[getClassData.m_cols * getClassData.m_rows];
        int indexTracker = 0;
        for (int i = 0; i<getClassData.m_cols; i++)
        {
            for(int j = 0; j<getClassData.m_rows; j++)
            {
                getClassData.m_Cells[indexTracker] = GridManager.Instance.m_grid[i, j].m_IsAlive.ToString();
                indexTracker++;
            }
        }
        return JsonUtility.ToJson(getClassData);
    }

    public async Task SaveToJson()
    {
        string json = SetData(); ;
        string filePath = UnityEngine.Application.persistentDataPath + "/Grids";
        byte[] encodedText = Encoding.UTF8.GetBytes(json);
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }
        string path = Path.Combine(filePath, $"{name}.json");
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    public async Task LoadJson()
    {
        string filePath = UnityEngine.Application.persistentDataPath + "/Grids";
        string path = Path.Combine(filePath, $"{name}.json");
        using var sourceStream =
        new FileStream(
            path,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        var sb = new StringBuilder();

        byte[] buffer = new byte[0x1000];
        int numRead;
        while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
        {
            string text = Encoding.UTF8.GetString(buffer, 0, numRead);
            sb.Append(text);
        }
        Grid grid = JsonUtility.FromJson<Grid>(sb.ToString());
        
        if (GridManager.Instance.m_grid != null)
        {
            foreach (var cell in GridManager.Instance.m_grid)
            {
                Destroy(cell.gameObject);
            }
        }
        GridManager.Instance.m_grid = new Cell[grid.m_cols, grid.m_rows];
        GridManager.Instance.ChangeCols(grid.m_cols);
        GridManager.Instance.ChangeRows(grid.m_rows);
        UIManager.UIM.ChangeCols(grid.m_cols);
        UIManager.UIM.ChangeRows(grid.m_rows);
        GridManager.Instance.UpdateCamera();

        int indexTracker = 0;
        for (int i = 0; i < grid.m_cols; i++)
        {
            for (int j = 0; j < grid.m_rows; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                Cell clone = Instantiate(GridManager.Instance.m_cellPrefab, pos, Quaternion.identity);
                if(grid.m_Cells[indexTracker].Equals("True"))
                {
                    var meshRenderer = clone.GetComponentInChildren<MeshRenderer>();
                    meshRenderer.sharedMaterial = InputManager.IM.m_aliveMaterial;
                    clone.m_IsAlive = true;
                }
                GridManager.Instance.m_grid[i, j] = clone;
                indexTracker++;
            }
        }
    }

}

[System.Serializable]
public class Grid
{
    public string m_name;
    public int m_rows;
    public int m_cols;
    public string[] m_Cells;
}
