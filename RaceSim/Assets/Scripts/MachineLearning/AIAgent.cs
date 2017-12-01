using System.Collections.Generic;
using UnityEngine;

public class AIAgent
{

    private bool hasFailed;
    private NeuralNetwork nn;
    private List<float> currentInputs = new List<float>();
    private float[] currentOutputs = new float[(int)ConstantManager.NNOutputs.OUTPUT_COUNT];

    public AIAgent()
    {
        nn = null;
        hasFailed = false;
    }

    public void ManualUpdate()
    {
        if (!hasFailed)
        {
            nn.SetInput(currentInputs);
            nn.UpdateNN();

            currentOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT] = 
                nn.GetOutput((int) ConstantManager.NNOutputs.OUTPUT_TURN_RIGHT);
            currentOutputs[(int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT] = 
                nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_TURN_LEFT);
            currentOutputs[(int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE] = 
                nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_ACCELERATE);
            currentOutputs[(int)ConstantManager.NNOutputs.OUTPUT_BRAKE] = 
                nn.GetOutput((int)ConstantManager.NNOutputs.OUTPUT_BRAKE);

            
        }
    }

    public void SetInputs(List<float> _inputs)
    {
        currentInputs.Clear();
        currentInputs = _inputs;
    }

    public float[] GetOutputs()
    {
        return currentOutputs;
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

    public void SetFailed()
    {
        hasFailed = true;
    }

}
