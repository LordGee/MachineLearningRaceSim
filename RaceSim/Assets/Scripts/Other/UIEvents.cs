﻿using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All UI is managed by the Event Manager to trigger the information in the display
/// as and when required to minimise the amount of updates required.
/// </summary>
public class UIEvents : MonoBehaviour {
    private Text generation, bestFitness, population, currentFitness;
    private string populationText;
    private List<float> populationList;
    private float best;

    void Awake() {
        generation = GameObject.Find(ConstantManager.UI_GENERATION).GetComponent<Text>();
        population = GameObject.Find(ConstantManager.UI_POPULATION).GetComponent<Text>();
        bestFitness = GameObject.Find("BestFitness").GetComponent<Text>();
        currentFitness = GameObject.Find(ConstantManager.UI_FITNESS).GetComponent<Text>();
        populationList = new List<float>();
        best = 0f;
    }

    void OnEnable() {
        EventManager.StartListening(ConstantManager.UI_GENERATION, GetSetGeneration);
        EventManager.StartListening(ConstantManager.UI_POPULATION, GetSetPopulation);
        EventManager.StartListening(ConstantManager.UI_FITNESS, GetSetCurrentFitness);
        EventManager.StartListening(ConstantManager.UI_BEST_FITNESS, SetBestFitness);
    }

    void OnDisable() {
        EventManager.StopListening(ConstantManager.UI_GENERATION, GetSetGeneration);
        EventManager.StopListening(ConstantManager.UI_POPULATION, GetSetPopulation);
        EventManager.StopListening(ConstantManager.UI_FITNESS, GetSetCurrentFitness);
        EventManager.StopListening(ConstantManager.UI_BEST_FITNESS, SetBestFitness);
    }

    public void GetSetGeneration(float _gen) {
        generation.GetComponent<Text>().text = _gen.ToString();
        populationList = new List<float>();
    }

    public void GetSetPopulation(float _fit) {
        populationList.Add(_fit);
        populationText = "";
        for (int i = 0; i < populationList.Count; i++) {
            populationText += (i + 1).ToString();
            populationText += " = " + populationList[i].ToString("F");
            populationText += "\n";
        }
        population.GetComponent<Text>().text = populationText;
        if (_fit > best) {
            best = _fit;
            bestFitness.GetComponent<Text>().text = best.ToString("F");
        }
    }

    public void SetBestFitness(float _fit) {
        best = _fit;
        bestFitness.GetComponent<Text>().text = best.ToString("F");
    }

    public void GetSetCurrentFitness(float _fit) {
        currentFitness.GetComponent<Text>().text = _fit.ToString("F");
    }
}
