using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class WheelSet {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

public class CarControls : MonoBehaviour {
    public List<WheelSet> wheelSets;
    public float maximumMotorTorque;
    public float maximumSteeringAngle;
    public float brakeForce;
    public Vector3 centerOfMassCorrection;
    public bool controllerEnabled;

    void Start() {
        GetComponent<Rigidbody>().centerOfMass = centerOfMassCorrection;
    }

    public void FixedUpdate() {
        if (controllerEnabled) {
            float motor = maximumMotorTorque * Input.GetAxis("Vertical");
            float steering = maximumSteeringAngle * Input.GetAxis("Horizontal");
            bool braking;
            if (Input.GetButton("Jump")) {
                braking = true;
            } else {
                braking = false;
            }
            foreach (WheelSet wheels in wheelSets) {
                if (wheels.steering) {
                    wheels.leftWheel.steerAngle = steering;
                    wheels.rightWheel.steerAngle = steering;
                }
                if (wheels.motor) {
                    wheels.leftWheel.motorTorque = motor;
                    wheels.rightWheel.motorTorque = motor;
                }
                if (braking) {
                    wheels.leftWheel.brakeTorque = brakeForce;
                    wheels.rightWheel.brakeTorque = brakeForce;
                } else {
                    wheels.leftWheel.brakeTorque = 0;
                    wheels.rightWheel.brakeTorque = 0;
                } 
            }
        }
    }
}
