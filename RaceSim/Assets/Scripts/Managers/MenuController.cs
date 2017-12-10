using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.Assertions.Must;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Button = UnityEngine.Experimental.UIElements.Button;

public class MenuController : MonoBehaviour {

    private PlayerPrefsController pp;
    private GameObject[] trackButtons = new GameObject[3];
    private int selectedTrack;
    private float selectedBestFitness;
    private List<int> trackNumber;
    private List<float> trackFitness;
    private GameObject displayFitness, displayTime, VSButton;
    private AudioSource thud;

    /// <summary>
    /// Constructor
    /// </summary>
    void Start() {
        pp = FindObjectOfType<PlayerPrefsController>();
        trackButtons[0] = GameObject.Find("ButtonTrack1");
        trackButtons[1] = GameObject.Find("ButtonTrack2");
        trackButtons[2] = GameObject.Find("ButtonTrack3");
        displayFitness = GameObject.Find("Fitness");
        displayTime = GameObject.Find("HumanTime");
        VSButton = GameObject.Find("ButtonVS");
        selectedTrack = 1;
        trackNumber = new List<int>();
        trackFitness = new List<float>();
        LoadList();
        HighlightSelectedTrack(0);
        thud = GetComponent<AudioSource>();
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape)) { Application.Quit(); }
    }

    /// <summary>
    /// Configures the game for a Human Player
    /// </summary>
    public void HumanPlayer() {
        pp.SetLearning(false);
        pp.SetLoading(false);
        pp.SetHUD(true);
        LoadTrack();
    }

    /// <summary>
    /// Configures the game for Machine Learning Mode
    /// </summary>
    public void MachineLearning() {
        pp.SetLearning(true);
        pp.SetLoading(false);
        pp.SetHUD(false);
        LoadTrack();
    }

    /// <summary>
    /// Configures the game for Battle Mode
    /// </summary>
    public void HumanVsMachine() {
        pp.SetLearning(false);
        pp.SetLoading(true);
        pp.SetHUD(true);
        LoadTrack();
    }

    /// <summary>
    /// Updates variables based on track selection
    /// </summary>
    /// <param name="_track">Track number</param>
    public void TrackSelected(int _track) {
        selectedTrack = _track;
        HighlightSelectedTrack(_track - 1);
    }

    /// <summary>
    /// Updates the buttons to provide a visual que of track selection
    /// </summary>
    /// <param name="_index">Index of button array</param>
    private void HighlightSelectedTrack(int _index) {
        Color notSelected = Color.white;
        Color isSelected = Color.green;
        for (int i = 0; i < trackButtons.Length; i++) {
            if (i == _index) {
                trackButtons[i].GetComponent<Image>().color = isSelected;
            } else {
                trackButtons[i].GetComponent<Image>().color = notSelected;
            }
        }
        UpdateFitness();
    }

    /// <summary>
    /// Loads a list of all stored NN to be presented for each track
    /// </summary>
    private void LoadList() {
        using (TextReader tr = new StreamReader(@"~\..\Assets\Data\List.csv")) {
            bool stillMore = true;
            while (stillMore) {
                string text;
                text = tr.ReadLine();
                if (text == null) {
                    stillMore = false;
                    break;
                }
                trackNumber.Add(Convert.ToInt32(text));
                text = tr.ReadLine();
                trackFitness.Add(float.Parse(text));
            }
        }
    }

    /// <summary>
    /// Updates the best fitness value based on the selected track
    /// </summary>
    private void UpdateFitness() {
        float bestFitness = 0f;
        for (int i = 0; i < trackNumber.Count; i++) {
            if (trackNumber[i] == selectedTrack) {
                if (trackFitness[i] > bestFitness) {
                    bestFitness = trackFitness[i];
                }
            }
        }
        selectedBestFitness = bestFitness;
        displayFitness.GetComponent<Text>().text = selectedBestFitness.ToString("F");
        displayTime.GetComponent<Text>().text = pp.GetBestTime(selectedTrack).ToString("F");
        if (selectedBestFitness == 0.0f) {
            VSButton.SetActive(false);
        } else {
            VSButton.SetActive(true);
        }
    }

    /// <summary>
    /// Loads the selected track and updates the player prefs values 
    /// to load in the correct information when the game scene starts.
    /// </summary>
    private void LoadTrack() {
        pp.SetTrack(selectedTrack);
        pp.SetFitness(selectedBestFitness);
        SceneManager.LoadScene(selectedTrack);
    }

    /// <summary>
    /// Called by the Buttons event trigger component when a pointer 
    /// enters the game object.
    /// </summary>
    public void OnButtonHover() {
        thud.Play();
    }
}
