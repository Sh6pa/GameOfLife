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
    [SerializeField] private TMP_Dropdown _jsonDropdown;
    [SerializeField] private TMP_Dropdown _pngDropdown;
    [SerializeField] private TMP_InputField _fileName;
    private string _jsonName;
    private string _pngName;
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
        GetPng();
        GetJson();
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

    public async void SaveToJson()
    {
        await dataManager.SaveToJson(_fileName.text);
    }

    public async void LoadJson()
    {
        await dataManager.LoadJson(_jsonName);
    }

    public async void SaveToPng()
    {
        await dataManager.SaveToPng(_fileName.text);
    }

    public async void LoadPng()
    {
        await dataManager.LoadPng(_pngName);
    }

    public void GetJson()
    {
        List<string> files = dataManager.GetJson();
        _jsonDropdown.ClearOptions();
        _jsonDropdown.AddOptions(files);
    }

    public void GetPng()
    {
        List<string> files = dataManager.GetPng();
        _pngDropdown.ClearOptions();
        _pngDropdown.AddOptions(files);
    }

    public void UpdateJsonName()
    {
        _jsonName = _jsonDropdown.options[_jsonDropdown.value].text.Substring(0, _jsonDropdown.options[_jsonDropdown.value].text.Length-5);
    }
    public void UpdatePngName()
    {
        _pngName = _pngDropdown.options[_pngDropdown.value].text.Substring(0, _pngDropdown.options[_pngDropdown.value].text.Length-4);
    }

}
