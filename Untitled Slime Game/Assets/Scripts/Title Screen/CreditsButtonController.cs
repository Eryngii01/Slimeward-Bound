using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CreditsButtonController : MonoBehaviour {
    [SerializeField]
    private Button _button;

    [SerializeField]
    private GameObject _credits;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        _credits.SetActive(true);
    }
}