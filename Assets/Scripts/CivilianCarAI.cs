using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class CivilianCarAI : MonoBehaviour {
    public WheelCollider wheelFL, wheelFR, wheelBL, wheelBR;
    public float maxMotorTorque = 1000f;
    public float maxBrakeTorque = 1000f;
    public float maxSpeed = 5f;
    public float maxSteerAngle = 30f;
    public float turnSpeed = 2.5f;

    private Rigidbody body;

    private List<Transform> roads;
    private Transform currentRoad, previousRoad;
    private bool keepGoingStraight;
    private bool turning;
    private int goToRoad;
    private float currentAngleBeforeCorner, roadAngleBeforeCorner;


    private void Start()
    {
        body = GetComponent<Rigidbody>();

        roads = new List<Transform>();
        foreach (Transform road in GameObject.Find("Roads").transform)
            roads.Add(road.transform);

        turning = false;

        goToRoad = Random.Range(1, 3);
        keepGoingStraight = Random.Range(0f, 1f) > 0.5f ? true : false;
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
            return;

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
    }

    private int RoundAngle(float n)
    {
        int a = (int)(n / 10) * 10; //Smallest multiple
        int b = a + 10; //Largest mulitple
        return (n - a > b - n) ? b : a; //Return closest of two
    }

    private void TurnWheels(float steerAngle)
    {
        if (steerAngle == 0f)
            transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, RoundAngle(transform.eulerAngles.y), transform.eulerAngles.z), Time.deltaTime * turnSpeed);

        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, RoundAngle(steerAngle), Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, RoundAngle(steerAngle), Time.deltaTime * turnSpeed);
    }

    private void FixedUpdate()
    {
        float speed = body.velocity.magnitude * 2.237f;
        wheelFL.motorTorque = speed < maxSpeed ? maxMotorTorque : 0f;
        wheelFR.motorTorque = speed < maxSpeed ? maxMotorTorque : 0f;

        ApplyLocalPositionToVisuals(wheelFL);
        ApplyLocalPositionToVisuals(wheelFR);
        ApplyLocalPositionToVisuals(wheelBL);
        ApplyLocalPositionToVisuals(wheelBR);
    }
}
