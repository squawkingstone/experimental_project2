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
    [SerializeField] private string attackAxis;
    
    [Header("Movement Parameters")]
    [SerializeField] private float movementSpeed;
    [SerializeField] private float groundAccel;
    [SerializeField] private float airAccel;
    [SerializeField] private float velocityTarget;
    [SerializeField] private float angularSpeed;
    [SerializeField] private float angularAccel;
    [SerializeField] private float angularTarget;
    [SerializeField][Range(0, 90)] private float maxWalkAngle;
    [SerializeField] private float jumpHeight;

    [Header("Sound Properties")]
    [SerializeField] float clinkTime;
    [SerializeField] AudioClip clinkSound;

    private Rigidbody rb;
    private bool canMove;
    private Vector3 groundedNormal;
    private float cosWalkAngle;
    private Vector3 movementAxisVector;
    private Quaternion cameraForwardRotation;
    private int frameCounter = 0;
    private float jumpForce;
    private bool jump = false;
    private PlayerTrackerCam cam;
    private float lastClink;
    private AudioSource source;
    private bool lastAttackPressed;
    private bool grounded;
    private PlayerAnim playerAnim;

    public bool actionDown{get; private set;}
    public bool attackDown{get; private set;}

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
        canMove = true;
        source = GetComponent<AudioSource>();
        lastClink = Time.time;
        playerAnim = GetComponent<PlayerAnim>();
        Cursor.lockState = CursorLockMode.Locked;
    }

    void Update()
    {
        movementAxisVector += new Vector3 (Input.GetAxisRaw(sidewaysAxis), 0, Input.GetAxisRaw(forwardAxis));
        jump |= Input.GetButtonDown(jumpAxis);
        cameraForwardRotation = cam.GetForwardRotator();
        actionDown = Input.GetButtonDown(actionAxis);
        bool attack = Input.GetAxisRaw(attackAxis) > 0.5f;
        attackDown = attack && !lastAttackPressed;
        lastAttackPressed = attack;
        if(Time.time - lastClink > clinkTime)
        {
            lastClink = Time.time;
            if(rb.velocity.sqrMagnitude > 0.05 && grounded && !source.isPlaying)
            {
                source.clip = clinkSound;
                source.Play();
            }
        }

        ++frameCounter;
    }

    void FixedUpdate()
    {
        Vector3 movementNormal = Vector3.up;
        Vector3 vel = new Vector3(rb.velocity.x, 0, rb.velocity.z);
        grounded = false;

        if(groundedNormal.sqrMagnitude > 0)
        {
            movementNormal = groundedNormal.normalized;
            grounded = true;
        }

        if(grounded && jump && canMove)
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
        
        if(!canMove)
        {
            movement = Vector3.zero;
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
        
        if(vel.sqrMagnitude > velocityTarget * velocityTarget)
        {
            vel = vel.normalized;

            float angleToTurn = Mathf.Deg2Rad * Vector3.SignedAngle(transform.forward, vel, Vector3.up);
            angleToTurn = Mathf.Clamp(angleToTurn, -angularAccel, angularAccel);
            if(Mathf.Abs(angleToTurn) < angularTarget)
            {
                transform.rotation = Quaternion.LookRotation(vel, Vector3.up);
                rb.angularVelocity = Vector3.zero;
            }
            else
            {
                rb.AddTorque(Vector3.up * angleToTurn);
                if(rb.angularVelocity.magnitude > angularSpeed)
                {
                    rb.angularVelocity = rb.angularVelocity.normalized * angularSpeed;
                }
            }
        }
        else
        {
            rb.angularVelocity = Vector3.zero;
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

    void OnDestroy()
    {
        GameObject[] go = FindObjectsOfType<GameObject>();
        foreach(GameObject g in go)
        {
            g.SendMessage("DropTarget", SendMessageOptions.DontRequireReceiver);
        }
    }

    public void setCanMove(bool value)
    {
        canMove = value;
        rb.useGravity = value;
        playerAnim.SetGrabbed(!value);
        if(!value)
        {
            
            rb.velocity = Vector3.zero;
        }
    }
}
