using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoBehaviour
{
    // ----------------- HP Bars -----------------
    [SerializeField]
    private Slider[] _currentStatus;
    [SerializeField]
    private Slider[] _status;

    private static int _maxStatusIndex = 0;
    // -------------------------------------------

    // ---------------- Bullet UI ----------------
    [SerializeField]
    private GameObject _bulletSelector, _unlockBulletMenu, _pauseMenu, _gameOverMenu, _winMenu;
    [SerializeField]
    private Image _unlockBulletMenuImage;
    private RectTransform _selectorTransform;
    [SerializeField]
    private GameObject[] _bulletImages, _targetPositions;

    [SerializeField]
    private float _selectorSpeed;
    // -------------------------------------------

    // --------------- Bookkeeping ---------------
    private bool _isSelectingBullet = false;
    
    private int _bulletColor;
    // -------------------------------------------


    void Awake() {
        _selectorTransform = _bulletSelector.GetComponent<RectTransform>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (_isSelectingBullet) {
            MoveBulletSelector();
        }
    }

    void MoveBulletSelector() {
        var step = _selectorSpeed * Time.deltaTime;
        Vector2 destination = _targetPositions[_bulletColor].GetComponent<RectTransform>().anchoredPosition;
        _selectorTransform.anchoredPosition = Vector2.MoveTowards(_selectorTransform.anchoredPosition, destination, step);

        if (Vector2.Distance(_selectorTransform.anchoredPosition, destination) < 0.001f) {
            _isSelectingBullet = false;
        }
    }

    void SwitchToClone(GameObject clone, int currentPlayerIndex) {
        _maxStatusIndex++;
        SwitchStatus(currentPlayerIndex);

        _bulletColor = clone.GetComponent<PlayerController>().currentBulletIndex;
        _isSelectingBullet = true;
    }

    void SwitchToOriginal(GameObject clone, GameObject original) {
        _currentStatus[0].GetComponent<HPGaugeController>().RevealElement();
        _currentStatus[1].GetComponent<HPGaugeController>().HideElement();

        _status[0].GetComponent<HPGaugeController>().HideElement();
        _status[1].GetComponent<HPGaugeController>().RevealElement();

        _bulletColor = original.GetComponent<PlayerController>().currentBulletIndex;
        _isSelectingBullet = true;
    }

    void DeletePlayer(GameObject currentPlayer, int prevPlayerIndex, int currentPlayerIndex) {
        // Case 1: Clone was deleted, so the original still exists
        if (prevPlayerIndex != currentPlayerIndex) {
            _currentStatus[prevPlayerIndex].GetComponent<HPGaugeController>().HideElement();
            _status[prevPlayerIndex].GetComponent<HPGaugeController>().HideElement();
        // Case 2: Original was deleted, so the clone was promoted
        } else {
            _currentStatus[prevPlayerIndex + 1].GetComponent<HPGaugeController>().HideElement();
            _status[prevPlayerIndex + 1].GetComponent<HPGaugeController>().HideElement();
        }

        _maxStatusIndex--;
        SwitchStatus(currentPlayerIndex);

        _bulletColor = currentPlayer.GetComponent<PlayerController>().currentBulletIndex;
        _isSelectingBullet = true;
    }

    void SwitchBullet(GameObject currentPlayer) {
        _bulletColor = currentPlayer.GetComponent<PlayerController>().currentBulletIndex;
        _isSelectingBullet = true;
    }

    void AddBullet(int newBulletIndex) {
        _bulletImages[newBulletIndex].SetActive(true);

        // Cause a popup to appear that essentially pauses the game until you click an "Okay" button
        _unlockBulletMenu.SetActive(true);
        _unlockBulletMenuImage.sprite = _bulletImages[newBulletIndex].GetComponent<Image>().sprite;
        GameManager.Instance.PauseGame();
    }

    void SwitchPlayer(GameObject currentPlayer, int currentPlayerIndex) {
        SwitchStatus(currentPlayerIndex);
        SwitchBullet(currentPlayer);
    }

    void SwitchStatus(int currentPlayerIndex) {
        for (int i = 0; i < _maxStatusIndex; i++) {
            if (i == currentPlayerIndex) {
                _currentStatus[i].GetComponent<HPGaugeController>().RevealElement();
                _status[i].GetComponent<HPGaugeController>().HideElement();
            } else {
                _currentStatus[i].GetComponent<HPGaugeController>().HideElement();
                _status[i].GetComponent<HPGaugeController>().RevealElement();
            }
        }
    }

    void UpdateHP(int HP, int currentPlayerIndex) {
        _currentStatus[currentPlayerIndex].GetComponent<HPGaugeController>().UpdateHealth(HP);
        _status[currentPlayerIndex].GetComponent<HPGaugeController>().UpdateHealth(HP);
    }

    void HandlePauseMenu() {
        _pauseMenu.SetActive(true);
    }

    void ReactivateHPGauge() {
        foreach(Slider HPBar in _currentStatus) {
            HPBar.GetComponent<HPGaugeController>().Resume();
        }

        foreach(Slider HPBar in _status) {
            HPBar.GetComponent<HPGaugeController>().Resume();
        }
    }

    void HandleGameOverMenu() {
        _gameOverMenu.SetActive(true);
    }

    void HandleWinMenu() {
        _winMenu.SetActive(true);
    }

    void OnEnable() {
        GameManager.addPlayerEvent += SwitchToClone;
        GameManager.removePlayerEvent += DeletePlayer;
        GameManager.switchPlayerEvent += SwitchPlayer;

        PlayerController.switchBulletEvent += SwitchBullet;
        PlayerController.unlockBulletEvent += AddBullet;
        Status.HPEvent += UpdateHP;

        PlayerController.pauseEvent += HandlePauseMenu;
        GameManager.resumeGameEvent += ReactivateHPGauge;
        GameManager.gameOverEvent += HandleGameOverMenu;
        GameManager.winEvent += HandleWinMenu;
    }

    void OnDisable() {
        GameManager.addPlayerEvent -= SwitchToClone;
        GameManager.removePlayerEvent -= DeletePlayer;
        GameManager.switchPlayerEvent -= SwitchPlayer;

        PlayerController.switchBulletEvent -= SwitchBullet;
        PlayerController.unlockBulletEvent -= AddBullet;
        Status.HPEvent -= UpdateHP;

        PlayerController.pauseEvent -= HandlePauseMenu;
        GameManager.resumeGameEvent -= ReactivateHPGauge;
        GameManager.gameOverEvent -= HandleGameOverMenu;
        GameManager.winEvent -= HandleWinMenu;

        _maxStatusIndex = 0;
    }
}
