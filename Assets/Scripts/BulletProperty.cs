using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletProperty : MonoBehaviour {

	public float travelTime, movSpeed, damage;
	private GameObject bulletSpawn;


	void Start () {
		bulletSpawn = GameObject.Find ("Bullet");
		this.transform.rotation = bulletSpawn.transform.rotation;
	}

	void Update () {
		travelTime -= 1 * Time.deltaTime;
		if (travelTime <= 0)
			Destroy (this.gameObject);

		this.transform.Translate (Vector3.forward * Time.deltaTime * movSpeed);
	}
}
