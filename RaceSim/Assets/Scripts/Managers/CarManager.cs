using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class started as a management tool for both player and AI
/// as the application developed it was required to seperate out 
/// these concerns into their own classes. This class manages the AI 
/// element.
/// </summary>
public class CarManager : MonoBehaviour {

    private GameController gc;
    private CarControls cc;
    private InputManager im;
    private EntityManager em;
    private PlayerPrefsController pp;
    private float lapTime, bestLap;

    public static bool machineAI, loadBest;
    public static float timeSpeed;

    /// <summary>
    /// Constructor
    /// Objtains the relevent components required within the function of this class
    /// Also if human and ai are active, will remove the Main Camera game object from
    /// this object. This includes the audio listener, however for one frame two audio
    /// listeners exist.
    /// todo: look into the two audio listener warning
    /// </summary>
    void Start() {
        pp = FindObjectOfType<PlayerPrefsController>();
        machineAI = pp.GetLearning();
        loadBest = pp.GetLoading();
        gc = FindObjectOfType<GameController>();
        cc = GetComponent<CarControls>();
        im = GetComponent<InputManager>();
        if (machineAI || loadBest) {
            em = new EntityManager();
        }
        if (loadBest) {
            transform.Find("Main Camera").gameObject.SetActive(false);
            lapTime = 0f;
            bestLap = 9999f;
        }
        timeSpeed = 1f;
    }
    
    /// <summary>
    /// Update function for both Machine Learning and Imported AI.
    /// For the machine learning element there key presses that can 
    /// speed up the time scale of the Update function. This will speed 
    /// up the process of training an agent for a specific track.
    /// The acceleration rate is added to the agents current fitness.
    /// For the Loaded AI the laptime is accumalated and reset when 
    /// required.
    /// </summary>
    void Update() {
        Time.timeScale = timeSpeed;
        if (machineAI) {
            if (Input.GetKeyDown("1")) { timeSpeed = 1; }
            if (Input.GetKeyDown("2")) { timeSpeed = 2; }
            if (Input.GetKeyDown("3")) { timeSpeed = 3; }
            if (Input.GetKeyDown("4")) { timeSpeed = 4; }
            UpdateAndExecute();
            em.SetNewFitness(im.GetAcceleration() * Time.deltaTime);
            if (em.GetResetPosition()) {
                gc.ResetCar(true); 
                em.CompleteResetPosition();
            }
        }
        if (loadBest) {
            lapTime += Time.deltaTime;
            UpdateAndExecute();
            if (em.GetResetPosition()) {
                lapTime = 0f;
                gc.ResetCar(true);
                em.CompleteResetPosition();
            }
        }
    }

    /// <summary>
    /// Used for both Machine learning and Loaded AI
    /// Calls the input manager to update its values and  adds the new values 
    /// to a new list. These are then passed to the entity manager for processing 
    /// within the Neural Network and returns the outputs. The outputs are then 
    /// processed into the car controls perform movement function.
    /// </summary>
    private void UpdateAndExecute()
    {
        im.UpdateInputs();
        List<float> currentInputs = new List<float>();
        for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
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
    }

    /// <summary>
    /// If collision with a barrier (aka wall) agent is reset
    /// </summary>
    /// <param name="col"></param>
    void OnCollisionEnter(Collision col) {
        if (col.transform.tag == "Barrier") {
            if (machineAI || loadBest) {
                em.AgentFailed();
            }
            gc.ResetCar(true);
        }
    }

    /// <summary>
    /// If reaching the finish line the Machine Learning will recieve bonus points.
    /// The loaded AI will recieve a laptime. In both cases the car is then reset.
    /// </summary>
    /// <param name="col"></param>
    void OnTriggerEnter(Collider col) {
        if (col.transform.tag == "FinishLine") {
            if (machineAI || loadBest) {
                if (loadBest) {
                    if (lapTime < bestLap)
                    {
                        EventManager.TriggerEvent(ConstantManager.UI_MACHINE, lapTime);
                        bestLap = lapTime;
                    }
                }
                em.AddCompletionFitness(ConstantManager.COMPLETION_BONUS);
                em.AgentFailed();
            }
            gc.ResetCar(true);
        }
    }

    /// <summary>
    /// This function implemented to prevent the cars continueing motion after respawning.
    /// The angular velocity and drag reset appeared to make the most difference.
    /// </summary>
    /// <param name="_pos"></param>
    /// <param name="_quat"></param>
    public void ResetPosition(Vector3 _pos, Quaternion _quat) {
        cc.CompleteStop();
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().angularVelocity = new Vector3(0f, 0f, 0f);
        GetComponent<Rigidbody>().drag = 0f;
        transform.position = _pos;
        transform.rotation = _quat;
    }
}
