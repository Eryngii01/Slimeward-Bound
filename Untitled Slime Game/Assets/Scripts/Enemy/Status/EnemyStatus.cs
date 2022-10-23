using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EnemyStatus : MonoBehaviour {
    protected Animator _anim; 
    protected SpriteRenderer _sRenderer;

    // Default enemy takes five hits until death
    protected int _hitPoints = 5;

    [SerializeField]
    protected Color _damagedColor;

    protected float _hurtTimer = -1f, _invincibilityPeriod = 1f;

    // Update is called once per frame
    virtual protected void Update() {
        if (_hurtTimer > 0) {
            _sRenderer.color = _damagedColor;
            _hurtTimer -= Time.deltaTime;
        } else {
            _sRenderer.color = Color.white;
        }
    }

    /** 
    Method to inflict damage on the enemy based on the bullet color, visually displaying
    when damage has been inflicted (with a brief invincibility period) on the enemy and 
    causing enemy death when the _hitPoints are depleted.

    The default method allows this object to be damaged by any bullet color.
    **/
    virtual public void TakeDamage(int bulletColor) {
        if (_hurtTimer < 0) {
            _hitPoints -= 1;

            if (_hitPoints < 1) {
                Die();
            } else {
                _hurtTimer = _invincibilityPeriod;
            }
        }
    }

    /** 
    Method to inflict death on the enemy, playing the death animations and disabling
    all the scripts that calculate movement and attacks.
    **/
    virtual protected void Die() {
        _anim.SetTrigger("isDead");
        _sRenderer.color = Color.white;
        this.gameObject.GetComponent<EnemyController>().enabled = false;
        this.enabled = false;
    }

    virtual protected void Resume() {
        this.gameObject.GetComponent<EnemyController>().enabled = true;
    }

    virtual protected void OnEnable() {
        GameManager.resumeGameEvent += Resume;
    }

    virtual protected void OnDisable() {
        GameManager.resumeGameEvent -= Resume;
    }
}
