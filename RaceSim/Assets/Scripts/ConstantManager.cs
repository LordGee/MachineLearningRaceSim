using UnityEngine;

public class ConstantManager : MonoBehaviour
{

    public const int MAXIMUM_GENOME_POPULATION = 15;

    public enum NeuralNetOutputs
    {
        OUTPUT_TURN_RIGHT,
        OUTPUT_TURN_LEFT,
        OUTPUT_ACCELERATE,
        OUTPUT_BRAKE,

        OUTPUT_COUNT
    };

    public enum RaycastInputs
    {
        RAYCAST_FORWARD,
        RAYCAST_FORWARD_RIGHT,
        RAYCAST_FORWARD_LEFT,

        RAYCAST_COUNT
    };

    public enum AgentLearningMode {
        USER_SUPERVISED,
        GENETIC_ALGORITHM
    };

}
