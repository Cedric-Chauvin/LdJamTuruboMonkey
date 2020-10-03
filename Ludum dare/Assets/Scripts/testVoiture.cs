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
    public Transform massCenter;

    [Header("Speed var")]
    public float maxSpeed;
    public float coefAcceleration;
    public float Torque = 20000;
    public float brake = 30000;

    [Header("Turn var")]
    public float maxAngle = 10;
    public float driftValue = 10;
    public float normaldriftValue = 0.2f;

    private List<WheelCollider> wheels = new List<WheelCollider>();
    private bool isDrifting 
    {
        set
        {
            foreach (var item in wheels)
            {
                WheelFrictionCurve sidewaysFriction = item.sidewaysFriction;
                if (value)
                    sidewaysFriction.extremumSlip = driftValue;
                else
                    sidewaysFriction.extremumSlip = normaldriftValue;
                item.sidewaysFriction = sidewaysFriction;
            }
        }
    }


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = massCenter.localPosition;
        wheels.Add(frontRight);
        wheels.Add(frontLeft);
        wheels.Add(backLeft);
        wheels.Add(backRight);
        WheelSetup();
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

        if(Input.GetKeyDown(KeyCode.Space))
            isDrifting = true;

        if (Input.GetKeyUp(KeyCode.Space))
            isDrifting = false;

        frontLeft.steerAngle = Input.GetAxis("Horizontal") * maxAngle;
        frontRight.steerAngle = Input.GetAxis("Horizontal") * maxAngle;

    }

    private void OnValidate()
    {
        WheelSetup();
    }

    void WheelSetup()
    {
        foreach (var item in wheels)
        {
            WheelFrictionCurve sidewaysFriction = item.sidewaysFriction;
            sidewaysFriction.extremumSlip = normaldriftValue;
            item.sidewaysFriction = sidewaysFriction;
        }
    }
}
