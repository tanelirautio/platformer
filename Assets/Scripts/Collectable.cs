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
            Heart = 0,
            Apple = 1,
            Bananas = 2,
            Cherries = 3,
            Kiwi = 4,
            Melon = 5,
            Orange = 6,
            Pineapple = 7,
            Strawberry = 8
        }

        public Type type;

        private Transform obj;
        private Transform points;
        private TextMeshPro text;

        private Transform collected;
        private Animator collectedAnim = null;

        private Player player;
        private PlayerScore score;
        private PlayerHealth health;
        private AchievementManager achievements;
        private AudioManager audioManager;

        // Animation states
        const string COLLECTED = "collected";

        private static Dictionary<Type, int> collectables;
        private bool initDone = false;

        private bool playerHasCollectedThis = false;

        private void Awake()
        {
            player = GameObject.Find("Player").GetComponent<Player>();
            score = GameObject.Find("Player").GetComponent<PlayerScore>();
            health = GameObject.Find("Player").GetComponent<PlayerHealth>();
            achievements = GameObject.Find("AchievementManager").GetComponent<AchievementManager>();
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();

            obj = transform.Find("Object");
            points = transform.Find("Points");
            text = points.GetComponent<TextMeshPro>();
            
            points.gameObject.SetActive(false);

            collected = transform.Find("Collected");
            collectedAnim = collected.gameObject.GetComponent<Animator>();
            collected.gameObject.SetActive(false);
        }

        void Start()
        {
            if (!initDone)
            {
                //TODO: read from json at startup(?)
                collectables = new Dictionary<Type, int>()
                {
                    { Type.Heart, 1000 },
                    { Type.Apple, 100 },
                    { Type.Bananas, 200 },
                    { Type.Cherries, 300 },
                    { Type.Kiwi, 400 },
                    { Type.Melon, 500 },
                    { Type.Orange, 600 },
                    { Type.Pineapple, 700 },
                    { Type.Strawberry, 800 }
                };
                initDone = true;
            }
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Player")
            {
                if(playerHasCollectedThis || player.IsDead == true || player.IsTeleporting == true)
                {
                    return;
                }
                playerHasCollectedThis = true;
                audioManager.PlaySound2D("Pickup");

                obj.gameObject.SetActive(false);           
                collected.gameObject.SetActive(true);
                collectedAnim.Play(COLLECTED);
                StatisticsManager.AddCollectedItem(type);

                if (type != Type.Heart)
                {
                    achievements.CheckCollectAchievement(type);
                    ShowFadingScore();
                }
                else
                {
                    if (!health.AddHealth())
                    {
                        ShowFadingScore();
                    }
                    else
                    {
                        StartCoroutine(WaitForDestroy(Defs.COLLECTABLE_FADE_TIME));
                    }
                }


            }
            //print("Collision between " + this.name + " and " + collision.gameObject.name);
        }

        private IEnumerator WaitForDestroy(float length)
        {
            yield return new WaitForSeconds(length);
            Destroy();
        }

        private void ShowFadingScore()
        {
            points.gameObject.SetActive(true);
            score.AddScore(collectables[type]);
            points.transform.DOMoveY(transform.position.y + 2, 1);
            Invoke("TweenFadingScore", Defs.COLLECTABLE_FADE_INVOKE_TIME);
            //float alpha = text.color.a;
            //DOTween.To(() => alpha, x => alpha = x, 0.0f, Defs.COLLECTABLE_FADE_TIME).OnUpdate(() => UpdateText(alpha)).OnComplete(Destroy);
        }

        private void TweenFadingScore()
        {
            float alpha = text.color.a;
            DOTween.To(() => alpha, x => alpha = x, 0.0f, Defs.COLLECTABLE_FADE_TIME).OnUpdate(() => UpdateText(alpha)).OnComplete(Destroy);
        }

        private void Destroy()
        {
            //print("Destroying gameobject: " + gameObject.name);
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
