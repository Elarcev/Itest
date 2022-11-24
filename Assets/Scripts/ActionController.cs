using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    public static Action<bool> s_ConnectionOpen;
    public static Action<bool> s_ChangeRandomStatus;
    public static Action<float> s_ChangeOdometerValue;
    public static Action<string> s_SendRequest;
    public static Action<string,string> s_SetServerInfo;
    public static Action<string> s_SetCastSource;
    public static Action<string,string> s_ChangeServerInfo;
    public static Action<string> s_ChangeCastSource;
    public static Action<string> s_ChangeDebugText;
    public static Action<float> s_MusicValueChange;
    public static Action<float> s_SoundValueChange;
    public static Action<bool> s_VideoPlaying;
}

