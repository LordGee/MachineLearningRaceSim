using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MenuController : MonoBehaviour
{

    private PlayerPrefsController pp;

    void Start()
    {
        pp = FindObjectOfType<PlayerPrefsController>();
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

    private void LoadTrack()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex + 1);
    }
    
}
