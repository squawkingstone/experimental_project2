using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerTrackerCam : MonoBehaviour
{
    [SerializeField] private string zoomAxis;
    [SerializeField] private bool invertY;
    [SerializeField] private float sensitivityY;
    [SerializeField][Range(0, 1)] private float startingPathDistance = 0.5f;
    [SerializeField] private AnimationCurve path;
    private float pathDistance;
    private Transform closePoint;
    private Transform farPoint;
    private float inversion; 

    void Awake()
    {
        closePoint = transform.parent.GetChild(0);
        farPoint = transform.parent.GetChild(1);
        pathDistance = startingPathDistance;
    }

    void Start()
    {
        Update();
    }

    void Update()
    {
        inversion = invertY ? -1 : 1;
        pathDistance = Mathf.Clamp01(pathDistance + Input.GetAxisRaw(zoomAxis) * inversion * sensitivityY * Time.deltaTime);
        Vector3 position = Vector3.Lerp(closePoint.localPosition, farPoint.localPosition, pathDistance);
        position.y = Mathf.Lerp(closePoint.localPosition.y, farPoint.localPosition.y, path.Evaluate(pathDistance));
        transform.localPosition = position;
        transform.localRotation = Quaternion.LookRotation(-transform.localPosition, Vector3.up);
    }

    public Quaternion GetForwardRotator()
    {
        return transform.parent.rotation;
    }
}
