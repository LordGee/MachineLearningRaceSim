using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

    private GameController gameControl;
    private CarControls cc;
    private InputManager im;
    private EntityManager em;

    [SerializeField] public bool machineAI;

    void Awake() {
        gameControl = FindObjectOfType<GameController>();
        cc = FindObjectOfType<CarControls>();
        im = GetComponent<InputManager>();
        if (machineAI)
        {
            em = new EntityManager();
        }
    }

    public static float timeSpeed = 1f;
    void Update()
    {
        Time.timeScale = timeSpeed;

        if (machineAI)
        {
            if (Input.GetKeyDown("1")) { timeSpeed = 1; }
            if (Input.GetKeyDown("2")) { timeSpeed = 2; }
            if (Input.GetKeyDown("3")) { timeSpeed = 3; }
            if (Input.GetKeyDown("4")) { timeSpeed = 4; }

            im.UpdateInputs();

            List<float> currentInputs = new List<float>();
            for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
                // get raycast events and add distance to the input list
                currentInputs.Add(im.GetInputByIndex(i));
            }

            em.PrepareInputs(currentInputs);
            em.ManualUpdate();
            float[] newOutputs = em.GetCurrentOutputs();
            cc.PerformMovement(
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT] - newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT], 
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE], 
                (newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_BRAKE] > 0.5), 
                true);
            em.SetNewFitness(im.GetAcceleration() * Time.deltaTime, timeSpeed);
            if (em.GetResetPosition())
            {
                gameControl.ResetCar(); 
                em.CompleteResetPosition();
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            em.AgentFailed();
            gameControl.ResetCar();
        }
    }

    private IEnumerator DelayRespawn() {
        yield return new WaitForSeconds(0.1f);
        // StartCoroutine(DelayRespawn());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "FinishLine") {
            em.AddCompletionFitness(1000f);
            em.AgentFailed();
            gameControl.ResetCar();
            // gameControl.FinishGame(gameObject);
        }
    }

    public bool GetMachineAI() { return machineAI; }

    public void ResetPosition(Vector3 _pos, Quaternion _quat)
    {
        cc.CompleteStop();
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().drag = 0f;
        transform.position = _pos;
        transform.rotation = _quat;
    }
}
