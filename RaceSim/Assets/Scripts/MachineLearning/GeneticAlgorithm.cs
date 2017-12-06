using System.Collections;
using System.Collections.Generic;
using System.Runtime.InteropServices;
using UnityEngine;

public class GeneticAlgorithm {

    // TODO : Come back and delete the following
    /*
     public Genome GetBestGenome()
    {
        int bestGenome = -1;
        float fitness = 0;
        for (int i = 0; i < population.Count; i++) {
            if (population[i].fitness > fitness) {
                fitness = population[i].fitness;
                bestGenome = i;
            }
        }
        return population[bestGenome];
    }
    public Genome GetWorstGenome()
    {
        int worstGenome = -1;
        float fitness = 999999999;
        for (int i = 0; i < population.Count; i++) {
            if (population[i].fitness < fitness) {
                fitness = population[i].fitness;
                worstGenome = i;
            }
        }
        return population[worstGenome];
    }
    public Genome GetGenome(int _index)
    {
        if (_index >= totalPopulation) {
            return null;
        }
        return population[_index];
    }
    public int GetCurrentGenomeID() { return population[currentGenome].ID; }

    public int GetCurrentGeneration() { return generation; }

    public int GetTotalPopulation() { return totalPopulation; }
    */

    private int currentGenome, genomeID, generation, totalPopulation;
    private List<Genome> population;
    private float bestEverFitness;
    private Genome bestEverGenome;

    /// <summary>
    /// Constructor, sets initial starting values
    /// </summary>
    public GeneticAlgorithm() {
        currentGenome = 0;
        totalPopulation = 0;
        genomeID = 0;
        generation = 1;
        population = new List<Genome>();
        bestEverFitness = 0f;
    }

    /// <summary>
    /// Deconstructor, Removes all population
    /// </summary>
    ~GeneticAlgorithm() {
        ClearPopulation();
    }

    /// <summary>
    /// Provides the next available Gennome, when required to the Entity Manager
    /// </summary>
    /// <returns>Returns the next Genome</returns>
    public Genome GetNextGenome() {
        currentGenome++;
        if (currentGenome >= population.Count || currentGenome < 0) {
            return null;
        }
        return population[currentGenome];
    }
    
    /// <summary>
    /// Returns the current genome index value
    /// </summary>
    /// <returns>Current Genome Index value</returns>
    public int GetCurrentGenomeIndex() { return currentGenome; }

    /// <summary>
    /// This function updates the List of Genome with a nominated number of the 
    /// top performing (highest fitness) genomes from the last generation / population.
    /// Currently the number of genomes obtained is set to 4, this could be later 
    /// updated to a percentage of the total max population to ensure consistancy.
    /// </summary>
    /// <param name="_totalGenomes">How many genomes do you want returned in the list</param>
    /// <param name="_out">List to be updated with the best genomes for cross breeding</param>
    private void GetBestCases(int _totalGenomes, ref List<Genome> _out) {
        int genomeCount = 0;
        int runCount = 0;
        while (genomeCount < _totalGenomes) {
            if (runCount > 10) {
                return;
            }
            runCount++;
            float bestFitness = 0;
            int bestIndex = -1;
            for (int i = 0; i < population.Count; i++) {
                if (population[i].fitness > bestFitness) {
                    bool isUsed = false;
                    for (int j = 0; j < _out.Count; j++) {
                        if (_out[j].ID == population[i].ID) {
                            isUsed = true;
                        }
                    }
                    if (!isUsed) {
                        bestIndex = i;
                        bestFitness = population[bestIndex].fitness;
                    }
                }
            }
            if (bestIndex != -1) {
                genomeCount++;
                _out.Add(population[bestIndex]);
            }
        }
        if (_out[0].fitness > bestEverFitness) {
            bestEverFitness = _out[0].fitness;
            bestEverGenome = new Genome();
            bestEverGenome = _out[0];
        }
    }

    /// <summary>
    /// This function takes in two existing Genomes and and takes the weights and 
    /// splits them at a random section and populates the remaining weights with 
    /// the second genome to ensure the Capacity remains unchanged, this produces 
    /// two babys which have a mixer of the two original genome.
    /// </summary>
    /// <param name="_g1">Original genome one</param>
    /// <param name="_g2">Original genome two</param>
    /// <param name="_baby1">Returned baby one</param>
    /// <param name="_baby2">Returned baby two</param>
    private void CrossBreed(Genome _g1, Genome _g2, ref Genome _baby1, ref Genome _baby2) {
        int totalWeights = _g1.weights.Capacity;
        int crossOver = Random.Range(0, totalWeights);
        _baby1 = new Genome();
        _baby1.ID = genomeID;
        _baby1.weights.Capacity = totalWeights;
        genomeID++;
        _baby2 = new Genome();
        _baby2.ID = genomeID;
        _baby2.weights.Capacity = totalWeights;
        genomeID++;
        for (int i = 0; i < crossOver; i++) {
            _baby1.weights.Add(_g1.weights[i]);
            _baby2.weights.Add(_g2.weights[i]);
        }
        for (int i = crossOver; i < totalWeights; i++) {
            _baby1.weights.Add(_g2.weights[i]);
            _baby2.weights.Add(_g1.weights[i]);
        }
    }

