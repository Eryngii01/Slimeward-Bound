using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class HPGaugeController : MonoBehaviour
{
    // --------------- Components ----------------
    private Slider _slider;

    [SerializeField]
    private Image _background;
    [SerializeField]
    private Image _fill;
    // -------------------------------------------

    // -------------- Slider Colors --------------
    private float _flashTimer;
    private float _timeBetweenFlash = 0.7f;

    private bool _isRed = false;

    private Color _backgroundColor;
    private Color _fillColor;
    // -------------------------------------------

    // ----------- Slider Value Method -----------
    private bool _isAnimating = false;
    private bool _isHidden = true;
    private float _animateSpeed = 20f;
    private float _targetHealth;
    // -------------------------------------------

    void Awake() {
        _slider = this.gameObject.GetComponent<Slider>();

        _flashTimer = _timeBetweenFlash;

        _backgroundColor = _background.color;
        _fillColor = _fill.color;
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {
        if (!_isHidden && _slider.value <= 20) {
            FlashRed();
        }

        if (_isAnimating) {
            AnimateHealth();
        }
    }

    private void FlashRed() {
        if (_flashTimer < 0) {
            if (_isRed) {
                // Change color back to original background color
                _background.color = _backgroundColor;
                _isRed = false;
            } else {
                // Change color to red
                _background.color = Color.red;
                _isRed = true;
            }

            _flashTimer = _timeBetweenFlash;
        } else {
            _flashTimer -= Time.deltaTime;
        }
    }

    private void AnimateHealth() {
        _slider.value = Mathf.MoveTowards(_slider.value, _targetHealth, _animateSpeed * Time.deltaTime);

        if (Mathf.Abs(_slider.value - _targetHealth) < 0.001f) {
            _isAnimating = false;

            if (_slider.value > 20) {
            // Change color back to original background color
            _background.color = _backgroundColor;
        }
        }
    }

    public void UpdateHealth(int newHealth) {
        _targetHealth = newHealth;
        _isAnimating = true;
    }

    public void HideElement() {
        _backgroundColor.a = 0f;
        _background.color = _backgroundColor;

        _fillColor.a = 0f;
        _fill.color = _fillColor;

        _isHidden = true;
    }

    public void RevealElement() {
        _backgroundColor.a = 1f;
        _background.color = _backgroundColor;

        _fillColor.a = 1f;
        _fill.color = _fillColor;
        
        _isHidden = false;
    }

    void Pause() {
        this.enabled = false;
    }

    public void Resume() {
        this.enabled = true;
    }

    void OnEnable() {
        GameManager.pauseGameEvent += Pause;
    }

    private void OnDisable() {
        GameManager.pauseGameEvent -= Pause;
    }
}
