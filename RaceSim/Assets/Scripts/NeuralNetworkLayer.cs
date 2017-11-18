using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NeuralNetworkLayer : MonoBehaviour
{

    public int numberOfNeurons, numberOfChildNeurons, numberOfParentNeurons;
    public float[,] weights, weightChanges;
    public float[] neuronValues, desiredValues, errors, biasWeights, biasValues;
    public float learningRate;
    public bool linearOutput, useMomentum;
    public float momentumFactor;
    public NeuralNetworkLayer parentLayer, childLayer;

    public NeuralNetworkLayer()
    {
        parentLayer = null;
        childLayer = null;
        linearOutput = false;
        useMomentum = false;
        momentumFactor = 0.9f;
    }

    public void Initialise(int _noNeurons, ref NeuralNetworkLayer _parent, ref NeuralNetworkLayer _child)
    {
        // int i, j;
        neuronValues = new float[numberOfNeurons];
        desiredValues = new float[numberOfNeurons];
        errors = new float[numberOfNeurons];

        if (_parent != null) {
            parentLayer = _parent;
        }
        if (_child != null)
        {
            childLayer = _child;

            weights = new float[numberOfNeurons, numberOfChildNeurons];
            weightChanges = new float[numberOfNeurons, numberOfChildNeurons];

            biasValues = new float[numberOfChildNeurons];
            biasWeights = new float[numberOfChildNeurons];
        } else {
            weights = null;
            weightChanges = null;
            biasValues = null;
            biasWeights = null;
        }
        for (int i = 0; i < numberOfNeurons; i++) {
            neuronValues[i] = 0f;
            desiredValues[i] = 0f;
            errors[i] = 0f;
            if (childLayer != null) {
                for (int j = 0; j < numberOfChildNeurons; j++) {
                    weights[i, j] = 0f;
                    weightChanges[i, j] = 0f;
                }
            }
        }
        if (childLayer != null) {
            for (int i = 0; i < numberOfChildNeurons; i++) {
                biasWeights[i] = 0f;
                biasValues[i] = 0f;
            }
        }
    }

    public void RandomWeights()
    {
        const float min = -1.0f, max = 1.0f;
        bool justOnce = true; // added to save creating a second loop
        for (int i = 0; i < numberOfNeurons; i++) {
            for (int j = 0; j < numberOfChildNeurons; j++) {
                weights[i, j] = Random.Range(min, max);
                if (justOnce)
                    biasWeights[j] = Random.Range(min, max);
            }
            justOnce = false;
        }
    }

    public void CalculateNeuronValues()
    {
        if (parentLayer != null) {
            for (int i = 0; i < numberOfNeurons; i++) {
                float x = 0f;
                for (int j = 0; j < numberOfParentNeurons; j++) {
                    x += parentLayer.neuronValues[j] * parentLayer.weights[j, i];
                }
                x += parentLayer.biasValues[i] * parentLayer.biasWeights[i];
                if (childLayer == null && linearOutput) {
                    neuronValues[i] = x;
                } else {
                    neuronValues[i] = 1.0f / (1 + Mathf.Exp(-x));
                }
            }
        }
    }

    public void CalculateErrors()
    {
        if (childLayer == null)
        {
            for (int i = 0; i < numberOfNeurons; i++)
            {
                errors[i] = (desiredValues[i] - neuronValues[i]) * neuronValues[i] * (1.0f - neuronValues[i]);
            }
        }
        else if (parentLayer == null)
        {
            for (int i = 0; i < numberOfNeurons; i++)
            {
                errors[i] = 0.0f;
            }
        }
        else
        {
            for (int i = 0; i < numberOfNeurons; i++)
            {
                float sum = 0f;
                for (int j = 0; j < numberOfChildNeurons; j++)
                {
                    sum += childLayer.errors[j] * weights[i, j];
                }
                errors[i] = sum * neuronValues[i] * (1.0f - neuronValues[i]);
            }
        }
    }

    public void AdjustWeights()
    {
        bool doOnce = true;
        if (childLayer != null)
        {
            for (int i = 0; i < numberOfNeurons; i++)
            {
                for (int j = 0; j < numberOfChildNeurons; j++)
                {
                    float dw = learningRate * childLayer.errors[j] * neuronValues[i];
                    if (useMomentum)
                    {
                        weights[i, j] += dw + momentumFactor * weightChanges[i, j];
                        weightChanges[i, j] = dw;
                    }
                    else
                    {
                        weights[i, j] += dw;
                    }
                    if (doOnce)
                    {
                        biasWeights[j] += learningRate * childLayer.errors[j] * biasWeights[j];
                    }
                }
                doOnce = false;
            }
        }
    }

}
