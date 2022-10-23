using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderStatus : EnemyStatus {
    void Awake() {
        _anim = GetComponent<Animator>();
        _sRenderer = GetComponent<SpriteRenderer>();

        _hitPoints = 3;
    }

    /** 
    Method to inflict damage on the enemy based on the bullet color, visually displaying
    when damage has been inflicted (with a brief invincibility period) on the enemy and 
    causing enemy death when the _hitPoints are depleted.

    This type of enemy can only be damaged by green type bullets.
    **/
    override public void TakeDamage(int bulletColor) {
        if (bulletColor == 1 || bulletColor == 2 && _hurtTimer < 0) {
            _hitPoints -= 1;
            MusicManager.Instance.PlayHurt();

            if (_hitPoints < 1) {
                Die();
            } else {
                _hurtTimer = _invincibilityPeriod;
            }
        }
    }
}
