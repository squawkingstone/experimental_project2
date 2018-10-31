using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwipe : MonoBehaviour
{
    [SerializeField] private float liveTime;
    [SerializeField] private float coolDown;
    private PlayerControl pc;
    private float attackTime;
    private Animator anim;
    private Attack attack;

    void Awake()
    {
        pc = GetComponent<PlayerControl>();
        attackTime = -coolDown;
        anim = GetComponentInChildren<Animator>();
        attack = GetComponentInChildren<Attack>();
    }

    void Update()
    {
        if(pc.attackDown && Time.time - attackTime >= coolDown)
        {
            attackTime = Time.time;
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        anim.SetBool("Attack", true);
        attack.Activate();
        yield return new WaitForSeconds(liveTime);
        anim.SetBool("Attack", false);
        attack.Deactivate();
    }
}
