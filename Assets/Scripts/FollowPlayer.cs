using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FollowPlayer : MonoBehaviour
{
    [SerializeField] private string panAxis;
    [SerializeField] private Transform trackedObject;
    [SerializeField] private float sensitivityX;
    [SerializeField] private bool invertX;
    private float heightOffset;
    [SerializeField][Range(0.0f, 1.0f)] private float responsiveness;
    private float inversion;
    
    void Awake()
    {
        inversion = invertX ? -1 : 1;
        heightOffset = transform.position.y - trackedObject.position.y;
    }

    void Update()
    {
        inversion = invertX ? -1 : 1;
        Move(responsiveness);
    }

    void Move(float coverDistance)
    {
        if(trackedObject != null)
        {
            transform.position =  Vector3.Lerp(transform.position, trackedObject.position + Vector3.up * heightOffset, responsiveness);
            transform.rotation *= Quaternion.Euler(0, inversion * Mathf.Rad2Deg * sensitivityX * Input.GetAxisRaw(panAxis) * Time.deltaTime, 0);
        }
    }

    void DropTarget()
    {
        trackedObject = null;
    }
}
