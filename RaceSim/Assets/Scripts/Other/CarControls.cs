using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Class of wheel sets, defines what objects are being controlled and how
/// </summary>
[System.Serializable]
public class WheelSet {
    public WheelCollider leftWheel;
    public WheelCollider rightWheel;
    public bool motor;
    public bool steering;
}

/// <summary>
/// Manages all car controls regardless of the user e.g. human or machine
/// </summary>
public class CarControls : MonoBehaviour {
    public List<WheelSet> wheelSets;
    public float maximumMotorTorque;
    public float maximumSteeringAngle;
    public float brakeForce;
    public Vector3 centerOfMassCorrection;
    private AudioSource engine;

    void Start() {
        GetComponent<Rigidbody>().centerOfMass = centerOfMassCorrection;
        engine = GetComponent<AudioSource>();
    }

    /// <summary>
    /// Applies the give values and translates them into movement for the car object.
    /// There are minor differences between the AI and Human controls here
    /// </summary>
    /// <param name="_steering">Horazontal Axis Value</param>
    /// <param name="_motor">Vertical Axis Value</param>
    /// <param name="_braking">Pass true if braking</param>
    /// <param name="_ai">Pass true if this movement is performed by the AI / ML</param>
    public void PerformMovement(float _steering, float _motor, bool _braking, bool _ai) {
        if (_motor > 0f && engine.pitch < 3) {
            engine.pitch += 0.1f;
        } else if (engine.pitch > 1) {
            engine.pitch -= 0.2f;
        }
        if (_ai) {
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

    /// <summary>
    /// Additional function to prevent additional movement after a respawn
    /// </summary>
    public void CompleteStop() {
        PerformMovement(0f, 0f, true, false);
        foreach (WheelSet wheels in wheelSets) {
            wheels.leftWheel.steerAngle = 0f;
            wheels.leftWheel.motorTorque = 0f;
            wheels.leftWheel.brakeTorque = 0f;
            wheels.rightWheel.steerAngle = 0f;
            wheels.rightWheel.motorTorque = 0f;
            wheels.rightWheel.brakeTorque = 0f;
        }
    }
}
