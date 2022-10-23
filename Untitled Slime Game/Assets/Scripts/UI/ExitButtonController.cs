using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ExitButtonController : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private GameObject _menu;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        GameManager.Instance.ResumeGame();
        MusicManager.Instance.PlayClick();

        // Close the menu
        _menu.SetActive(false);
    }
}
