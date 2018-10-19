using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FoodSupply : MonoBehaviour {

	public double foodValue = 10;
	private Rigidbody rb;

	void Start () {
		rb = GetComponent<Rigidbody> ();
	}
	

	void Update () {
		
	}

	void OnTriggerEnter(Collider other)
	{
			
	}
}
