using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoContanctDamage : MonoBehaviour
{
    [SerializeField] private float damageCooldown;
    [SerializeField] private int damage;

    private Dictionary<Damage, float> damageTimers;

    void Awake()
    {
        damageTimers = new Dictionary<Damage, float>();
    }

    void OnCollisionEnter(Collision col)
    {
        if(!enabled) return;
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null)
        {
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
            }
        }
    }

    void OnCollisionExit(Collision col)
    {
        if(!enabled) return;
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null && damageTimers.ContainsKey(d))
        {
            damageTimers.Remove(d);
        }
    }
}