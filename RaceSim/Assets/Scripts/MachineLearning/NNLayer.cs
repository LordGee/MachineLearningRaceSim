using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class NNLayer
{

    private int totalNeurons;
    private List<Neuron> neurons;

    public NNLayer() {
        totalNeurons = -1;
        neurons = new List<Neuron>();
    }

    public void Evaluate(List<float> _input, ref List<float> _output)
    {
        int inputIndex = 0;
        for (int i = 0; i < totalNeurons; i++)
        {
            float activation = 0.0f;
            for (int j = 0; j < neurons[i].numberOfInputs - 1; j++)
            {
                activation += _input[inputIndex] * neurons[i].weights[j];
                inputIndex++;
            }
            activation += neurons[i].weights[neurons[i].numberOfInputs] * ConstantManager.BIAS;
            _output.Add(Sigmoid(activation, 1.0f));
            inputIndex = 0;
        }
    }

    public void SaveLayer(ref TextWriter _file)
    {
        _file.WriteLine(neurons.Count);
        for (int i = 0; i < neurons.Count; i++) {
            _file.WriteLine(neurons[i].weights.Count);
            for (int j = 0; j < neurons[i].weights.Count; j++) {
                _file.WriteLine(neurons[i].weights[j]);
            }
        }
        /*
         fileOut << "<NLayer>" << std::endl;
		fileOut << "Type=" << layerType << std::endl;
		fileOut << "NNInputs=" << this->totalInputs << std::endl;
		fileOut << "Neurons=" << this->neurons.size() << std::endl;
		fileOut << "-Build-" << std::endl;
		for (unsigned int i = 0; i < this->neurons.size(); i++)
		{
			fileOut << "<Neuron>" << std::endl;
			fileOut << "Weights=" << this->neurons[i].weights.size() << std::endl;
			for (unsigned int j = 0; j < neurons[i].weights.size(); j++)
			{
				fileOut << "W=" << neurons[i].weights[j] << std::endl; 
			}
			fileOut << "</Neuron>" << std::endl;
		}
			
		fileOut << "</NLayer>" << std::endl;
         */
    }

    public void LoadLayer(List<Neuron> _neuron)
    {
        totalNeurons = _neuron.Count;
        neurons = _neuron;
    }

    public void PopulateLayer(int _neurons, int _inputs)
    {
        totalNeurons = _neurons;
        neurons.Capacity = _inputs;
        for (int i = 0; i < neurons.Count; i++)
        {
            neurons[i].Populate(_inputs);
        }
    }

    public void SetWeights(List<float> _weights, int _neurons, int _inputs)
    {
        int index = 0;
        totalNeurons = _neurons;
        neurons.Capacity = _neurons;
        for (int i = 0; i < _neurons; i++)
        {
            neurons[i].weights.Capacity = _inputs;
            for (int j = 0; j < _inputs; j++)
            {
                neurons[i].weights[j] = _weights[index];
                index++;
            }
        }
    }

    public void GetWeights(ref List<float> _out)
    {
        int size = 0;
        for (int i = 0; i < totalNeurons; i++)
        {
            size += neurons[i].weights.Capacity;
        }
        _out.Capacity = size;

        for (int i = 0; i < totalNeurons; i++)
        {
            for (int j = 0; j < neurons[i].weights.Capacity; j++)
            {
                _out[totalNeurons * i + j] = neurons[i].weights[j];
            }
        }
    }

    public void SetNeurons(List<Neuron> _neurons, int _numberNeurons, int _inputs)
    {
        totalNeurons = _numberNeurons;
        neurons = _neurons;
    }


    // To be moved to a math class
    public float Sigmoid(float _a, float _p) {
        float ap = (-_a) / _p;
        return (1 / (1 + Mathf.Exp(ap)));
    }
}
