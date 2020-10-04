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
    public AnimationCurve turnCurve;
    public float handBrakeFrictionMultiplier;

    private List<WheelCollider> wheels = new List<WheelCollider>();


    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        rigidbody.centerOfMass = massCenter.localPosition;
        wheels.Add(frontRight);
        wheels.Add(frontLeft);
        wheels.Add(backLeft);
        wheels.Add(backRight);
    }

    // Update is called once per frame
    void Update()
    {
        speed = rigidbody.velocity.magnitude * 3.6f;

        if (Input.GetKey(KeyCode.Z))
        {
            backLeft.brakeTorque = 0;
            backRight.brakeTorque = 0;
            backLeft.motorTorque = Time.deltaTime * Torque * coefAcceleration;
            backRight.motorTorque = Time.deltaTime * Torque * coefAcceleration;
            rigidbody.drag = 0.005f;
        }

        if(!Input.GetKey(KeyCode.Z))
        {
            backLeft.motorTorque = 0;
            backRight.motorTorque = 0;
            backLeft.brakeTorque = Time.deltaTime * brake * coefAcceleration;
            backRight.brakeTorque = Time.deltaTime * brake * coefAcceleration;
            rigidbody.drag = 1;
        }

        /*if(Input.GetKeyDown(KeyCode.Space))
            isDrifting = true;

        if (Input.GetKeyUp(KeyCode.Space))
            isDrifting = false;*/

        SteerVehicle();
        AddDownForce();
        Friction();
    }

    private void SteerVehicle()
    {
        float horizontal = Input.GetAxis("Horizontal");
        if (horizontal > 0)
        {
            //rear tracks size is set to 1.5f       wheel base has been set to 10
            wheels[0].steerAngle = turnCurve.Evaluate(speed) * horizontal;
            wheels[1].steerAngle = turnCurve.Evaluate(speed) * horizontal;
        }
        else if (horizontal < 0)
        {
            wheels[0].steerAngle = turnCurve.Evaluate(speed) * horizontal;
            wheels[1].steerAngle = turnCurve.Evaluate(speed) * horizontal;
            //transform.Rotate(Vector3.up * steerHelping);

        }
        else
        {
            wheels[0].steerAngle = 0;
            wheels[1].steerAngle = 0;
        }

        Debug.Log(Mathf.Rad2Deg * Mathf.Atan(2f / (speed - (1.5f / 2))) * horizontal);

    }

    private void AddDownForce()
    {
        rigidbody.AddForce(-transform.up * 10 * rigidbody.velocity.magnitude);
    }

    private void Friction()
    {
        WheelFrictionCurve forwardFriction = wheels[0].forwardFriction;
        WheelFrictionCurve sidewaysFriction = wheels[0].sidewaysFriction;

        forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue =
            ((speed * handBrakeFrictionMultiplier) / 300) + 1;

        for (int i = 0; i < 4; i++)
        {
            wheels[i].forwardFriction = forwardFriction;
            wheels[i].sidewaysFriction = sidewaysFriction;

        }
    }
}
