using System.Collections.Generic;

/// <summary>
/// Individual Neurons, that contribute to a given layer.
/// </summary>
public class Neuron {
    public int numberOfInputs { get; set; }
    public List<float> weights { get; set; }

    /// <summary>
    /// Constructor
    /// </summary>
    public Neuron() {
        numberOfInputs = 0;
        weights = new List<float>();
    }
}
