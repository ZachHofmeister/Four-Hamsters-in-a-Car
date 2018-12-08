using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class Road
{
    [SerializeField]
    private Transform _transform;

    [SerializeField]
    private Road _previous;

    [SerializeField]
    private Vector3 _origin;

    [SerializeField]
    private string _name;
    

    [SerializeField]
    private int _gCost, _hCost, _fCost;
    
    public Road(Transform transform)
    {
        this._transform = transform;
        this.name = transform.gameObject.name;

        if (transform.eulerAngles.y == 180)
            this.origin = transform.position + new Vector3(-5f, 0f, 5f);
        else if (transform.eulerAngles.y == 0f)
            this.origin = transform.position + new Vector3(5f, 0f, -5f);
        else if (transform.eulerAngles.y == 270f)
            this.origin = transform.position + new Vector3(5f, 0f, 5f);
        else if (transform.eulerAngles.y == 90f)
            this.origin = transform.position + new Vector3(-5f, 0f, -5f);
    }

    public Road previous
    {
        get { return _previous; }
        set { _previous = value; }
    }

    public Transform transform
    {
        get { return _transform; }
        set { _transform = value; }
    }

    public bool drivable
    {
        get
        {
            return !Physics.CheckSphere(origin, 10f, 1 << LayerMask.NameToLayer("NPC Car"));
        }
    }

    public Vector3 origin
    {
        get { return _origin; }
        set { _origin = value; }
    }

    public string name
    {
        get { return _name; }
        set { _name = value; }
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

    private CarController player;

    private List<Road> currentPath;

    // Use this for initialization
    void Start() {
        body = GetComponent<Rigidbody>();

        roads = new List<Road>();
        foreach (Transform road in GameObject.Find("Roads").transform)
        {
            roads.Add(new Road(road.transform));
        }

        player = FindObjectOfType<CarController>();
        currentPath = new List<Road>();
    }

    public void ApplyLocalPositionToVisuals(WheelCollider collider, Vector3 offset)
    {
        if (collider.transform.childCount == 0)
            return;

        Transform visualWheel = collider.transform.GetChild(0);

        Vector3 position;
        Quaternion rotation;
        collider.GetWorldPose(out position, out rotation);
        collider.transform.position = position;
        collider.transform.rotation = rotation;
    }

    private int ManhattanDistance(Road start, Road end)
    {
        return Mathf.RoundToInt(Mathf.Abs(end.origin.x - start.origin.x) + Mathf.Abs(end.origin.z - start.origin.z));
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
        open.Add(start);

        HashSet<Road> closed = new HashSet<Road>();
        int counter = 0;

        while (open.Count > 0)
        {
            Road current = open[0];

            for (int i = 0; i < open.Count; i++)
            {
                if (open[i].fCost < current.fCost || open[i].fCost == current.fCost && open[i].hCost < current.hCost)
                {
                    current = open[i];
                }
            }

            if (current.transform == end.transform)
            {
                ConstructPath(start, end);
                return;
            }

            open.Remove(current);
            closed.Add(current);

            foreach (Road neighbor in GetNearByRoads(current))
            {
                if (!neighbor.drivable || closed.Contains(neighbor))
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

            counter++;
        }
    }

    private int RoundAngle(float n)
    {
        int a = (int)(n / 10) * 10; //Smallest multiple
        int b = a + 10; //Largest mulitple
        return (n - a > b - n) ? b : a; //Return closest of two
    }

    private void ConstructPath(Road start, Road end)
    {
        List<Road> path = new List<Road>();
        Road current = end;

        while (current.transform != start.transform)
        {
            path.Add(current);
            current = current.previous;
        }
        path.Reverse();

        currentPath = path;
    }

    private void ShowPath()
    {
        if (currentPath.Count <= 0)
            return;

        for (int i = currentPath.Count - 1; i > 0; i--)
        {
            Vector3 start = currentPath[i].origin + Vector3.up;
            Vector3 end = currentPath[i - 1].origin + Vector3.up;

            Debug.DrawLine(start, end);
        }

        Vector3 next = currentPath[0].origin + Vector3.up;
        Vector3 current = FindRoadWith(transform).origin + Vector3.up;
        Debug.DrawLine(next, current);
    }

    private void TurnWheels(float steerAngle)
    {
        wheelFL.steerAngle = Mathf.Lerp(wheelFL.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
        wheelFR.steerAngle = Mathf.Lerp(wheelFR.steerAngle, steerAngle, Time.deltaTime * turnSpeed);
    }

    private Road FindRoadWith(Transform car)
    {
        RaycastHit ground;

        if (Physics.Raycast(car.position, -car.up, out ground, Mathf.Infinity, 1 << LayerMask.NameToLayer("Road")))
        {
            return roads.Find(road => ground.collider.transform == road.transform);
        }

        return null;
    }

    // Update is called once per frame
    private void Update()
    {
        Road currentRoad = FindRoadWith(transform);
        Road playerRoad = FindRoadWith(player.transform);
        
        FindPathBetween(currentRoad, playerRoad);
        ShowPath();
    }

    void FixedUpdate () {
        
        currentSpeed = body.velocity.magnitude * 2.237f;
        wheelFL.motorTorque = currentSpeed < maxSpeed ? maxMotorTorque : 0f;
        wheelFR.motorTorque = currentSpeed < maxSpeed ? maxMotorTorque : 0f;
        
        if (currentPath.Count > 0)
        {
            Road nextRoad = currentPath[0];
            Vector3 nextVector = transform.InverseTransformPoint(nextRoad.origin);
            float nextSteer = (nextVector.x / nextVector.magnitude) * maxSteerAngle;
            TurnWheels(nextSteer);
        }
        

        ApplyLocalPositionToVisuals(wheelFL, Vector3.forward * -0.5f);
        ApplyLocalPositionToVisuals(wheelFR, Vector3.forward * 0.5f);
        ApplyLocalPositionToVisuals(wheelBL, Vector3.forward * -0.5f);
        ApplyLocalPositionToVisuals(wheelBR, Vector3.forward * 0.5f);
    }
}
