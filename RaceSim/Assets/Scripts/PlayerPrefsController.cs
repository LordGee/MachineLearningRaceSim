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
    public void SetTrack(int _value) {
        PlayerPrefs.SetInt(ConstantManager.PP_TRACK, _value);
    }
    public void SetFitness(float _value) {
        PlayerPrefs.SetFloat(ConstantManager.PP_FITNESS, _value);
    }
    public void SetBestTime(float _value, int _track) {
        switch (_track)
        {
            case (1) :
                PlayerPrefs.SetFloat(ConstantManager.GG_TrackOne, _value);
                break;
            case (2):
                PlayerPrefs.SetFloat(ConstantManager.GG_TrackTwo, _value);
                break;
            case (3):
                PlayerPrefs.SetFloat(ConstantManager.GG_TrackThree, _value);
                break;
        } 
    }

    public bool GetLearning() { return PlayerPrefs.GetInt(ConstantManager.PP_LEARNING) == 1; }
    public bool GetLoading() { return PlayerPrefs.GetInt(ConstantManager.PP_LOADING) == 1; }
    public bool GetHUD() { return PlayerPrefs.GetInt(ConstantManager.PP_SPEEDHUD) == 1; }
    public static int GetTrack() { return PlayerPrefs.GetInt(ConstantManager.PP_TRACK); }
    public static float GetFitness() { return PlayerPrefs.GetFloat(ConstantManager.PP_FITNESS); }
    public float GetBestTime(int _track) {
        switch (_track) {
            case (1):
                return PlayerPrefs.GetFloat(ConstantManager.GG_TrackOne);
            case (2):
                return PlayerPrefs.GetFloat(ConstantManager.GG_TrackTwo);
            case (3):
                return PlayerPrefs.GetFloat(ConstantManager.GG_TrackThree);
            default:
                return 0f;
        }
    }
}

