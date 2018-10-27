using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSupply : MonoBehaviour
{

	public double hungerValue = 10;
	public int hungerSupply = 50;

	void Start()
	{

	}


	void Update()
	{
		if (hungerSupply <= 100)
		{
			hungerSupply += 1;
		}
	}

	void OnTriggerEnter(Collider other)
	{
		if (hungerSupply > 0 && hungerSupply <= 100)
		{
			if (other.gameObject.CompareTag("//Name of hamster player"))
			{
				GameObject FoodSupply = GameObject.Find("Stats UI");
				FoodSupply.GetComponent<HamsterStats>().currentHunger += 10;
				hungerSupply -= 4;
			}
		}
	}
}
