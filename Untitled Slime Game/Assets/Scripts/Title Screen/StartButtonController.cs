using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class StartButtonController : MonoBehaviour {
    [SerializeField]
    private Button _button;
    [SerializeField]
    private GameObject _narrationMenu;
    [SerializeField]
    private NarrationBase _narration;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        _narrationMenu.SetActive(true);

        // Trigger the narration to begin
        NarrationManager.Instance.EnqueueText(_narration);
        MusicManager.Instance.PlayClick();
    }
}
