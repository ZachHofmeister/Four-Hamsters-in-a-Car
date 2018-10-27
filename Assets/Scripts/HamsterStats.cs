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
	public bool waitScript = true;

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

	void OnCollisionEnter(Collision other)
	{
		if(other.gameObject.CompareTag("FoodSupply"))
		{
			currentHunger += 10;
		}
		if(other.gameObject.CompareTag("WaterSupply"))
		{
			currentThirst += 10;
		}
		if(other.gameObject.CompareTag("GasSupply"))
		{
			currentGas += 10;
		}
	}
}
