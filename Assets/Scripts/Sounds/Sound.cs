using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(AudioSource))]
public class Sound : MonoBehaviour
{
    
    [SerializeField] private AudioClip[] _sounds;
    [SerializeField] private Transform[] _points;
    private AudioSource _audioSource;
    private float _timeToPlay = 2f; 
    private float _timer = 0;

    private void OnEnable()
    {
        _audioSource= GetComponent<AudioSource>();
        ActionController.s_SoundValueChange += Volume;
    }
    private void OnDisable()
    {
        ActionController.s_SoundValueChange -= Volume;
    }

    private void Update()
    {
        if (_timer < _timeToPlay)
        {
            _timer += Time.deltaTime;
        }
        else
        {
            _audioSource.clip =  _sounds[Random.Range(0, _sounds.Length)];
            _audioSource.gameObject.transform.position = _points[Random.Range(0, _points.Length)].position;
            _audioSource.Play();
            _timer = 0;
            _timeToPlay = _audioSource.clip.length + 5f;
        }
    }
    private void Volume(float value)
    {
        _audioSource.volume = value;
    }
}
