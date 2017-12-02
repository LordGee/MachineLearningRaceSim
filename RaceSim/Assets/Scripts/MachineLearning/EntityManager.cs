using System;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class EntityManager
{
    private AIAgent testAiAgent;
    private float currentFitness;
    private float bestFitness;
    private int totalWeights;

    private NeuralNetwork nn;
    private GeneticAlgorithm ga;
    private List<float> inputs;
    private float[] outputs;
    private float newFitness, timeScale;
    private bool resetPosition;

    public EntityManager()
    {
        ga = new GeneticAlgorithm();
        totalWeights = GetTotalWeight();
        ga.GenerateNewPopulation(
            ConstantManager.MAXIMUM_GENOME_POPULATION,
            totalWeights);
        currentFitness = 0.0f;
        bestFitness = 0.0f;

        nn = new NeuralNetwork();
        Genome g = ga.GetNextGenome();
        nn.FromGenome(ref g, 
            (int)ConstantManager.NNInputs.INPUT_COUNT,
            ConstantManager.HIDDEN_LAYER_NEURONS,
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT);
        
        testAiAgent = new AIAgent();
        resetPosition = true;
        testAiAgent.AttachNeuralNetwork(nn);
    }

    private int GetTotalWeight() {
        return (int)ConstantManager.NNInputs.INPUT_COUNT *
            ConstantManager.HIDDEN_LAYER_NEURONS *
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT +
            ConstantManager.HIDDEN_LAYER_NEURONS +
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT;
    } 

    public void ExportCurrentAgent() {
        // testAiAgent.GetNeuralNetwork().ExportNN(@"C:\GameProjects\MachineLearningRaceSim\RaceSim\Assets\Data\best.csv");
        testAiAgent.GetNeuralNetwork().ExportNN(@"~\..\Assets\Data\best.csv");
    }

    public void ImportExistingAgent() {
        testAiAgent.GetNeuralNetwork().ImportNN(@"~\..\Assets\Data\best.csv");
    }

    public void NextTestSubject() {
        ga.SetGenomeFitness(currentFitness, ga.GetCurrentGenomeIndex());
        currentFitness = 0.0f;
        Genome g = ga.GetNextGenome();
        nn.FromGenome(ref g, 
            (int)ConstantManager.NNInputs.INPUT_COUNT, 
            ConstantManager.HIDDEN_LAYER_NEURONS, 
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT);
        testAiAgent = new AIAgent();
        resetPosition = true;
        testAiAgent.AttachNeuralNetwork(nn);
    }

    public void BreedPopulation() {
        ga.ClearPopulation();
        totalWeights = GetTotalWeight();
        ga.GenerateNewPopulation(ConstantManager.MAXIMUM_GENOME_POPULATION, totalWeights);
    }

    public void EvolveGenomes() {
        ga.BreedPopulation();
        NextTestSubject();
    }

    public int GetCurrentEntityFromThePopulation() {
        return ga.GetCurrentGenomeIndex();
    }

    public void ForceToNextAgent() {
        if (ga.GetCurrentGenomeIndex() == ConstantManager.MAXIMUM_GENOME_POPULATION - 1) {
            EvolveGenomes();
        } else {
            NextTestSubject();
        }
    }

    private int counter;
    public void ManualUpdate() {
        testAiAgent.SetInputs(inputs);
        testAiAgent.ManualUpdate();
        outputs = testAiAgent.GetOutputs();
        if (counter > 60) 
        {
            AgentFailed();
            counter = 0; 
        }

        if (newFitness == 0) { counter++; }
        else { counter = 0; }
        currentFitness += (newFitness / 2.0f) * timeScale;
        EventManagerOneArg.TriggerEvent(ConstantManager.UI_FITNESS, currentFitness);
        if (testAiAgent.HasAgentFailed()) {
            if (currentFitness > bestFitness) {
                bestFitness = currentFitness;
                ExportCurrentAgent();
                Debug.Log("EXPORTING");
            }
            EventManagerOneArg.TriggerEvent(ConstantManager.UI_POPULATION, currentFitness);
            
            ForceToNextAgent();
        }
    }

    public void PrepareInputs(List<float> _currentInputs) { inputs = _currentInputs; }
    public float[] GetCurrentOutputs() { return outputs; }

    public void SetNewFitness(float _fit, float _timeScale)
    {
        newFitness = _fit;
        timeScale = _timeScale;
    }
    public void AddCompletionFitness(float _value) { currentFitness += _value; }
    public void AgentFailed() { testAiAgent.SetFailed(); }
    public bool GetResetPosition() { return resetPosition; }
    public void CompleteResetPosition() { resetPosition = false; }
    
}
