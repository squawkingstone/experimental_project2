using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CloseWindow : MonoBehaviour {

	[SerializeField] GameObject m_Window;

	Button button;

	void Start()
	{ 
		button = GetComponent<Button>(); 
		button.onClick.AddListener(() => { 
			m_Window.SetActive(false);
		});
	}

}
