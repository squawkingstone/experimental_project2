using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Damage : MonoBehaviour
{
    [SerializeField] uint maxHealth;
    
    private int health;

    void Start()
    {
        health = (int)maxHealth;
    }

    public bool TakeDamage(int amount)
    {
        health -= amount;
        if(health <= 0)
        {
            Destroy(gameObject);
            return true;
        }

        return false;
    }
}
