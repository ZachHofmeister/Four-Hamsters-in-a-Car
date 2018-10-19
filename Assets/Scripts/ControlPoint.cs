using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControlType { WHEEL, PEDAL}

public class ControlPoint : MonoBehaviour {

	public ControlType controlType;
	public HamsterController hamster;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		if (hamster != null) {
			switch (controlType) {
				case ControlType.WHEEL:
					hamster.rb.position = transform.position + new Vector3 (0, 0.25f, 0);
					break;
				case ControlType.PEDAL:

					break;
			}
		}
	}
}
