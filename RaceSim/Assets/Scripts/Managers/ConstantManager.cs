
/// <summary>
/// Home for all Constant Values contained throughout this application
/// </summary>
public class ConstantManager
{
    // AI Constants
    public const int MAXIMUM_GENOME_POPULATION = 20;
    public const int HIDDEN_LAYER_NEURONS = 8;
    public const int NUMBER_OF_HIDDEN_LAYERS = 1;
    public const int NUMBER_OF_GENOMES_TO_BREED = 4;
    public const float RAY_LENGTH = 100f;
    public const float BIAS = -1.0f;
    public const float COMPLETION_BONUS = 1000f;
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

    // UI Event Values
    public const string UI_GENERATION = "Generation";
    public const string UI_POPULATION = "Population";
    public const string UI_FITNESS = "Fitness";
    public const string UI_BEST_FITNESS = "BestFitness";

    public const string UI_HUMAN = "HumanBestTime";
    public const string UI_MACHINE = "MachineBestTime";
    public const string UI_TIMER = "GameTimer";

    // Main Menu Selections
    public const string PP_LEARNING = "Learning";
    public const string PP_LOADING = "Loading";
    public const string PP_FILENAME = "FileName";
    public const string PP_SPEEDHUD = "SpeedHUD";
    public const string PP_FITNESS = "BestFitness";
    public const string PP_TRACK = "TrackNumber";

    // Game Times
    public const string GG_TrackOne = "Track1";
    public const string GG_TrackTwo = "Track2";
    public const string GG_TrackThree = "Track3";
}
