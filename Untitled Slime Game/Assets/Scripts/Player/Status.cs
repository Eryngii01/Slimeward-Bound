using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Status : MonoBehaviour
{
    // --------------- Components ----------------
    private Animator _anim;
    private Rigidbody _rBody;
    private SpriteRenderer _sRenderer;

    [SerializeField]
    private GameObject _burnEffect;
    // -------------------------------------------

    // ------------- Observer Events -------------
    public static event Action<int, int> HPEvent;
    // -------------------------------------------

    // ------------- Invincibility ---------------
    private float _invincibilityTimer = -1, _timeToBeInvincible = 3f;

    private float _flashTimer = -1, _timeBetweenFlash = 0.2f;
    private bool _isRed = false;

    [SerializeField]
    private Color _redFlash;
    // -------------------------------------------

    private int _currentHP;
    public int CurrentHP {
        get {return _currentHP;}
    }

    private int _maxHP = 100;
    public int MaxHP {
        get {return _maxHP;}
    }

    private bool _isFirst;

    void Awake() {
        _anim = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody>();
        _sRenderer = GetComponent<SpriteRenderer>();
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (_invincibilityTimer > 0) {
            FlashRed();
            _invincibilityTimer -= Time.deltaTime;
        } else if (_isRed) {
            _sRenderer.color = Color.white;
        }
    }

    private void FlashRed() {
        if (_flashTimer < 0) {
            if (_isRed) {
                // Change color back to original background color
                _sRenderer.color = Color.white;
                _isRed = false;
            } else {
                // Change color to red
                _sRenderer.color = _redFlash;
                _isRed = true;
            }

            _flashTimer = _timeBetweenFlash;
        } else {
            _flashTimer -= Time.deltaTime;
        }
    }

    public void Burn() {
        _burnEffect.SetActive(true);
    }

    public void CoolDown() {
        _burnEffect.SetActive(false);
    }

    /** 
    Instantiating method to set the player's current health upon creation.
    **/
    public void SetHealth(int health, bool firstPlayer) {
        _currentHP = health;
        _isFirst = firstPlayer;

        transform.localScale = Vector2.one * (1 + (0.03f * _currentHP));

        // Alert Observers that the HP has changed
        HPEvent.Invoke(_currentHP, GameManager.Instance.currentPlayer);
    }

    /** 
    Method to change the player's current HP with positive and/or negative increments. 
    If the current HP reaches zero or below, causes the gameObject to be destroyed.
    Caps the current HP at _maxHP.
    **/
    public void AdjustHealth(int increment) {
        if (_invincibilityTimer < 0 && increment < 0) {
            MusicManager.Instance.PlayHurt();
        }

        if (_invincibilityTimer < 0 || increment >= 0) {
            int prevHP = _currentHP;
            _currentHP += increment;
            transform.localScale = Vector2.one * (1 + (0.03f * _currentHP));

            if (_currentHP <= 0) {
                InvokeDeath();
            } else if (_currentHP > _maxHP) {
                _currentHP = _maxHP;
            }

            if (increment < 0) {
                _invincibilityTimer = _timeToBeInvincible;
            }
            
            // Alert Observers that the HP has changed
            HPEvent.Invoke(_currentHP, GameManager.Instance.currentPlayer);
        }
    }

    public void ChangePlayerOrder(int currentPlayerIndex) {
        HPEvent.Invoke(_currentHP, currentPlayerIndex);
    }

    private void InvokeDeath() {
        GetComponent<PlayerController>().SubtractPlayer();

        // The animator will play the death animation
        _anim.SetTrigger("isDead");

        // Turn off the scripts that enable controls and collision responses
        this.enabled = false;
    }
}
