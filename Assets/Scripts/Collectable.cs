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
            Carrot
        }

        public Type type;

        private Transform obj;
        private Transform points;
        private TextMeshPro text;

        private PlayerScore score;

        private Dictionary<Type, int> collectables;

        private void Awake()
        {
            score = GameObject.Find("Player").GetComponent<PlayerScore>();

            obj = transform.Find("Object");
            points = transform.Find("Points");
            text = points.GetComponent<TextMeshPro>();

            points.gameObject.SetActive(false);
        }

        void Start()
        {
            collectables = new Dictionary<Type, int>()
            {
                { Type.Apple, 100 },
                { Type.Carrot, 50 },
                { Type.Strawberry, 10000 }
            };
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
                    points.gameObject.SetActive(true);

                    score.AddScore(collectables[type]);

                    //TODO: create DOTween sequence which contains all tweens
                    points.transform.DOMoveY(transform.position.y + 2, 1);

                    float alpha = text.color.a;
                    float fadeTime = 2.0f;
                    DOTween.To(() => alpha, x => alpha = x, 0.0f, fadeTime).OnUpdate(() => UpdateText(alpha)).OnComplete(Destroy);
                }
            }
            print("Collision between " + this.name + " and " + collision.gameObject.name);
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
