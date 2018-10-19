using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HamsterStats : MonoBehaviour {

	public double maxGas = 1000, maxHunger = 1000, maxThirst = 1000, currentGas, currentHunger, currentThirst;
	public Text GasText, HungerText, ThirstText;
	public bool waitScript = true;

	void Start () { //Assign stats to max at beginning
		
		currentGas = maxGas;
		currentHunger = maxHunger;
		currentThirst = maxThirst;
	}


	void FixedUpdate () { //Prototype altering of values overtime
		currentGas = currentGas - 0.001;
		currentHunger = currentHunger - 0.001;
		currentThirst = currentThirst - 0.001;
		AlterStatsText ();
	}

	void AlterStatsText(){ //Assigns new values to the UI
		GasText.text = "Gas: " + currentGas.ToString ();
		HungerText.text = "Hunger: " + currentHunger.ToString ();
		ThirstText.text = "Thirst: " + currentThirst.ToString ();


		if (currentThirst <= 0 || currentHunger <= 0) //Loads death scene upon losing hunger/thirst // Could be changed later
		{
			SceneManager.LoadScene ("Death Screen");
		}
	}
}
