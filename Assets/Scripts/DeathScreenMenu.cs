using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreenMenu : MonoBehaviour {

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
		if (Input.GetKeyDown (KeyCode.O)) 
		{
			SceneManager.LoadScene ("UITest");	
		}
		else if (Input.GetKeyDown (KeyCode.I))
		{
			SceneManager.LoadScene ("MainMenu");
		}
		else if (Input.GetKeyDown (KeyCode.P)) 
		{
			Application.Quit ();
		}
	}
}
