using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Teleport : MonoBehaviour
    {
        public Vector3 teleportPoint;

        private SpriteRenderer spriteRenderer;
        private AudioManager audioManager;
        private Animator anim;

        //animation states
        const string TELEPORT_IDLE = "teleport_idle";
        const string TELEPORT_HIT = "teleport_hit";

        public bool Activated { get; private set; } = false;

        private void Awake()
        {
            spriteRenderer = GetComponent<SpriteRenderer>();
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            anim = GetComponent<Animator>();
        }

        void Start()
        {
            anim.Play(TELEPORT_IDLE);
        }

        private void Update()
        {
            if(Activated)
            {
                if (anim.GetCurrentAnimatorStateInfo(0).normalizedTime > 1 && !anim.IsInTransition(0))
                {
                    spriteRenderer.enabled = false;
                }
            }
        }

        public void Activate()
        {
            if(Activated)
            {
                return;
            }

            Activated = true;
            anim.Play(TELEPORT_HIT);
            audioManager.PlaySound2D("Teleport");
        }

        public void ChangePosition(Transform player)
        {
            player.position = teleportPoint + transform.position;
        }

        /*
        private IEnumerator WaitForDestroy(float length)
        {
            yield return new WaitForSeconds(length);
            Destroy();
        }*/
        public void Destroy()
        {
            print("Destroying gameobject: " + gameObject.name);
            Destroy(gameObject);
        }

        private void OnDrawGizmos()
        {
            Gizmos.color = Color.red;
            float size = 0.3f;
            Vector3 teleportPointPos = teleportPoint + transform.position;
            Gizmos.DrawLine(teleportPointPos - Vector3.up * size, teleportPointPos + Vector3.up * size);
            Gizmos.DrawLine(teleportPointPos - Vector3.left * size, teleportPointPos + Vector3.left * size);
        }
    }
}
