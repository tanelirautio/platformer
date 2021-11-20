using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class GameManager : MonoBehaviour
    {
        void Update()
        {
            // TODO: this is for debugging
            if (Input.GetKeyDown(KeyCode.Escape))
            {
#if UNITY_EDITOR
                UnityEditor.EditorApplication.isPlaying = false;
#else
                Application.Quit();
#endif
            }

            if(Input.GetKeyDown(KeyCode.S))
            {
                SaveSystem.Save();
                print("Saved!");
            }

            if(Input.GetKeyDown(KeyCode.L))
            {
                SaveData s = SaveSystem.Load();
                print("saved character is: " + s.selectedCharacter);
                print("saved level is: " + s.currentLevel);
                print("saved score is: " + s.score);
                print("saved health is: " + s.health);
            }
        }

    }
}
