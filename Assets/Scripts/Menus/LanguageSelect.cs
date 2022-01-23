using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace pf
{
    public class LanguageSelect : MonoBehaviour
    {
        public TextMeshProUGUI title;
        [SerializeField] private List<TextMeshProUGUI> languageSelections = new List<TextMeshProUGUI>();
        
        private int selectedLanguage;
        private bool isInSelectionMode = false;

        private void Start()
        {
            ChangeLanguage(PlayerStats.Language);
        }

        public void ChangeLanguage(int selection)
        {
            // wrap selections around
            if (selection >= languageSelections.Count)
            {
                selection = 0;
            }
            else if(selection < 0)
            {
                selection = languageSelections.Count - 1;
            }

            Color selected = Color.white;
            Color notSelected = Color.gray;
            if (isInSelectionMode)
            {
                selected = new Color(192f / 255f, 50f / 255f, 127f / 255f);
            }

            if(selection == (int)LocalizationManager.Language.English || selection == (int)LocalizationManager.Language.Finnish)
            {
                for (int i = 0; i < languageSelections.Count; i++)
                {
                    if (i == selection)
                    {
                        languageSelections[i].DOColor(selected, 0.01f);
                        selectedLanguage = i;
                    }
                    else
                    {
                        languageSelections[i].DOColor(notSelected, 0.01f);
                    }
                }
            }
        }

        public int GetSelectedLanguage()
        {
            return selectedLanguage;
        }

        public void SelectionMode(bool value)
        {
            isInSelectionMode = value;
            if (isInSelectionMode) 
            {
                title.DOColor(Color.black, 0.01f);
                ChangeLanguage(PlayerStats.Language);
            }
            else
            {
                title.DOColor(Color.white, 0.01f);
                ChangeLanguage(PlayerStats.Language);
            }
        }
    }
}
