using UnityEngine;

public class ConstantManager
{

    public const int MAXIMUM_GENOME_POPULATION = 15;
    //public const int INVALID_ID = -1;
    //public const int MAX_POPULATION = 15;
    public const int HIDDEN_LAYER_NEURONS = 8;
    //public const float MAX_FRAME_DELTA = 1 / 20.0f;
    //public const float MUTATION_RATE = 0.15f;
   // public const float MAX_PERBETUATION = 0.3f;

    public enum NeuralNetOutputs
    {
        OUTPUT_TURN_RIGHT,
        OUTPUT_TURN_LEFT,
        OUTPUT_ACCELERATE,
        OUTPUT_BRAKE,
        
        OUTPUT_COUNT
    };

    public enum Inputs
    {
        RAYCAST_FORWARD,
        RAYCAST_FORWARD_RIGHT,
        RAYCAST_FORWARD_LEFT,
        ACCELERATION,

        RAYCAST_COUNT
    };

    public enum AgentLearningMode {
        USER_SUPERVISED,
        GENETIC_ALGORITHM
    };

}
