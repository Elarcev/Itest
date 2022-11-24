using System.Collections;
using System.Collections.Generic;
using System.Runtime.ConstrainedExecution;
using UnityEngine;

public class SignalLamp : MonoBehaviour
{
    private Color _colorStart = Color.red;
    private Color _colorEnd = Color.green;
    private bool _colorChanged = true;
    private float _changeColorTime = 0.2f;
    private float _timer = 0;
    private Renderer _rend;
    [SerializeField] private Light _lightSource;
    private void Start()
    {
        _rend = GetComponent<Renderer>();
        _rend.material.color = _colorStart;
        _lightSource.color = _colorStart;
    }

    private void OnEnable()
    {
        ActionController.s_ConnectionOpen += ChangeColor;
    }

    private void OnDestroy()
    {
        ActionController.s_ConnectionOpen -= ChangeColor;
    }

    private void Update()
    {
        if (_timer < _changeColorTime && !_colorChanged)
        {
            _timer += Time.deltaTime;
            Color current = Color.Lerp(_colorStart, _colorEnd, _timer / _changeColorTime);
            _rend.material.SetColor("_EmissionColor", current);
            _lightSource.color = current;
        }
        else if (_timer >= _changeColorTime) {_colorChanged = true; _timer = 0; }
    }

    private void ChangeColor(bool connStatus)
    {
        if (connStatus) {_colorStart =  Color.red; _colorEnd = Color.green; }
        else { _colorStart = Color.green; _colorEnd = Color.red; }
        _colorChanged = false;
    }
}
