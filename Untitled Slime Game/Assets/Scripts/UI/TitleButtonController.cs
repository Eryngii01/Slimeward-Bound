using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class TitleButtonController : MonoBehaviour {
    [SerializeField]
    private Button _button;

    void Awake() {
        _button.onClick.AddListener(OnClick);
    }

    // OnClick event handler
    void OnClick() {
        MusicManager.Instance.PlayClick();
        SceneManager.LoadScene("Title Screen");
    }
}
