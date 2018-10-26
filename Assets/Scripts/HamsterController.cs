using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HamsterController : MonoBehaviour {

	public Rigidbody rb;
	public float speed = 4.0f, jumpSpeed = 3.0f, gravity = 1.0f;
	public bool jumping, grounded, beenControlling;
	public ControlPoint cpCurrent;
	public CarController car;
	public Camera cam;

	// Use this for initialization
	void Start () {
		rb = GetComponent<Rigidbody>(); //Grabs the rigidbody component of the hamster
		car = GameObject.FindGameObjectWithTag ("Car").GetComponent<CarController>();
		cam = GameObject.FindGameObjectWithTag ("MainCamera").GetComponent<Camera> ();
	}
	
	// Update is called once per frame
	void Update () {
		if (cpCurrent != null && Input.GetKeyDown (KeyCode.E) && beenControlling) {
			cpCurrent.hamster = null;
			cpCurrent = null;
		} else if (cpCurrent != null && !beenControlling) {
			beenControlling = true;
		}

		if (cpCurrent == null && beenControlling) {
			beenControlling = false;
		}
	}

	void FixedUpdate() {
		Vector3 vertical, horizontal, moveForce;
		vertical = transform.forward * Input.GetAxisRaw ("Vertical"); //Movement force for forward and back
		horizontal = transform.right * Input.GetAxisRaw ("Horizontal"); //Movement force for left/right
		moveForce = vertical + horizontal; //Combined movement forces
		moveForce = car.transform.TransformDirection (moveForce);
		Vector3 velocityChange = moveForce.normalized * speed - new Vector3 (rb.velocity.x, 0, rb.velocity.z); //The difference in velocity between past and future.
		if (cpCurrent == null) {
			rb.AddForce (velocityChange, ForceMode.VelocityChange); //Applies movement

			if ((Input.GetKeyDown (KeyCode.Space) || Input.GetKey(KeyCode.Space)) && !jumping && grounded) {
				rb.velocity += transform.up * jumpSpeed / gravity;
				jumping = true;
			}

			if (!grounded) {
				rb.AddForce (transform.up * -gravity * 10 * rb.mass); //Applies gravity
			}
		
			grounded = false;
		} else {
			rb.velocity = Vector3.zero;
		}

		transform.GetChild (0).LookAt (transform.position + moveForce); //Animates the face looking in the direction of movement
		rb.velocity += car.GetComponent<Rigidbody> ().velocity;
	}

	void OnCollisionStay(Collision col) {
		grounded = true;
		jumping = false;
	}

	void OnTriggerStay (Collider col) {
		Debug.Log (col.name);
		if (col.tag == "ControlPoint" && col.GetComponent<ControlPoint> ().hamster == null && cpCurrent == null  && !beenControlling && Input.GetKeyDown (KeyCode.E)) {
			Debug.Log ("STUFF");
			ControlPoint cp = col.GetComponent<ControlPoint> ();
			cp.hamster = this;
			Debug.Log (cp.hamster.name);
			cpCurrent = cp;
		}
	}
}
