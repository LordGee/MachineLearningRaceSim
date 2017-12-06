using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIPlayerEvents : MonoBehaviour {

    private Text humanBest, machineBest, gameTimer;

    void Awake() {
        humanBest = GameObject.Find("HumanTime").GetComponent<Text>();
        machineBest = GameObject.Find("MachineTime").GetComponent<Text>();
        gameTimer = GameObject.Find("GameTimer").GetComponent<Text>();
    }

    void OnEnable() {
        EventManagerOneArg.StartListening(ConstantManager.UI_HUMAN, SetHumanTime);
        EventManagerOneArg.StartListening(ConstantManager.UI_MACHINE, SetMachineTime);
        EventManagerOneArg.StartListening(ConstantManager.UI_TIMER, UpdateGameTimer);
    }

    void OnDisable() {
        EventManagerOneArg.StopListening(ConstantManager.UI_HUMAN, SetHumanTime);
        EventManagerOneArg.StopListening(ConstantManager.UI_MACHINE, SetMachineTime);
        EventManagerOneArg.StopListening(ConstantManager.UI_TIMER, UpdateGameTimer);
    }

    public void SetHumanTime(float _time) {
        humanBest.GetComponent<Text>().text = _time.ToString("F");
    }

    public void SetMachineTime(float _time) {
        machineBest.GetComponent<Text>().text = _time.ToString("F");
    }

    public void UpdateGameTimer(float _time) {
        int time = (int)Mathf.Floor(_time);
        gameTimer.GetComponent<Text>().text = time.ToString();
    }

}

