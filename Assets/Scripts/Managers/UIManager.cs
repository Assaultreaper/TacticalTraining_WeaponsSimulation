using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    public TMP_Dropdown scenarioSelection;
    void Start()
    {
        SetDropDownData<Scenarios>(scenarioSelection, setscenario);
        setscenario(0);
    }
    private void SetDropDownData<T>(TMP_Dropdown dropdown, UnityAction<int> SetValue) where T : Enum
    {
        dropdown.ClearOptions();

        var enumNames = Enum.GetNames(typeof(T));
        List<TMP_Dropdown.OptionData> options = new();
        foreach(var name in enumNames)
        {
            options.Add(new TMP_Dropdown.OptionData(name));
        }
        dropdown.AddOptions(options);

        dropdown.onValueChanged.AddListener(SetValue);
    }

    public void setscenario(int value)
    {
        GameManager.Instance.CurrentScenario = (Scenarios)Enum.Parse(typeof(Scenarios), scenarioSelection.options[value].text);
        GameManager.Instance.SceneSelectionFromScenario();
    }
}
