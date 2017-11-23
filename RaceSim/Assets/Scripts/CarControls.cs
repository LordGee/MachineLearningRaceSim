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

    private CarManager cm;
    // private float motor;
    // private float steering;
    // private bool braking;

    void Start() {
        GetComponent<Rigidbody>().centerOfMass = centerOfMassCorrection;
        cm = FindObjectOfType<CarManager>();
    }

    public void FixedUpdate() {
        if (!cm.GetMachineAI())
        {
            float motor = maximumMotorTorque * Input.GetAxis("Vertical");
            float steering = maximumSteeringAngle * Input.GetAxis("Horizontal");
            bool braking;
            if (Input.GetButton("Jump"))
            {
                braking = true;
            }
            else
            {
                braking = false;
            }
            PerformMovement(steering, motor,braking, false);
        }
        else
        {
            // Managed By AI
        }
        
    }

    private void PerformMovement()
    {
        /*
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
        */
    }

    public void PerformMovement(float _steering, float _motor, bool _braking, bool _ai)
    {
        if (_ai)
        {
            _steering = maximumSteeringAngle * _steering;
            _motor = maximumMotorTorque * _motor;
        }
        

        foreach (WheelSet wheels in wheelSets) {
            if (wheels.steering) {
                wheels.leftWheel.steerAngle = _steering;
                wheels.rightWheel.steerAngle = _steering;
            }
            if (wheels.motor) {
                wheels.leftWheel.motorTorque = _motor;
                wheels.rightWheel.motorTorque = _motor;
            }
            if (_braking) {
                wheels.leftWheel.brakeTorque = brakeForce;
                wheels.rightWheel.brakeTorque = brakeForce;
            } else {
                wheels.leftWheel.brakeTorque = 0;
                wheels.rightWheel.brakeTorque = 0;
            }
        }
    }

    public void CompleteStop()
    {
        foreach (WheelSet wheels in wheelSets)
        {
            wheels.leftWheel.steerAngle = 0f;
            wheels.leftWheel.motorTorque = 0f;
            wheels.leftWheel.brakeTorque = 0f;
            wheels.rightWheel.steerAngle = 0f;
            wheels.rightWheel.motorTorque = 0f;
            wheels.rightWheel.brakeTorque = 0f;
        }
    }
}
