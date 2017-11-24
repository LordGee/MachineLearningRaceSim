using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Genome
{

    public float fitness;
    public int ID;
    public List<float> weights;

    public Genome() {
        fitness = 0.0f;
        ID = -1;
        weights = new List<float>();
    }

}
