using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class CivilianCarAI : MonoBehaviour
{
    public WheelCollider wheelFL, wheelFR, wheelBL, wheelBR;
    public float maxMotorTorque = 1000f;
    public float maxBrakeTorque = 1000f;
    public float maxSpeed = 5f;
    public float maxSteerAngle = 30f;
    public float turnSpeed = 2.5f;
    public bool stopping = false;

    private Rigidbody body;

    private List<GameObject> roads;
    private GameObject currentRoad, nextRoad;
    private HashSet<GameObject> previousRoads;
    private int angleBeforeTurn;

    private bool turning = false;

    private void Start()
    {
        body = GetComponent<Rigidbody>();

        roads = new List<GameObject>();
        foreach (Transform road in GameObject.Find("Roads").transform)
            roads.Add(road.gameObject);

        previousRoads = new HashSet<GameObject>();
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider)
    {
        if (collider.transform.childCount == 0)
            return;

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        collider.transform.rotation = rotation;
    }

    private int RoundTo(float n, int by)
    {
        int a = (int)(n / by) * by; //Smallest multiple
        int b = a + by; //Largest mulitple
        return (n - a > b - n) ? b : a; //Return closest of two
    }

    private void TurnWheelsTo(float steerAngle)
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, steerAngle, Time.deltaTime * turnSpeed);

        wheelFL.steerAngle = Mathf.Clamp(wheelFL.steerAngle, -maxSteerAngle, maxSteerAngle);
        wheelFR.steerAngle = Mathf.Clamp(wheelFR.steerAngle, -maxSteerAngle, maxSteerAngle);
    }

    private Vector3 GetOrigin(GameObject root)
    {
        Vector3 origin = root.transform.position;

        if (root.transform.eulerAngles.y == 180f)
            origin += new Vector3(-5f, 0f, 5f);
        else if (root.transform.eulerAngles.y == 0f)
            origin += new Vector3(5f, 0f, -5f);
        else if (root.transform.eulerAngles.y == 270f)
            origin += new Vector3(5f, 0f, 5f);
        else if (root.transform.eulerAngles.y == 90f)
            origin +=  new Vector3(-5f, 0f, -5f);

        return origin;
    }

    private List<GameObject> GetNearByRoads(GameObject root)
    {
        List<GameObject> nearByRoads = new List<GameObject>();
        if (root == null)
            return nearByRoads;

        RaycastHit leftHit, rightHit, upHit, downHit;
        if (Physics.Raycast(GetOrigin(root), Vector3.left, out leftHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (leftHit.collider != null)
            {
                if (leftHit.collider.transform != root.transform)
                {
                    GameObject leftRoad = roads.Find(road => road.transform == leftHit.collider.transform);
                    nearByRoads.Add(leftRoad);
                }
            }
        }

        if (Physics.Raycast(GetOrigin(root), Vector3.right, out rightHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (rightHit.collider != null)
            {
                if (rightHit.collider.transform != root.transform)
                {
                    GameObject rightRoad = roads.Find(road => road.transform == rightHit.collider.transform);
                    nearByRoads.Add(rightRoad);
                }
            }
        }

        if (Physics.Raycast(GetOrigin(root), Vector3.forward, out upHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (upHit.collider != null)
            {
                if (upHit.collider.transform != root.transform)
                {
                    GameObject upRoad = roads.Find(road => road.transform == upHit.collider.transform);
                    nearByRoads.Add(upRoad);
                }
            }
        }

        if (Physics.Raycast(GetOrigin(root), Vector3.back, out downHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (downHit.collider != null)
            {
                if (downHit.collider.transform != root.transform)
                {
                    GameObject downRoad = roads.Find(road => road.transform == downHit.collider.transform);
                    nearByRoads.Add(downRoad);
                }
            }
        }


        return nearByRoads;
    }

    private void SteerLeft(float maxAngle = 15f, float speed = 2f)
    {
        TurnWheelsTo(-maxSteerAngle);
        maxSteerAngle = maxAngle;
        turnSpeed = speed;
    }

    private void SteerRight(float maxAngle = 30f, float speed = 3f)
    {
        TurnWheelsTo(maxSteerAngle);
        maxSteerAngle = maxAngle;
        turnSpeed = speed;
    }

    private GameObject GetCarOnRoad(GameObject root)
    {
        GameObject car = null;
        Collider[] cars = Physics.OverlapSphere(GetOrigin(root), 10f, 1 << LayerMask.NameToLayer("Car"));
        for (int i = 0; i < cars.Length; i++)
        {
            if (cars[i].tag == "Hamster Car" || cars[i].tag == "Police Car" || (cars[i].tag == tag && cars[i].transform != transform))
            {
                car = cars[i].gameObject;
            }
        }

        return car;
    }

    private void FixedUpdate()
    {
        //Driving
        float speed = body.velocity.magnitude * 2.237f;
        wheelFL.motorTorque = speed < maxSpeed && !stopping ? maxMotorTorque : 0f;
        wheelFR.motorTorque = speed < maxSpeed && !stopping ? maxMotorTorque : 0f;
        wheelBL.brakeTorque = stopping ? maxBrakeTorque : 0f;
        wheelBR.brakeTorque = stopping ? maxBrakeTorque : 0f;

        RaycastHit groundHit;
        if (Physics.Raycast(transform.position, -transform.up, out groundHit, Mathf.Infinity, 1 << LayerMask.NameToLayer("Road")))
        {
            currentRoad = roads.Find(road => groundHit.collider.transform == road.transform);
            previousRoads.Add(currentRoad);
        }

        if (currentRoad != null)
        {
            //Avoid Other Cars
            List<GameObject> intersectingRoads = GetNearByRoads(currentRoad);
            intersectingRoads.RemoveAll(road => previousRoads.Contains(road));
            //Selection.objects = intersectingRoads.ToArray();
            foreach (GameObject road in intersectingRoads)
            {
                if (GetCarOnRoad(road) == null)
                {
                    stopping = false;
                    continue;
                }

                stopping = true;
                break;
            }


            //Steering
            int currentRoadAngle = Mathf.RoundToInt(currentRoad.transform.eulerAngles.y);
            if (currentRoad.name.Contains("Straight"))
            {
                if (turning)
                {
                    transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(transform.eulerAngles.x, RoundTo(transform.eulerAngles.y, 90), transform.eulerAngles.z), Time.deltaTime);
                    if (!(wheelFL.steerAngle == 0 && wheelFR.steerAngle == 0))
                    {
                        TurnWheelsTo(0f);
                    }
                    else
                    {
                        if (RoundTo(transform.eulerAngles.y, 10) % 90 == 0)
                        {
                            turning = false;
                            previousRoads.Clear();
                        }
                    }
                }
            }
            else if (currentRoad.name.Contains("Corner"))
            {
                if (!turning)
                {
                    angleBeforeTurn = RoundTo(transform.eulerAngles.y, 90);
                    turning = true;
                }

                if (previousRoads.Count > 1)
                { 
                    if (currentRoadAngle == angleBeforeTurn % 360)
                        SteerLeft();
                    else
                    {
                        if (currentRoadAngle - angleBeforeTurn == -90)
                            SteerRight();
                    }
                }
            }

            else if (currentRoad.name.Contains("T") || currentRoad.name.Contains("Intersection"))
            {
                if (!turning)
                {
                    angleBeforeTurn = RoundTo(transform.eulerAngles.y, 90);

                    nextRoad = intersectingRoads[0]; //intersectingRoads[Random.Range(0, intersectingRoads.Count)];
                    turning = true;
                }
                else
                {
                    if (nextRoad != null)
                    {
                        Vector3 originDiff = GetOrigin(nextRoad) - GetOrigin(currentRoad);
                        originDiff.Normalize();
                        if (angleBeforeTurn % 360 == 0)
                        {
                            if (originDiff.x < 0)
                                SteerLeft();
                            else if (originDiff.x > 0)
                                SteerRight();
                        }
                        else if(angleBeforeTurn == 180)
                        {
                            if (originDiff.x > 0)
                                SteerLeft();
                            else if (originDiff.x < 0)
                                SteerRight();
                        }
                        else if(angleBeforeTurn == 90)
                        {
                            if (originDiff.z > 0)
                                SteerLeft();
                            else if (originDiff.z < 0)
                                SteerRight();
                        }
                        else if (angleBeforeTurn == 270)
                        {
                            if (originDiff.z < 0)
                                SteerLeft();
                            else if (originDiff.z > 0)
                                SteerRight();
                        }
                    }
                }
            }
            else if(currentRoad.name.Contains("End"))
            {
                if (!turning)
                {
                    angleBeforeTurn = RoundTo(transform.eulerAngles.y, 90);
                    turning = true;
                }

                float angleAfterTurn = 0f;
                if (angleBeforeTurn >= 180)
                    angleAfterTurn = 180 - angleBeforeTurn;
                else if (angleBeforeTurn < 180f)
                    angleAfterTurn = angleBeforeTurn + 180;

                SteerLeft(45f, 5f);
            }
        }

        ApplyLocalPositionToVisuals(wheelFL);
        ApplyLocalPositionToVisuals(wheelFR);
        ApplyLocalPositionToVisuals(wheelBL);
        ApplyLocalPositionToVisuals(wheelBR);
    }
}