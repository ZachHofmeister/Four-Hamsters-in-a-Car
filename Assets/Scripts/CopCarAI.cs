using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEditor;

public class Road
{
    public Transform transform;

    [SerializeField]
    private Road _previous;
    private Vector3 _origin;

    [SerializeField]
    private int _gCost, _hCost, _fCost;
    
    public Road(Transform transform)
    {
        this.transform = transform;

        if (transform.eulerAngles.y == 180)
            this.origin = this.transform.position + new Vector3(-5f, 0f, 5f);
        else if (transform.eulerAngles.y == 0f)
            this.origin = this.transform.position + new Vector3(5f, 0f, -5f);
        else if (transform.eulerAngles.y == 270f)
            this.origin = this.transform.position + new Vector3(5f, 0f, 5f);
        else if (transform.eulerAngles.y == 90f)
            this.origin = this.transform.position + new Vector3(-5f, 0f, -5f);
    }

    public Road previous
    {
        get { return _previous; }
        set { _previous = value; }
    }

    public Vector3 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }

    public int gCost
    {
        get { return _gCost; }
        set { _gCost = value; }
    }

    public int hCost
    {
        get { return _hCost; }
        set { _hCost = value; }
    }

    public int fCost
    {
        get { return _fCost; }
        set { _fCost = value; }
    }


}

public class CopCarAI : MonoBehaviour {

    public WheelCollider wheelFL, wheelFR, wheelBL, wheelBR;
    public float maxMotorTorque = 1000f;
    public float maxBrakeTorque = 1000f;
    public float maxSpeed = 5f;
    public float maxSteerAngle = 30f;
    public float turnSpeed = 5f;

    private Rigidbody body;
    private bool turning = false;
    private float currentSpeed;

    private List<Road> roads;
    private Road currentRoad, previousRoad;

    private List<Road> path;
    private bool foundPath = false;

    private CarController player;
    private Road playerRoad;


