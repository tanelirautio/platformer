using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DataLoader : MonoBehaviour
{

    private SaveData saveData = null;
    private static bool initialized = false;

    public bool ShowLoadOption { get; set; }

    void Start()
    {
        if (!initialized)
        {
            ParseData();
            initialized = true;
        }

        saveData = SaveSystem.Load();
        if (saveData != null)
        {
            ShowLoadOption = true;

            // Level objectives
            Debug.Log("levelObjectives.Length: " + saveData.levelObjectivesCompleted.Length);

            for (int i = 0; i < saveData.levelObjectivesCompleted.GetLength(0); i++)
            {
                if (i < PlayerStats.CompletedObjectives.Count)
                {
                    PlayerStats.CompletedObjectives[i].CompletedNoDeaths = saveData.levelObjectivesCompleted[i, 0];
                    PlayerStats.CompletedObjectives[i].CompletedPoints = saveData.levelObjectivesCompleted[i, 1];
                    PlayerStats.CompletedObjectives[i].CompletedTime = saveData.levelObjectivesCompleted[i, 2];
                }
            }
        }
        else
        {
            ShowLoadOption = false;
        }
    }

    public SaveData GetSaveData()
    {
        return saveData;
    }

    private void ParseData()
    {
        TextAsset levelObjectivesText = Resources.Load<TextAsset>("levelObjectives");
        if(levelObjectivesText != null)
        {
            LevelObjectives[] obj = JsonHelper.FromJson<LevelObjectives>(levelObjectivesText.text);
            if (obj != null)
            {
                for (int i = 0; i < obj.Length; i++)
                {
                    PlayerStats.CompletedObjectives.Add(obj[i]);
                }
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
