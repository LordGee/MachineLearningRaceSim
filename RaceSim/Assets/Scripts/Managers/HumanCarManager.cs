using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HumanCarManager : MonoBehaviour {

    private GameController gc;
    private CarControls cc;
    private PlayerPrefsController pp;
    private float lapTime, previousTime, bestLapTime;

    void Start() {
        gc = FindObjectOfType<GameController>();
        pp = FindObjectOfType<PlayerPrefsController>();
        cc = GetComponent<CarControls>();
        lapTime = previousTime = 0f;
        bestLapTime = pp.GetBestTime(SceneManager.GetActiveScene().buildIndex);
        if (bestLapTime == 0f)
        {
            bestLapTime = 9999f;
        }
        EventManager.TriggerEvent(ConstantManager.UI_HUMAN, bestLapTime);
    }

    void Update() {
        lapTime += Time.deltaTime;
        if (lapTime - previousTime > 1f) {
            EventManager.TriggerEvent(ConstantManager.UI_TIMER, lapTime);
            previousTime = lapTime;
        }
        float motor = cc.maximumMotorTorque * Input.GetAxis("Vertical");
        float steering = cc.maximumSteeringAngle * Input.GetAxis("Horizontal");
        bool braking;
        if (Input.GetButton("Jump")) {
            braking = true;
        } else {
            braking = false;
        }
        cc.PerformMovement(steering, motor, braking, false);
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            gc.ResetCar(false);
        }
    }

    void OnTriggerEnter(Collider col) {
        if (col.transform.tag == "FinishLine") {
            UpdateTimerResults();
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
        lapTime = previousTime = 0f;
        EventManager.TriggerEvent(ConstantManager.UI_TIMER, lapTime);
    }

    private void UpdateTimerResults() {
        if (lapTime < bestLapTime) {
            bestLapTime = lapTime;
            EventManager.TriggerEvent(ConstantManager.UI_HUMAN, lapTime);
            pp.SetBestTime(lapTime, SceneManager.GetActiveScene().buildIndex);
        }
    }
}

