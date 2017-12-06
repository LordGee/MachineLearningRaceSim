using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{

    private PlayerPrefsController pp;
    private GameObject[] trackButtons = new GameObject[3];
    private int selectedTrack;
    private float selectedBestFitness;
    private List<int> trackNumber;
    private List<float> trackFitness;
    private GameObject displayFitness, displayTime;

    void Start()
    {
        pp = FindObjectOfType<PlayerPrefsController>();
        trackButtons[0] = GameObject.Find("ButtonTrack1");
        trackButtons[1] = GameObject.Find("ButtonTrack2");
        trackButtons[2] = GameObject.Find("ButtonTrack3");
        displayFitness = GameObject.Find("Fitness");
        displayTime = GameObject.Find("HumanTime");
        selectedTrack = 1;
        trackNumber = new List<int>();
        trackFitness = new List<float>();
        LoadList();
        HighlightSelectedTrack(0);
    }

    public void HumanPlayer()
    {
        pp.SetLearning(false);
        pp.SetLoading(false);
        pp.SetHUD(true);
        LoadTrack();
    }

    public void MachineLearning()
    {
        pp.SetLearning(true);
        pp.SetLoading(false);
        pp.SetHUD(false);
        LoadTrack();
    }

    public void HumanVsMachine()
    {
        pp.SetLearning(false);
        pp.SetLoading(true);
        pp.SetHUD(true);
        LoadTrack();
    }

    public void TrackSelected(int _track)
    {
        selectedTrack = _track;
        HighlightSelectedTrack(_track - 1);
    }

    private void HighlightSelectedTrack(int _index)
    {
        Color notSelected = Color.white;
        Color isSelected = Color.green;
        for (int i = 0; i < trackButtons.Length; i++)
        {
            if (i == _index)
            {
                trackButtons[i].GetComponent<Image>().color = isSelected;
            }
            else
            {
                trackButtons[i].GetComponent<Image>().color = notSelected;
            }
        }
        UpdateFitness();
    }

    private void LoadList()
    {
        using (TextReader tr = new StreamReader(@"~\..\Assets\Data\List.csv"))
        {
            bool stillMore = true;
            while (stillMore)
            {
                string text;
                text = tr.ReadLine();
                if (text == null)
                {
                    stillMore = false;
                    break;
                }
                trackNumber.Add(Convert.ToInt32(text));
                text = tr.ReadLine();
                trackFitness.Add(float.Parse(text));
            }
        }
    }

    private void UpdateFitness()
    {
        float bestFitness = 0f;
        for (int i = 0; i < trackNumber.Count; i++)
        {
            if (trackNumber[i] == selectedTrack)
            {
                if (trackFitness[i] > bestFitness)
                {
                    bestFitness = trackFitness[i];
                }
            }
        }
        selectedBestFitness = bestFitness;
        displayFitness.GetComponent<Text>().text = selectedBestFitness.ToString("F");
        displayTime.GetComponent<Text>().text = pp.GetBestTime(selectedTrack).ToString("F");
    }

    private void LoadTrack()
    {
        pp.SetTrack(selectedTrack);
        pp.SetFitness(selectedBestFitness);
        SceneManager.LoadScene(selectedTrack);
    }
    
}
