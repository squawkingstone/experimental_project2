using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoContanctDamage : MonoBehaviour
{
    [SerializeField] private float damageCooldown;
    [SerializeField] private int damage;
    [SerializeField] private AudioClip attackClip;

    private Animator anim;
    private AudioSource source;
    private Dictionary<Damage, float> damageTimers;

    void Awake()
    {
        anim = GetComponentInChildren<Animator>();
        damageTimers = new Dictionary<Damage, float>();
        source = GetComponent<AudioSource>();
    }

    void OnCollisionEnter(Collision col)
    {
        if(!enabled) return;
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null)
        {
            anim.SetBool("Attack", true);
            damageTimers[d] = damageCooldown;
        }
    }

    void OnCollisionStay(Collision col)
    {
        if(!enabled) return;
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null && damageTimers.ContainsKey(d))
        {
            damageTimers[d] -= Time.fixedDeltaTime;
            if(damageTimers[d] <= 0)
            {
                damageTimers[d] = damageCooldown;
                d.TakeDamage(damage);
                source.clip = attackClip;
                source.Play();
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if(!enabled) return;
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null && damageTimers.ContainsKey(d))
        {
            anim.SetBool("Attack", false);
            damageTimers.Remove(d);
        }
    }
}