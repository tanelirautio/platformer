using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;

    public enum Scenes
    {
        MainMenu = 0,
        CharacterSelect = 1,
        Continue = 2,
        StartLevel = 3
    }

    public void LoadScene(int levelIndex)
    {
        StartCoroutine(MakeTransition(levelIndex));
    }

    public void LoadNextScene()
    {
        StartCoroutine(MakeTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    public void TriggerTransitionOnly(bool fadeToBlack = true)
    {
        if (fadeToBlack)
        {
            transition.SetTrigger("Start");
        }
        else
        {
            transition.SetTrigger("End");
        }
    }

    public int GetCurrentSceneIndex()
    {
        return SceneManager.GetActiveScene().buildIndex;
    }

    private IEnumerator MakeTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
