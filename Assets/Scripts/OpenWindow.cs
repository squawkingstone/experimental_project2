using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class OpenWindow : MonoBehaviour {

	[SerializeField] GameObject m_Window;
    [SerializeField] private AudioClip soundClip;

	CloseWindow m_CloseWindow;

	GameObject player;
	float m_DistanceToPlayer = 4f;
    private AudioSource source;

	void Start()
	{
		m_CloseWindow = m_Window.GetComponentInChildren<CloseWindow>();
		m_Window.SetActive(false);
		player = GameObject.FindGameObjectWithTag("Player");
        source = GetComponent<AudioSource>();
	}

	void Update()
	{
		if (Input.GetKeyDown(KeyCode.E) 
			&& Vector3.Distance(player.transform.position, transform.position) <= m_DistanceToPlayer)
            {
			    m_Window.SetActive(true);
                source.clip = soundClip;
                source.Play();
            }

		if (m_Window.activeInHierarchy && Vector3.Distance(player.transform.position, transform.position) > m_DistanceToPlayer)
		{	
			m_Window.SetActive(false);
		}

	}

}
