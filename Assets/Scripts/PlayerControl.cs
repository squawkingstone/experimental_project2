using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerControl : MonoBehaviour
{
    [Header("Input")]
    [SerializeField] private string forwardAxis;
    [SerializeField] private string sidewaysAxis;
    [SerializeField] private string jumpAxis;
    [SerializeField] private string actionAxis;
    
    [Header("Movement Parameters")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float groundAccel;
    [SerializeField] private float airAccel;
    [SerializeField] private float angularSpeed;
    [SerializeField][Range(0, 90)] private float maxWalkAngle;
    [SerializeField] private float jumpHeight;

    private Rigidbody rb;
    private Vector3 groundedNormal;
    private float cosWalkAngle;
    private Vector3 movementAxisVector;
    private Quaternion cameraForwardRotation;
    private int frameCounter = 0;
    private float jumpForce;
    private bool jump = false;
    PlayerTrackerCam cam;

    public bool actionDown{get; private set;}

    void Awake()
    {
        rb = GetComponent<Rigidbody>();
        groundedNormal = Vector3.zero;
        cosWalkAngle = Mathf.Cos(Mathf.Deg2Rad * maxWalkAngle);
        jumpForce = Mathf.Sqrt(2 * Mathf.Abs(Physics.gravity.y) * jumpHeight);
        jump = false;
        movementAxisVector = Vector3.zero;
        cameraForwardRotation = Quaternion.identity;
        cam = FindObjectOfType<PlayerTrackerCam>();
    }

    void Update()
    {
        movementAxisVector += new Vector3 (Input.GetAxisRaw(sidewaysAxis), 0, Input.GetAxisRaw(forwardAxis));
        jump |= Input.GetButtonDown(jumpAxis);
        cameraForwardRotation = cam.GetForwardRotator();
        actionDown = Input.GetButtonDown(actionAxis);
        ++frameCounter;
    }

    void FixedUpdate()
    {
        Vector3 movementNormal = Vector3.up;
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        bool grounded = false;

        if(groundedNormal.sqrMagnitude > 0)
        {
            movementNormal = groundedNormal.normalized;
            grounded = true;
        }

        if(grounded && jump)
        {
            rb.AddForce(movementNormal * jumpForce * rb.mass, ForceMode.Impulse);
        }

        Vector3 movement = vel;
        if(frameCounter > 0)
        {
            movement = cameraForwardRotation * movementAxisVector/frameCounter;

        }
        float moveLength = movement.magnitude;

        if(moveLength > 0)
        {
            movement = (movement - Vector3.Dot(movementNormal, movement) * movementNormal).normalized;
            movement *= moveLength * movementSpeed;
        }

        float accel = grounded ? groundAccel : airAccel;
        float deltaAccel = accel * Time.fixedDeltaTime;

        Vector3 velDiff = movement - vel;
        
        if(velDiff.sqrMagnitude < deltaAccel * deltaAccel)
        {
            rb.AddForce(velDiff * rb.mass, ForceMode.Impulse);
        }
        else
        {
            velDiff = velDiff.normalized;
            rb.AddForce(velDiff * accel * rb.mass);
        }
        
        frameCounter = 0;
        jump = false;
        movementAxisVector = Vector3.zero;
        groundedNormal = Vector3.zero;
    }

    void OnCollisionStay(Collision col)
    {
        foreach(ContactPoint contact in col.contacts)
        {
            float cosNormalAngle = Vector3.Dot(contact.normal, Vector3.up);

            if(cosNormalAngle > cosWalkAngle)
            {
                groundedNormal += contact.normal;
            }
        }
    }
}
