using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class GameManager : MonoBehaviour
    {
        // Start is called before the first frame update
        void Start()
        {
            print("gamemanager start");
        }

        // Update is called once per frame
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
        }
    }
}
