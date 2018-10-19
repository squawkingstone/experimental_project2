using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnim : MonoBehaviour {

	[SerializeField] Animator anim;
	[SerializeField] Rigidbody rb;
	
	void FixedUpdate()
	{
		if (rb.velocity.magnitude > 0.01f && !anim.GetBool("IsRunning"))
		{
			anim.SetBool("IsRunning", true);
		}
		else if (rb.velocity.magnitude <= 0.01f && anim.GetBool("IsRunning"))
		{
			anim.SetBool("IsRunning", false);
		}
	}
}
