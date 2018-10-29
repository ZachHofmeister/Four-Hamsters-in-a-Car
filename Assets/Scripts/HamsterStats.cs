using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HamsterStats : MonoBehaviour
{

	public double maxGas = 100, maxHunger = 100, maxThirst = 100, currentGas, currentHunger, currentThirst;
	public double resourceDecrease;
	public Text GasText, HungerText, ThirstText;

	void Start()
	{ //Assign stats to max at beginning

		currentGas = maxGas;
		currentHunger = maxHunger;
		currentThirst = maxThirst;
		resourceDecrease = 1;
	}


	void FixedUpdate()
	{ //Prototype altering of values overtime
		currentGas = currentGas - 0.001;
		currentHunger = currentHunger - 0.001 * resourceDecrease;
		currentThirst = currentThirst - 0.001 * resourceDecrease;
		AlterStatsText();
	}

	void AlterStatsText()
	{ //Assigns new values to the UI
		GasText.text = "Gas: " + currentGas.ToString();
		HungerText.text = "Hunger: " + currentHunger.ToString();
		ThirstText.text = "Thirst: " + currentThirst.ToString();
		resourceDecrease += 0.05;
		if (currentThirst <= 0 || currentHunger <= 0) //Loads death scene upon losing hunger/thirst // Could be changed later
		{
			SceneManager.LoadScene("Death Screen");
		}
	}

	void OnTriggerStay(Collider other)
	{//Detects resources and adds them to stats
		if (other.name == "FoodSupply")
		{
			if (currentHunger > 0 && currentHunger <= 100)
			{
				currentHunger += 1;
				HungerText.text = "Hunger: " + currentHunger.ToString();
			}

		}
		if (other.name == "WaterSupply")
		{
			if (currentThirst > 0 && currentThirst <= 100)
			{
				currentThirst += 1;
				ThirstText.text = "Thirst: " + currentThirst.ToString();
			}

		}
		if (other.name == "GasSupply")
		{
			if (currentHunger > 0 && currentHunger <= 100)
			{
				currentGas += 1;
				GasText.text = "Gas: " + currentGas.ToString();
			}
		}
	}
}
