using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

    private GameController gc;
    private CarControls cc;
    private InputManager im;
    private EntityManager em;
    private PlayerPrefsController pp;

    public static bool machineAI, loadBest;
    public static float timeSpeed = 1f;

    void Start()
    {
        pp = FindObjectOfType<PlayerPrefsController>();
        machineAI = pp.GetLearning();
        loadBest = pp.GetLoading();
        gc = FindObjectOfType<GameController>();
        cc = FindObjectOfType<CarControls>();
        im = GetComponent<InputManager>();
        if (machineAI || loadBest)
        {
            em = new EntityManager();
        }
    }
    
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
                (newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_BRAKE] > 0.8), 
                true);
            em.SetNewFitness(im.GetAcceleration() * Time.deltaTime);
            if (em.GetResetPosition())
            {
                gc.ResetCar(); 
                em.CompleteResetPosition();
            }
        }
        if (loadBest)
        {
            // Obtain current inputs
            im.UpdateInputs();
            List<float> currentInputs = new List<float>();
            for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
                // get raycast events and add distance to the input list
                currentInputs.Add(im.GetInputByIndex(i));
            }

            // Set up entity
            em.PrepareInputs(currentInputs); // Adds inputs to entity variable
            em.ManualUpdate();
            float[] newOutputs = em.GetCurrentOutputs();
            cc.PerformMovement(
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT] - newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT],
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE],
                (newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_BRAKE] > 0.8),
                true);
            // em.SetNewFitness(im.GetAcceleration() * Time.deltaTime);
            if (em.GetResetPosition()) {
                gc.ResetCar();
                em.CompleteResetPosition();
            }
        }
    }

    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            if (machineAI || loadBest)
            {
                em.AgentFailed();
            }
            gc.ResetCar();
        }
    }

    private IEnumerator DelayRespawn() {
        yield return new WaitForSeconds(0.1f);
        // StartCoroutine(DelayRespawn());
    }

    void OnTriggerEnter(Collider col)
    {
        if (col.transform.tag == "FinishLine") {
            if (machineAI || loadBest)
            {
                em.AddCompletionFitness(ConstantManager.COMPLETION_BONUS);
                em.AgentFailed();
            }
            gc.ResetCar();
            // gc.FinishGame(gameObject);
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
