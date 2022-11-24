using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Music : MonoBehaviour
{
    private AudioSource _audioSource;

    private void OnEnable()
    {
        _audioSource = GetComponent<AudioSource>();
        ActionController.s_MusicValueChange += Volume;
    }

    private void OnDisable()
    {
        ActionController.s_MusicValueChange -= Volume;
    }

    private void Volume(float value)
    {
        _audioSource.volume = value;
    }
}
