using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class LevelObjectiveCompletion : MonoBehaviour
{
    public struct CompletedObjectives
    {
        public CompletedObjectives(int l, bool p, bool t, bool d)
        {
            Level = l;
            CompletedPoints = p;
            CompletedTime = t;
            CompletedNoDeaths = d;
        }

        public int Level { get; set; }
        public bool CompletedPoints { get; set; }
        public bool CompletedTime { get; set; }
        public bool CompletedNoDeaths { get; set; }
    }

    private List<CompletedObjectives> completedObjectives;

    // Start is called before the first frame update
    void Start()
    {
        TextAsset t = Resources.Load<TextAsset>("completedObjectives");
        if(t == null)
        {
            Reset();
        }
        else
        {
            print("File found!");
        }
    }

    void Reset()
    {
        for(int i=0; i<10; i++)
        {
            completedObjectives.Add(new CompletedObjectives(i, false, false, false));
        }

        var t = JsonUtility.ToJson(completedObjectives);


    }


}
