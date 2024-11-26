using System;
using System.Collections.Generic;
using UnityEngine;

public class Rudder : MonoBehaviour
{
    //public Rigidbody boat;
    //public float rotationSpeed = 5.5f; 
    //public float maxAngle = 15f;
    //private Vector3 rudderThrust = new Vector3();
    //// Start is called before the first frame update
    //void Start()
    //{
    //}
//
    //// Update is called once per frame
    //void FixedUpdate()
    //{
    //    applySteeringThrust();
    //    //Vector3 rudderThrust = new Vector3(gameObject.transform.forward.x,0,gameObject.transform.forward.z) * (boat.velocity.magnitude*20000* angularEfficiency);
    //    //Debug.Log("rudder Angle"+Vector3.SignedAngle(transform.forward,transform.parent.forward,Vector3.up));
    //}
//
    //public void turnLeft(){
    //    float angularEfficiency = 1-Vector3.Angle(new Vector3(gameObject.transform.forward.x, 0, gameObject.transform.forward.z), new Vector3(boat.velocity.x, 0, boat.velocity.z))/180f;
    //    Debug.Log("kerroin" + gameObject.transform.localRotation.y/maxAngle+" angular "+angularEfficiency + " rotation " + gameObject.transform.localRotation.eulerAngles);
    //    rudderThrust = (gameObject.transform.rotation.y/maxAngle) * transform.right * (boat.velocity.magnitude*10000* angularEfficiency);
    //    rudderThrust.y = 0;
    //    if(Vector3.SignedAngle(transform.forward,transform.parent.forward,Vector3.up) < maxAngle){
    //        //Debug.Log("turning left because" + Vector3.SignedAngle(transform.forward,transform.parent.forward,Vector3.up)+ " < " + maxAngle);
    //        gameObject.transform.Rotate(-gameObject.transform.up*rotationSpeed);
    //    }
    //}
//
    //public void turnRight(){
    //    float angularEfficiency = 1-Vector3.Angle(new Vector3(gameObject.transform.forward.x, 0, gameObject.transform.forward.z), new Vector3(boat.velocity.x, 0, boat.velocity.z))/180f;
    //    Debug.Log("kerroin" + gameObject.transform.localRotation.y/maxAngle+" angular "+angularEfficiency + " rotation " + gameObject.transform.localRotation.eulerAngles);
    //    rudderThrust = (gameObject.transform.rotation.y/maxAngle) * transform.right * (boat.velocity.magnitude*10000* angularEfficiency);
    //    rudderThrust.y = 0;
    //    if(Vector3.SignedAngle(transform.forward,transform.parent.forward,Vector3.up) > -maxAngle){
    //        //Debug.Log("turning right because" + Vector3.SignedAngle(transform.forward,transform.parent.forward,Vector3.up)+ " > " + -maxAngle);
    //        gameObject.transform.Rotate(gameObject.transform.up*rotationSpeed);
    //    }
    //}
//
    //public void applySteeringThrust(){
    //    boat.AddForceAtPosition(rudderThrust,transform.position);
    //    Debug.DrawRay(transform.position,rudderThrust);
    //}
    public float rudderAngle = 30f; // Maximum angle the rudder can turn
    public float turnSpeed = 10f; // Speed at which the rudder turns
    public float thrustForce = 5f; // Force applied for steering

    private float currentRudderAngle = 0f; // Current angle of the rudder
    private Rigidbody boatRigidbody;

    void Start()
    {
        boatRigidbody = GetComponentInParent<Rigidbody>();
    }

    void Update()
    {
        // Clamp the current rudder angle to be within the maximum allowed angle
        currentRudderAngle = Mathf.Clamp(currentRudderAngle, -rudderAngle, rudderAngle);
        applySteeringThrust();
    }

    public void turnLeft()
    {
        // Turn the rudder left
        currentRudderAngle -= turnSpeed * Time.deltaTime;
        currentRudderAngle = Mathf.Clamp(currentRudderAngle, -rudderAngle, rudderAngle);
        applySteering();
    }

    public void turnRight()
    {
        // Turn the rudder right
        currentRudderAngle += turnSpeed * Time.deltaTime;
        currentRudderAngle = Mathf.Clamp(currentRudderAngle, -rudderAngle, rudderAngle);
        applySteering();
    }

    public void applySteeringThrust()
    {
        // Apply a force to the boat based on the current rudder angle
        float forwardVelocity = Vector3.Dot(boatRigidbody.velocity, boatRigidbody.transform.forward);
        float rudderToWaterAngle = Vector3.Angle(new Vector3(transform.right.x,0,transform.right.z),new Vector3(boatRigidbody.velocity.x,0,boatRigidbody.velocity.z));
        float rudderThrustModifier = Mathf.Abs(rudderToWaterAngle-90)/90;
        //Debug.Log(Mathf.Abs(rudderToWaterAngle-90)/90);
        Vector3 steeringForce = transform.right * currentRudderAngle * thrustForce * rudderThrustModifier *  forwardVelocity;
        boatRigidbody.AddForceAtPosition(steeringForce, transform.position);
    }

    private void applySteering()
    {
        // Adjust the rudder's rotation based on the current angle
        transform.localRotation = Quaternion.Euler(0f, currentRudderAngle, 0f);
    }
}

