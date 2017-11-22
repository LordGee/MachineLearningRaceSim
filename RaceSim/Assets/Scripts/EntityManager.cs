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
        totalWeights =
            (int) ConstantManager.Inputs.RAYCAST_COUNT *
            ConstantManager.HIDDEN_LAYER_NEURONS *
            (int) ConstantManager.NeuralNetOutputs.OUTPUT_COUNT +
            ConstantManager.HIDDEN_LAYER_NEURONS +
            (int) ConstantManager.NeuralNetOutputs.OUTPUT_COUNT;
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

}
