using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterData : MonoBehaviour
{
    public int SelectedCharacter { get; set; }

    private void Awake()
    {
        DontDestroyOnLoad(transform.gameObject);
    }

    void PrintSelectedCharacter()
    {
        print(SelectedCharacter);
    }

}
