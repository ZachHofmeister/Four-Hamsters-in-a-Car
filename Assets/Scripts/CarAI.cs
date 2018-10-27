using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class CarAI : MonoBehaviour {

    public Transform target;

    private NavMeshAgent navMesh;

	// Use this for initialization
	void Start () {
        navMesh = GetComponent<NavMeshAgent>();
	}

    private void FixedUpdate()
    {
        navMesh.SetDestination(target.position);

    }
}
