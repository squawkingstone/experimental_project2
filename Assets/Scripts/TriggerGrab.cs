using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TriggerGrab : MonoBehaviour {
    [SerializeField] private GrabAndMove activate;

    private bool activated;
    private Collider trigger;

    void Awake()
    {
        activated = false;
        trigger = GetComponent<Collider>();
    }

    void OnTriggerEnter(Collider col)
    {
        if(activated) return;

        PlayerControl pc = col.gameObject.GetComponent<PlayerControl>();
        if(pc != null)
        {
            activate.Activate(pc);
            activated = true;
        }

        trigger.enabled = false;
    }

    public void Reset()
    {
        trigger.enabled = true;
    }
}
