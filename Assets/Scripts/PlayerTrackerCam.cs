using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackerCam : MonoBehaviour
{
    [SerializeField] private string zoomAxis;
    [SerializeField] private string panAxis;
    [SerializeField] private Vector3 trackOffset;
    [SerializeField] private Vector3 pathClosePoint;
    [SerializeField] private Vector3 pathFarPoint;
    [SerializeField] private AnimationCurve path;
    [SerializeField] private Transform trackedObject;
    [SerializeField] private Vector2 sensitivity;
    [SerializeField] private bool invertX;
    [SerializeField] private bool invertY;
    [SerializeField][Range(0.0f, 1.0f)] private float responsiveness;

    private float pathDistance;
    private Quaternion forwardRotation;
    private Vector2 inversions;

    void Awake()
    {
        forwardRotation = Quaternion.LookRotation(new Vector3(trackedObject.position.x - transform.position.x, transform.position.y, trackedObject.position.z - transform.position.z));
        transform.rotation = forwardRotation;
        pathDistance = 0.5f;
        Move(1.0f);
        inversions = Vector2.one;
    }

    void Update()
    {
        inversions.x = invertX ? -1 : 1;
        inversions.y = invertY ? -1 : 1;
        Move(responsiveness);
    }

    void Move(float coverDistance)
    {
        forwardRotation *= Quaternion.Euler(0, inversions.x * sensitivity.x * Input.GetAxisRaw(panAxis) * Time.deltaTime, 0); 
        Vector3 trackedPoint =  trackedObject.position + forwardRotation * trackOffset;
        Vector3 closePoint = forwardRotation * pathClosePoint + trackedPoint;
        Vector3 farPoint = forwardRotation * pathFarPoint + trackedPoint;
        pathDistance = Mathf.Clamp01(pathDistance + Input.GetAxisRaw(zoomAxis) * inversions.y * sensitivity.y * Time.deltaTime);
        Vector3 position = Vector3.Lerp(closePoint, farPoint, pathDistance);
        position.y = Mathf.Lerp(closePoint.y, farPoint.y, path.Evaluate(pathDistance));
        transform.position = Vector3.Lerp(transform.position, position, coverDistance);
        transform.rotation = Quaternion.LookRotation(trackedPoint - transform.position, Vector3.up);
    }

    public Quaternion GetForwardRotator()
    {
        return forwardRotation;
    }
}
