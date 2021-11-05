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
        value = LocalizationManager.GetLocalizedValue(key);

        textField.text = value;

        // Notify if key == value (localization missing?)
        if (value.Length == 0)
        {
            textField.color = Color.red;
            textField.text = "LOCALIZATION MISSING";
        }
    }

}
