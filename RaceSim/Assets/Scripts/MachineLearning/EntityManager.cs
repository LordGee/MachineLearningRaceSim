﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.InteropServices;
using UnityEngine;
using UnityEngine.SceneManagement;

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
    private float newFitness;
    private bool resetPosition;

    public EntityManager()
    {
        if (CarManager.machineAI)
        {
            ga = new GeneticAlgorithm();
            totalWeights = GetTotalWeight();
            ga.GenerateNewPopulation(
                ConstantManager.MAXIMUM_GENOME_POPULATION,
                totalWeights);
            currentFitness = 0.0f;
            bestFitness = PlayerPrefsController.GetFitness();
            EventManagerOneArg.TriggerEvent(ConstantManager.UI_BEST_FITNESS, bestFitness);
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
        if (CarManager.loadBest)
        {
            nn = new NeuralNetwork();
            testAiAgent = new AIAgent();
            testAiAgent.AttachNeuralNetwork(nn);
            
            ImportExistingAgent();
            resetPosition = true;
        }
    }

    private int GetTotalWeight() {
        return (int)ConstantManager.NNInputs.INPUT_COUNT *
            ConstantManager.HIDDEN_LAYER_NEURONS *
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT +
            ConstantManager.HIDDEN_LAYER_NEURONS +
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT;
    } 

    public void ExportCurrentAgent() {
        testAiAgent.GetNeuralNetwork().ExportNN(@"~\..\Assets\Data\" + PlayerPrefsController.GetTrack() + @"\" + currentFitness + ".csv");
        AddToList();
    }

    public void ImportExistingAgent() {
        testAiAgent.GetNeuralNetwork().ImportNN(@"~\..\Assets\Data\" + PlayerPrefsController.GetTrack() + @"\" + PlayerPrefsController.GetFitness() + ".csv");
    }

    private void AddToList()
    {
        using (TextWriter tw = new StreamWriter(@"~\..\Assets\Data\List.csv", true))
        {
            tw.WriteLine(PlayerPrefsController.GetTrack());
            tw.WriteLine(currentFitness);
        }
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

    private void ReloadExistingAgent()
    {
        nn = new NeuralNetwork();
        testAiAgent = new AIAgent();
        resetPosition = true;
        testAiAgent.AttachNeuralNetwork(nn);
        ImportExistingAgent();
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
        if (counter > 60) // agent not moved for 60x frames
        {
            AgentFailed();
            counter = 0; 
        }
        if (CarManager.loadBest)
        {
            if (testAiAgent.HasAgentFailed())
            {
                ReloadExistingAgent();
            }
        }
        if (CarManager.machineAI)
        {
            if (newFitness == 0) { counter++; } else { counter = 0; }
            currentFitness += (newFitness / 2.0f);
            EventManagerOneArg.TriggerEvent(ConstantManager.UI_FITNESS, currentFitness);
            if (testAiAgent.HasAgentFailed()) {
                if (currentFitness > bestFitness) {
                    bestFitness = currentFitness;
                    if (currentFitness > ConstantManager.COMPLETION_BONUS)
                    {
                        ExportCurrentAgent();
                    }
                }
                EventManagerOneArg.TriggerEvent(ConstantManager.UI_POPULATION, currentFitness);

                ForceToNextAgent();
            }
        }
    }

    public void PrepareInputs(List<float> _currentInputs) { inputs = _currentInputs; }
    public float[] GetCurrentOutputs() { return outputs; }

    public void SetNewFitness(float _fit) { newFitness = _fit; }
    public void AddCompletionFitness(float _value) { currentFitness += _value; }
    public void AgentFailed() { testAiAgent.SetFailed(); }
    public bool GetResetPosition() { return resetPosition; }
    public void CompleteResetPosition() { resetPosition = false; }
    
}
