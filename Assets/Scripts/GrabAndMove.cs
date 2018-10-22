using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndMove : MonoBehaviour 
{
    [SerializeField] private float patience;
    [SerializeField] private float swingTime;
    [SerializeField] private float placeTime;
    [SerializeField] private float returnTime;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private Vector3[] targets;

    private Vector3 initialPos;
    private Vector3 grabPos;
    private Vector3 targetPos;
    private int state;
    private float triggerTime;
    private PlayerControl target;

    void Awake()
    {
        state = -1;
        initialPos = transform.position;
    }

    void Update()
    {
        switch (state)
        {
        case 0:
            StartCoroutine(WaitPatiently());
            state = 1;
            break;
        case 1:
            break;
        case 2:
            transform.position = Vector3.Lerp(initialPos, target.transform.position, (Time.time - triggerTime)/swingTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/swingTime);
            if(Time.time - triggerTime >= swingTime)
            {
                state = 3;
                target.setCanMove(false);
                target.transform.parent = transform;
            }
            break;
        case 3:
            state = 4;
            triggerTime = Time.time;
            grabPos = transform.position;
            SelectTarget();
            break;
        case 4:
            transform.position = Vector3.Lerp(grabPos, targetPos, (Time.time - triggerTime)/placeTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/placeTime);
            if(Time.time - triggerTime >= placeTime)
            {
                state = 5;
                target.setCanMove(true);
                target.transform.parent = null;
            }
            break;
        case 5:
            state = 6;
            triggerTime = Time.time;
            break;
        case 6:
            transform.position = Vector3.Lerp(grabPos, targetPos, (Time.time - triggerTime)/returnTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/returnTime);
            if(Time.time - triggerTime >= returnTime)
            {
                state = 0;
            }
            break;
        default:
            break;
        }
    }

    public void Activate(PlayerControl player)
    {
        target = player;
        state = 0;
    }
    IEnumerator WaitPatiently()
    {
        yield return new WaitForSeconds(patience);
        triggerTime = Time.time;
        state = 2;
    }

    private void SelectTarget()
    {
        float minDist = (targets[0] - transform.position).sqrMagnitude;
        targetPos = targets[0];

        for(int i = 1; i < targets.Length; ++i)
        {
            float dist = (targets[i] - transform.position).sqrMagnitude;
            if(dist < minDist)
            {
                targetPos = targets[i];
            }
        }
    }
}