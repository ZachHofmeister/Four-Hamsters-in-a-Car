using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraFollow : MonoBehaviour {

    public Transform target;
    public float distance, height;
    public float rotationDampening, heightDampening;
    public float zoomRatio, defaultFOV;

    private Vector3 _rotation;

    private void FixedUpdate()
    {
        Vector3 localVelocity = target.InverseTransformDirection(target.GetComponent<Rigidbody>().velocity);
        if (localVelocity.z < -0.5f)
            _rotation.y = target.eulerAngles.y + 180;
        else
            _rotation.y = target.eulerAngles.y;

        float accleration = target.GetComponent<Rigidbody>().velocity.magnitude;
        Camera.main.fieldOfView = defaultFOV + accleration * zoomRatio;
    }

    private void LateUpdate()
    {
        float desiredAngle = _rotation.y;
        float desiredHeight = target.position.y + height;
        float currentAngle = transform.eulerAngles.y;
        float currentHeight = transform.position.y;

        currentAngle = Mathf.LerpAngle(currentAngle, desiredAngle, rotationDampening * Time.deltaTime);
        currentHeight = Mathf.Lerp(currentHeight, desiredHeight, heightDampening * Time.deltaTime);

        Quaternion currentRotation = Quaternion.Euler(0f, currentAngle, 0f);

        transform.position = target.position;
        transform.position -= currentRotation * Vector3.forward * distance;
        transform.position = new Vector3(transform.position.x, currentHeight, transform.position.z);
        transform.LookAt(target);
    }
}
