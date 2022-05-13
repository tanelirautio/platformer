using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace pf
{
    [RequireComponent(typeof(TextMeshPro))]
    public class TextLocalizer : MonoBehaviour
    {
        TextMeshPro textField;

        public string key;
        private bool localizationMissing = false;
        public Color Color { set { textField.color = value; } }

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

        public string GetText()
        {
            return textField.text;
        }

        public void SetText(string text)
        {
            textField.text = text;
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
}
