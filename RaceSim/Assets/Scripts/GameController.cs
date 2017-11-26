using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject car;
    private float gameTimer;
    private int gameAttempts;
    private Vector3 spawnPoint;
    private Quaternion quaternion = Quaternion.AngleAxis(90f, Vector3.up);
    private Transform resetPosition;
    private CarManager cm;

    private void Start() {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        gameAttempts = 1;
        ResetGame();
        cm = FindObjectOfType<CarManager>();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
    }

    public void ResetGame(GameObject _car) {
        Destroy(_car);
        gameAttempts++;
        ResetGame();
    }

    public void ResetGame() {
        gameTimer = 0f;
        if (GameObject.FindGameObjectsWithTag("Player").Length <= 1)
        {
            Instantiate(car, spawnPoint, Quaternion.AngleAxis(90f, Vector3.up));
        }
    }

    public void ResetCar()
    {
        gameAttempts++;
        cm.ResetPosition(spawnPoint, quaternion);
    }

    public void FinishGame(GameObject _car) { 
        print("Finish Time = " + gameTimer);
        print("It took you " + gameAttempts + " attempts");
        ResetGame(_car);
    }
}
