using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class SceneInterface : MonoBehaviour
{
    [SerializeField] private TextMeshProUGUI _odometerText;
    [SerializeField] private Button _requestValue;
    [SerializeField] private Button _requestStatus;
    [SerializeField] private Button _startCast;
    [SerializeField] private Toggle _status;
    [SerializeField] private TextMeshProUGUI _debugText;
    [SerializeField] private Renderer _screen;
    private VLCPlayer _VLCplayer;
    private string _customPath;
    private float _lastOdometerValue = 0f;
    private float _newOdometerValue;
    private float _changeValueTime = 0.5f;
    private float _timer = 0;
    private bool _valueChanged;


    private void Start()
    {
        NewVLCPlayer();
    }
    void Update()
    {
        if (_timer < _changeValueTime && !_valueChanged)
        {
            _timer += Time.deltaTime;
            float value = Mathf.Lerp(_lastOdometerValue, _newOdometerValue, _timer / _changeValueTime);
            _odometerText.text = value.ToString();
        }
        else if (_timer >= _changeValueTime) { _valueChanged = true; _timer = 0; _lastOdometerValue = _newOdometerValue; }
    }

    private void ChangeOdometer(float currentValue)
    {
        _valueChanged = false;
        _newOdometerValue = currentValue;
    }

    private void ChangeRandomStatus(bool status)
    {
        _status.isOn = status;
    }
    private void RequestValue()
    {
        ActionController.s_SendRequest?.Invoke("{\"operation\":\"getCurrentOdometer\"}");
    }

    private void RequestStatus()
    {
        ActionController.s_SendRequest?.Invoke("{\"operation\":\"getRandomStatus\"}");
    }
    private void NewVLCPlayer(string path = null)
    {
        if (!string.IsNullOrWhiteSpace(path)) _customPath = path;
        else _customPath = "rtsp://wowzaec2demo.streamlock.net/vod/mp4:BigBuckBunny_115k.mp4";

        if (_VLCplayer == null)
        {
            _VLCplayer = new GameObject("VLCPlayer").AddComponent<VLCPlayer>();
            _VLCplayer.playOnAwake = false;
            _VLCplayer.screen = _screen;
            _VLCplayer.path = _customPath;
            ActionController.s_SetCastSource?.Invoke(_customPath);
        }
        else
        {
            _VLCplayer.Stop();
            Destroy(_VLCplayer.gameObject);
            _VLCplayer = null;
            NewVLCPlayer(_customPath);
        }
        print(_customPath + path);
    }
    private void StartCast()
    {
        if (_VLCplayer != null)
            _VLCplayer.Open();
        else
            NewVLCPlayer();
        if(_VLCplayer.mediaPlayer.Channel != LibVLCSharp.AudioOutputChannel.Error)
        {
            ActionController.s_VideoPlaying?.Invoke(true);
        }
    }
    private void OnEnable()
    {
        ActionController.s_ChangeOdometerValue += ChangeOdometer;
        ActionController.s_ChangeRandomStatus += ChangeRandomStatus;
        ActionController.s_ChangeDebugText += DebugTextChanged;
        ActionController.s_ChangeCastSource += NewVLCPlayer;
        _requestValue.onClick.AddListener(RequestValue);
        _requestStatus.onClick.AddListener(RequestStatus);
        _startCast.onClick.AddListener(StartCast);
    }
    private void OnDestroy()
    {
        ActionController.s_ChangeOdometerValue -= ChangeOdometer;
        ActionController.s_ChangeRandomStatus -= ChangeRandomStatus;
        ActionController.s_ChangeDebugText -= DebugTextChanged;
    }

    private void OnApplicationQuit()
    {
        Destroy(_VLCplayer.gameObject);
    }

    private void DebugTextChanged(string text) => _debugText.text = text;
}