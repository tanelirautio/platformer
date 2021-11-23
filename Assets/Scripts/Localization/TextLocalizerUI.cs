using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

[RequireComponent(typeof(TextMeshProUGUI))]
public class TextLocalizerUI : MonoBehaviour
{
    TextMeshProUGUI textField;

    public string key;
    private bool localizationMissing = false;

    void Start()
    {
        Localize();
    }

    public void Localize()
    {
        textField = GetComponent<TextMeshProUGUI>();
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

