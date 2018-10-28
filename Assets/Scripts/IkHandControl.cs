using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Animator))] 
public class IkHandControl : MonoBehaviour
{
    [SerializeField] public Transform trackedObject;
    [SerializeField] public Vector3 offset;
    [SerializeField] public Vector3 backOfHand;
    protected Animator animator;

    void Awake()
    {
        animator = GetComponent<Animator>();
    }

    void OnAnimatorIK(int layerIndex)
    {
        if(animator != null)
        {
            animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKPosition(AvatarIKGoal.LeftHand, trackedObject.position + offset);
            animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
            animator.SetIKRotation(AvatarIKGoal.LeftHand, Quaternion.LookRotation(-offset.normalized, backOfHand));
        }
    }
}
