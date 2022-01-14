using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Music : MonoBehaviour
    {
        public bool IsPlaying { get; private set; }

        public void Play(string song)
        {
            AudioManager.Instance.PlayMusic(song, 0);
            IsPlaying = true;
        }

        public void Stop()
        {
            AudioManager.Instance.StopMusic();
            IsPlaying = false;
        }

        public void PlayFade(string song, float fadeTime)
        {
            StartCoroutine(AudioManager.Instance.PlayMusicFade(song, fadeTime));
            IsPlaying = true;
        }        
        
        public void StopFade(float fadeTime)
        {
            StartCoroutine(AudioManager.Instance.StopMusicFade(fadeTime));
            IsPlaying = false;
        }
    }
}
