using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;


public class NeuralNetwork : MonoBehaviour
{

    private int inputAmount, outputAmount;
    private List<float> inputs;
    private NNLayer inputLayer;
    private List<NNLayer> hiddenLayers;
    private NNLayer outputLayer;
    private List<float> outputs;
    private List<float> weights;

    public NeuralNetwork()
    {
        inputLayer = null;
        outputLayer = null;
    }

    ~NeuralNetwork()
    {
        if (inputLayer != null) {
            inputLayer = null;
        }
        if (outputLayer != null) {
            outputLayer = null;
        }
        for (int i = 0; i < hiddenLayers.Capacity; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        } // not sure if I need this loop
        hiddenLayers.Clear();
    }

    public void UpdateNN()
    {
        outputs.Clear();
        for (int i = 0; i < hiddenLayers.Capacity; i++)
        {
            if (i > 0)
            {
                inputs = outputs;
            }
            hiddenLayers[i].Evaluate(inputs, ref outputs);
        }
        inputs = outputs;
        outputLayer.Evaluate(inputs, ref outputs);
    }

    public void SetInput(List<float> _in)
    {
        inputs = _in;
    }

    public float GetOutput(int _ID)
    {
        if (_ID >= outputAmount)
        {
            return 0.0f;
        }
        return outputs[_ID];
    }

    public int GetTotalOutputs()
    {
        return outputAmount;
    }

    public void ExportNN(string _filename)
    {
        /*
         char buff[128] = {0};
		sprintf(buff, "ExportedNNs/%s", filename);
		std::ofstream file;
		file.open(buff);

		file << "<NeuralNetwork>" << endl;
		file <<"TotalOuputs=" << this->outputAmount << endl;
		file <<"TotalInputs=" << this->inputAmount << endl;
		// Export hidden layerss.
		for (unsigned int i = 0; i < hiddenLayers.size(); i++)
		{
			hiddenLayers[i]->SaveLayer(file, "Hidden");
		}
		// Export output layer.
		outputLayer->SaveLayer(file, "Output");
		file << "</NeuralNetwork>" << endl;

		file.close();
         */
    }

    public void ImportNN(string _filename)
    {
        /*
         		FILE* file = fopen(filename,"rt");

		if(file!=NULL)
		{
			enum LayerType
			{
				HIDDEN,
				OUTPUT,
			};

			float weight = 0.0f;
			int totalNeurons = 0;
			int totalWeights = 0;
			int totalInputs = 0;
			int currentNeuron = 0;
			std::vector<Neuron> neurons;
			std::vector<float> weights;
			LayerType type = HIDDEN;


			char buffComp[1024] ={0};

			while(fgets(buffComp,1024,file))
			{
				char buff[1024] = {0};

				if(buffComp[strlen(buffComp)-1]=='\n')
				{
					for(unsigned int i = 0; i<strlen(buffComp)-1;i++)
					{
						buff[i] = buffComp[i];
					}
				}

				if(0 == strcmp(buff, "<NeuralNetwork>"))
				{
				}
				else if (0 == strcmp(buff,"</NeuralNetwork>"))
				{
					break;
				}
				else if (0 == strcmp(buff,"<NLayer>"))
				{
					weight = 0.0f;
					totalNeurons = 0;
					totalWeights = 0;
					totalInputs = 0;
					currentNeuron = 0;
					neurons.clear();
					type = HIDDEN;
				}
				else if (0 == strcmp(buff,"</NLayer>"))
				{
					NLayer* layer = new NLayer();
					layer->SetNeurons(neurons, neurons.size(), totalInputs);
					switch (type)
					{
					case HIDDEN:
						this->hiddenLayers.push_back(layer);
						layer = NULL;
						break;
					case OUTPUT:
						this->outputLayer = layer;
						layer = NULL;
						break;
					};
				}
				else if (0 == strcmp(buff,"<Neuron>"))
				{
					weights.clear();
				}
				else if (0 == strcmp(buff,"</Neuron>"))
				{
					neurons[currentNeuron].Initilise(weights, totalInputs);
					currentNeuron++;
				}
			
				else
				{
					char* token = strtok(buff, "=");
					if(token != NULL)
					{
						char* value = strtok(NULL,"=");


						if (0 == strcmp(token,"Type"))
						{
							if (0 == strcmp("Hidden", value))
							{
								type = HIDDEN;
							}
							else if (0 == strcmp("Output", value))
							{
								type = OUTPUT;
							}
						}
						else if (0 == strcmp(token,"NNInputs"))
						{
							totalInputs = atoi(value);
						} 
						else if (0 == strcmp(token,"Neurons"))
						{
							totalNeurons = atoi(value);
						} 
						else if (0 == strcmp(token,"Weights"))
						{
							totalWeights = atoi(value);
						} 
						else if (0 == strcmp(token,"W"))
						{
							weight = (float)atof(value);
						} 
						else if (0 == strcmp(token,"TotalOuputs"))
						{
							outputAmount = atoi(value);
						} 
						else if (0 == strcmp(token,"TotalInputs"))
						{
							inputAmount = atoi(value);
						} 
					}
				}
			}
			fclose(file);
		}
         */
    }

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

    public void ReleaseNN()
    {
        if (inputLayer != null) {
            inputLayer = null;
        }
        if (outputLayer != null) {
            outputLayer = null;
        }
        for (int i = 0; i < hiddenLayers.Capacity; i++) {
            if (hiddenLayers[i] != null) {
                hiddenLayers[i] = null;
            }
        } // not sure if I need this loop
        hiddenLayers.Clear();
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
            weights.Clear();
            hiddenLayers[i].GetWeights(ref weights);
            for (int j = 0; j < weights.Capacity; j++)
            {
                genome.weights.Add(weights[j]);
            }
        }

        weights.Clear();
        outputLayer.GetWeights(ref weights);
        for (int i = 0; i < weights.Capacity; i++)
        {
            genome.weights.Add(weights[i]);
        }

        return genome;
    }

    public void FromGenome(ref Genome _genome, int _input, int _neuronsPerHidden, int _output)
    {
        ReleaseNN();
        outputAmount = _output;
        inputAmount = _input;
        int weightsForHidden = _input * _neuronsPerHidden;
        NNLayer hidden = new NNLayer();
        List<Neuron> neurons = null;
        neurons.Capacity = _neuronsPerHidden;
        for (int i = 0; i < _neuronsPerHidden; i++)
        {
            weights.Clear();
            weights.Capacity = _input + 1;
            for (int j = 0; j < _input + 1; j++)
            {
                weights[j] = _genome.weights[i * _neuronsPerHidden + j];
            }
            neurons[i].Initilise(weights, _input);
        }
        hidden.LoadLayer(neurons);
        hiddenLayers.Add(hidden);

        int weightsForOutput = _neuronsPerHidden * _output;
        neurons.Clear();
        neurons.Capacity = _output;
        for (int i = 0; i < _output; i++)
        {
            weights.Clear();
            weights.Capacity = _neuronsPerHidden + 1;
            for (int j = 0; j < _neuronsPerHidden + 1; j++)
            {
                weights[j] = _genome.weights[i * _neuronsPerHidden + j];
            }
            neurons[i].Initilise(weights, _neuronsPerHidden);
        }
        outputLayer = new NNLayer();
        outputLayer.LoadLayer(neurons);
    }

}

/*
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
*/

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