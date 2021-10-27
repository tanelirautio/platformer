using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerScore : MonoBehaviour
{
    private int currentScore;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void AddScore(int score)
    {
        currentScore += score;

        //debug
        print("Current score: " + currentScore);
    }

    public int GetScore()
    {
        return currentScore;
    }
}
