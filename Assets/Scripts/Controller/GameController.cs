using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public enum PlatformType { Movement, Fall, Standing}

public class GameController : Singleton<GameController>
{
    #region Variables & Properties
    [Header("Inverted world of a particular player")]
    [SerializeField] private GameObject[] _worlds;
    [SerializeField] private GameObject[] _player;
    [SerializeField] private Dictionary<GameObject, GameObject> _dicWorldPlayer = new Dictionary<GameObject, GameObject>(); 

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
    private int _indexPlayerInControl;
    private bool _canChangePlayer = true;

    public bool ListPlayerIsLarge()
    {
        return _listPlayersCanControl.Count == 1 ? false : true;
    }

    #region Unity Methods
    public override void Init()
    {
        InitializeDictionaryPlayerWorld();
    }

    private void Start()
    {
        _invertedWorld.gameObject.SetActive(false);        
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.Q))
        {
            ChangeWorld();
        }
    }
    #endregion

    #region Lists
    private void InitializeDictionaryPlayerWorld()
    {
        if (_worlds.Length == _player.Length)
        {
            for (int i = 0; i < _worlds.Length; i++)
            {
                _dicWorldPlayer.Add(_player[i], _worlds[i]);
            }
        }
    }

    public void InitializeListPlayer(GameObject _player)
    {
        _listPlayersCanControl.Add(_player);
    }
    #endregion

    #region Switch World Mechanics
    public void NextPlayerControl()
    {        
        if (!_canChangePlayer) { return ;}

        _canChangePlayer = false;

        StartCoroutine(DelayCanChange());

        if(_indexPlayerInControl == 0 && _listPlayersCanControl.Count > 1)
        {
            print("1"); 
            NextPlayer(_indexPlayerInControl, _indexPlayerInControl+1);
            _indexPlayerInControl++;       
        }

        else if(_indexPlayerInControl > 0 &&  _indexPlayerInControl != _listPlayersCanControl.Count -1)
        {  
            print("2");
            NextPlayer(_indexPlayerInControl, _indexPlayerInControl+1);
            _indexPlayerInControl++;  
        }
        else if (_indexPlayerInControl == _listPlayersCanControl.Count -1)
        {
            print("3");
            NextPlayer(_indexPlayerInControl, 0);
            _indexPlayerInControl = 0;  
        }
    }

    private void NextPlayer(int indexOld, int indexNew)
    {
        _listPlayersCanControl[indexOld].TryGetComponent(out PlayerController playerScript);
             
        playerScript.CanControl = false;
        playerScript.FeedbackControl = false;
        ActiveWorld(playerScript.gameObject, false);

        _listPlayersCanControl[indexNew].TryGetComponent(out PlayerController nextPlayerScript);
        nextPlayerScript.CanControl = true;
        nextPlayerScript.FeedbackControl = true;
        ActiveWorld(nextPlayerScript.gameObject, false);
    }

    IEnumerator DelayCanChange()
    {
        yield return new WaitForSeconds(0.2f);
        _canChangePlayer = true;
    }

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
