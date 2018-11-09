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
		Stars = 0;
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
		nowGas = (int)currentGas;
		nowThirst = (int)currentThirst;
		nowHunger = (int)currentHunger;
		AlterStatsText();
		//StarsDetection (Stars);
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

	//void StarsDetection ();
	//{
	//	if(resourceDecrease >= 10 && resourceDecrease <= 20)
	//	{
	//		Stars = 1;
	//		StarsText.Text = "STARS: " + Stars.ToString();
	//		return Stars;
	//	}
	//	else if(resourceDecrease >= 21 && resourceDecrease <= 40)
	//	{
	//		Stars = 2;
	//		StarsText.Text = "STARS: " + Stars.ToString();
	//		return Stars;
	//	}
	//	else if(resourceDecrease >= 41 && resourceDecrease <= 60)
	//	{
	//		Stars = 3;
	//		/StarsText.Text = "STARS: " + Stars.ToString();
		//	return Stars;
		//}
		//else if(resourceDecrease >= 61 && resourceDecrease <= 80)
		//{
			//Stars = 4;
			//StarsText.Text = "STARS: " + Stars.ToString();
			//return Stars;
		//}
		//else if(resourceDecrease >= 80 && resourceDecrease <= 100)
		//{
			//Stars = 5;
			///StarsText.Text = "STARS: " + Stars.ToString();
			//return Stars;
		//}
	//}

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
