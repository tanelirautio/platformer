using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Music : MonoBehaviour
    {
        private static Dictionary<string, string> musicDictionary = new Dictionary<string, string>()
        {
            { "Achievements", "Intro" },
            { "CharacterSelect", "Intro" },
            { "LevelSelect", "Intro" },
            { "MainMenu", "Intro" },
            { "Settings", "Intro" },
            { "Statistics", "Intro" },
            { "Credits", "Credits" },
            { "Level_0", "Level_0" },
            { "Level_1", "Level_1" },
            { "Level_2", "Level_2" },
            { "Level_3", "Level_3" },
            { "Level_4", "Level_4" },
            { "Level_5", "Level_5" },
            { "Level_6", "Level_6" },
            { "Level_7", "Level_7" },
            { "Level_8", "Level_8" },
            { "Level_9", "Level_9" },
            { "End", "End" },
            { "TestLevel", "Credits" }
        };
        
        public string CurrentMusic { get; private set; }

        public bool IsPlaying { get; private set; }

        public void Play(string scene)
        {
            string song;
            if (musicDictionary.TryGetValue(scene, out song))
            {
                if ((!IsPlaying) || (IsPlaying && CurrentMusic != song)) {
                    _Play(song);
                }
            }
        }

        private void _Play(string song)
        {
            AudioManager.Instance.PlayMusic(song, 0);
            CurrentMusic = song;
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
