using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class QuitButtonController : MonoBehaviour
{
    [SerializeField]
    private Button _button;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        MusicManager.Instance.PlayClick();
        Application.Quit();
    }
}
