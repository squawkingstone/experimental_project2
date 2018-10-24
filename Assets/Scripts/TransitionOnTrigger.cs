using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TransitionOnTrigger : MonoBehaviour 
{
	[SerializeField] string ToScene;

	void OnTriggerEnter(Collider col)
	{
		if (col.tag == "Player")
		{
			UnityEngine.SceneManagement.SceneManager.LoadScene(ToScene);
		}
	}

}
