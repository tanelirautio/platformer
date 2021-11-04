using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextLocalizer : MonoBehaviour
{
    TextMeshPro textField;

    public string key;

    void Start()
    {
        textField = GetComponent<TextMeshPro>();
        string value;
        if (string.IsNullOrEmpty(key))
        {
            key = textField.text;
        } 
        value = Localization.GetLocalizedValue(key);
        textField.text = value;
    }

}
