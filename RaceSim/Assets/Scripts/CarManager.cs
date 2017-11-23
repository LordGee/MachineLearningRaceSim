using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

    private GameController gameControl;
    private CarControls cc;
    private EntityManager em;

    [SerializeField] public bool machineAI;

    void Start() {
        gameControl = FindObjectOfType<GameController>();
        cc = FindObjectOfType<CarControls>();
        if (machineAI)
        {
            em = new EntityManager();
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            gameControl.ResetCar();
        }
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "FinishLine") {
            gameControl.FinishGame(gameObject);
        }
    }

    public bool GetMachineAI() { return machineAI; }

    public void ResetPosition(Vector3 _pos, Quaternion _quat)
    {
        transform.position = _pos;
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        cc.CompleteStop();
        transform.rotation = _quat;
    }
}
