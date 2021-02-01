using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SoundUI : MonoBehaviour
{
    [SerializeField] private AudioSource _playerAudioSfx;
    [SerializeField] private AudioClip[] _uiSfx;

    void Start()
    {
        _playerAudioSfx = GetComponent<AudioSource>();
    }

    public void PlayButtonSfx(int indexSound)
    {
        _playerAudioSfx.PlayOneShot(_uiSfx[indexSound]);
    }
}
