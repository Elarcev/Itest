using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MenuController : MonoBehaviour
{
    [SerializeField] private Menu _menuPanel;
    [SerializeField] private Button _menuButton;
    private void Start()
    {
        _menuButton.onClick.AddListener(OpenMenu);
    }
    private void OpenMenu()
    {
        _menuPanel.Anim.SetBool("Opened", true);
    }
}
