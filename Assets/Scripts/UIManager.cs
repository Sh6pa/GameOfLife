using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class UIManager : MonoBehaviour
{
    [SerializeField] private Slider _rowsSlider;
    [SerializeField] private TMP_Text _rowsText;
    [SerializeField] private Slider _colsSlider;
    [SerializeField] private TMP_Text _colsText;
    [SerializeField] private Slider _speedSlider;
    [SerializeField] private TMP_Text _speedText;
    public DataManager dataManager;
    public GridManager gridManager;

    public static UIManager UIM;
    private void Awake()
    {
        if (UIM == null)
        {
            UIM = this;
            DontDestroyOnLoad(this);
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        ChangeRows(_rowsSlider.value);
        ChangeCols(_colsSlider.value);
        ChangeSpeed(_speedSlider.value);
    }
    public void ChangeRows(float value)
    {
        _rowsText.text = ((int)value).ToString();
    }

    public void ChangeCols(float value)
    {
        _colsText.text = ((int)value).ToString();
    }
    public void ChangeSpeed(float value)
    {
        _speedText.text = ((int)value).ToString();
    }

    public async void SavetoJson()
    {
        await dataManager.SaveToJson();
    }

    public async void LoadJson()
    {
        await dataManager.LoadJson();
    }

    public async void SavetoPng()
    {
        await dataManager.SaveToPng();
    }

}
