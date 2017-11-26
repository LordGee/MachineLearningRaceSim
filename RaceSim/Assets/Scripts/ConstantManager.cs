using UnityEngine;

public class ConstantManager
{

    public const int MAXIMUM_GENOME_POPULATION = 20;
    //public const int INVALID_ID = -1;
    //public const int MAX_POPULATION = 15;
    public const int HIDDEN_LAYER_NEURONS = 8;

    public const int NUMBER_OF_HIDDEN_LAYERS = 1;
    //public const float MAX_FRAME_DELTA = 1 / 20.0f;
    //public const float MUTATION_RATE = 0.15f;
   // public const float MAX_PERBETUATION = 0.3f;

    public const float RAY_LENGTH = 100f;

    public enum NNOutputs
    {
        OUTPUT_TURN_RIGHT,
        OUTPUT_TURN_LEFT,
        OUTPUT_ACCELERATE,
        OUTPUT_BRAKE,
        
        OUTPUT_COUNT
    };

    public enum NNInputs
    {
        RAYCAST_FORWARD,
        RAYCAST_FORWARD_RIGHT,
        RAYCAST_FORWARD_LEFT,
        ACCELERATION,

        INPUT_COUNT
    };

    public enum AgentLearningMode {
        USER_SUPERVISED,
        GENETIC_ALGORITHM
    };

    public const float BIAS = -1.0f;
    // public enum EvaluationFunction { EVAL_SIGMOID, EVAL_STEP, EVAL_BIPOLAR };


    public const string UI_GENERATION = "Generation";
    public const string UI_POPULATION = "Population";
    public const string UI_FITNESS = "Fitness";
}
