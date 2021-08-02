using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

namespace UnityStandardAssets.Characters.FirstPerson
{

    public class EnemyAI : MonoBehaviour
    {
        public GameObject player;
        public Transform playerTransform;
        public SphereCollider sphereCollider;
        private bool inRadius = false;
        public AudioSource enemySpottedSound;

        // Start is called before the first frame update
        void Start()
        {
            playerTransform = FindObjectOfType<FirstPersonController>().gameObject.transform;
            enemySpottedSound = GetComponent<AudioSource>();
        }

        // Update is called once per frame
        void Update()
        {
            if(inRadius == true)
            {
                gameObject.GetComponent<NavMeshAgent>().SetDestination(playerTransform.position);
            }
        }
        
        void OnTriggerEnter(Collider other)
        {
            if (other.tag == "Player")
            {
                inRadius = true;
                enemySpottedSound.Play();
            }
        }
       
    }
}
