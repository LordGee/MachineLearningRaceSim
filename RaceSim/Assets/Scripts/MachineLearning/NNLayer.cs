using System.Collections.Generic;
using System.IO;
using UnityEngine;

/// <summary>
/// Individual layer and a collection of neurons
/// </summary>
public class NNLayer {

    private int totalNeurons;
    private List<Neuron> neurons;

    /// <summary>
    /// Constructor
    /// </summary>
    public NNLayer() {
        totalNeurons = -1;
        neurons = new List<Neuron>();
    }

    /// <summary>
    /// This function evaluates the inputs of the previous layer, by iterating through each neuron
    /// performing a calculation that will be a primer for the output value which is accumalated by 
    /// (input value * each weight of a neuron). An additional value is also added is an additrional 
    /// neuron weight that is mutiplied by the bias which is -1.0f. this is then added to the list 
    /// of outputs after perfroming a Sigmoid calculation.
    /// </summary>
    /// <param name="_input">List of input values (output from previous layer if not the first)</param>
    /// <param name="_output">Output list result after evaluating the results</param>
    public void Evaluate(List<float> _input, ref List<float> _output) {
        int inputIndex = 0;
        for (int i = 0; i < totalNeurons; i++) {
            float activation = 0.0f;
            for (int j = 0; j < neurons[i].numberOfInputs - 1; j++) {
                activation += _input[inputIndex] * neurons[i].weights[j];
                inputIndex++;
            }
            activation += neurons[i].weights[neurons[i].numberOfInputs] * ConstantManager.BIAS;
            _output.Add(Sigmoid(activation, 1.0f));
            inputIndex = 0;
        }
    }

    /// <summary>
    /// Called by the Export function which saves the current layer to the csv file
    /// </summary>
    /// <param name="_file">Instantiate text writer file</param>
    public void SaveLayer(ref TextWriter _file) {
        _file.WriteLine(neurons.Count);
        for (int i = 0; i < neurons.Count; i++) {
            _file.WriteLine(neurons[i].weights.Count);
            for (int j = 0; j < neurons[i].weights.Count; j++) {
                _file.WriteLine(neurons[i].weights[j]);
            }
        }
    }

    /// <summary>
    /// This function loads in the neurons for a specific layer when 
    /// populating a layer from a genome.
    /// </summary>
    /// <param name="_neuron">List of neurons</param>
    public void LoadLayer(List<Neuron> _neuron) {
        totalNeurons = _neuron.Count;
        neurons = _neuron;
    }

    /// <summary>
    /// Called by the import function to load back in the neurons for a given layer.
    /// </summary>
    /// <param name="_neurons">List of neurons to be added</param>
    /// <param name="_numberNeurons">Number of neurons for this layer</param>
    /// <param name="_inputs">Number of inputs for this layer</param>
    public void SetNeurons(List<Neuron> _neurons, int _numberNeurons, int _inputs)
    {
        totalNeurons = _numberNeurons;
        neurons = _neurons;
    }

    /// <summary>
    /// Math function Sigmoid
    /// </summary>
    /// <param name="_a"></param>
    /// <param name="_p"></param>
    /// <returns>Returns result of sigmoid</returns>
    public float Sigmoid(float _a, float _p) {
        float ap = (-_a) / _p;
        return (1 / (1 + Mathf.Exp(ap)));
    }
}
