using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Audio;

public class Target : MonoBehaviour
{

    public float health = 50f;
    public int destoyTime;

    public void TakeDamage(float amount)
    {
        health -= amount;

        if (health <= 0f)
        {
            Die();
        }
    }

    void Die()
    {
        gameObject.SetActive(false);
        Destroy(gameObject, 2f);
    }
}

