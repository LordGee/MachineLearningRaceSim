using System.Collections.Generic;
using UnityEngine;

public class Agent : MonoBehaviour
{

    private bool hasFailed;
    private float distanceDelta;
    private NeuralNetwork nn;

    public Agent()
    {
        nn = null;
        hasFailed = false;
        distanceDelta = 0.0f;
    }

    void Update()
    {
        if (!hasFailed)
        {
            List<float> inputs = null;
            for (int i = 0; i < (int)ConstantManager.Inputs.RAYCAST_COUNT; i++)
            {
                // Todo: get raycast events and add distance to the input list
                float raycast = 1.253f;
                inputs.Add(raycast);
            }
            nn.SetInput(inputs);
            nn.UpdateNN();

            float rightForce = nn.GetOutput((int) ConstantManager.NeuralNetOutputs.OUTPUT_TURN_RIGHT);
            float leftForce = nn.GetOutput((int)ConstantManager.NeuralNetOutputs.OUTPUT_TURN_LEFT);
            float accelerate = nn.GetOutput((int)ConstantManager.NeuralNetOutputs.OUTPUT_ACCELERATE);
            float brake = nn.GetOutput((int)ConstantManager.NeuralNetOutputs.OUTPUT_BRAKE);

            // todo: update car movement...

        }
    }

    public void SetPosition()
    {
        // todo: Set car position to spawn point

    }

    public void AttachNeuralNetwork(NeuralNetwork _net) { nn = _net; }

    public NeuralNetwork GetNeuralNetwork() { return nn; }

    public float GetDistanceDelta() {

        return 1f;
    }

    public bool HasAgentFailed() {
        return hasFailed;
    }

}
