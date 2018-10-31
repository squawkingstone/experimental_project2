using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DungeonNarration : MonoBehaviour 
{
	[SerializeField] float m_FightClipChance;
	[SerializeField] float m_MiscClipChance;
	[SerializeField] float m_ClipTimer;

	[SerializeField] AudioClip[] m_FightClips;
	[SerializeField] AudioClip[] m_MiscClips;

	AudioSource m_Audio;
	float m_Timer = 0f;
		
	void Awake ()
	{
		m_Audio = GetComponent<AudioSource>();
	}

	void Update () 
	{
		if (m_Timer >= m_ClipTimer)
		{
			if (Random.Range(0f, 1f) < m_MiscClipChance)
			{
				m_Audio.clip = m_MiscClips[Random.Range(0, m_MiscClips.Length)];
				m_Audio.Play();
				m_Timer = 0f;
			}
		}
		m_Timer += Time.deltaTime;
		Debug.Log(m_Timer);
	}

	public void PlayFightClip()
	{
		if (Random.Range(0f, 1f) < m_FightClipChance && !m_Audio.isPlaying)
		{
			m_Audio.clip = m_FightClips[Random.Range(0, m_FightClips.Length)];
			m_Audio.Play();
			m_Timer = 0f;
		}
		
	}
}
