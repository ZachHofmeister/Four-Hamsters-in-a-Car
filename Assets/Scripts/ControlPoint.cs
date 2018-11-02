using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType { WHEEL, PEDAL, LEVER}

public class ControlPoint : MonoBehaviour {

	public ControlType controlType;
	public HamsterController hamster;
	public bool leverState = true;
	public Transform lever;

	private Quaternion up, down;

	// Use this for initialization
	void Start () {
		switch (controlType) {
			case ControlType.WHEEL:
				break;
			case ControlType.PEDAL:
				break;
			case ControlType.LEVER:
				lever = transform.GetChild (0);
				up = Quaternion.Euler (new Vector3(30,0,0));
				down = Quaternion.Euler (new Vector3 (-30, 0, 0));
				break;
		}
	}
	
	// Update is called once per frame
	void Update () {
		switch (controlType) {
			case ControlType.WHEEL:
				break;
			case ControlType.PEDAL:
				break;
			case ControlType.LEVER:
				if (leverState) {
					lever.localRotation = Quaternion.RotateTowards (lever.localRotation, up, 100 * Time.deltaTime);
				} else {
					lever.localRotation = Quaternion.RotateTowards (lever.localRotation, down, 100 * Time.deltaTime);
				}
				break;
		}

		if (hamster != null) {
			switch (controlType) {
				case ControlType.WHEEL:
					hamster.rb.position = transform.position + new Vector3 (0, 0.25f, 0);
					break;
				case ControlType.PEDAL:
					hamster.rb.position = transform.position + new Vector3 (0, 0.25f, 0);
					break;
				case ControlType.LEVER:
					hamster.rb.position = transform.position + new Vector3 (0, 0.25f, 0);
					if (Input.GetKey (KeyCode.W)) {
						leverState = true;
					} else if (Input.GetKey (KeyCode.S)) {
						leverState = false;
					}
				break;
			}
		}
	}
}
