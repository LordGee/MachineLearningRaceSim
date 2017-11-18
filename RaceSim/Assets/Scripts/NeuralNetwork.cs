using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class NeuralNetwork : MonoBehaviour
{
    public NeuralNetworkLayer inputLayer, hiddenLayer, outputLayer;

    public void Initialise(int _numberNeuronInputs, int _numberNeuronHidden, int _numberNeuronOutputs)
    {
        NeuralNetworkLayer nullLayer = null;

        inputLayer.numberOfNeurons = _numberNeuronInputs;
        inputLayer.numberOfChildNeurons = _numberNeuronHidden;
        inputLayer.numberOfParentNeurons = 0;
        inputLayer.Initialise(_numberNeuronInputs, ref nullLayer, ref hiddenLayer);
        inputLayer.RandomWeights();

        hiddenLayer.numberOfNeurons = _numberNeuronHidden;
        hiddenLayer.numberOfChildNeurons = _numberNeuronOutputs;
        hiddenLayer.numberOfParentNeurons = _numberNeuronInputs;
        hiddenLayer.Initialise(_numberNeuronHidden, ref inputLayer, ref outputLayer);
        hiddenLayer.RandomWeights();

        outputLayer.numberOfNeurons = _numberNeuronOutputs;
        outputLayer.numberOfChildNeurons = 0;
        outputLayer.numberOfParentNeurons = _numberNeuronHidden;
        outputLayer.Initialise(_numberNeuronOutputs, ref hiddenLayer, ref nullLayer);
        outputLayer.RandomWeights();
    }

    public void SetInput(int _i, float _value)
    {
        if (_i >= 0 && _i < inputLayer.numberOfNeurons)
        {
            inputLayer.neuronValues[_i] = _value;
        }
    }

    public float GetOutput(int _i)
    {
        if (_i >= 0 && _i < outputLayer.numberOfNeurons)
        {
            return outputLayer.neuronValues[_i];
        }
        else
        {
            return (float)Int32.MaxValue;
        }
    }

    public void SetDesiredOutput(int _i, float _value)
    {
        if (_i >= 0 && _i < outputLayer.numberOfNeurons)
        {
            outputLayer.desiredValues[_i] = _value;
        }
    }

    public void FeedForward()
    {
        inputLayer.CalculateNeuronValues();
        hiddenLayer.CalculateNeuronValues();
        outputLayer.CalculateNeuronValues();
    }

    public void BackPropagate()
    {
        outputLayer.CalculateErrors();
        hiddenLayer.CalculateErrors();

        hiddenLayer.AdjustWeights();
        inputLayer.AdjustWeights();
    }

    public int GetMaxOutputID()
    {
        int id = 0;
        float maxValue = outputLayer.neuronValues[0];

        for (int i = 0; i < outputLayer.numberOfNeurons; i++)
        {
            if (outputLayer.neuronValues[i] > maxValue)
            {
                maxValue = outputLayer.neuronValues[i];
                id = i;
            }
        }

        return id;
    }

    public float CalculateError()
    {
        float error = 0;

        for (int i = 0; i < outputLayer.numberOfNeurons; i++)
        {
            error += Mathf.Pow(outputLayer.neuronValues[i] - outputLayer.desiredValues[i], 2);
        }
        error = error / outputLayer.numberOfNeurons;

        return error;
    }

    public void SetLearningRate(float _rate)
    {
        inputLayer.learningRate = _rate;
        hiddenLayer.learningRate = _rate;
        outputLayer.learningRate = _rate;
    }

    public void SetLinearOutput(bool _use)
    {
        inputLayer.linearOutput = _use;
        hiddenLayer.linearOutput = _use;
        outputLayer.linearOutput = _use;
    }

    public void SetMomentum(bool _use, float _factor)
    {
        inputLayer.useMomentum = _use;
        hiddenLayer.useMomentum = _use;
        outputLayer.useMomentum = _use;

        inputLayer.momentumFactor = _factor;
        hiddenLayer.momentumFactor = _factor;
        outputLayer.momentumFactor = _factor;
    }

    public void DumpData(string _fileName)
    {
        // create writestream
    }
}



/*
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
*/