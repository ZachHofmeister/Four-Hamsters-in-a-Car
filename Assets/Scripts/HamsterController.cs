using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterController : MonoBehaviour {

	public Rigidbody rb;
	public float speed = 4.0f;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>(); //Grabs the rigidbody component of the hamster
	}
	
	// Update is called once per frame
	void Update () {
		
	}

	void FixedUpdate() {
		Vector3 vertical, horizontal, moveForce;
		vertical = transform.forward * Input.GetAxisRaw("Vertical"); //Movement force for forward and back
		horizontal = transform.right * Input.GetAxisRaw("Horizontal"); //Movement force for left/right
		moveForce = vertical + horizontal; //Combined movement forces
		Vector3 velocityChange = moveForce.normalized * speed - rb.velocity; //The difference in velocity between past and future.
		rb.AddForce(velocityChange, ForceMode.VelocityChange); //Applies movement
		rb.AddForce(transform.up * -70f); //Applies gravity

		

		transform.GetChild(0).LookAt(transform.position + moveForce); //Animates the face looking in the direction of movement
	}
}
