using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GeneticAlgorithm
{

    private int currentGenome, genomeID, generation, totalGenomeWeight;
    private int totalPopulation;
    private List<Genome> population;
    private int[] crossoverSplits;

    public GeneticAlgorithm()
    {
        currentGenome = 0;
        totalPopulation = 0;
        genomeID = 0;
        generation = 1;
        population = new List<Genome>();
    }

    ~GeneticAlgorithm()
    {
        ClearPopulation();
    }

    public Genome GetNextGenome()
    {
        currentGenome++;
        if (currentGenome >= population.Count || currentGenome < 0) {
            return null;
        }
        return population[currentGenome];
    }

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

    public int GetCurrentGenomeIndex() { return currentGenome; }

    public int GetCurrentGenomeID() { return population[currentGenome].ID; }

    public int GetCurrentGeneration() { return generation; }

    public int GetTotalPopulation() { return totalPopulation; }


    private void GetBestCases(int _totalGenomes, ref List<Genome> _out)
    {
        int genomeCount = 0;
        int runCount = 0;
        while (genomeCount < _totalGenomes) {
            if (runCount > 10) {
                return;
            }
            runCount++;
            // Find the best cases for cross breeding based on fitness score.
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
    }

    private void CrossBreed(Genome _g1, Genome _g2, ref Genome _baby1, ref Genome _baby2)
    {
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

        for (int i = 0; i < crossOver; i++)
        {
            _baby1.weights.Add(_g1.weights[i]);
            _baby2.weights.Add(_g2.weights[i]);
        }

        for (int i = crossOver; i < totalWeights; i++)
        {
            _baby1.weights.Add(_g2.weights[i]);
            _baby2.weights.Add(_g1.weights[i]);
        }
    }

    private Genome CreateNewGenome(int _totalWeights) {
        Genome genome = new Genome();
        genome.ID = genomeID;
        genome.fitness = 0.0f;
        genome.weights.Capacity = _totalWeights;
        for (int i = 0; i < _totalWeights; i++)
        {
            genome.weights.Add(Random.Range(-1.0f, 1.0f));
        }
        genomeID++;
        return genome;
    }

    public void GenerateNewPopulation(int _newTotalPopulation, int _totalWeights)
    {
        generation = 1;
        ClearPopulation();
        currentGenome = 0;
        totalPopulation = _newTotalPopulation;
        population.Capacity = _newTotalPopulation;
        for (int i = 0; i < population.Capacity; i++) {
            Genome genome = new Genome();
            genome.ID = genomeID;
            genome.fitness = 0.0f;
            genome.weights.Capacity = _totalWeights;
            for (int j = 0; j < _totalWeights; j++) {
                genome.weights.Add(Random.Range(-1.0f, 1.0f));
            }
            genomeID++;
            population.Add(genome);
        }
        EventManagerOneArg.TriggerEvent(ConstantManager.UI_GENERATION, generation);
    }

    public void BreedPopulation()
    {
        List<Genome> bestGenomes = new List<Genome>();
        // Find the four best genomes
        GetBestCases(4, ref bestGenomes);

        List<Genome> children = new List<Genome>();

        Genome topGenome = new Genome();
        topGenome.fitness = 0.0f;
        topGenome.ID = bestGenomes[0].ID;
        topGenome.weights = bestGenomes[0].weights;
        children.Add(topGenome); // Add original top without mutation

        topGenome = new Genome();
        topGenome.fitness = 0.0f;
        topGenome.weights = bestGenomes[0].weights;
        topGenome.ID = genomeID;
        genomeID++;
        Mutate(topGenome);
        children.Add(topGenome); // After mutation

        topGenome = new Genome();
        topGenome.fitness = 0.0f;
        topGenome.weights = bestGenomes[1].weights;
        topGenome.ID = genomeID;
        genomeID++;
        Mutate(topGenome);
        children.Add(topGenome); // Add Second top genome with mutation

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
        for (int i = 0; i < remainingChildren; i++)
        {
            children.Add(CreateNewGenome(bestGenomes[0].weights.Count));
        }

        ClearPopulation();
        population = children;

        currentGenome = 0;
        generation++;
        EventManagerOneArg.TriggerEvent(ConstantManager.UI_GENERATION, generation);
    }

    public void ClearPopulation()
    {
        if (population.Count > 0)
        {
            for (int i = 0; i < population.Count; i++) {
                if (population[i] != null) {
                    population[i] = null;
                }
            }
        }
        // population.Clear();
        population = new List<Genome>();
    }

    private void Mutate(Genome _genome)
    {
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

    public void SetGenomeFitness(float _fitness, int _index)
    {
        if (_index >= population.Count || _index < 0)
        {
            return;
        }
        population[_index].fitness = _fitness;
    }

   
}
