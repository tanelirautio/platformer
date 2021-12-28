using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace pf
{

    public class Credits : MonoBehaviour
    {
        public float yOffset = -13.0f;   
        public TextMeshPro creditsText;
        TextAsset credits;

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
            Bounds bounds = creditsText.textBounds;
            print(bounds.size);

            creditsText.transform.position = new Vector3(0, yOffset - bounds.size.y/2, 0);

        }

        void Update()
        {

        }
    }
}
