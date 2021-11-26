using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LevelLoader : MonoBehaviour
{

    public Animator transition;
    public float transitionTime = 1f;

    private static int previousSceneIndex = -1;

    public enum Scenes
    {
        MainMenu = 0,
        CharacterSelect = 1,
        Continue = 2,
        StartLevel = 3
    }

    public static int GetPreviousSceneIndex()
    {
        return previousSceneIndex;
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

    public int GetTotalSceneCount()
    {
        return SceneManager.sceneCountInBuildSettings;
    }

    private IEnumerator MakeTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        previousSceneIndex = GetCurrentSceneIndex();
        SceneManager.LoadScene(levelIndex);
    }
}
