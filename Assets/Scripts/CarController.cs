using System.Collections;
using System.Collections.Generic;
using UnityEngine;

<<<<<<< HEAD
[System.Serializable]
public class AxleInfo
{
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
	public bool steering;
}

public class CarController : MonoBehaviour {
    public List<AxleInfo> axleInfos;
    public float maxMotorTorque;
	public float maxBrakeTorque;
    public float maxSteeringTorque;
	public float gasPercent, brakePercent;
	public ControlPoint cpGas, cpBrake, cpSteering;

	public void ApplyLocalPositionToVisuals(WheelCollider collider) {
        if (collider.transform.childCount == 0)
            return;
=======
public class CarController : MonoBehaviour {

    public float motorForce, steerForce, brakeForce;
    public WheelCollider leftBackWheel, leftFrontWheel, rightBackWheel, rightFrontWheel;

    private float v, h;
>>>>>>> 73627d04f29ce23c1b03f95db840882ec3c81c9f

    private void Update()
    {
        Vector3 leftBackPosition, rightBackPosition, leftFrontPosition, rightFrontPosition;
        Quaternion leftBackRotation, rightBackRotation, leftFrontRotation, rightFrontRotation;

        leftBackWheel.GetWorldPose(out leftBackPosition, out leftBackRotation);
        leftFrontWheel.GetWorldPose(out leftFrontPosition, out leftFrontRotation);
        rightBackWheel.GetWorldPose(out rightBackPosition, out rightBackRotation);
        rightFrontWheel.GetWorldPose(out rightFrontPosition, out rightFrontRotation);

        leftBackWheel.transform.GetChild(0).position = leftBackPosition;
        rightBackWheel.transform.GetChild(0).position = rightBackPosition;
        leftFrontWheel.transform.GetChild(0).position = leftFrontPosition;
        rightFrontWheel.transform.GetChild(0).position = rightFrontPosition;

        leftBackWheel.transform.GetChild(0).rotation = leftBackRotation;
        rightBackWheel.transform.GetChild(0).rotation = rightBackRotation;
        leftFrontWheel.transform.GetChild(0).rotation = leftFrontRotation;
        rightFrontWheel.transform.GetChild(0).rotation = rightFrontRotation;
    }

<<<<<<< HEAD
    public void FixedUpdate() {
		brakePercent += (cpBrake.hamster != null && Input.GetKey (KeyCode.S) )? 0.1f : -0.1f;
		brakePercent = Mathf.Clamp (brakePercent, 0f, 1f);
		gasPercent += (cpGas.hamster != null && Input.GetKey (KeyCode.W))? 0.1f : -0.1f;
		gasPercent -= brakePercent; //Allows brakes to override gas, otherwise car would continue forwards if brake is held after and while gas is held.
		gasPercent = Mathf.Clamp (gasPercent, 0f, 1f);

		float motor = maxMotorTorque * gasPercent; //Seperate gas pedal
		float brake = maxBrakeTorque * brakePercent; //Seperate brake pedal

		float steering = maxSteeringTorque * (cpSteering.hamster != null? Input.GetAxis("Horizontal") : 0); //Seperate steering

        foreach (AxleInfo axleInfo in axleInfos) {
            if (axleInfo.steering) {
                axleInfo.leftWheel.steerAngle = steering;
                axleInfo.rightWheel.steerAngle = steering;
            }

            if (axleInfo.motor) {
                axleInfo.leftWheel.motorTorque = motor;
				axleInfo.leftWheel.brakeTorque = brake;
				axleInfo.rightWheel.motorTorque = motor;
				axleInfo.rightWheel.brakeTorque = brake;
			}

            ApplyLocalPositionToVisuals(axleInfo.leftWheel);
            ApplyLocalPositionToVisuals(axleInfo.rightWheel);
=======
    private void FixedUpdate()
    {
        float v = Input.GetAxis("Vertical") * motorForce;
        float h = Input.GetAxis("Horizontal") * steerForce;

        leftBackWheel.motorTorque = v;
        rightBackWheel.motorTorque = v;

        leftFrontWheel.steerAngle = h;
        rightFrontWheel.steerAngle = h;

        if (Input.GetKey(KeyCode.Space))
        {
            leftBackWheel.brakeTorque = brakeForce;
            rightBackWheel.brakeTorque = brakeForce;
        }
        if(Input.GetKeyUp(KeyCode.Space))
        {
            leftBackWheel.brakeTorque = 0;
            rightBackWheel.brakeTorque = 0;
        }

        if (Input.GetAxis("Vertical") == 0)
        {
            leftBackWheel.brakeTorque = brakeForce;
            rightBackWheel.brakeTorque = brakeForce;
        }
        else
        {
            leftBackWheel.brakeTorque = 0;
            rightBackWheel.brakeTorque = 0;
>>>>>>> 73627d04f29ce23c1b03f95db840882ec3c81c9f
        }
    }

}
