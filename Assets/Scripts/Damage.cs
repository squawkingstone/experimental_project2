using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] uint maxHealth;
    [SerializeField] Text text;
    
    private int health;

    void Start()
    {
        health = (int)maxHealth;
        if(text != null)
        {
            text.text = health.ToString();
        }
    }

    public bool TakeDamage(int amount)
    {
        health -= amount;
        if(text != null)
        {
            text.text = health.ToString();
        }
        if(health <= 0)
        {
            DieAndFallover daf = GetComponent<DieAndFallover>();
            if(daf != null)
            {
                daf.Die();
            }
            else
            {
                Destroy(gameObject);
            }
            return true;
        }

        return false;
    }
}
