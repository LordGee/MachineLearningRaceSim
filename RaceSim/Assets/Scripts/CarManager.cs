using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CarManager : MonoBehaviour {

    private GameController gameControl;
    private CarControls cc;
    private InputManager im;
    private EntityManager em;

    [SerializeField] public bool machineAI;

    void Start() {
        gameControl = FindObjectOfType<GameController>();
        cc = FindObjectOfType<CarControls>();
        im = GetComponent<InputManager>();
        if (machineAI)
        {
            em = new EntityManager();
        }
    }

    void FixedUpdate()
    {
        if (machineAI)
        {
            im.UpdateInputs();

            List<float> currentInputs = new List<float>();
            for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++) {
                // get raycast events and add distance to the input list
                currentInputs.Add(im.GetInputByIndex(i));
            }

            em.PrepareInputs(currentInputs);
            em.ManualUpdate();
            // todo pass into the em, the distance / speed since the last update to provide fitness
            float[] newOutputs = em.GetCurrentOutputs();
            cc.PerformMovement(
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT] - newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT], 
                newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE], 
                (newOutputs[(int)ConstantManager.NNOutputs.OUTPUT_BRAKE] > 0.5), 
                true);
            em.SetNewFitness(im.GetAcceleration());
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
        transform.position = _pos;
        GetComponent<Rigidbody>().velocity = new Vector3(0f, 0f, 0f);
        cc.CompleteStop();
        transform.rotation = _quat;
    }
}
