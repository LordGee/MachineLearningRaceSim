using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

     private GameController gameControl;

    void Start() {
        gameControl = FindObjectOfType<GameController>();
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            gameControl.ResetGame(gameObject);
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "FinishLine") {
            gameControl.FinishGame(gameObject);
        }
    }
}
