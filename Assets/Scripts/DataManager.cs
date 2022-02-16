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
        string filePath = Application.persistentDataPath + "/Grids";
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
        string filePath = Application.persistentDataPath + "/Grids";
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

    public async Task SaveToPng() 
    {
        int width = GridManager.Instance.m_numCol;
        int height = GridManager.Instance.m_numRow;
        Texture2D tex = new Texture2D(width, height, TextureFormat.RGB24, false);
        for(int i = 0; i < GridManager.Instance.m_numCol; i++) 
        {
            for(int j = 0; j < GridManager.Instance.m_numRow; j++)
            {
                if(GridManager.Instance.m_grid[i, j].m_IsAlive)
                {
                    tex.SetPixel(i, j ,Color.white);
                    tex.Apply();
                } else
                {
                    tex.SetPixel(i, j, Color.black);
                    tex.Apply();
                }
            }
        }
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);
        string filePath = Application.persistentDataPath + "/Grids";
        string path = Path.Combine(filePath, $"{name}.png");
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(bytes, 0, bytes.Length);
        };
    }

    public async Task LoadPng()
    {
        string filePath = Application.persistentDataPath + "/Grids";
        string path = Path.Combine(filePath, $"{name}.png");
        Texture2D tex = new Texture2D(2,2);
        byte[] buffer = new byte[4096];
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.ReadAsync(buffer, 0, buffer.Length);
            tex.LoadImage(buffer);
        };
        if (GridManager.Instance.m_grid != null)
        {
            foreach (var cell in GridManager.Instance.m_grid)
            {
                Destroy(cell.gameObject);
            }
        }
        GridManager.Instance.m_grid = new Cell[tex.width, tex.height];
        GridManager.Instance.ChangeCols(tex.width);
        GridManager.Instance.ChangeRows(tex.height);
        UIManager.UIM.ChangeCols(tex.width);
        UIManager.UIM.ChangeRows(tex.height);
        GridManager.Instance.UpdateCamera();
        for (int i = 0; i < tex.width; i++)
        {
            for (int j = 0; j < tex.height; j++)
            {
                Vector3Int pos = new Vector3Int(i, j, 0);
                Cell clone = Instantiate(GridManager.Instance.m_cellPrefab, pos, Quaternion.identity);
                if (tex.GetPixel(i, j) == Color.white)
                {
                    var meshRenderer = clone.GetComponentInChildren<MeshRenderer>();
                    meshRenderer.sharedMaterial = InputManager.IM.m_aliveMaterial;
                    clone.m_IsAlive = true;
                }
                GridManager.Instance.m_grid[i, j] = clone;
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
