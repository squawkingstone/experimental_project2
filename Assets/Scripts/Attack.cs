using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Attack : MonoBehaviour
{
    [SerializeField] private int damage;

    void OnTriggerEnter(Collider col)
    {
        Damage d = col.gameObject.GetComponent<Damage>();
        if(d != null) d.TakeDamage(damage);
    }
}
