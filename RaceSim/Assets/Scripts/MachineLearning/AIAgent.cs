using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// This class managers the current AI agent that is active
/// </summary>
public class AIAgent {

    // TODO: come back and delete this if not needed 
    /*
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
    public void ClearFailiure()
    {
        hasFailed = false;
    }
    */

    private bool hasFailed;
    private NeuralNetwork nn;
    private List<float> currentInputs = new List<float>();
    private float[] currentOutputs = new float[(int)ConstantManager.NNOutputs.OUTPUT_COUNT];

    /// <summary>
    /// Constructor for a new AI Agent
    /// </summary>
    public AIAgent() {
        nn = null;
        hasFailed = false;
    }

    /// <summary>
    /// Called every frame from the EntityManager
    /// If the AI Agent has not failed this function will:
    /// 1. Update the Neural Network with the current input values
    /// 2. The NN will evaluate these values
    /// 3. Output value can then be updated which can then be requested by the Entity Manager
    /// </summary>
    public void ManualUpdate() {
        if (!hasFailed) {
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

    /// <summary>
    /// Sets the most current input values released from the Car Manager
    /// </summary>
    /// <param name="_inputs">A List of Floats that contain input values from the sensors</param>
    public void SetInputs(List<float> _inputs) {
        currentInputs.Clear();
        currentInputs = _inputs;
    }

    /// <summary>
    /// Attach a constructed Neural Network to the AI Agenet
    /// </summary>
    /// <param name="_net">A constructed Neural Network</param>
    public void AttachNeuralNetwork(NeuralNetwork _net) { nn = _net; }

    /// <summary>
    /// Returns the current Neural Network for this AI Agent
    /// </summary>
    /// <returns>Returns the current Neural Network</returns>
    public NeuralNetwork GetNeuralNetwork() { return nn; }

    /// <summary>
    /// Provides a quick check function to identify if this test agent has failed or not
    /// </summary>
    /// <returns>Returns true if the agent has failed</returns>
    public bool HasAgentFailed() { return hasFailed; }

    /// <summary>
    /// Sets the has failed booleon to true to indicate to other functions that the AI Agent has failed
    /// </summary>
    public void SetFailed() { hasFailed = true; }

}
