using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameController : MonoBehaviour {

    [SerializeField] private GameObject car, humanCar;
    private float gameTimer;
    private int gameAttempts;
    private Vector3 spawnPoint, humanSpawnPoint;
    private Quaternion quaternion = Quaternion.AngleAxis(90f, Vector3.up);
    private Transform resetPosition;
    private CarManager cm;
    private HumanCarManager hcm;
    private PlayerPrefsController pp;

    private void Start() {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        humanSpawnPoint = GameObject.FindGameObjectWithTag("SpawnHuman").transform.position;
        gameAttempts = 1;
        pp = FindObjectOfType<PlayerPrefsController>();
        ResetGame();
        if (pp.GetLearning() || pp.GetLoading())
        { // Check that each car uses the right Car Manager Script
            cm = FindObjectOfType<CarManager>();
        }
        if (!pp.GetLearning())
        {
            hcm = FindObjectOfType<HumanCarManager>();
        }
        SetCorrectHUD();
    }

    private void Update()
    {
        gameTimer += Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(0);
        }
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
            if (pp.GetLearning() || pp.GetLoading())
            {
                Instantiate(car, spawnPoint, Quaternion.AngleAxis(90f, Vector3.up));
            }
            if (!pp.GetLearning() && !pp.GetLoading() || !pp.GetLearning() && pp.GetLoading())
            {
                Instantiate(humanCar, humanSpawnPoint, Quaternion.AngleAxis(90f, Vector3.up));
            }
        }
    }

    public void ResetCar(bool _cm)
    {
        gameAttempts++;
        if (_cm)
        {
            cm.ResetPosition(spawnPoint, quaternion);
        }
        else
        {
            hcm.ResetPosition(humanSpawnPoint, quaternion);
        }
        
    }

    public void FinishGame(GameObject _car) { 
        print("Finish Time = " + gameTimer);
        print("It took you " + gameAttempts + " attempts");
        ResetGame(_car);
    }

    private void SetCorrectHUD()
    {
        GameObject speed = GameObject.Find("SpeedHUDCanvas");
        GameObject genetic = GameObject.Find("GeneticHUDCanvas");
        if (pp.GetHUD())
        {
            genetic.SetActive(false);
        }
        else
        {
            speed.SetActive(false);
        }
    }
}
