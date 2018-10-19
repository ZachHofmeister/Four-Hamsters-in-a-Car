using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour {

    public bool canLookAround = false;

    public float turnSpeed = 4.0f;
    public Transform target;

    void Update()
    {
        bool ctrlKeyPressed = Input.GetKeyDown(KeyCode.LeftControl) || Input.GetKeyDown(KeyCode.RightControl);
        if (ctrlKeyPressed)
            canLookAround = !canLookAround;
    }

    void LateUpdate()
    {
        if (canLookAround)
        {
            transform.RotateAround(target.position, Vector3.up, Input.GetAxis("Mouse X") * turnSpeed);

        }
        else
        {
            transform.localPosition = Vector3.Lerp(transform.localPosition, Vector3.zero, turnSpeed * Time.deltaTime);
            transform.localRotation = Quaternion.Lerp(transform.localRotation, Quaternion.identity, turnSpeed * Time.deltaTime);
        }
    }

}
