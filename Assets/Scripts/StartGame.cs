using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class StartGame : MonoBehaviour 
{
	void Start () 
	{
		Button b = GetComponent<Button>();
		b.onClick.AddListener(() => { SceneManager.LoadScene("Town"); });
	}
	
}
