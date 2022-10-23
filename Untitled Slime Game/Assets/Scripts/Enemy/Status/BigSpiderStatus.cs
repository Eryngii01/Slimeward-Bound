using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSpiderStatus : EnemyStatus {
    public static event Action defeatBossEvent;

    void Awake() {
        _anim = GetComponent<Animator>();
        _sRenderer = GetComponent<SpriteRenderer>();

        _hitPoints = 20;
    }

    /** 
    Method to inflict damage on the enemy based on the bullet color, visually displaying
    when damage has been inflicted (with a brief invincibility period) on the enemy and 
    causing enemy death when the _hitPoints are depleted.

    This type of enemy can only be damaged by red type bullets.
    **/
    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 0 && _hurtTimer < 0) {
            _hitPoints -= 1;
            MusicManager.Instance.PlayHurt();

            if (_hitPoints < 1) {
                Die();
            } else {
                _hurtTimer = _invincibilityPeriod;
            }
        }
    }

    override protected void Die() {
        base.Die();

        defeatBossEvent?.Invoke();
        Destroy(this.gameObject);
    }
}
