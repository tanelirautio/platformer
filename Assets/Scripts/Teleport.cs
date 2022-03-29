using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace pf
{
    public class Teleport : MonoBehaviour
    {
        public Vector3 teleportPoint;

        private AudioManager audioManager;
        private Animator anim;

        //animation states
        const string TELEPORT_IDLE = "teleport_idle";
        const string TELEPORT_HIT = "teleport_hit";

        public bool Activated { get; private set; } = false;

        private void Awake()
        {
            audioManager = GameObject.Find("AudioSystem/TinyAudioManager").GetComponent<AudioManager>();
            anim = GetComponent<Animator>();
        }

        // Start is called before the first frame update
        void Start()
        {
            anim.Play(TELEPORT_IDLE);
        }

        // Update is called once per frame
        void Update()
        {
         
        }

        public void Activate(Transform player)
        {
            if(Activated)
            {
                return;
            }

            Activated = true;

            anim.Play(TELEPORT_HIT);
            audioManager.PlaySound2D("Teleport");

            //TODO: player disable controller -> play disable effect & play sound -> play appear effect in teleport point -> show player -> enable controller

            player.position = teleportPoint + transform.position;
            StartCoroutine(WaitForDestroy(Defs.COLLECTABLE_FADE_TIME));
        }

        private IEnumerator WaitForDestroy(float length)
        {
            yield return new WaitForSeconds(length);
            Destroy();
        }
        private void Destroy()
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
