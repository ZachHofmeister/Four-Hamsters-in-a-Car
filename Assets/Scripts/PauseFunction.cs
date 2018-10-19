using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PauseFunction : MonoBehaviour {

	[SerializeField] private GameObject pausePanel;
	public Text pause;
	void Start () {
		pause.text = " ";
		pausePanel.SetActive (false);
	}
	

	void Update () {
		if (Input.GetKeyDown (KeyCode.P)) {
			if (!pausePanel.activeInHierarchy) {
				Pause ();
			}
			else if (pausePanel.activeInHierarchy) {
				Continue ();
			}
		}
	}

	private void Pause() //Disables script and stops time
	{
		pause.text = "PAUSE";
		Time.timeScale = 0;
		pausePanel.SetActive (true);
	}

	private void Continue() //Enables scripts and activates time
	{
		pause.text = " ";
		Time.timeScale = 1;
		pausePanel.SetActive (false);
	}
}
