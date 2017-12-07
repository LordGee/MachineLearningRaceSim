using UnityEngine;
using UnityEngine.UI;

/// <summary>
/// All UI is managed by the Event Manager to trigger the information in the display
/// as and when required to minimise the amount of updates required.
/// </summary>
public class UIPlayerEvents : MonoBehaviour {

    private Text humanBest, machineBest, gameTimer;

    void Awake() {
        humanBest = GameObject.Find("HumanTime").GetComponent<Text>();
        machineBest = GameObject.Find("MachineTime").GetComponent<Text>();
        gameTimer = GameObject.Find("GameTimer").GetComponent<Text>();
    }

    void OnEnable() {
        EventManager.StartListening(ConstantManager.UI_HUMAN, SetHumanTime);
        EventManager.StartListening(ConstantManager.UI_MACHINE, SetMachineTime);
        EventManager.StartListening(ConstantManager.UI_TIMER, UpdateGameTimer);
    }

    void OnDisable() {
        EventManager.StopListening(ConstantManager.UI_HUMAN, SetHumanTime);
        EventManager.StopListening(ConstantManager.UI_MACHINE, SetMachineTime);
        EventManager.StopListening(ConstantManager.UI_TIMER, UpdateGameTimer);
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

