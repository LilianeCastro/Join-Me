using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class Timer : MonoBehaviour
{
    [Header("Timer")]
    [SerializeField] private Text _timer = default;
    [SerializeField] private float _timeRemaining = 80.0f;
    private float _minutes;
    private float _seconds;

    [SerializeField] private GameObject _gameOver = default;
    [SerializeField] private float _timeToReloadScene = 2.0f;

    private void Awake()
    {
        _gameOver.SetActive(false);
    }

    private void LateUpdate()
    {
        if (GameController.Instance.GameOver) { return ; }

        if (!Pause.Instance.IsPaused) 
        {
            _timeRemaining -= Time.deltaTime;

            _minutes = Mathf.FloorToInt(_timeRemaining / 60); 
            _seconds = Mathf.FloorToInt(_timeRemaining % 60);

            _timer.text = string.Format("{0:00}:{1:00}", _minutes, _seconds);
            
            if (_timeRemaining <= 0.15f)
            {
                StartCoroutine(GameOverActiveText());
                GameController.Instance.GameOver = true;
            }
        }
        else
        {
            _timer.text = "";
        }    
    }

    IEnumerator GameOverActiveText()
    {
        Debug.Log("Ent");
        _timer.text = "";
        _gameOver.SetActive(true);
        yield return new WaitForSeconds(_timeToReloadScene);
        SceneManager.LoadScene(SceneManager.GetActiveScene().name, LoadSceneMode.Single);
    }
}
