using System;
using System.Collections.Generic;
using System.IO;

/// <summary>
/// This class manages the heart of the Neural Network
/// gamthering inputs and executing the updates to provide
/// outputs. Functions for exporting and importing take 
/// place here.
/// </summary>
public class NeuralNetwork {

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

    /// <summary>
    /// Updates the input values which have been provided by the Input Manager
    /// in preparation for the update of  the Neural Network (next function 
    /// UpdateNN()).
    /// </summary>
    /// <param name="_in">Input values to be evaluated by the NN</param>
    public void SetInput(List<float> _in) {
        inputs = _in;
    }

    /// <summary>
    /// This function handles the updating of the Neural Network.
    /// 1. Creates an empty list for the outputs for each layer
    /// 2. Starts by iterating through the hidden layer(s) then 
    /// the output layer
    /// 3. If its the first iteration that the inputs are 
    /// provided by the input manager e.g. from the senors.
    /// 4. For all other iteration the outputs from the previous layer 
    /// become the inputs.
    /// </summary>
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


    /// <summary>
    /// Getter for the output values by id
    /// </summary>
    /// <param name="_ID">Defines which output to return</param>
    /// <returns>Desired output value</returns>
    public float GetOutput(int _ID) {
        if (_ID >= outputAmount) {
            return 0.0f;
        }
        return outputs[_ID];
    }

    /// <summary>
    /// Exports to a .csv file the current values within the hidden 
    /// and output layers which can be imported later to perform 
    /// the test again
    /// </summary>
    /// <param name="_filename">Filename and Location</param>
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

    /// <summary>
    /// Returns the state of the Neural Network back to a previously 
    /// exported start that will reproduce the same outcome
    /// </summary>
    /// <param name="_filename">Filename and Location</param>
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

    /// <summary>
    /// Clears the Neural Network in prepration to create a new one.
    /// </summary>
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

    /// <summary>
    /// This class populates each neuron form the weight provided from the Genome. 
    /// Generating a new Neural Network, this provides the infrastructure for the 
    /// decision making process.
    /// </summary>
    /// <param name="_genome">Current genome to be executed</param>
    /// <param name="_input">Total number of inputs</param>
    /// <param name="_neuronsPerHidden">Total number of neurons per hidden layer</param>
    /// <param name="_output">Total number of outputs</param>
    public void PopulateNeuronsFromGenome(ref Genome _genome, int _input, int _neuronsPerHidden, int _output) {
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