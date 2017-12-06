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

    void Start() {
        GetComponent<Rigidbody>().centerOfMass = centerOfMassCorrection;
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
        PerformMovement(0f, 0f, true, false);
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
