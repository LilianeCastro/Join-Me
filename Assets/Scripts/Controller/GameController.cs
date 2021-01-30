using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
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

    private void Start()
    {
        if (_worlds.Length == _player.Length)
        {
            for (int i = 0; i < _worlds.Length; i++)
            {
                _dicWorldPlayer.Add(_player[i], _worlds[i]);
            }
        }
    }

    public void ActiveWorld(GameObject namePlayerRelativeWorldToActive, bool status)
    {
        print("world");
        print(_dicWorldPlayer[namePlayerRelativeWorldToActive].gameObject);
        _dicWorldPlayer[namePlayerRelativeWorldToActive].gameObject.SetActive(status); //o nome do mundo a ser ativado é o mesmo nome do player que estiver ativo
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeWorld();
        }
    }

    private void ChangeWorld()
    {
        _isCommonWorldActive = !_isCommonWorldActive;
        _commonWorld.gameObject.SetActive(_isCommonWorldActive);

        _isInvertedWorldActive = !_isInvertedWorldActive;
        _invertedWorld.gameObject.SetActive(_isInvertedWorldActive);
    }

    /*[SerializeField] private GameObject _commonWorld;
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

    public override void Init()
    {
        _commonWorld.gameObject.SetActive(_isCommonWorldActive);
        _invertedWorld.gameObject.SetActive(_isInvertedWorldActive);
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.X))
        {
            ChangeWorld();
        }
    }

    private void ChangeWorld()
    {
        _isCommonWorldActive = !_isCommonWorldActive;
        _commonWorld.gameObject.SetActive(_isCommonWorldActive);

        _isInvertedWorldActive = !_isInvertedWorldActive;
        _invertedWorld.gameObject.SetActive(_isInvertedWorldActive);
    }*/
}
