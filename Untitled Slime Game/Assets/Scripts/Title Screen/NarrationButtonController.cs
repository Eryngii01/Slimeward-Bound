using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NarrationButtonController : MonoBehaviour {
    [SerializeField]
    private Button _button;
    private bool isNarrating;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        NarrationManager.Instance.DequeueText();
        MusicManager.Instance.PlayClick();
    }

    public void ChangeText() {
        GetComponentInChildren<TextMeshProUGUI>().text = "Let's Go!";
    }
}
