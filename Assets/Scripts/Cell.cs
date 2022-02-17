using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cell : MonoBehaviour
{
    public int m_Neighbors = 0;
    public bool m_IsAlive = false;

}

// lighter cell without inheritance for transparent bigger map
public class LightCell
{
    public int m_Neighbors = 0;
    public bool m_IsAlive = false;
}
