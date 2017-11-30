using UnityEngine;

public class ConstantManager
{

    public const int MAXIMUM_GENOME_POPULATION = 18;
    public const int HIDDEN_LAYER_NEURONS = 8;
    public const int NUMBER_OF_HIDDEN_LAYERS = 1;

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
        RAYCAST_FORWARD_FORWARD_RIGHT,
        RAYCAST_FORWARD_FORWARD_LEFT,
        RAYCAST_FORWARD_RIGHT,
        RAYCAST_FORWARD_LEFT,
        RAYCAST_RIGHT,
        RAYCAST_LEFT,
        ACCELERATION,

        INPUT_COUNT
    };

    public const float BIAS = -1.0f;

    public const string UI_GENERATION = "Generation";
    public const string UI_POPULATION = "Population";
    public const string UI_FITNESS = "Fitness";
}
