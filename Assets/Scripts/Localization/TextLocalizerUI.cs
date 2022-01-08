using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using TMPro;

namespace pf
{
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
            else
            {
                textField.color = Color.white;
                localizationMissing = false;
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

