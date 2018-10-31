using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Damage : MonoBehaviour
{
    [SerializeField] private uint maxHealth;
    [SerializeField] private bool godMode = false;
    [SerializeField] private Text text;
    [SerializeField] private AudioClip playOnHurt;
    [SerializeField] private AudioClip playOnDeath;
    
    private int health;
    private new AudioSource audio;

    public bool isAlive{get; private set;}

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        isAlive = true;
    }

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
        if(!godMode)
        {
            health -= amount;
        }
        if(text != null)
        {
            text.text = health.ToString();
            audio.clip = playOnHurt;
            audio.Play();
        }
        if(health <= 0)
        {
            DieAndFallover daf = GetComponent<DieAndFallover>();
            if(daf != null)
            {
                daf.Die();
                isAlive = false;
                audio.clip = playOnDeath;
                audio.Play();
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
