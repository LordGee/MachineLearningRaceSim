using System.Collections.Generic;
using System.IO;

/// <summary>
/// This class provides a liason between the Car Manager and the AI AGents, 
/// Nerual Network, Genetic Algorithm and the Genome Population.
/// </summary>
public class EntityManager {

    private AIAgent aiAgent;
    private NeuralNetwork nn;
    private GeneticAlgorithm ga;
    private float currentFitness, bestFitness, newFitness;
    private int totalWeights, failCounter;
    private List<float> inputs;
    private float[] outputs;
    private bool resetPosition;

    /// <summary>
    /// Constructor for Generating a new Entity Manager
    /// This constructor has two paths which are seperated by two bool values obtained froom the car manager:
    /// 1. For generating the machine learning set-up
    /// 2. For importing / loading and existing agent.
    /// 
    /// Option 1:
    /// A new Genetic Algoritm class is generated, the total weights are obtained
    /// and a population of Genomes are generated which are initally random values.
    /// The first Genome in the population is obtained who's values are populated 
    /// into the neural network, ready for evaluating. The AI agent is then constructed 
    /// and the NN is applied to that agent.
    /// 
    /// Option 2:
    /// A Neural Network and AI Agent are constructed and the NN is attached to the AI Agent.
    /// After which the saved informaation can then be imported into the AI Agents Neural Network 
    /// </summary>
    public EntityManager() {
        if (CarManager.machineAI) {
            ga = new GeneticAlgorithm();
            totalWeights = GetTotalWeight();
            ga.GenerateNewPopulation(
                ConstantManager.MAXIMUM_GENOME_POPULATION,
                totalWeights);
            currentFitness = 0.0f;
            bestFitness = PlayerPrefsController.GetFitness();
            EventManager.TriggerEvent(ConstantManager.UI_BEST_FITNESS, bestFitness);
            nn = new NeuralNetwork();
            Genome g = ga.GetNextGenome();
            nn.PopulateNeuronsFromGenome(ref g,
                (int)ConstantManager.NNInputs.INPUT_COUNT,
                ConstantManager.HIDDEN_LAYER_NEURONS,
                (int)ConstantManager.NNOutputs.OUTPUT_COUNT);
            aiAgent = new AIAgent();
            resetPosition = true;
            aiAgent.AttachNeuralNetwork(nn);
        }
        if (CarManager.loadBest) {
            nn = new NeuralNetwork();
            aiAgent = new AIAgent();
            aiAgent.AttachNeuralNetwork(nn);
            ImportExistingAgent();
            resetPosition = true;
        }
    }

    /// <summary>
    /// Calculates the total weight required based on the settings for "Inputs", 
    /// "Outputs" and the number of neurons int the "Hidden Layer" values within the
    /// Constant Manager Class.
    /// </summary>
    /// <returns>Returns total weights required for this NN</returns>
    private int GetTotalWeight() {
        return (int)ConstantManager.NNInputs.INPUT_COUNT *
            ConstantManager.HIDDEN_LAYER_NEURONS *
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT +
            ConstantManager.HIDDEN_LAYER_NEURONS +
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT;
    } 

