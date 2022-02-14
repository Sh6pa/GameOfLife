using System.Collections;
using System.Collections.Generic;
using System.Text;
using System.IO;
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

    public async void SaveToJson()
    {

        string json = SetData(); ;
        Debug.Log(json);
        string filePath = Application.persistentDataPath + "/Grids";
        byte[] encodedText = Encoding.Unicode.GetBytes(json);
        DirectoryInfo info = new DirectoryInfo(filePath);
        if (!info.Exists)
        {
            info.Create();
        }
        string path = Path.Combine(filePath, $"{name}.json");
        using (FileStream sourceStream = new FileStream(path,
            FileMode.Append, FileAccess.Write, FileShare.None,
            bufferSize: 4096, useAsync: true))
        {
            await sourceStream.WriteAsync(encodedText, 0, encodedText.Length);
        };
    }

    public async void LoadJson()
    {

    }
}

public class Grid : MonoBehaviour
{
    public string m_name;
    public int m_rows;
    public int m_cols;
    public string[] m_Cells;
}
