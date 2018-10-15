using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeekInRange : MonoBehaviour
{
    [SerializeField] private float maxAccel;
    [SerializeField] private float maxSpeed;
    [SerializeField] private float slowRadius;
    [SerializeField] private float accelTime;
    [SerializeField] private float seekRadius;

    Transform tracked;
    Rigidbody rb;

    void Start()
    {
        tracked = FindObjectOfType<PlayerControl>().transform;
        rb = GetComponent<Rigidbody>();
    }

    void FixedUpdate()
    {
        Vector3 toTarget = tracked.position - transform.position;
        Vector3 vel = Vector3.zero;

        if(toTarget.sqrMagnitude <= seekRadius * seekRadius)
        {
            
            float dist = toTarget.magnitude;
            float speed = maxSpeed;
            if(dist < slowRadius)
            {
                speed = maxSpeed * dist/slowRadius;
            }
            
            vel = new Vector3(toTarget.x, 0, toTarget.z).normalized * speed; 
        }

        Vector3 accel = vel - new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(accel.sqrMagnitude > maxAccel * maxAccel)
        {
            accel = accel.normalized * maxAccel;
        }
        
        rb.AddForce(accel * rb.mass);

        vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        if(vel.sqrMagnitude > maxSpeed * maxSpeed)
        {
            vel = vel.normalized * maxSpeed;
            rb.velocity = new Vector3(0, rb.velocity.y, 0) + vel;
        }
    }
}