    /// <summary>
    /// Saves the current AI Agents Neural Network values to a csv file which can later be imported.
    /// </summary>
    public void ExportCurrentAgent() {
        aiAgent.GetNeuralNetwork().ExportNN(@"~\..\Assets\Data\" + PlayerPrefsController.GetTrack() + @"\" + currentFitness + ".csv");
        AddToList();
    }

    /// <summary>
    /// Loads back in a previously exported Neural Network values.
    /// </summary>
    public void ImportExistingAgent() {
        aiAgent.GetNeuralNetwork().ImportNN(@"~\..\Assets\Data\" + PlayerPrefsController.GetTrack() + @"\" + PlayerPrefsController.GetFitness() + ".csv");
    }

    /// <summary>
    /// When exporting various NN's, this can get messy and this helps organise all exported
    /// data when obtaining the data in the Main Menu.
    /// </summary>
    private void AddToList() {
        using (TextWriter tw = new StreamWriter(@"~\..\Assets\Data\List.csv", true)) {
            tw.WriteLine(PlayerPrefsController.GetTrack());
            tw.WriteLine(currentFitness);
        }
    }

    /// <summary>
    /// After an AI Agent fails this function will get the next Genome values for the next AI Agent
    /// </summary>
    public void NextTestSubject() {
        ga.SetGenomeFitness(currentFitness, ga.GetCurrentGenomeIndex());
        currentFitness = 0.0f;
        Genome g = ga.GetNextGenome();
        nn.PopulateNeuronsFromGenome(ref g, 
            (int)ConstantManager.NNInputs.INPUT_COUNT, 
            ConstantManager.HIDDEN_LAYER_NEURONS, 
            (int)ConstantManager.NNOutputs.OUTPUT_COUNT);
        aiAgent = new AIAgent();
        resetPosition = true;
        aiAgent.AttachNeuralNetwork(nn);
    }

    /// <summary>
    /// This function is only used when existing data has been imported. 
    /// This allows the agent to maintain the current data after completion.
    /// </summary>
    private void ReloadExistingAgent() {
        nn = new NeuralNetwork();
        aiAgent = new AIAgent();
        resetPosition = true;
        aiAgent.AttachNeuralNetwork(nn);
        ImportExistingAgent();
    }

    /// <summary>
    /// Once an AI Agent has failed we need a new Genome, this function 
    /// checks if the current Genome is at the end of the population. If 
    /// it is the population is then evolved and a new generation is generated. 
    /// Either way the next available genome is then obtained.
    /// </summary>
    public void ForceToNextAgent() {
        if (ga.GetCurrentGenomeIndex() == ConstantManager.MAXIMUM_GENOME_POPULATION - 1) {
            ga.BreedPopulation();
        } 
        NextTestSubject();
    }

    /// <summary>
    /// This manual Update function is called every frame from the car manager
    /// This passes in the currently available inputs to the AI Agent and forces 
    /// the manual update process within that class. It checks if the agent has 
    /// failed to generate sufficent progress within 60 frame, this prevent the 
    /// machine learning process from stalling when an AI Agent decides to just 
    /// hold down the brake. Other checks are perormed to ensure the AI Agents 
    /// status and events are triggered to update the GUI Canvas and fitness 
    /// statistics. Current fitness is also calculated here.
    /// </summary>
    public void ManualUpdate() {
        aiAgent.SetInputs(inputs);
        aiAgent.ManualUpdate();
        outputs = aiAgent.GetOutputs();
        if (failCounter > 60) {
            AgentFailed();
            failCounter = 0; 
        }
        if (CarManager.loadBest) {
            if (aiAgent.HasAgentFailed()) {
                ReloadExistingAgent();
            }
        }
        if (CarManager.machineAI) {
            if (newFitness == 0) { failCounter++; } else { failCounter = 0; }
            currentFitness += (newFitness / 2.0f);
            EventManager.TriggerEvent(ConstantManager.UI_FITNESS, currentFitness);
            if (aiAgent.HasAgentFailed()) {
                if (currentFitness > bestFitness) {
                    bestFitness = currentFitness;
                    if (currentFitness > ConstantManager.COMPLETION_BONUS) {
                        ExportCurrentAgent();
                    }
                }
                EventManager.TriggerEvent(ConstantManager.UI_POPULATION, currentFitness);
                ForceToNextAgent();
            }
        }
    }

    /// <summary>
    /// Input values are passed in to the Entity Manager from the Car Manager via this function
    /// </summary>
    /// <param name="_currentInputs">The current list of input values</param>
    public void PrepareInputs(List<float> _currentInputs) { inputs = _currentInputs; }

    /// <summary>
    /// After the NN has processed the inputs and hidden layer values the outputs are 
    /// then obtainable via this function prior to performing the actual movement operation.
    /// </summary>
    /// <returns></returns>
    public float[] GetCurrentOutputs() { return outputs; }

    /// <summary>
    /// Obtains the next fitness value to be accumalated to the current fitness
    /// </summary>
    /// <param name="_fit">Fitness amount to be added to the current AI Agent</param>
    public void SetNewFitness(float _fit) { newFitness = _fit; }

    /// <summary>
    /// If the AI Agent completed track a bonus fitness is added to ensure there is 
    /// a bigger difference between completing the track and almost finishing.
    /// </summary>
    /// <param name="_value">Bonus fitness for completion of a track</param>
    public void AddCompletionFitness(float _value) { currentFitness += _value; }

    /// <summary>
    /// If the AI Agent fails this triggers and sets the has failed booleon to true
    /// </summary>
    public void AgentFailed() { aiAgent.SetFailed(); }

    /// <summary>
    /// Informs the car manager if the AI Agents position needs to be reset to the start position.
    /// </summary>
    /// <returns>Returns true if the Agent needs the position to be reset</returns>
    public bool GetResetPosition() { return resetPosition; }

    /// <summary>
    /// Once the Agents position has been reset, this function just resets the value to false 
    /// </summary>
    public void CompleteResetPosition() { resetPosition = false; }
    
}
