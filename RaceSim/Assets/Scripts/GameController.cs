using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject car;
    private float gameTimer;
    private int gameAttempts;
    private Vector3 spawnPoint;

    private void Start() {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        gameAttempts = 1;
        ResetGame();
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

    public void FinishGame(GameObject _car) { 
        print("Finish Time = " + gameTimer);
        print("It took you " + gameAttempts + " attempts");
        ResetGame(_car);
    }
}
