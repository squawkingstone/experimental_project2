using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWindow : MonoBehaviour {

	[SerializeField] GameObject m_Window;

	GameObject player;
	float m_DistanceToPlayer = 4f;

	void Start()
	{
		m_Window.SetActive(false);
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void OnMouseDown() 
	{
		if (Vector3.Distance(transform.position, player.transform.position) < m_DistanceToPlayer)
			m_Window.SetActive(true);
	}

}
