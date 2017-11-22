using System;
using System.Collections.Generic;
using UnityEngine;

public class EntityManager : MonoBehaviour
{
    private Agent testAgent;
    private List<Agent> agents;
    private float currentFitness;
    private float bestFitness;
    private float currentTimer;
    private int totalWeights;

    private NeuralNetwork nn;
    private GeneticAlgorithm ga;

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
            (int)ConstantManager.Inputs.RAYCAST_COUNT,
            ConstantManager.HIDDEN_LAYER_NEURONS,
            (int)ConstantManager.NeuralNetOutputs.OUTPUT_COUNT);
        
        testAgent = new Agent();
        testAgent.SetPosition();
        testAgent.AttachNeuralNetwork(nn);
    }

    private int GetTotalWeight() {
        return (int)ConstantManager.Inputs.RAYCAST_COUNT *
            ConstantManager.HIDDEN_LAYER_NEURONS *
            (int)ConstantManager.NeuralNetOutputs.OUTPUT_COUNT +
            ConstantManager.HIDDEN_LAYER_NEURONS +
            (int)ConstantManager.NeuralNetOutputs.OUTPUT_COUNT;
    } 

    public void ExportCurrentAgent() {
        testAgent.GetNeuralNetwork().ExportNN("NeuralNetwork-" + DateTime.UtcNow.ToShortDateString() + ".txt");
    }

    public void NextTestSubject() {
        ga.SetGenomeFitness(currentFitness, ga.GetCurrentGenomeIndex());
        currentFitness = 0.0f;
        Genome g = ga.GetNextGenome();
        nn.FromGenome(ref g, 
            (int)ConstantManager.Inputs.RAYCAST_COUNT, 
            ConstantManager.HIDDEN_LAYER_NEURONS, 
            (int)ConstantManager.NeuralNetOutputs.OUTPUT_COUNT);
        testAgent = new Agent();
        testAgent.SetPosition();
        testAgent.AttachNeuralNetwork(nn);
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

    private void Update() {
        if (testAgent.HasAgentFailed()) {
            ForceToNextAgent();
        }
        currentFitness += testAgent.GetDistanceDelta() / 2.0f;
        if (currentFitness > bestFitness) {
            bestFitness = currentFitness;
        }

    }

    public void ForceToNextAgent() {
        if (ga.GetCurrentGenomeIndex() == ConstantManager.MAXIMUM_GENOME_POPULATION - 1) {
            EvolveGenomes();
        } else {
            NextTestSubject();
        }
    }


}
