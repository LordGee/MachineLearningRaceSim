using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron {

    public int numberOfInputs;
    public List<float> weights;
    public const float BIAS = -1.0f;
    public enum EvaluationFunction { EVAL_SIGMOID, EVAL_STEP, EVAL_BIPOLAR };

    public void Populate(int _inputs)
    {
        numberOfInputs = _inputs;
        for (int i = 0; i < _inputs + 1; i++)
        {
            weights.Add(Random.Range(-1.0f, 1.0f));
        }
    }

    public void Initilise(List<float> _weightsIn, int _inputs)
    {
        numberOfInputs = _inputs;
        weights = _weightsIn;
    }

    public float GetBias() { return BIAS; }
}
