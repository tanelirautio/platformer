using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace pf
{
    public class AudioSystem : MonoBehaviour
    {
        [Tooltip("Names of the scenes this object should stay alive in when transitioning into")]
        public List<string> sceneNames;

        [Tooltip("A unique string identifier for this object, must be shared across scenes to work correctly")]
        public string instanceName;

        // for singleton-like behaviour: we need the first object created to check for other objects and delete them in the scene during a transition
        // since Awake() callback preceded OnSceneLoaded(), place initialization code in Start()
        private void Start()
        {
            DontDestroyOnLoad(this.gameObject);

            // subscribe to the scene load callback
            SceneManager.sceneLoaded += OnSceneLoaded;
        }

        void OnSceneLoaded(Scene scene, LoadSceneMode mode)
        {
            // delete any potential duplicates that might be in the scene already, keeping only this one 
            CheckForDuplicateInstances();

            // check if this object should be deleted based on the input scene names given 
            CheckIfSceneInList();
        }

        void CheckForDuplicateInstances()
        {
            // cache all objects containing this component
            AudioSystem[] collection = FindObjectsOfType<AudioSystem>();

            // iterate through the objects with this component, deleting those with matching identifiers
            foreach (AudioSystem obj in collection)
            {
                if (obj != this) // avoid deleting the object running this check
                {
                    if (obj.instanceName == instanceName)
                    {
                        Debug.Log("Duplicate object in loaded scene, deleting now...");
                        DestroyImmediate(obj.gameObject);
                    }
                }
            }
        }

        void CheckIfSceneInList()
        {
            // TRa 14.1.2022: For testing purposes try to keep audio system alive in every scene

            /*
            // check what scene we are in and compare it to the list of strings 
            string currentScene = SceneManager.GetActiveScene().name;

            if (sceneNames.Contains(currentScene))
            {
                // keep the object alive 
            }
            else
            {
                // unsubscribe to the scene load callback
                SceneManager.sceneLoaded -= OnSceneLoaded;
                DestroyImmediate(this.gameObject);
            }
            */
        }
    }

}