﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class World : MonoBehaviour
{
    [SerializeField] private string _nameWorldRelativePlayer;

    public string NameWorldRelativePlayer
    {
        get
        {
            return _nameWorldRelativePlayer;
        }
    }
}
