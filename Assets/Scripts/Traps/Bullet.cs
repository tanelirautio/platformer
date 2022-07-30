using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Bullet : MonoBehaviour
    {
        private Vector3 direction;

        private float speed = 5f;

        // Start is called before the first frame update
        void Start()
        {

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
                print("ground collision");
                Destroy(gameObject);
            }
        }
    }
}