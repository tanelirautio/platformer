using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

namespace pf
{
    public class AchievementMenu : MonoBehaviour
    {

        public GameObject achievement;
        //public Transform parent;

        // Start is called before the first frame update
        void Start()
        {
            DataLoader.ParseData();

            float x = achievement.transform.position.x;
            float y = achievement.transform.position.y;
            float offset = 8f;

            for(int i=0; i < PlayerStats.Achievements.Count; i++)
            {
                GameObject go = Instantiate(achievement, new Vector3(0, 0, 0), Quaternion.identity);
                go.transform.SetParent(this.transform, false);
                go.name = achievement.name + "_" + i;
                Vector3 pos = go.transform.position;
                pos.y = offset - i * 4;
                go.transform.position = pos;

                TextMeshProUGUI title = go.transform.Find("AchieveTitle").GetComponent<TextMeshProUGUI>();
                title.text = PlayerStats.Achievements[i].title;
                

                TextMeshProUGUI desc = go.transform.Find("AchieveDesc").GetComponent<TextMeshProUGUI>();
                desc.text = PlayerStats.Achievements[i].desc;

                print(title.text + ": " + desc.text);
            }
        }

        // Update is called once per frame
        void Update()
        {

        }
    }

}