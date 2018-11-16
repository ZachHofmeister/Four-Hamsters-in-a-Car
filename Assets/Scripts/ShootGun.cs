using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ShootGun : MonoBehaviour {

	public GameObject bullet;
	public GameObject bulletSpawn;
	public float fireRate;
	private Transform _bullet;

	void Start () {

	}

	void Update () {
		if (Input.GetKeyDown (KeyCode.X))
			fire();
	}

	public void fire()
	{
		_bullet =	Instantiate (bullet.transform, bullet.transform.position, Quaternion.identity);
		_bullet.rotation = bulletSpawn.transform.rotation;
	}
}
