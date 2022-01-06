using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using DG.Tweening;

namespace pf
{

    public class Credits : MonoBehaviour
    {
        public float yOffsetStart = -13.0f;
        public float yOffsetEnd = 13.0f;

        public TextMeshPro creditsText;

        private Bounds bounds;
        private TextAsset credits;
        private LevelLoader levelLoader;
        private MenuMusic menuMusic;

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
            menuMusic = GameObject.Find("MenuAudio").GetComponent<MenuMusic>();

            // We don't need GameAudio object in the menus
            GameObject gameAudio = GameObject.Find("GameAudio");
            if (gameAudio)
            {
                Destroy(gameAudio);
            }
        }

        void Start()
        {
            menuMusic.Play("Credits");

            string filename;
            switch (LocalizationManager.language)
            {

                case LocalizationManager.Language.Finnish:
                {
                    filename = "credits_fin";
                    break;
                }
                case LocalizationManager.Language.English:
                default:
                {
                    filename = "credits_eng";
                    break;
                }


            }
            credits = Resources.Load(filename) as TextAsset;
            print(credits);

            creditsText.text = credits.text;

            creditsText.ForceMeshUpdate();
            bounds = creditsText.textBounds;
            print(bounds.size);

            creditsText.transform.position = new Vector3(0, yOffsetStart - bounds.size.y/2, 0);

            creditsText.transform.DOMoveY(yOffsetEnd + bounds.size.y / 2, Defs.CREDITS_TIME).SetEase(Ease.Linear).OnComplete(CreditsDone);

        }

        private void OnDestroy()
        {
            DOTween.KillAll();

        }

        void CreditsDone()
        {
            LoadNextScene();
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                LoadNextScene();
            }
        }

        private void LoadNextScene()
        {
            menuMusic.Stop();
            levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
        }
    }
}
