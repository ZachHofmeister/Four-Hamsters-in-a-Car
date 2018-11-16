using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class HamsterStats : MonoBehaviour
{

	public double maxGas = 100, maxHunger = 100, maxThirst = 100, currentGas, currentHunger, currentThirst;
	public double resourceDecrease;
	public int nowGas, nowThirst, nowHunger, Stars;
	public Text GasText, HungerText, ThirstText, StarsText;

	void Start()
	{ //Assign stats to max at beginning
		currentGas = maxGas;
		currentHunger = maxHunger;
		currentThirst = maxThirst;
		resourceDecrease = 1;
		Stars = 0;
	}


	void FixedUpdate()
	{ //Prototype altering of values overtime
		currentGas = currentGas - 0.001;
		currentHunger = currentHunger - 0.001 * resourceDecrease;
		currentThirst = currentThirst - 0.001 * resourceDecrease;
		nowGas = (int)currentGas;
		nowThirst = (int)currentThirst;
		nowHunger = (int)currentHunger;
		AlterStatsText();
		StarsDetection ();
	}

	void AlterStatsText()
	{ //Assigns new values to the UI
		GasText.text = "Gas: " + nowGas.ToString();
		HungerText.text = "Hunger: " + nowHunger.ToString();
		ThirstText.text = "Thirst: " + nowThirst.ToString();
		resourceDecrease += 0.05;
		if (currentThirst <= 0 || currentHunger <= 0) //Loads death scene upon losing hunger/thirst // Could be changed later
		{
			SceneManager.LoadScene("Death Screen");
		}
	}

	void StarsDetection ()
	{ //Determines how many stars the players have
		if(resourceDecrease <= 9)
		{
			Stars = 0;
			StarsText.text = "STARS: " + Stars.ToString();
		}
		else if(resourceDecrease >= 10 && resourceDecrease <= 50)
		{
			Stars = 1;
			StarsText.text = "STARS: " + Stars.ToString();
		}
		else if(resourceDecrease >= 51 && resourceDecrease <= 100)
		{
			Stars = 2;
			StarsText.text = "STARS: " + Stars.ToString();
		}
		else if(resourceDecrease >= 101 && resourceDecrease <= 150)
		{
			Stars = 3;
			StarsText.text = "STARS: " + Stars.ToString();
		}
		else if(resourceDecrease >= 151 && resourceDecrease <= 200)
		{
			Stars = 4;
			StarsText.text = "STARS: " + Stars.ToString();
		}
		else if(resourceDecrease >= 201 && resourceDecrease <= 300)
		{
			Stars = 5;
			StarsText.text = "STARS: " + Stars.ToString();
		}
	}

	void OnTriggerStay(Collider other)
	{//Detects resources and adds them to stats
		if (other.name == "FoodSupply")
		{
			if (currentHunger > 0 && currentHunger <= 100)
			{
				currentHunger += 2;
				HungerText.text = "Hunger: " + nowHunger.ToString();
			}

		}
		if (other.name == "WaterSupply")
		{
			if (currentThirst > 0 && currentThirst <= 100)
			{
				currentThirst += 2;
				ThirstText.text = "Thirst: " + nowThirst.ToString();
			}

		}
		if (other.name == "GasSupply")
		{
			if (currentHunger > 0 && currentHunger <= 100)
			{
				currentGas += 2;
				GasText.text = "Gas: " + nowGas.ToString();
			}
		}
	}
}
