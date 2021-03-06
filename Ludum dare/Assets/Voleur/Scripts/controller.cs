﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class controller : MonoBehaviour
{
    internal enum driveType{
        frontWheelDrive,
        rearWheelDrive,
        allWheelDrive
    }
    [SerializeField]private driveType drive;

    private carEffects CarEffects;
    [HideInInspector]public bool test; //engine sound boolean

    [Header("Variables")]
    public float turnMultiplicateur = 1;
    public float handBrakeFrictionMultiplier = 2f;
    public float maxRPM , minRPM;
    public float[] gears;
    public float[] gearChangeSpeed;
    public AnimationCurve enginePower;
    public float OffRoadMalus = 1;


    public int gearNum = 1;
    [HideInInspector]public bool playPauseSmoke = false,hasFinished;
    [HideInInspector]public float KPH;
    [HideInInspector]public float engineRPM;
    [HideInInspector]public bool reverse = false;
    [HideInInspector]public float nitrusValue;
    [HideInInspector]public bool nitrusFlag =false;


    private GameObject wheelMeshes,wheelColliders;
    private WheelCollider[] wheels = new WheelCollider[4];
    private GameObject[] wheelMesh = new GameObject[4];
    private GameObject centerOfMass;
    private Rigidbody rigidbody;

    //car Shop Values
    public int carPrice ;
    public string carName;
    private float smoothTime = 0.09f;
    private float currentOffRoadValue = 0.1f;


	private WheelFrictionCurve  forwardFriction,sidewaysFriction;
    private float radius = 6, brakPower = 0, DownForceValue = 10f,wheelsRPM ,driftFactor, lastValue ,horizontal , vertical,totalPower;
    private bool flag=false;

    private float obstacleSlowValue = 0;
    private Coroutine SlowRoutine;

    private void Awake() {

        if(SceneManager.GetActiveScene().name == "awakeScene")return;
        getObjects();
        StartCoroutine(timedLoop());
    }

    private void Update() {

        if(SceneManager.GetActiveScene().name == "awakeScene")return;

        horizontal = Input.GetAxis("Horizontal");
        vertical = Input.GetAxis("Vertical");

        

        lastValue = engineRPM;


        addDownForce();
        animateWheels();
        steerVehicle();
        calculateEnginePower();
        if(gameObject.tag == "AI")return;
        adjustTraction();
        activateNitrus();
    }

    private void calculateEnginePower(){

        wheelRPM();

            if (vertical != 0 ){
                rigidbody.drag = 0.005f; 
            }
            if (vertical == 0){
                rigidbody.drag = 0.1f;
            }
            totalPower = 3.6f * enginePower.Evaluate(engineRPM) * (vertical);

        


        float velocity  = 0.0f;
        if (engineRPM >= maxRPM || flag ){
            engineRPM = Mathf.SmoothDamp(engineRPM, maxRPM - 500, ref velocity, 0.05f);

            flag = (engineRPM >= maxRPM - 450)?  true : false;
            test = (lastValue > engineRPM) ? true : false;
        }
        else { 
            engineRPM = Mathf.SmoothDamp(engineRPM,1000 + (Mathf.Abs(wheelsRPM) * 3.6f * (gears[gearNum])), ref velocity , smoothTime);
            test = false;
        }
        if (engineRPM >= maxRPM + 1000) engineRPM = maxRPM + 1000; // clamp at max
        moveVehicle();
    shifter();
    }

    private void wheelRPM(){
        float sum = 0;
        int R = 0;
        for (int i = 0; i < 4; i++)
        {
            sum += wheels[i].rpm;
            R++;
        }
        wheelsRPM = (R != 0) ? sum / R : 0;
 
        if(wheelsRPM < 0 && !reverse ){
            reverse = true;
        }
        else if(wheelsRPM > 0 && reverse){
            reverse = false;
        }
    }

    private bool checkGears(){
        if(KPH >= gearChangeSpeed[gearNum] ) return true;
        else return false;
    }

    private void shifter(){

        if(!isGrounded())return;
            //automatic
        if(engineRPM > maxRPM && gearNum < gears.Length-1 && !reverse && checkGears()){
            gearNum ++;
            return;
        }
        if(engineRPM < minRPM && gearNum > 0){
            gearNum --;
        }

    }
 
    public bool isGrounded(){
        if(wheels[0].isGrounded &&wheels[1].isGrounded &&wheels[2].isGrounded &&wheels[3].isGrounded )
            return true;
        else
            return false;
    }

    private void moveVehicle(){

        brakeVehicle();

        if (drive == driveType.allWheelDrive){
            for (int i = 0; i < wheels.Length; i++){
                wheels[i].motorTorque = ((totalPower / 4) * turnMultiplicateur)/ (currentOffRoadValue * 10);
                wheels[i].brakeTorque = brakPower * turnMultiplicateur;
            }
        }else if(drive == driveType.rearWheelDrive){
            wheels[2].motorTorque = ((totalPower / 2) * turnMultiplicateur) / (currentOffRoadValue * 10);
            wheels[3].motorTorque = ((totalPower / 2) * turnMultiplicateur) / (currentOffRoadValue * 10);

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower * turnMultiplicateur;
            }
        }
        else{
            wheels[0].motorTorque = ((totalPower / 2) * turnMultiplicateur) / (currentOffRoadValue * 10);
            wheels[1].motorTorque = ((totalPower / 2) * turnMultiplicateur) / (currentOffRoadValue * 10);

            for (int i = 0; i < wheels.Length; i++)
            {
                wheels[i].brakeTorque = brakPower * turnMultiplicateur;
            }
        }
        KPH = rigidbody.velocity.magnitude * 3.6f;
        UIManager.Instance.updateSpeed(KPH);

    }

    private void brakeVehicle(){

        if (vertical < 0){
            brakPower =(KPH >= 50)? 500 : 0;
        }
        else if (vertical == 0 &&(KPH <= 10 || KPH >= -10)){
            brakPower = 10;
        }
        else{
            brakPower = 0;
        }


    }
  
    private void steerVehicle(){


        //acerman steering formula
        //steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontalInput;

        if (horizontal > 0 ) {
				//rear tracks size is set to 1.5f       wheel base has been set to 2.55f
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
        } else if (horizontal < 0 ) {                                                          
            wheels[0].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius - (1.5f / 2))) * horizontal;
            wheels[1].steerAngle = Mathf.Rad2Deg * Mathf.Atan(2.55f / (radius + (1.5f / 2))) * horizontal;
			//transform.Rotate(Vector3.up * steerHelping);

        } else {
            wheels[0].steerAngle =0;
            wheels[1].steerAngle =0;
        }

    }

    private void animateWheels ()
	{
		Vector3 wheelPosition = Vector3.zero;
		Quaternion wheelRotation = Quaternion.identity;

		for (int i = 0; i < 4; i++) {
			wheels [i].GetWorldPose (out wheelPosition, out wheelRotation);
			wheelMesh [i].transform.position = wheelPosition;
			wheelMesh [i].transform.rotation = wheelRotation;
		}
	}
   
    private void getObjects(){
        //else aIcontroller = GetComponent<AIcontroller>();
        CarEffects = GetComponent<carEffects>();
        rigidbody = GetComponent<Rigidbody>();
        wheelColliders = gameObject.transform.Find("wheelColliders").gameObject;
        wheelMeshes = gameObject.transform.Find("wheelMeshes").gameObject;
        wheels[0] = wheelColliders.transform.Find("0").gameObject.GetComponent<WheelCollider>();
        wheels[1] = wheelColliders.transform.Find("1").gameObject.GetComponent<WheelCollider>();
        wheels[2] = wheelColliders.transform.Find("2").gameObject.GetComponent<WheelCollider>();
        wheels[3] = wheelColliders.transform.Find("3").gameObject.GetComponent<WheelCollider>();

        wheelMesh[0] = wheelMeshes.transform.Find("0").gameObject;
        wheelMesh[1] = wheelMeshes.transform.Find("1").gameObject;
        wheelMesh[2] = wheelMeshes.transform.Find("2").gameObject;
        wheelMesh[3] = wheelMeshes.transform.Find("3").gameObject;





        centerOfMass = gameObject.transform.Find("mass").gameObject;
        rigidbody.centerOfMass = centerOfMass.transform.localPosition;   
    }

    private void addDownForce(){

        rigidbody.AddForce(-transform.up * DownForceValue * rigidbody.velocity.magnitude );

    }

    private void adjustTraction(){
            //tine it takes to go from normal drive to drift 
        float driftSmothFactor = .7f * Time.deltaTime;

		if(Input.GetKey(KeyCode.Space)){
            sidewaysFriction = wheels[0].sidewaysFriction;
            forwardFriction = wheels[0].forwardFriction;

            float velocity = 0;
            sidewaysFriction.extremumValue =sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =
                Mathf.SmoothDamp(forwardFriction.asymptoteValue,driftFactor * handBrakeFrictionMultiplier,ref velocity ,driftSmothFactor );

            for (int i = 0; i < 4; i++) {
                wheels [i].sidewaysFriction = sidewaysFriction;
                wheels [i].forwardFriction = forwardFriction;
            }

            sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = forwardFriction.extremumValue = forwardFriction.asymptoteValue =  1.1f;
                //extra grip for the front wheels
            for (int i = 0; i < 2; i++) {
                wheels [i].sidewaysFriction = sidewaysFriction;
                wheels [i].forwardFriction = forwardFriction;
            }
		}
            //executed when handbrake is being held
        else{

			forwardFriction = wheels[0].forwardFriction;
			sidewaysFriction = wheels[0].sidewaysFriction;

			forwardFriction.extremumValue = forwardFriction.asymptoteValue = sidewaysFriction.extremumValue = sidewaysFriction.asymptoteValue = 
                ((KPH * handBrakeFrictionMultiplier) / 300) + 1;

			for (int i = 0; i < 4; i++) {
				wheels [i].forwardFriction = forwardFriction;
				wheels [i].sidewaysFriction = sidewaysFriction;

			}
        }

            //checks the amount of slip to control the drift
		for(int i = 2;i<4 ;i++){

            WheelHit wheelHit;

            wheels[i].GetGroundHit(out wheelHit);
                //smoke

            if(wheelHit.sidewaysSlip >= 0.3f || wheelHit.sidewaysSlip <= -0.3f)
                playPauseSmoke = true;
            else
                playPauseSmoke = false;

            if (wheelHit.collider && wheelHit.collider.tag == "OffRoad")
                currentOffRoadValue = OffRoadMalus;
            else
                currentOffRoadValue = 0.1f;
                        

			if(wheelHit.sidewaysSlip < 0 )	driftFactor = (1 + -Input.GetAxis("Horizontal")) * Mathf.Abs(wheelHit.sidewaysSlip) ;

			if(wheelHit.sidewaysSlip > 0 )	driftFactor = (1 + Input.GetAxis("Horizontal"))* Mathf.Abs(wheelHit.sidewaysSlip );
		}	
		
	}

	private IEnumerator timedLoop(){
		while(true){
			yield return new WaitForSeconds(.7f);
            radius = 6 + KPH /5;
            
		}
	}

    public void activateNitrus(){
        if (!Input.GetKey(KeyCode.LeftShift) && nitrusValue <= 10 ){
            nitrusValue += Time.deltaTime / 2;
        }
        else{
            nitrusValue -= (nitrusValue <= 0 ) ? 0 : Time.deltaTime;
        }

        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (nitrusValue > 0) { 
                CarEffects.startNitrusEmitter();
                rigidbody.AddForce(transform.forward * 5000);
            } 
            else CarEffects.stopNitrusEmitter();
        }
        else CarEffects.stopNitrusEmitter();

    }

    public void ObstacleSlowDown(float time, float slow)
    {
        if (SlowRoutine!=null)
            StopCoroutine(SlowRoutine);
        //SlowRoutine = StartCoroutine(SlowDown(time, slow));
        Debug.Log(-transform.forward * slow * rigidbody.velocity.magnitude * 2);
        rigidbody.AddForce(-transform.forward * slow*rigidbody.velocity.magnitude*5000);
        
    }

    IEnumerator SlowDown(float time, float slow)
    {
        float timer = 0;
        obstacleSlowValue += slow;
        while (timer<time)
        {
            timer += 0.1f;
            yield return new WaitForSeconds(0.1f);
        }
        obstacleSlowValue -= slow;
        SlowRoutine = null;
    }
}
