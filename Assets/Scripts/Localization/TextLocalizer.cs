using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshPro))]
public class TextLocalizer : MonoBehaviour
{
    TextMeshPro textField;

    public string key;
    private bool localizationMissing = false;

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

        if (String.IsNullOrEmpty(value) && key != "empty")
        {
            localizationMissing = true;
        }  
    }

    private void LateUpdate()
    {
        if (localizationMissing)
        {
            textField.color = Color.red;
            textField.text = "*LOCALIZATION MISSING*";
        }
    }
}
