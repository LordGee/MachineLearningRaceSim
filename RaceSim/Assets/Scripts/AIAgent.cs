using System.Collections.Generic;
using UnityEngine;

public class AIAgent : MonoBehaviour
{

    private bool hasFailed;
    private float distanceDelta;
    private NeuralNetwork nn;
    private CarControls cc;

    public AIAgent()
    {
        nn = null;
        cc = FindObjectOfType<CarControls>();
        hasFailed = false;
        distanceDelta = 0.0f;
    }

    void Update()
    {
        if (!hasFailed)
        {
            List<float> inputs = null;
            for (int i = 0; i < (int)ConstantManager.NNInputs.INPUT_COUNT; i++)
            {
                // Todo: get raycast events and add distance to the input list
                float raycast = 1.253f;
                inputs.Add(raycast);
            }
            nn.SetInput(inputs);
            nn.UpdateNN();

            float rightForce = nn.GetOutput((int) ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT);
            float leftForce = nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT);
            float accelerate = nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE);
            float brake = nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_BRAKE);

            // todo: update car movement...
            cc.PerformMovement(rightForce - leftForce, accelerate, (brake > 0), true);
            Debug.Log("Performed Movement");
        }
    }

    public float GetDistanceDelta() {

        // distanceDelta = heading.Magnitude(); ?
        return distanceDelta;
    }

    public void SetPosition()
    {
        // todo: Set car position to spawn point

    }

    public void CreateNeuralNetwork()
    {
        nn.ReleaseNN();
        nn.CreateNN(ConstantManager.NUMBER_OF_HIDDEN_LAYERS,
            (int)ConstantManager.NNInputs.INPUT_COUNT,
            ConstantManager.HIDDEN_LAYER_NEURONS,
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT);
    }

    public void AttachNeuralNetwork(NeuralNetwork _net) { nn = _net; }

    public NeuralNetwork GetNeuralNetwork() { return nn; }

    public void ClearFailiure()
    {
        hasFailed = false;
    }

    public bool HasAgentFailed() {
        return hasFailed;
    }

}
