using System.Collections.Generic;

/// <summary>
/// Individual genome
/// </summary>
public class Genome
{
    public float fitness;
    public int ID;
    public List<float> weights;

    /// <summary>
    /// Constructor
    /// </summary>
    public Genome() {
        fitness = 0.0f;
        ID = -1;
        weights = new List<float>();
    }
}
