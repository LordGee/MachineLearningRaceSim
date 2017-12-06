using System;
using System.Collections.Generic;
using UnityEngine;

public class HumanCarManager : MonoBehaviour
{

    private GameController gc;
    private CarControls cc;

    void Start()
    {
        gc = FindObjectOfType<GameController>();
        cc = GetComponent<CarControls>();
    }

    void FixedUpdate()
    {
        float motor = cc.maximumMotorTorque * Input.GetAxis("Vertical");
        float steering = cc.maximumSteeringAngle * Input.GetAxis("Horizontal");
        bool braking;
        if (Input.GetButton("Jump")) {
            braking = true;
        } else {
            braking = false;
        }
        // Debug.Log("Performed Movement");
        cc.PerformMovement(steering, motor, braking, false);
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            gc.ResetCar(false);
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.transform.tag == "FinishLine") {
            gc.ResetCar(false);
        }
    }

    public void ResetPosition(Vector3 _pos, Quaternion _quat) {
        cc.CompleteStop();
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().drag = 0f;
        transform.position = _pos;
        transform.rotation = _quat;
    }
}

