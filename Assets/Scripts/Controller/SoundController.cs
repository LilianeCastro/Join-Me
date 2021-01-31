using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundController : Singleton<SoundController>
{
    [SerializeField] private AudioSource _audioSourceCommonWorld;
    [SerializeField] private AudioSource _audioSourceInverseWorld;

    public void ChangeSoundWorld(bool isCommonWorldActive)
    {
        if (isCommonWorldActive)
        {
            ActiveSoundCommonWorld();
        }
        else
        {
            ActiveSoundInverseWolrd();
        }
    }
    
    private void ActiveSoundCommonWorld()
    {
        _audioSourceCommonWorld.volume = 1.0f;
        _audioSourceInverseWorld.volume = 0.0f;
    }

    private void ActiveSoundInverseWolrd()
    {
        _audioSourceCommonWorld.volume = 0.0f;
        _audioSourceInverseWorld.volume = 1.0f;
    }
}
