using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
using System.Threading.Tasks;
using UnityEngine;

public class DataManager : MonoBehaviour
{
    public async Task SaveToJson(string name)
    {
        Grid gridData = SetData();
        string json = null;
        byte[] encodedText = null;
        string filePath = Application.persistentDataPath + "/Grids";
        
        // multithreading to encode to json
        Task jsonEncoded = Task.Run(() =>
        {
           json = JsonUtility.ToJson(gridData);
           encodedText = Encoding.UTF8.GetBytes(json);
        });
        await Task.WhenAll(jsonEncoded);

        // writes the json file
        string path = Path.Combine(filePath, $"{name}.json");
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    public async Task LoadJson(string name)
    {
        string filePath = Application.persistentDataPath + "/Grids";
        
        string path = Path.Combine(filePath, $"{name}.json");
        // get file info
        using var sourceStream =
        new FileStream(
            path,
            FileMode.Open, FileAccess.Read, FileShare.Read,
            bufferSize: 4096, useAsync: true);
        
        var sb = new StringBuilder();
        byte[] buffer = new byte[0x1000];
        int numRead;
        string text = null;
        // multithreading
        Task byteConversion = Task.Run(async () =>
        {
            while ((numRead = await sourceStream.ReadAsync(buffer, 0, buffer.Length)) != 0)
            {
                text = Encoding.UTF8.GetString(buffer, 0, numRead);
                sb.Append(text);
            }
        });
        await Task.WhenAll(byteConversion);
        
        Grid grid = JsonUtility.FromJson<Grid>(sb.ToString());

        // destroys old cells, etc...
        GridManager.Instance.DeleteOldGridAndCreateNewOneWithSize(grid.m_cols, grid.m_rows);

        // fill current grid with Cells
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

    public async Task SaveToPng(string name) 
    {
        int width = GridManager.Instance.m_numCol;
        int height = GridManager.Instance.m_numRow;
        // creates texture the size of the current grid with white px => alive and black px => dead
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
        // encoding to png and add file
        byte[] bytes = tex.EncodeToPNG();
        Object.Destroy(tex);
        string filePath = Application.persistentDataPath + "/Grids";
        string path = Path.Combine(filePath, $"{name}.png");
        // write png file async
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Create, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(bytes, 0, bytes.Length);
        };
    }

    public async Task LoadPng(string name)
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

        // delete previous cell in grid, sets camera with new empty grid (but with good size)
        GridManager.Instance.DeleteOldGridAndCreateNewOneWithSize(tex.width, tex.height);

        // fill the grid with texture info
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

    // give the list of saved json file for dropdown
    public List<string> GetJson()
    {
        
        string path = Application.persistentDataPath + "/Grids";
       
        string [] files = Directory.GetFiles(path, "*.json*");
        List<string> f = new List<string>();
        f.Add("Select Json");
        foreach(string file in files)
        {
            f.Add(file.Substring(path.Length+1));
        }
        return f;

    }

    // give the list of saved png file for dropdown
    public List<string> GetPng()
    {
        string path = Application.persistentDataPath + "/Grids";
        
        string [] files = Directory.GetFiles(path, "*.png*");
        List<string> f = new List<string>();
        f.Add("Select Png");
        foreach(string file in files)
        {
            f.Add(file.Substring(path.Length+1));
        }
        return f;
    }

    // check if the folder location exists and creates it
    private void Start()
    {
        
        string filePath = Application.persistentDataPath + "/Grids";
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }
    }

    // get all the data ton be JSon friendly
    private Grid SetData()
    {
        Grid getClassData = new Grid();
        getClassData.m_name = name;
        getClassData.m_cols = GridManager.Instance.m_numCol;
        getClassData.m_rows = GridManager.Instance.m_numRow;
        getClassData.m_Cells = new string[getClassData.m_cols * getClassData.m_rows];
        int indexTracker = 0;
        for (int i = 0; i < getClassData.m_cols; i++)
        {
            for (int j = 0; j < getClassData.m_rows; j++)
            {
                getClassData.m_Cells[indexTracker] = GridManager.Instance.m_grid[i, j].m_IsAlive.ToString();
                indexTracker++;
            }
        }
        return getClassData;
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



























#region What are you doing HERE
// like seriously ?
#endregion