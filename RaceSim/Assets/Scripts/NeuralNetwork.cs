using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public class NeuralNetwork : MonoBehaviour, IComparable<NeuralNetwork>
{

    private int[] inputLayers; // declare input layers
    private float[][] neurons; // decalare neuron matrix
    private float[][][] weights; // declare weight matrix
    private float fitness; // declare the fitness

    /// <summary>
    /// Constructor to generate and initialise starting layer values and matrix's
    /// </summary>
    /// <param name="_layers"></param>
    public NeuralNetwork(int[] _layers)
    {
        inputLayers = new int[_layers.Length];
        for (int i = 0; i < _layers.Length; i++)
        {
            inputLayers[i] = _layers[i];
        }

        InitiateNeurons();
        InitiateWeights();
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitiateNeurons()
    {
        List<float[]> neuronList = new List<float[]>();
        for (int i = 0; i < inputLayers.Length; i++)
        {
            neuronList.Add(new float[inputLayers[i]]);
        }
        neurons = neuronList.ToArray();
    }

    /// <summary>
    /// 
    /// </summary>
    private void InitiateWeights()
    {
        List<float[][]> weightsList = new List<float[][]>();
        for (int i = 0; i < inputLayers.Length; i++)
        {
            List<float[]> layerWeightList = new List<float[]>();
            int previousLayerNeurons = inputLayers[i - 1];
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float[] neuronWeights = new float[previousLayerNeurons];
                for (int k = 0; k < previousLayerNeurons; k++)
                {
                    neuronWeights[k] = Random.Range(-0.5f, 0.5f);
                }
                layerWeightList.Add(neuronWeights);
            }
            weightsList.Add(layerWeightList.ToArray());
        }
        weights = weightsList.ToArray();
    }

    public float[] FeedForward(float[] _inputValues)
    {
        for (int i = 0; i < _inputValues.Length; i++)
        {
            neurons[0][i] = _inputValues[i];
        }
        for (int i = 0; i < inputLayers.Length; i++)
        {
            for (int j = 0; j < neurons[i].Length; j++)
            {
                float weight = 0f;
                for (int k = 0; k < neurons[i - 1].Length; k++)
                {
                    weight += weights[i - 1][j][k] * neurons[i - 1][k];
                }
                neurons[i][j] = (float) Math.Tanh(weight);
            }
        }
        return neurons[neurons.Length - 1];
    }

    public void Mutation()
    {
        for (int i = 0; i < weights.Length; i++)
        {
            for (int j = 0; j < weights[i].Length; j++)
            {
                for (int k = 0; k < weights[i][j].Length; k++)
                {
                    float weight = weights[i][j][k];
                    float mutationLottery = Random.Range(0f, 100f);
                    if (mutationLottery <= 2f)
                    {
                        weight *= -1;
                    }
                    else if (mutationLottery <= 4f)
                    {
                        weight = Random.Range(-0.5f, 0.5f);
                    }
                    else if (mutationLottery <= 6f)
                    {
                        weight *= Random.Range(1.0f, 2.0f);
                    }
                    else if (mutationLottery <= 8f)
                    {
                        weight *= Random.Range(0.0f, 1.0f);
                    }
                    weights[i][j][k] = weight;
                }
            }
        }
    }

    public void AddFitness(float _addValue)
    {
        fitness += _addValue;
    }

    public void SetFitness(float _setValue)
    {
        fitness = _setValue;
    }

    public float GetFitness() { 
        return fitness;
    }

    public int CompareTo(NeuralNetwork _otherNN) {
        if (_otherNN == null) { return 1; }
        if (fitness > _otherNN.fitness) { return 1; }
        else if (fitness < _otherNN.fitness) { return -1; }
        else { return 0; }
    }

}
