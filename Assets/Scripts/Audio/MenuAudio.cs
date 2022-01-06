using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class MenuAudio : MonoBehaviour
    {
        private static MenuAudio instance = null;
        public static MenuAudio Instance
        {
            get { return instance; }
        }
        void Awake()
        {
            if (instance != null && instance != this)
            {
                Destroy(this.gameObject);
                return;
            }
            else
            {
                instance = this;
            }
            DontDestroyOnLoad(this.gameObject);
        }
    }

}