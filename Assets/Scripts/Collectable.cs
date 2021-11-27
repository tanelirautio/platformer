using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;

namespace pf
{
    public class Collectable : MonoBehaviour
    {

        public enum Type
        {
            Apple,
            Strawberry,
            Carrot,
            Heart
        }

        public Type type;

        private Transform obj;
        private Transform points;
        private TextMeshPro text;

        private Transform collected;

        private PlayerScore score;
        private PlayerHealth health;

        private static Dictionary<Type, int> collectables;
        private bool initDone = false;

        private void Awake()
        {
            score = GameObject.Find("Player").GetComponent<PlayerScore>();
            health = GameObject.Find("Player").GetComponent<PlayerHealth>();

            obj = transform.Find("Object");
            points = transform.Find("Points");
            text = points.GetComponent<TextMeshPro>();
            collected = transform.Find("Collected");

            points.gameObject.SetActive(false);
            if(collected != null)
            {
                collected.gameObject.SetActive(false);
            }
        }

        void Start()
        {
            if (!initDone)
            {
                //TODO: read from json at startup(?)
                collectables = new Dictionary<Type, int>()
                {
                    { Type.Apple, 100 },
                    { Type.Carrot, 1000 },
                    { Type.Strawberry, 50 },
                    { Type.Heart, 1000 }
                };
                initDone = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if (obj.gameObject.activeSelf)
                {
                    obj.gameObject.SetActive(false);
                }

                if (!points.gameObject.activeSelf)
                {
                    if (type != Type.Heart)
                    {

                        ShowFadingScore();

                    }
                    else
                    {
                        //TODO: nice disappear animation(?)
                        if (!health.AddHealth())
                        {
                            ShowFadingScore();
                        }
                        else
                        {
                            Destroy();
                        }
                    }
                }
            }
            print("Collision between " + this.name + " and " + collision.gameObject.name);
        }

        private void ShowFadingScore()
        {
            points.gameObject.SetActive(true);
            score.AddScore(collectables[type]);
            points.transform.DOMoveY(transform.position.y + 2, 1);
            float alpha = text.color.a;
            float fadeTime = 2.0f;
            DOTween.To(() => alpha, x => alpha = x, 0.0f, fadeTime).OnUpdate(() => UpdateText(alpha)).OnComplete(Destroy);
        }

        private void Destroy()
        {
            print("Destroying gameobject: " + gameObject.name);
            Destroy(gameObject);
        }

        private void UpdateText(float value)
        {
            Color c = text.color;
            c.a = value;
            text.color = c;
        }
    }
}
