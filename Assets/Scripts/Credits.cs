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

        private void Awake()
        {
            levelLoader = GameObject.Find("LevelLoader").GetComponent<LevelLoader>();
        }

        void Start()
        {
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

        void CreditsDone()
        {
            levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
        }

        void Update()
        {
            if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.Space))
            {
                levelLoader.LoadScene((int)LevelLoader.Scenes.MainMenu);
            }
        }
    }
}
