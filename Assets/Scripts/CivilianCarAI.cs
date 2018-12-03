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

        visualWheel.transform.position = position;

        rotation *= Quaternion.Euler(90f, 0f, 90f);
        visualWheel.transform.rotation = rotation;
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
        WheelHit wheelFLHit, wheelFRHit, wheelBLHit, wheelBRHit;
        wheelFL.GetGroundHit(out wheelFLHit);
        wheelFR.GetGroundHit(out wheelFRHit);
        wheelBL.GetGroundHit(out wheelBLHit);
        wheelBR.GetGroundHit(out wheelBRHit);

        float speed = body.velocity.magnitude * 2.237f;
        wheelFL.motorTorque = speed < maxSpeed ? maxMotorTorque : 0f;
        wheelFR.motorTorque = speed < maxSpeed ? maxMotorTorque : 0f;

        if (!(wheelFLHit.collider == null || wheelFRHit.collider == null || wheelBLHit.collider == null || wheelBRHit.collider == null))
        {
            if (wheelFLHit.collider == wheelBLHit.collider && wheelFRHit.collider == wheelBRHit.collider)
            {
                currentRoad = roads.Find(road => road == wheelFLHit.collider.transform && road == wheelFRHit.collider.transform);

                if (currentRoad == null)
                    return;

                if (turning)
                {
                    if (RoundAngle(transform.eulerAngles.y) % 90f == 0)
                    {
                        if (wheelFL.steerAngle == 0f && wheelFR.steerAngle == 0f)
                        {
                            goToRoad = Random.Range(1, 3);
                            keepGoingStraight = Random.Range(0f, 1f) > 0.5f ? true : false;

                            turning = false;
                        }
                        else
                        {
                            TurnWheels(0f);
                        }
                    }
                }
            }
            else
            {
                currentRoad = roads.Find(road => road == wheelFLHit.collider.transform && road == wheelFRHit.collider.transform);
                previousRoad = roads.Find(road => road == wheelBLHit.collider.transform && road == wheelBRHit.collider.transform);

                if (currentRoad == null || previousRoad == null)
                    return;

                int currentAngle = RoundAngle(transform.eulerAngles.y);
                int roadAngle = RoundAngle(currentRoad.eulerAngles.y);

                if (currentRoad.name.Contains("Intersection"))
                {
                    if (!keepGoingStraight)
                    {
                        turning = true;

                        if (goToRoad == 1)
                            TurnWheels(-maxSteerAngle);
                        else if (goToRoad == 2)
                            TurnWheels(maxSteerAngle);
                    }
                }
                else if (currentRoad.name.Contains("T"))
                {
                    if (roadAngle == currentAngle || roadAngle == 360f - currentAngle)
                    {
                        turning = true;

                        if (goToRoad == 1)
                            TurnWheels(-maxSteerAngle);
                        else if (goToRoad == 2)
                            TurnWheels(maxSteerAngle);
                    }
                    else if (roadAngle > currentAngle || roadAngle > 360f - currentAngle)
                    {
                        if (!keepGoingStraight)
                        {
                            turning = true;
                            TurnWheels(-maxSteerAngle);
                        }
                    }
                    else if (roadAngle < currentAngle || roadAngle < 360f - currentAngle)
                    {
                        if (!keepGoingStraight)
                        {
                            turning = true;

                            if (currentAngle == 90f)
                                TurnWheels(maxSteerAngle);
                            else if (currentAngle == 360f - 90f)
                                TurnWheels(-maxSteerAngle);
                        }
                    }
                }
                else if (currentRoad.name.Contains("Corner"))
                {
                    if (!turning)
                    {
                        currentAngleBeforeCorner = currentAngle;
                        roadAngleBeforeCorner = roadAngle;
                        turning = true;
                    }
                    else
                    {
                        if      (currentAngleBeforeCorner == 0f   && roadAngleBeforeCorner == 270f)        TurnWheels(maxSteerAngle);
                        else if (currentAngleBeforeCorner == 270f && roadAngleBeforeCorner == 180f)        TurnWheels(maxSteerAngle);
                        else if (currentAngleBeforeCorner == 90f  && roadAngleBeforeCorner == 0f)          TurnWheels(maxSteerAngle);
                        else if (currentAngleBeforeCorner == 180f && roadAngleBeforeCorner == 90f)         TurnWheels(maxSteerAngle);
                        else if (currentAngleBeforeCorner == 90f  && roadAngleBeforeCorner == 90f)         TurnWheels(-maxSteerAngle);
                        else if (currentAngleBeforeCorner == 360f && roadAngleBeforeCorner == 0f)          TurnWheels(-maxSteerAngle);
                        else if (currentAngleBeforeCorner == 270f && roadAngleBeforeCorner == 270f)        TurnWheels(-maxSteerAngle);
                        else if (currentAngleBeforeCorner == 180f && roadAngleBeforeCorner == 180f)        TurnWheels(-maxSteerAngle);
                    }
                }
            }
        }

        ApplyLocalPositionToVisuals(wheelFL);
        ApplyLocalPositionToVisuals(wheelFR);
        ApplyLocalPositionToVisuals(wheelBL);
        ApplyLocalPositionToVisuals(wheelBR);
    }
}
