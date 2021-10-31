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
        CharacterSelect = 0,
        Continue = 1,
        Game = 2
    }

    public void LoadScene(int levelIndex)
    {
        StartCoroutine(MakeTransition(levelIndex));
    }

    public void LoadNextScene()
    {
        StartCoroutine(MakeTransition(SceneManager.GetActiveScene().buildIndex + 1));
    }

    private IEnumerator MakeTransition(int levelIndex)
    {
        transition.SetTrigger("Start");
        yield return new WaitForSeconds(transitionTime);
        SceneManager.LoadScene(levelIndex);
    }
}
