using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 direction;

        private float speed = 5f;

        private static List<GameObject> instancedBullets = new List<GameObject>();

        private void Start()
        {
            instancedBullets.Add(this.gameObject);
        }

        public void SetDirection(Vector2 dir)
        {
            direction = dir;
        }

        // Update is called once per frame
        void Update()
        {
            transform.Translate(direction * speed * Time.deltaTime);
        }

        private void OnTriggerEnter2D(Collider2D collision)
        {
            if (collision.gameObject.tag == "Ground")
            {
                instancedBullets.Remove(this.gameObject);
                Destroy(gameObject);
            }
        }

        public static void Uninit()
        {
            for (int i = 0; i < instancedBullets.Count; i++)
            {
                GameObject obj = instancedBullets[i];
                Destroy(obj);
            }
            instancedBullets.Clear();
        }
    }
}