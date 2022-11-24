using TMPro;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Animator))]
public class Menu : MonoBehaviour
{
    [SerializeField] private Button _closeButton;
    [SerializeField] private Toggle _soundToggle;
    [SerializeField] private Toggle _musicToggle;
    [SerializeField] private Slider _soundSlider;
    [SerializeField] private Slider _musicSlider;
    [SerializeField] private TMP_InputField _serverIp;
    [SerializeField] private Button _serverIpApply;
    [SerializeField] private TMP_InputField _serverPort;
    [SerializeField] private Button _serverPortApply;
    [SerializeField] private TMP_InputField _castPath;
    [SerializeField] private Button _castPathApply;
    
    public Animator Anim;

    private bool _musicToggleCheck = true;
    private bool _soundToggleCheck = true;
    private void Awake()
    {
        ActionController.s_SetServerInfo += SetServerInfo;
        ActionController.s_SetCastSource += SetCastPath;
        ActionController.s_VideoPlaying += AudioEnable;
    }
    private void OnEnable()
    {
        Anim = GetComponent<Animator>();
        Anim.SetBool("Opened",false);
        _closeButton.onClick.AddListener(HideMenu);
        _serverIpApply.onClick.AddListener(ChangeServerInfo);
        _serverPortApply.onClick.AddListener(ChangeServerInfo);
        _castPathApply.onClick.AddListener(ChangeCastSource);
        _musicSlider.onValueChanged.AddListener(delegate { MusicValueChange(); });
        _musicToggle.onValueChanged.AddListener(delegate { MusicValueChange(); });
        _soundSlider.onValueChanged.AddListener(delegate { SoundValueChange(); });
        _soundToggle.onValueChanged.AddListener(delegate { SoundValueChange(); });
    }
    private void OnDestroy()
    {
        ActionController.s_SetServerInfo -= SetServerInfo;
        ActionController.s_SetCastSource -= SetCastPath;
    }

    private void HideMenu()
    {
        Anim.SetBool("Opened", false);
    }

    private void SetServerInfo(string ip, string port)
    {
        _serverIp.text = ip; 
        _serverPort.text = port;
    }
    private void SetCastPath(string path)
    {
        _castPath.text = path;
    }
    private void ChangeServerInfo()
    {
        ActionController.s_ChangeServerInfo?.Invoke(_serverIp.text, _serverPort.text);
    }
    private void ChangeCastSource()
    {
        ActionController.s_ChangeCastSource?.Invoke(_castPath.text);
    }

    private void MusicValueChange()
    {
        if (_musicToggle.isOn)
               
                ActionController.s_MusicValueChange?.Invoke(_musicSlider.value);
        else
            ActionController.s_MusicValueChange?.Invoke(0);
        
    }

    private void SoundValueChange()
    {
        if (_soundToggle.isOn)
            ActionController.s_SoundValueChange?.Invoke(_soundSlider.value);
        else 
            ActionController.s_SoundValueChange?.Invoke(0);
        
    }

    private void AudioEnable(bool enabled)
    {
        if (enabled)
        {
            _musicToggleCheck = _musicToggle.isOn;
            _soundToggleCheck = _soundToggle.isOn;
            _musicToggle.isOn = !enabled;
            _soundToggle.isOn = !enabled;
        }
        else
        {
            _musicToggle.isOn = _musicToggleCheck;
            _soundToggle.isOn = _soundToggleCheck;
        }
            
    }
}
