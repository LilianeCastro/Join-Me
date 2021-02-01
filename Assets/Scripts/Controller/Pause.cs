using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Pause : MonoBehaviour
{
    [SerializeField] private GameObject _pausePanel;
    private bool _isPaused = false;

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
        }

    }
    public void PauseGame()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(true);
        Time.timeScale = 0f;
    }

    public void ContinueGame()
    {
        _isPaused = !_isPaused;
        _pausePanel.SetActive(false);
        Time.timeScale = 1f;
    }
    
}
