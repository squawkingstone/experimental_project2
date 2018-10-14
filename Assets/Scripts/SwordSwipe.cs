using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwordSwipe : MonoBehaviour
{
    [SerializeField] private float liveTime;
    [SerializeField] private float coolDown;
    [SerializeField] private GameObject swipe;
    private PlayerControl pc;
    private float attackTime;

    void Awake()
    {
        pc = GetComponent<PlayerControl>();
        attackTime = -coolDown;
    }

    void Start()
    {
        swipe.SetActive(false);
    }

    void Update()
    {
        if(pc.attackDown && Time.time - attackTime >= coolDown)
        {
            StartCoroutine(Swing());
        }
    }

    IEnumerator Swing()
    {
        swipe.SetActive(true);
        yield return new WaitForSeconds(liveTime);
        swipe.SetActive(false);
    }
}
