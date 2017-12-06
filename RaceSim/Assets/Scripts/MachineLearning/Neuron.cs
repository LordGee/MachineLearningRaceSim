using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Neuron {

    // todo COme back and delete this if not needed
    /*
         public void Initilise(List<float> _weightsIn, int _inputs)
    {
        numberOfInputs = _inputs;
        weights = _weightsIn;
    }

    public float GetBias() { return ConstantManager.BIAS; }
    */

    public int numberOfInputs { get; set; }
    public List<float> weights { get; set; }

    public Neuron() {
        numberOfInputs = 0;
        weights = new List<float>();
    }

    public void Populate(int _inputs) {
        numberOfInputs = _inputs;
        for (int i = 0; i < _inputs + 1; i++) {
            weights.Add(Random.Range(-1.0f, 1.0f));
        }
    }


}
