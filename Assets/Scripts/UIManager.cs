using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider _rowsSlider;
    [SerializeField] private TMP_Text _rowsText;
    [SerializeField] private Slider _colsSlider;
    [SerializeField] private TMP_Text _colsText;

    private void Start()
    {
        ChangeRows(_rowsSlider.value);
        ChangeCols(_colsSlider.value);
    }
    public void ChangeRows(float value)
    {
        _rowsText.text = ((int)value).ToString();
    }

    public void ChangeCols(float value)
    {
        _colsText.text = ((int)value).ToString();
    }
}
