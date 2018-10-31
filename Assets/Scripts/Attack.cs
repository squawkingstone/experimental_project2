using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int damage;
    [SerializeField] private AudioClip playOnStart;
    [SerializeField] private AudioClip playOnHit;
    [SerializeField] private AudioClip playOnWall;

    private new AudioSource audio;
    private Collider trigger;

    void Awake()
    {
        audio = GetComponent<AudioSource>();
        trigger = GetComponent<Collider>();
        Deactivate();
    }

    void OnTriggerEnter(Collider col)
    {
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null)
        {
            if(d.isAlive) 
            {
                audio.clip = playOnHit;
                audio.Play();
            }
            d.TakeDamage(damage);
        }
        else
        {
            audio.clip = playOnWall;
            audio.Play();
        }
    }

    public void Activate()
    {
        PlayStartSound();
        trigger.enabled = true;
    }

    void PlayStartSound()
    {
        audio.clip = playOnStart;
        audio.Play();
    }

    public void Deactivate()
    {
        trigger.enabled = false;
    }
}
