using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OpenWindow : MonoBehaviour {

	[SerializeField] GameObject m_Window;

	CloseWindow m_CloseWindow;

	GameObject player;
	float m_DistanceToPlayer = 4f;

	void Start()
	{
		m_CloseWindow = m_Window.GetComponentInChildren<CloseWindow>();
		m_Window.SetActive(false);
		player = GameObject.FindGameObjectWithTag("Player");
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E) 
			&& Vector3.Distance(player.transform.position, transform.position) <= m_DistanceToPlayer)
			m_Window.SetActive(true);

		if (m_Window.activeInHierarchy && Vector3.Distance(player.transform.position, transform.position) > m_DistanceToPlayer)
			m_Window.SetActive(false);
	}

}
