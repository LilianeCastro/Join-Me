using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : Singleton<GameManager>
{
    public override void Init()
    {
        DontDestroyOnLoad(this.gameObject);

        SceneManager.LoadScene("Menu", LoadSceneMode.Single);
    }
}
