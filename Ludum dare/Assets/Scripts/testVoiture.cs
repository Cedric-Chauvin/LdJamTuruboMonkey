using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class testVoiture : MonoBehaviour
{
    [Header("Debug")]
    public float speed;

    [Header("Wheel")]
    public WheelCollider frontRight;
    public WheelCollider frontLeft;
    public WheelCollider backLeft;
    public WheelCollider backRight;
    public Rigidbody rigidbody;

    [Header("Speed var")]
    public float maxSpeed;
    public float coefAcceleration;
    public float Torque = 20000;
    public float brake = 30000;

    [Header("Turn var")]
    public float maxAngle = 10;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = new Vector3(0, 0, 0f);
    }

    // Update is called once per frame
    void Update()
    {
        speed = GetComponent<Rigidbody>().velocity.magnitude * 3.6f;

        if (Input.GetKey(KeyCode.Z))
        {
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;
            backLeft.motorTorque = Time.deltaTime * Torque * coefAcceleration;
            backRight.motorTorque = Time.deltaTime * Torque * coefAcceleration;
        }

        if(!Input.GetKey(KeyCode.Z))
        {
            backLeft.motorTorque = 0;
            backRight.motorTorque = 0;
            backLeft.brakeTorque = Time.deltaTime * brake * coefAcceleration;
            backRight.brakeTorque = Time.deltaTime * brake * coefAcceleration;
        }

        frontLeft.steerAngle = Input.GetAxis("Horizontal") * maxAngle;
        frontRight.steerAngle = Input.GetAxis("Horizontal") * maxAngle;

    }
}
