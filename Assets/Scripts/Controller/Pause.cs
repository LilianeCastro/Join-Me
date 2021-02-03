using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : Singleton<Pause>
{
    [SerializeField] private GameObject _pausePanel;
    private bool _isPaused = false;
    public bool IsPaused
    {
        get
        {
            return _isPaused;
        }
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            _isPaused = !_isPaused;
            PauseGame();
        }

    }
    public void PauseGame()
    {
        if (_isPaused)
        {
            _pausePanel.SetActive(true);
            Time.timeScale = 0f;
        }
        else
        {
            _pausePanel.SetActive(false);
            Time.timeScale = 1.0f;
        }    
    }
    public void Resume()
    {
        _isPaused = !_isPaused;
        PauseGame();
    }  
}
