using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarController : MonoBehaviour {

    public float motorForce, steerForce, brakeForce;
    public WheelCollider leftBackWheel, leftFrontWheel, rightBackWheel, rightFrontWheel;

    private float v, h;

    private void Update()
    {
        float v = Input.GetAxis("Vertical") * motorForce;
        float h = Input.GetAxis("Horizontal") * steerForce;

        leftBackWheel.motorTorque = v;
        rightBackWheel.motorTorque = v;

        leftFrontWheel.steerAngle = h;
        rightFrontWheel.steerAngle = h;


        Vector3 leftBackPosition, rightBackPosition, leftFrontPosition, rightFrontPosition;
        Quaternion leftBackRotation, rightBackRotation, leftFrontRotation, rightFrontRotation;

        leftBackWheel.GetWorldPose(out leftBackPosition, out leftBackRotation);
        leftFrontWheel.GetWorldPose(out leftFrontPosition, out leftFrontRotation);
        rightBackWheel.GetWorldPose(out rightBackPosition, out rightBackRotation);
        rightFrontWheel.GetWorldPose(out rightFrontPosition, out rightFrontRotation);


        leftBackWheel.transform.GetChild(0).rotation = leftBackRotation;
        rightBackWheel.transform.GetChild(0).rotation = rightBackRotation;
        leftFrontWheel.transform.GetChild(0).rotation = leftFrontRotation;
        rightFrontWheel.transform.GetChild(0).rotation = rightFrontRotation;

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
        }
    }

}
