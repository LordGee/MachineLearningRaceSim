﻿using System.Collections.Generic;
using UnityEngine;

public class AIAgent
{

    private bool hasFailed;
    private float distanceDelta;
    private NeuralNetwork nn;
    private CarControls cc;
    private InputManager im;
    private List<float> currentInputs = new List<float>();
    private float[] currentOutputs = new float[(int)ConstantManager.NNOutputs.OUTPUT_COUNT];

    public AIAgent()
    {
        nn = null;
        //cc = FindObjectOfType<CarControls>();
        //im = FindObjectOfType<InputManager>();
        hasFailed = false;
        distanceDelta = 0.0f;
    }

    public void ManualUpdate()
    {
        if (!hasFailed)
        {
            // todo We are going to need to address CC abd IM access
            // current inputs is prepared by the CarManager via the Entity Manager.
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

    public void SetFailed()
    {
        hasFailed = true;
    }

}