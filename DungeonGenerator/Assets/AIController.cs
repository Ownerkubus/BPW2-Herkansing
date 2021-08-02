using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class AIController : MonoBehaviour
{
    public List<string> enemyTags = new List<string>();
    public float HitPoints = 10f;
    public float MoveSpeed = 5f;
    public float DetectionRadius = 20f;
    public float AttackRange = 7f;
    public GameObject AlertedIcon;
    public float period = 1f;
    private float nextTimeToExecute = 0f;

    Animator anim;
    private GameObject Target;
    private bool isStunned = false;
    private bool enemyFound = false;

    void Start()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        if (!isStunned)
        {
            ScanForObjects(gameObject.transform.position, DetectionRadius);
            if (Target != null)
            {
                FollowTarget();
            }
        }
    }

    public void ReceiveDamage(float damage)
    {
        isStunned = true;
        HitPoints -= damage;
        if (HitPoints <= 0)
        {
            anim.SetBool("isDead", true);
        }
        else
        {
            anim.SetTrigger("isDamaged");
        }
    }
    public void ResumeFromDamage()
    {
        isStunned = false;
    }
    public void CharacterDead()
    {
        Destroy(gameObject);
    }

    protected void FollowTarget()
    {
        if (Target != null)
        {
            float distance = Vector3.Distance(transform.position, Target.transform.position);
            if (distance <= AttackRange)
            {
                anim.SetBool("isRunning", false);
                anim.SetTrigger("attack");
                transform.LookAt(Target.transform);

                if (Time.time > nextTimeToExecute && distance <= AttackRange)
                {
                    nextTimeToExecute += period;
                }
            }
            else
            {
                transform.position += transform.forward * MoveSpeed * Time.deltaTime;
                anim.SetBool("isRunning", true);
            }

        }
    }

    private void ScanForObjects(Vector3 center, float radius)
    {
        Collider[] hitColliders = Physics.OverlapSphere(center, radius);
        int i = 0;
        enemyFound = false;
        while (i < hitColliders.Length)
        {
            if (enemyTags.Contains(hitColliders[i].tag))
            {
                Target = hitColliders[i].gameObject;
                transform.LookAt(Target.transform);
                enemyFound = true;
                AlertedIcon.SetActive(true);
            }
            i++;
        }
        if (!enemyFound)
        {
            Target = null;
            anim.SetBool("isRunning", false);
            AlertedIcon.SetActive(false);
        }
    }
}

