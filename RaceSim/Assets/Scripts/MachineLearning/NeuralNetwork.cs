using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using UnityEngine;

public class NeuralNetwork
{

    private int inputAmount, outputAmount;
    private List<float> inputs;
    private NNLayer inputLayer;
    private List<NNLayer> hiddenLayers;
    private NNLayer outputLayer;
    private List<float> outputs;

    public NeuralNetwork()
    {
        inputLayer = new NNLayer();
        outputLayer = new NNLayer();
        hiddenLayers = new List<NNLayer>();
        outputs = new List<float>();
    }

    ~NeuralNetwork()
    {
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
        } // not sure if I need this loop
        hiddenLayers.Clear();
    }

    public void UpdateNN()
    {
        outputs = new List<float>();
        for (int i = 0; i < hiddenLayers.Count; i++)
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
        TextWriter write = new StreamWriter(_filename);
        write.WriteLine(inputAmount);
        write.WriteLine(outputAmount);
        // Hidden Layers
        write.WriteLine(hiddenLayers.Count);
        for (int i = 0; i < hiddenLayers.Count; i++) {
            hiddenLayers[i].SaveLayer(ref write);
        }
        // Outer Layer
        outputLayer.SaveLayer(ref write);
        write.Dispose();


    
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
        for (int i = 0; i < hiddenLayers.Count; i++) {
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

    public void FromGenome(ref Genome _genome, int _input, int _neuronsPerHidden, int _output)
    {
        ReleaseNN();
        outputAmount = _output;
        inputAmount = _input;
        NNLayer hidden = new NNLayer();
        List<Neuron> hiddenNeurons = new List<Neuron>();
        hiddenNeurons.Capacity = _neuronsPerHidden;
        
        for (int i = 0; i < _neuronsPerHidden; i++)
        {
            List<float> weights = new List<float>();
            weights.Capacity = _input + 1;
            for (int j = 0; j < _input + 1; j++)
            {
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
        for (int i = 0; i < _output; i++)
        {
            List<float> weights = new List<float>();
            weights.Capacity = _neuronsPerHidden + 1;
            for (int j = 0; j < _neuronsPerHidden + 1; j++)
            {
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