    /// <summary>
    /// Generates a new genome and populates starting weight float values 
    /// between -1.0 and +1.0. Typically only used at the start
    /// </summary>
    /// <param name="_totalWeights">Total weights required for this configuration</param>
    /// <returns></returns>
    private Genome CreateNewGenome(int _totalWeights) {
        Genome genome = new Genome();
        genome.ID = genomeID;
        genome.fitness = 0.0f;
        genome.weights.Capacity = _totalWeights;
        for (int i = 0; i < _totalWeights; i++) {
            genome.weights.Add(Random.Range(-1.0f, 1.0f));
        }
        genomeID++;
        return genome;
    }

    /// <summary>
    /// Populates a brand new population of genomes up to the 
    /// maximum population total.
    /// </summary>
    /// <param name="_newTotalPopulation">Maximum population for all generations</param>
    /// <param name="_totalWeights">Total weights required for this confiuration</param>
    public void GenerateNewPopulation(int _newTotalPopulation, int _totalWeights) {
        generation = 1;
        ClearPopulation();
        currentGenome = 0;
        totalPopulation = _newTotalPopulation;
        population.Capacity = _newTotalPopulation;
        for (int i = 0; i < population.Capacity; i++) {
            Genome genome = CreateNewGenome(_totalWeights);
            population.Add(genome);
        }
        EventManagerOneArg.TriggerEvent(ConstantManager.UI_GENERATION, generation);
    }

    /// <summary>
    /// This function is called when the previous generation has completed
    /// all tests. The four best genomes from the previous generation are identified
    /// The four best are then added to the next generation after the mutation process
    /// has ben complete. It then adds the overall best performing genome which does 
    /// not recieve mutation. After this a cross breeding process takes place, each 
    /// child that is returned is then also mutated. Lastly if there are any remaining
    /// spaces to reach maximum population then brand new random genomes are generated
    /// and added to the next population.
    /// </summary>
    public void BreedPopulation()
    {
        List<Genome> bestGenomes = new List<Genome>();
        GetBestCases(4, ref bestGenomes);
        List<Genome> children = new List<Genome>();
        Genome topGenome = new Genome();
        for (int i = 0; i < bestGenomes.Count; i++) {
            SetUpTopGenome(ref topGenome, bestGenomes[i]);
            Mutate(topGenome);
            children.Add(topGenome);
        }
        SetUpTopGenome(ref topGenome, bestEverGenome);
        children.Add(topGenome);
        Genome child1 = null;
        Genome child2 = null;
        int crossBreedIteration = Mathf.Abs((ConstantManager.MAXIMUM_GENOME_POPULATION - children.Count) / (bestGenomes.Count + 2));
        for (int i = 0; i < crossBreedIteration; i++) {
            for (int j = 1; j < bestGenomes.Count; j++) {
                CrossBreed(bestGenomes[i], bestGenomes[j], ref child1, ref child2);
                Mutate(child1);
                children.Add(child1);
                Mutate(child2);
                children.Add(child2);
            }
        }
        int remainingChildren = totalPopulation - children.Count;
        for (int i = 0; i < remainingChildren; i++) {
            children.Add(CreateNewGenome(bestGenomes[0].weights.Count));
        }
        ClearPopulation();
        population = children;
        currentGenome = 0;
        generation++;
        EventManagerOneArg.TriggerEvent(ConstantManager.UI_GENERATION, generation);
    }

    /// <summary>
    /// Configures the new child to be added into the children list in the previous function.
    /// </summary>
    /// <param name="_g">New genome</param>
    /// <param name="_add">Existing genome</param>
    /// <returns>Returns a configured genome</returns>
    public Genome SetUpTopGenome(ref Genome _g, Genome _add) {
        _g = new Genome();
        _g.fitness = 0.0f;
        _g.ID = genomeID;
        genomeID++;
        _g.weights = _add.weights;
        return _g;
    }

    /// <summary>
    /// Clears all information from the current population, in
    /// preparation for the next.
    /// </summary>
    public void ClearPopulation() {
        if (population.Count > 0) {
            for (int i = 0; i < population.Count; i++) {
                if (population[i] != null) {
                    population[i] = null;
                }
            }
        }
        population = new List<Genome>();
    }

    /// <summary>
    /// The class provides an 8% mutation rate for a genome that is 
    /// passed into it. Depending on the random hit, provides different
    /// effects on the current weight value.
    /// </summary>
    /// <param name="_genome">Genome that requires to be mutated</param>
    private void Mutate(Genome _genome) {
        for (int i = 0; i < _genome.weights.Count; i++) {
            float mutationLottery = Random.Range(0f, 100f);
            if (mutationLottery <= 2f) {
                _genome.weights[i] *= -1;
            } else if (mutationLottery <= 4f) {
                _genome.weights[i] = Random.Range(-0.5f, 0.5f);
            } else if (mutationLottery <= 6f) {
                _genome.weights[i] *= Random.Range(1.0f, 2.0f);
            } else if (mutationLottery <= 8f) {
                _genome.weights[i] *= Random.Range(0.0f, 1.0f);
            }
        }
    }

    /// <summary>
    /// Sets the fitness of a genome after its test
    /// </summary>
    /// <param name="_fitness">New fitness</param>
    /// <param name="_index">Genome index value</param>
    public void SetGenomeFitness(float _fitness, int _index) {
        if (_index >= population.Count || _index < 0) {
            return;
        }
        population[_index].fitness = _fitness;
    }

   
}
