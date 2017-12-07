using UnityEngine;
using UnityEngine.SceneManagement;

/// <summary>
/// Manages the overall running of the game
/// </summary>
public class GameController : MonoBehaviour {

    [SerializeField] private GameObject car, humanCar;
    private Vector3 spawnPoint, humanSpawnPoint;
    private readonly Quaternion quaternionStartRotation = Quaternion.AngleAxis(90f, Vector3.up);
    private CarManager cm;
    private HumanCarManager hcm;
    private PlayerPrefsController pp;

    /// <summary>
    /// Constructor
    /// </summary>
    private void Start() {
        spawnPoint = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        humanSpawnPoint = GameObject.FindGameObjectWithTag("SpawnHuman").transform.position;
        pp = FindObjectOfType<PlayerPrefsController>();
        SetupInitialGame();
        if (pp.GetLearning() || pp.GetLoading()) {
            cm = FindObjectOfType<CarManager>();
        }
        if (!pp.GetLearning()) {
            hcm = FindObjectOfType<HumanCarManager>();
        }
        SetCorrectHUD();
    }

    /// <summary>
    /// Update function, checks for key press that will return the game to the main menu
    /// </summary>
    private void Update() {
    if (Input.GetKeyDown(KeyCode.Escape)) {
            SceneManager.LoadScene(0);
        }
    }

    /// <summary>
    /// Instantiates the initial cars to the scene depending on the mode selected
    /// </summary>
    public void SetupInitialGame() {
        if (GameObject.FindGameObjectsWithTag("Player").Length <= 1) {
            if (pp.GetLearning() || pp.GetLoading()) {
                Instantiate(car, spawnPoint, Quaternion.AngleAxis(90f, Vector3.up));
            }
            if (!pp.GetLearning() && !pp.GetLoading() || !pp.GetLearning() && pp.GetLoading()) {
                Instantiate(humanCar, humanSpawnPoint, Quaternion.AngleAxis(90f, Vector3.up));
            }
        }
    }

    /// <summary>
    /// Resets the relevent car position back to its starting location
    /// </summary>
    /// <param name="_cm">True if AI / ML controlled car</param>
    public void ResetCar(bool _cm) {
        if (_cm) {
            cm.ResetPosition(spawnPoint, quaternionStartRotation);
        } else {
            hcm.ResetPosition(humanSpawnPoint, quaternionStartRotation);
        }
        
    }

    /// <summary>
    /// There are two difference canvas's active when the scene loads.
    /// This function removes the one that is not needeed depending on game mode
    /// </summary>
    private void SetCorrectHUD() {
        GameObject speed = GameObject.Find("SpeedHUDCanvas");
        GameObject genetic = GameObject.Find("GeneticHUDCanvas");
        if (pp.GetHUD()) {
            genetic.SetActive(false);
        } else {
            speed.SetActive(false);
        }
    }
}
