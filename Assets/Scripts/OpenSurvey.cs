using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class OpenSurvey : MonoBehaviour {

	[SerializeField] string url;

	void Start()
	{
		Button b = GetComponent<Button>();
		b.onClick.AddListener(() => {Application.OpenURL(url);});
	}

}
