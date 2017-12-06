using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NeuralNetwork {

    // TODO Come back and delete this if no longer required
    /*
    public void CreateNN(int _hidden, int _input, int _neuronsPerHidden, int _output)
    {
        inputAmount = _input;
        outputAmount = _output;
        for (int i = 0; i < _hidden; i++)
        {
            NNLayer layer = new NNLayer();
            layer.PopulateLayer(_neuronsPerHidden, _input);
            hiddenLayers.Add(layer);
        }
        outputLayer = new NNLayer();
        outputLayer.PopulateLayer(_output, _neuronsPerHidden);
    }
    
            public int GetNumberOfHiddenLayers()
    {
        return hiddenLayers.Capacity;
    }

    public Genome ToGenome()
    {
        Genome genome = new Genome();
        for (int i = 0; i < hiddenLayers.Capacity; i++)
        {
            List<float> hiddenWeights = new List<float>();
            hiddenLayers[i].GetWeights(ref hiddenWeights);
            for (int j = 0; j < hiddenWeights.Capacity; j++)
            {
                genome.weights.Add(hiddenWeights[j]);
            }
        }
        List<float> outputWeights = new List<float>();
        outputLayer.GetWeights(ref outputWeights);
        for (int i = 0; i < outputWeights.Capacity; i++)
        {
            genome.weights.Add(outputWeights[i]);
        }
        return genome;
    }
    */

    private int inputAmount, outputAmount;
    private List<float> inputs;
    private NNLayer inputLayer;
    private List<NNLayer> hiddenLayers;
    private NNLayer outputLayer;
    private List<float> outputs;

    /// <summary>
    /// Constructor for the input, output and hidden layers
    /// </summary>
    public NeuralNetwork() {
        inputLayer = new NNLayer();
        outputLayer = new NNLayer();
        hiddenLayers = new List<NNLayer>();
        outputs = new List<float>();
    }

    /// <summary>
    /// Deconstructor
    /// </summary>
    ~NeuralNetwork() {
        if (inputLayer != null) {
            inputLayer = null;
        }
        if (outputLayer != null) {
            outputLayer = null;
        }
        for (int i = 0; i < hiddenLayers.Count; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        } // not sure if I need this loop, will test later
        hiddenLayers.Clear();
    }

    public void UpdateNN() {
        outputs = new List<float>();
        for (int i = 0; i < hiddenLayers.Count; i++) {
            if (i > 0) {
                inputs = outputs;
            }
            hiddenLayers[i].Evaluate(inputs, ref outputs);
        }
        inputs = outputs;
        outputLayer.Evaluate(inputs, ref outputs);
    }

    public void SetInput(List<float> _in) {
        inputs = _in;
    }

    public float GetOutput(int _ID) {
        if (_ID >= outputAmount) {
            return 0.0f;
        }
        return outputs[_ID];
    }

    public int GetTotalOutputs() {
        return outputAmount;
    }

    public void ExportNN(string _filename) {
        TextWriter file = new StreamWriter(_filename);
        file.WriteLine(inputAmount);
        file.WriteLine(outputAmount);
        // Hidden Layers
        file.WriteLine(hiddenLayers.Count);
        for (int i = 0; i < hiddenLayers.Count; i++) {
            hiddenLayers[i].SaveLayer(ref file);
        }
        // Outer Layer
        outputLayer.SaveLayer(ref file);
        file.Dispose();
    }

    public void ImportNN(string _filename) {
        TextReader file = new StreamReader(_filename);
        if (file != null) {
            float weight = 0.0f;
            int totalHidden = 0;
            int totalNeurons = 0;
            int totalWeights = 0;
            List<Neuron> neurons = new List<Neuron>();
            inputAmount = Convert.ToInt32(file.ReadLine());
            outputAmount = Convert.ToInt32(file.ReadLine());
            // Hidden Layer
            totalHidden = Convert.ToInt32(file.ReadLine());
            for (int i = 0; i < totalHidden; i++) {
                totalNeurons = Convert.ToInt32(file.ReadLine());
                neurons.Capacity = totalNeurons;
                for (int j = 0; j < totalNeurons; j++) {
                    totalWeights = Convert.ToInt32(file.ReadLine());
                    neurons.Add(null);
                    neurons[j] = new Neuron();
                    neurons[j].numberOfInputs = inputAmount;
                    for (int k = 0; k < totalWeights; k++) {
                        weight = float.Parse(file.ReadLine());
                        neurons[j].weights.Add(weight);
                    }
                }
            }
            NNLayer hLayer = new NNLayer();
            hLayer.SetNeurons(neurons, neurons.Count, inputAmount);
            hiddenLayers.Add(hLayer);
            // Output Layer
            totalNeurons = Convert.ToInt32(file.ReadLine());
            neurons = new List<Neuron>();
            neurons.Capacity = totalNeurons;
            for (int j = 0; j < totalNeurons; j++) {
                totalWeights = Convert.ToInt32(file.ReadLine());
                neurons.Add(null);
                neurons[j] = new Neuron();
                neurons[j].numberOfInputs = inputAmount;
                for (int k = 0; k < totalWeights; k++) {
                    weight = float.Parse(file.ReadLine());
                    neurons[j].weights.Add(weight);
                }
            }
            NNLayer oLayer = new NNLayer();
            oLayer.SetNeurons(neurons, neurons.Count, inputAmount);
            hiddenLayers.Add(oLayer);
            file.Dispose();            
        }
    }

    public void ReleaseNN() {
        if (inputLayer != null) {
            inputLayer = null;
        }
        if (outputLayer != null) {
            outputLayer = null;
        }
        for (int i = 0; i < hiddenLayers.Count; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        } // todo: not sure if I need this loop, come back to this later
        hiddenLayers.Clear();
    }

    public void FromGenome(ref Genome _genome, int _input, int _neuronsPerHidden, int _output)
    {
        ReleaseNN();
        outputAmount = _output;
        inputAmount = _input;
        NNLayer hidden = new NNLayer();
        List<Neuron> hiddenNeurons = new List<Neuron>();
        hiddenNeurons.Capacity = _neuronsPerHidden;
        for (int i = 0; i < _neuronsPerHidden; i++) {
            List<float> weights = new List<float>();
            weights.Capacity = _input + 1;
            for (int j = 0; j < _input + 1; j++) {
                weights.Add(_genome.weights[i * _neuronsPerHidden + j]);
            }
            hiddenNeurons.Add(null);
            hiddenNeurons[i] = new Neuron();
            hiddenNeurons[i].weights = weights;
            hiddenNeurons[i].numberOfInputs = _input;
        }
        hidden.LoadLayer(hiddenNeurons);
        hiddenLayers.Add(hidden);
        List<Neuron> outputNeurons = new List<Neuron>();
        outputNeurons.Capacity = _output;
        for (int i = 0; i < _output; i++) {
            List<float> weights = new List<float>();
            weights.Capacity = _neuronsPerHidden + 1;
            for (int j = 0; j < _neuronsPerHidden + 1; j++) {
                weights.Add(_genome.weights[i * _neuronsPerHidden + j]);
            }
            outputNeurons.Add(null);
            outputNeurons[i] = new Neuron();
            outputNeurons[i].weights = weights;
            outputNeurons[i].numberOfInputs = _input;
        }
        outputLayer = new NNLayer();
        outputLayer.LoadLayer(outputNeurons);
    }
}