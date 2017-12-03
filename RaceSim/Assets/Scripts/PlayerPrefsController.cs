using System;
using System.Collections.Generic;
using UnityEngine;

public class PlayerPrefsController : MonoBehaviour {

    void Awake() {
        DontDestroyOnLoad(gameObject);
    }

    public void SetLearning(bool _value) {
        PlayerPrefs.SetInt(ConstantManager.PP_LEARNING, _value ? 1 : 0);
    }
    public void SetLoading(bool _value) {
        PlayerPrefs.SetInt(ConstantManager.PP_LOADING, _value ? 1 : 0);
    }
    public void SetHUD(bool _value) {
        PlayerPrefs.SetInt(ConstantManager.PP_SPEEDHUD, _value ? 1 : 0);
    }

    public bool GetLearning() { return PlayerPrefs.GetInt(ConstantManager.PP_LEARNING) == 1; }
    public bool GetLoading() { return PlayerPrefs.GetInt(ConstantManager.PP_LOADING) == 1; }
    public bool GetHUD() { return PlayerPrefs.GetInt(ConstantManager.PP_SPEEDHUD) == 1; }
}

