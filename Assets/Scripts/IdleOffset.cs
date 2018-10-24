using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class IdleOffset : MonoBehaviour 
{
	Animator anim;
	void Awake() { anim = GetComponent<Animator>(); }
	void Start() { anim.SetFloat("Offset", Random.Range(0f, 1f)); }
}
