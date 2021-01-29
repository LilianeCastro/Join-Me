using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameController : Singleton<GameController>
{
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
    }
}
