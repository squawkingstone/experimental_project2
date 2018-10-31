using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlaySoundOnLoad : MonoBehaviour
{
    [SerializeField] private List<AudioClip> clips;

    private AudioSource source;
    void Start()
    {
        source = GetComponent<AudioSource>();
        PlaySound();
        
    }

    public void PlaySound()
    {
        if(clips.Count == 0) return;
        int idx = Random.Range(0, clips.Count);
        source.clip = clips[idx];
        source.Play();
        clips.RemoveAt(idx);
    }
}