    // Use this for initialization
    void Start () {
        body = GetComponent<Rigidbody>();

        roads = new List<Road>();
        foreach (Transform road in GameObject.Find("Roads").transform)
        {
            roads.Add(new Road(road.transform));
        }
        path = new List<Road>();

        player = FindObjectOfType<CarController>();
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

    private Road FindRoadPlayerIsOn()
    {
        WheelHit wheelFLHit, wheelFRHit;
        player.axleInfos[0].leftWheel.GetGroundHit(out wheelFLHit);
        player.axleInfos[0].rightWheel.GetGroundHit(out wheelFRHit);

        if (wheelFLHit.collider == null && wheelFRHit.collider == null)
            return null;

        return roads.Find(road => road.transform == wheelFLHit.collider.transform && road.transform == wheelFRHit.collider.transform);

    }

    private int ManhattanDistance(Road start, Road end)
    {
        return Mathf.RoundToInt(Mathf.Abs(end.origin.x - start.origin.x) + Mathf.Abs(end.origin.z - start.origin.z));
    }

    private Road GetRoadStraightAhead(Road root)
    {
        RaycastHit hit;
        if (Physics.Raycast(root.origin, transform.forward, out hit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (hit.collider != null)
            {
                if (hit.collider.transform != root.transform)
                {
                    return roads.Find(road => road.transform == hit.collider.transform);
                }
            }
        }

        return null;
    }

    private List<Road> GetNearByRoads(Road root)
    {
        List<Road> neighbors = new List<Road>();

        RaycastHit leftHit, rightHit, upHit, downHit;

        if (Physics.Raycast(root.origin, Vector3.left, out leftHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (leftHit.collider != null)
            {
                if (leftHit.collider.transform != root.transform)
                {
                    Road leftRoad = roads.Find(road => road.transform == leftHit.collider.transform);
                    neighbors.Add(leftRoad);
                }
            }
        }

        if (Physics.Raycast(root.origin, Vector3.right, out rightHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (rightHit.collider != null)
            {
                if (rightHit.collider.transform != root.transform)
                {
                    Road rightRoad = roads.Find(road => road.transform == rightHit.collider.transform);
                    neighbors.Add(rightRoad);
                }
            }
        }

        if (Physics.Raycast(root.origin, Vector3.forward, out upHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (upHit.collider != null)
            {
                if (upHit.collider.transform != root.transform)
                {
                    Road upRoad = roads.Find(road => road.transform == upHit.collider.transform);
                    neighbors.Add(upRoad);
                }
            }
        }

        if (Physics.Raycast(root.origin, Vector3.back, out downHit, 10f, 1 << LayerMask.NameToLayer("Road")))
        {
            if (downHit.collider != null)
            {
                if (downHit.collider.transform != root.transform)
                {
                    Road downRoad = roads.Find(road => road.transform == downHit.collider.transform);
                    neighbors.Add(downRoad);
                }
            }
        }


        return neighbors;
    }

    private void FindPathBetween(Road start, Road end)
    {
        if (start == null || end == null)
            return;

        List<Road> open = new List<Road>();
        List<Road> closed = new List<Road>();

        open.Add(start);
        while (open.Count > 0)
        {
            Road current = open[0];
            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost)
                    current = open[i];
                else if(open[i].fCost == current.fCost)
                {
                    if (open[i].gCost > current.gCost)
                        current = open[i];
                    else if(open[i].gCost == current.gCost)
                    {
                        if (open[i].hCost < current.hCost)
                            current = open[i];
                    }
                }
            }

            if (current.transform == end.transform)
            {
                ConstructPath(end);
                return;
            }

            open.Remove(current);
            closed.Add(current);

            foreach (Road neighbor in GetNearByRoads(current))
            {
                if (closed.Contains(neighbor))
                    continue;

                int possibleGCost = current.gCost + ManhattanDistance(neighbor, current);
                if (!open.Contains(neighbor))
                    open.Add(neighbor);
                else
                {
                    if (possibleGCost >= neighbor.gCost)
                        continue;
                }

                neighbor.gCost = possibleGCost;
                neighbor.hCost = ManhattanDistance(neighbor, end);
                neighbor.fCost = neighbor.gCost + neighbor.hCost;

                neighbor.previous = current;
            }   
        }
    }

    private int RoundAngle(float n)
    {
        int a = (int)(n / 10) * 10; //Smallest multiple
        int b = a + 10; //Largest mulitple
        return (n - a > b - n) ? b : a; //Return closest of two
    }

    private void ConstructPath(Road end)
    {
        Road current = end;

        while (current.previous != null)
        {
            path.Add(current);
            current = current.previous;
        }

        path.Add(current);
        path.Reverse();
        foundPath = true;
    }

    private void ShowPath()
    {
        GameObject[] pathObjects = new GameObject[path.Count];
        for (int i = 0; i < path.Count; i++)
            pathObjects[i] = path[i].transform.gameObject;
        Selection.objects = pathObjects;
    }

    private void TurnWheels(float steerAngle)
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
    }

    // Update is called once per frame
    void FixedUpdate () {
        WheelHit wheelFLHit, wheelFRHit, wheelBLHit, wheelBRHit;
        wheelFL.GetGroundHit(out wheelFLHit);
        wheelFR.GetGroundHit(out wheelFRHit);
        wheelBL.GetGroundHit(out wheelBLHit);
        wheelBR.GetGroundHit(out wheelBRHit);

        currentSpeed = body.velocity.magnitude * 2.237f;
        wheelFL.motorTorque = currentSpeed < maxSpeed ? maxMotorTorque : 0f;
        wheelFR.motorTorque = currentSpeed < maxSpeed ? maxMotorTorque : 0f;

        if (!(wheelFLHit.collider == null || wheelFRHit.collider == null || wheelBLHit.collider == null || wheelBRHit.collider == null))
        {
            playerRoad = FindRoadPlayerIsOn();
            if (playerRoad == null)
                return;

            currentRoad = roads.Find(road => road.transform == wheelFLHit.collider.transform && road.transform == wheelFRHit.collider.transform);
            if (currentRoad == null)
                return;

            if (!foundPath)
            {
                Road forwardRoad = GetRoadStraightAhead(currentRoad);
                if (forwardRoad == null)
                    return;
                FindPathBetween(forwardRoad, playerRoad);
            }
            else
            {

                if (path.IndexOf(currentRoad) > 0 && path.IndexOf(currentRoad) < path.Count - 1)
                {
                    Road nextRoad = path[path.IndexOf(currentRoad) + 1];
                    Vector3 nextVector = transform.InverseTransformPoint(nextRoad.origin);
                    float nextSteer = (nextVector.x / nextVector.magnitude) * maxSteerAngle;

                    TurnWheels(nextSteer);
                }
            }
        }

        ApplyLocalPositionToVisuals(wheelFL);
        ApplyLocalPositionToVisuals(wheelFR);
        ApplyLocalPositionToVisuals(wheelBL);
        ApplyLocalPositionToVisuals(wheelBR);
    }
}
