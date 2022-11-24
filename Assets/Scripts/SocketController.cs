using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using NativeWebSocket;
using System.Net;
using System.Globalization;
using System.Xml;
using UnityEditor;
using System.IO;
using System;

public class SocketController : MonoBehaviour
{

    [SerializeField] private TextAsset _serverConfig;
    private WebSocket _websocket;
    private string _serverIp;
    private string _serverPort;
    private bool _reconnecting = false;
    private bool _serverConnected = false;

    private void Start()
    {
        ServerParse(_serverConfig);
    }


    private void Update()
    {
#if !UNITY_WEBGL || UNITY_EDITOR
        _websocket.DispatchMessageQueue();
#endif
    }

    private async void SocketConnect(string ip, string port)
    {
        _websocket = new WebSocket($"ws://{ip}:{port}/ws");
        SocketStatus();
        await _websocket.Connect();
    }
    private async void OnApplicationQuit()
    {
        await _websocket.Close();
    }
    private void SocketStatus()
    {
        _websocket.OnOpen += () =>
        {
            ActionController.s_ConnectionOpen?.Invoke(true);
            _serverConnected = true;
            if (_reconnecting) { StopCoroutine(Reconnect()); _reconnecting = false; }
            DebugInfo("Соединение открыто!");
        };
        _websocket.OnMessage += (bytes) =>
        {
            //getting the message as a string
            var message = System.Text.Encoding.UTF8.GetString(bytes);
            var data = JsonUtility.FromJson<Data>(message);
            data.DataProcessing();
            Debug.Log("OnMessage! " + message);
        };
        _websocket.OnError += (e) =>
        {
            Debug.Log("Ошибка! " + e);
            DebugInfo("Ошибка! " + e);
            if (!_reconnecting) StartCoroutine(Reconnect());
        };
        _websocket.OnClose += (e) =>
        {
            ActionController.s_ConnectionOpen?.Invoke(false);
            DebugInfo("Соединение закрыто!");
            _serverConnected = false;
        };
        
    }
    private async void SendWebSocketMessage(string request)
    {
        if (_websocket.State == WebSocketState.Open)
        {
            // Sending plain text
            await _websocket.SendText(request);
        }
    }

    private void OnEnable()
    {
        ActionController.s_SendRequest += SendWebSocketMessage;
        ActionController.s_ChangeServerInfo += ServerChange;
    }
    private void OnDestroy()
    {
        ActionController.s_SendRequest -= SendWebSocketMessage;
        ActionController.s_ChangeServerInfo -= ServerChange;
        StopAllCoroutines();
    }

    private void ServerParse(TextAsset config)
    {
        string[] data = config.text.Split("\n");
        _serverIp = data[0].Trim().Split(":")[1].Trim();
        _serverPort = data[1].Trim().Split(":")[1].Trim();
        ActionController.s_SetServerInfo?.Invoke(_serverIp, _serverPort);
        SocketConnect(_serverIp, _serverPort);
    }
    private async void ServerChange(string ip, string port)
    {
        if(!string.IsNullOrWhiteSpace(ip)) _serverIp = ip;
        if (!string.IsNullOrWhiteSpace(port)) _serverPort = port;
        string _serverInfo = string.Join(" ", "IP:", _serverIp, "\nPort:", _serverPort);
        File.WriteAllText(Application.dataPath + "/config.txt", _serverInfo);
        AssetDatabase.Refresh();
        await _websocket.Close();
        ServerParse(_serverConfig);
    }

    private IEnumerator Reconnect()
    {
        _reconnecting = true;
        int _attempts = 0;
        while (_attempts < 10 && _reconnecting)
        {
            _attempts++;
            SocketConnect(_serverIp, _serverPort);
            DebugInfo($"Попытка переподключиться к серверу ({_attempts})");
            yield return new WaitForSeconds(5f);
        }
        if(!_serverConnected) DebugInfo("Подключиться к серверу не удалось. Проверьте параметры подключениия.");
        yield return new WaitForSeconds(5f);
        _reconnecting = false;
    }

    private void DebugInfo(string text)
    {
        ActionController.s_ChangeDebugText?.Invoke(text);
    }
}

public class Data
{
    [SerializeField] private string operation;
    [SerializeField] private string odometer;
    [SerializeField] private string value;
    [SerializeField] private bool status;
    public void DataProcessing()
    {
        switch (operation)
        {
            case "odometer_val":
                ActionController.s_ChangeOdometerValue?.Invoke(float.Parse(value,CultureInfo.InvariantCulture));
                break;
            case "currentOdometer":
                ActionController.s_ChangeOdometerValue?.Invoke(float.Parse(odometer, CultureInfo.InvariantCulture));
                break;
            case "randomStatus":
                ActionController.s_ChangeRandomStatus?.Invoke(status);
                break;
        }
    }
}


