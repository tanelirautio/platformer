using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class MenuMusic : MonoBehaviour
    {
        // any other methods you need
        public void Play(string song)
        {
            AudioManager.Instance.PlayMusic(song, 0); 
        }

        public void Stop()
        {
            AudioManager.Instance.StopMusic(); 
        }

        public void PlayFade(string song, float fadeTime)
        {
            StartCoroutine(AudioManager.Instance.PlayMusicFade(song, fadeTime));
        }        
        
        public void StopFade(float fadeTime)
        {
            StartCoroutine(AudioManager.Instance.StopMusicFade(fadeTime));
        }
    }
}
