using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GrabAndMove : MonoBehaviour 
{
    [SerializeField] private float patience;
    [SerializeField] private float swingTime;
    [SerializeField] private float placeTime;
    [SerializeField] private float returnTime;
    [SerializeField] private float grabHeightOffset;
    [SerializeField][Range(0, 1)] private float linearSnappiness;
    [SerializeField][Range(0, 1)] private float angularSnappiness;
    [SerializeField] private AnimationCurve heightCurve;
    [SerializeField] private Vector3[] targets;
    [SerializeField] private Vector3 talkPos;
    [SerializeField] private Vector3 endPos;
    [SerializeField] private int maxCycles;
    [SerializeField] private string[] moveStrings;
    [SerializeField] private string[] talkStrings;
    [SerializeField] private AudioClip pickupClip;
    [SerializeField] private AudioClip talkClip;
    [SerializeField] private AudioSource audioSource;

    private Vector3 initialPos;
    private Vector3 grabPos;
    private Vector3 targetPos;
    private int state;
    private float triggerTime;
    private PlayerControl target;
    private int cycles;
    bool movingPlayer;
    TextScroll textScroll;
    TriggerGrab tg;

    void Awake()
    {
        state = -1;
        initialPos = transform.position;
        cycles = maxCycles;
        movingPlayer = false;
        textScroll = FindObjectOfType<TextScroll>();
        tg = GetComponent<TriggerGrab>();
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
            transform.position = Vector3.Lerp(initialPos, target.transform.position + Vector3.up * grabHeightOffset, (Time.time - triggerTime)/swingTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/swingTime);
            if(Time.time - triggerTime >= swingTime)
            {
                state = 3;
                target.setCanMove(false);
                audioSource.loop = false;
                audioSource.clip = pickupClip;
                audioSource.Play();
                movingPlayer = true;
            }
            break;
        case 3:
            if(cycles == 0)
            {
                state = 7;
                textScroll.ScrollText(talkStrings, FinishTalking);
            }
            else
            {
                state = 4;
                --cycles;
                int index = Random.Range(0, moveStrings.Length);
                textScroll.ScrollText(new[] {moveStrings[index]});
            }
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
                movingPlayer = false;
            }
            break;
        case 5:
            state = 6;
            triggerTime = Time.time;
            break;
        case 6:
            transform.position = Vector3.Lerp(targetPos, initialPos, (Time.time - triggerTime)/returnTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/returnTime);
            if(Time.time - triggerTime >= returnTime)
            {
                state = 0;
            }
            break;
        case 7:
            transform.position = Vector3.Lerp(grabPos, talkPos, (Time.time - triggerTime)/placeTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/placeTime);
            if(Time.time - triggerTime >= placeTime)
            {
                state = 8;
                audioSource.clip = talkClip;
                audioSource.loop = true;
                audioSource.Play();
            }
            break;
        case 8:
            break;
        case 9:
            if(audioSource.isPlaying) audioSource.Stop();
            transform.position = Vector3.Lerp(talkPos, endPos, (Time.time - triggerTime)/placeTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/placeTime);
            if(Time.time - triggerTime >= placeTime)
            {
                state = 10;
                triggerTime = Time.time;
                target.setCanMove(true);
                movingPlayer = false;
            }
            break;
        case 10:
            transform.position = Vector3.Lerp(endPos, initialPos, (Time.time - triggerTime)/returnTime);
            transform.position += Vector3.up * heightCurve.Evaluate((Time.time - triggerTime)/returnTime);
            if(Time.time - triggerTime >= returnTime)
            {
                state = -1;
                cycles = maxCycles;
                tg.Reset();
            }
            break;
        default:
            break;
        }
    }

    void FixedUpdate()
    {
        if(movingPlayer)
        {
            target.transform.position = Vector3.Lerp(target.transform.position, transform.position - Vector3.up * grabHeightOffset, linearSnappiness);
            target.transform.rotation = Quaternion.Slerp(target.transform.rotation, transform.rotation, angularSnappiness);
        }
    }

    public void Activate(PlayerControl player)
    {
        target = player;
        state = 0;
    }

    public void FinishTalking()
    {
        if(state == 8)
        {
            state = 9;
            triggerTime = Time.time;
        }
    }
    
    IEnumerator WaitPatiently()
    {
        yield return new WaitForSeconds(patience);
        triggerTime = Time.time;
        state = 2;
    }

    void SelectTarget()
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