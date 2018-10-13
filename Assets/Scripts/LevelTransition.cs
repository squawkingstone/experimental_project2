using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Linq;

public class LevelTransition : MonoBehaviour
{
    [SerializeField] private string nextLevel;
    [SerializeField] private Color activeColor = Color.yellow;
    
    private MeshRenderer[] renderers;
    private Color[] startColors;

    void Awake()
    {
        renderers = GetComponentsInChildren<MeshRenderer>();
    }

    void Start()
    {
        startColors = renderers.Select(r => r.material.color).ToArray();
    }

    void OnTriggerEnter(Collider col)
    {
        if(col.gameObject.GetComponent<PlayerControl>() == null) return;
        
        foreach(MeshRenderer mr in renderers)
        {
            mr.material.color = activeColor;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if(col.gameObject.GetComponent<PlayerControl>() == null) return;

        for(int i = 0; i < renderers.Length; ++i)
        {
            renderers[i].material.color = startColors[i];
        }
    }

    void OnTriggerStay(Collider col)
    {
        PlayerControl player = col.gameObject.GetComponent<PlayerControl>();
        if(player == null) return;

        if(player.actionDown)
        {
            SceneManager.LoadScene(nextLevel);
        }
    }
}
