using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class AxleInfo {
	public WheelCollider leftWheel;
	public WheelCollider rightWheel;
	public bool motor;
	public bool steering;
}

public class CarController : MonoBehaviour {
	public List<AxleInfo> axleInfos;
	public float maxMotorTorque, maxBrakeTorque, maxSteeringTorque, gasPercent, brakePercent;
	public ControlPoint cpSteering, cpBrake, cpGas; 

	public void ApplyLocalPositionToVisuals (WheelCollider collider) {
		if (collider.transform.childCount == 0)
			return;

		Transform visualWheel = collider.transform.GetChild (0);

		Vector3 position;
		Quaternion rotation;
		collider.GetWorldPose (out position, out rotation);

		visualWheel.transform.position = position;

		rotation *= Quaternion.Euler (90f, 0f, 90f);
		visualWheel.transform.rotation = rotation;
	}

	public void FixedUpdate () {
		brakePercent += (cpBrake.hamster != null && Input.GetKey (KeyCode.S)) ? 0.1f : -0.1f;
		brakePercent = Mathf.Clamp (brakePercent, 0f, 1f);
		gasPercent += (cpGas.hamster != null && Input.GetKey (KeyCode.W)) ? 0.1f : -0.1f;
		gasPercent -= brakePercent; //Allows brakes to override gas, otherwise car would continue forwards if brake is held after and while gas is held.
		gasPercent = Mathf.Clamp (gasPercent, 0f, 1f);

		float motor = maxMotorTorque * gasPercent; //Seperate gas pedal
		float brake = maxBrakeTorque * brakePercent; //Seperate brake pedal
		float steering = maxSteeringTorque * Input.GetAxis ("Horizontal");

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}

			if (axleInfo.motor) {
				axleInfo.leftWheel.motorTorque = motor;
				axleInfo.rightWheel.motorTorque = motor;
			}

			ApplyLocalPositionToVisuals (axleInfo.leftWheel);
			ApplyLocalPositionToVisuals (axleInfo.rightWheel);
		}
	}
}