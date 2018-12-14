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
	public float maxMotorTorque, maxBrakeTorque, maxSteeringTorque, gasPercent, brakePercent, accelPercent, decelPercent;
	AudioSource Car_Running;

    private Rigidbody body;

    private void Start()
    {
        body = GetComponent<Rigidbody>();
        body.centerOfMass = Vector3.down;
    }

    public void ApplyLocalPositionToVisuals (WheelCollider collider) {
		if (collider.transform.childCount == 0)
			return;

		Vector3 position;
		Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        collider.transform.rotation = rotation;
	}

	public void FixedUpdate () {

        float motor = maxMotorTorque * Input.GetAxis("Vertical");
        float steering = maxSteeringTorque * Input.GetAxis("Horizontal");

		foreach (AxleInfo axleInfo in axleInfos) {
			if (axleInfo.steering) {
				axleInfo.leftWheel.steerAngle = steering;
				axleInfo.rightWheel.steerAngle = steering;
			}

			if (axleInfo.motor) {
                if (Input.GetAxis("Vertical") != 0)
                {
                    axleInfo.leftWheel.motorTorque = motor;
                    axleInfo.rightWheel.motorTorque = motor;

                    axleInfo.leftWheel.brakeTorque = 0f;
                    axleInfo.rightWheel.brakeTorque = 0f;
                }
                else
                {
                    axleInfo.leftWheel.motorTorque = 0f;
                    axleInfo.rightWheel.motorTorque = 0f;

                    axleInfo.leftWheel.brakeTorque = motor;
                    axleInfo.rightWheel.brakeTorque = motor;
                }
			}
			Car_Running = GetComponent<AudioSource> ();
			Car_Running.Play (0);
			if(gasPercent > 0){
				if (Car_Running == false) {
				
				}
				else
				{
					Car_Running.UnPause();
				}
			}
			else if(gasPercent == 0){
				Car_Running.Pause ();
			}
			ApplyLocalPositionToVisuals (axleInfo.leftWheel);
			ApplyLocalPositionToVisuals (axleInfo.rightWheel);
		}
	}
}