using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlatformType { Movement, Fall, Standing}

public class GameController : Singleton<GameController>
{
    #region Variables & Properties
    [Header("Inverted world of a particular player")]
    [SerializeField] private World[] _worlds;
    [SerializeField] private GameObject[] _player;
    [SerializeField] private Dictionary<GameObject, World> _dicWorldPlayer = new Dictionary<GameObject, World>(); 

    [Header("Common world for all players")]
    [SerializeField] private GameObject _commonWorld;
    private bool _isCommonWorldActive = true;
    public bool IsCommonWorldActive
    {
        get
        {
            return _isCommonWorldActive;
        }
    }
    [SerializeField] private GameObject _invertedWorld;
    private bool _isInvertedWorldActive = false;
    public bool IsInvertedWorldActive
    {
        get
        {
            return _isInvertedWorldActive;
        }
    }
    #endregion

    private List<GameObject> _listPlayersCanControl = new List<GameObject>();

    #region Unity Methods
    private void Start()
    {
        _invertedWorld.gameObject.SetActive(false);

        if (_worlds.Length == _player.Length)
        {
            for (int i = 0; i < _worlds.Length; i++)
            {
                _dicWorldPlayer.Add(_player[i], _worlds[i]);
            }
        }
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeWorld();
        }
    }
    #endregion

    #region Switch World Mechanics
    public void ActiveWorld(GameObject playerRelativeWorldToActive, bool status)
    {
        _dicWorldPlayer[playerRelativeWorldToActive].gameObject.SetActive(status); //o nome do mundo a ser ativado é o mesmo nome do player que estiver ativo
    }

    public void ChangeWorld()
    {
        _isCommonWorldActive = !_isCommonWorldActive;
        _commonWorld.gameObject.SetActive(_isCommonWorldActive);

        _isInvertedWorldActive = !_isInvertedWorldActive;
        _invertedWorld.gameObject.SetActive(_isInvertedWorldActive);

        SoundController.Instance.ChangeSoundWorld(_isCommonWorldActive);
    }
    #endregion

    public void ChangeScene(string nameSceneToLoad)
    {
        SceneManager.LoadScene(nameSceneToLoad, LoadSceneMode.Single);
    }
}
