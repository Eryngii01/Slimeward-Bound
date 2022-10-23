using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntStatus : EnemyStatus {
    [SerializeField]
    private GameObject _collectable;

    private bool _isEntering = true;

    void Awake() {
        _sRenderer = GetComponent<SpriteRenderer>();

        _hitPoints = 2;
    }

    override protected void Update() {
        base.Update();

        Fade();
    }

    void Fade() {
        if (_isEntering) {
            _sRenderer.color += new Color(0, 0, 0, 0.001f);
            
            if (_sRenderer.color.a == 1) {
                _isEntering = false;
            }
        }
    }

    /** 
    Method to inflict damage on the enemy based on the bullet color, visually displaying
    when damage has been inflicted (with a brief invincibility period) on the enemy and 
    causing enemy death when the _hitPoints are depleted.

    This type of enemy can only be damaged by green type bullets.
    **/
    override public void TakeDamage(int bulletColor) {
        if (bulletColor != 2 && _hurtTimer < 0) {
            _hitPoints -= 1;
            MusicManager.Instance.PlayHurt();

            if (_hitPoints < 1) {
                Die();
            } else {
                _hurtTimer = _invincibilityPeriod;
            }
        }
    }

    protected override void Die() {
        AntController antController = GetComponent<AntController>();
        // Spawn one water droplet by default; if the ant has stolen some slime, spawn two
        Instantiate(_collectable, transform.position, _collectable.transform.rotation);
    
        if (antController._hasSlime) {
            Instantiate(_collectable, transform.position, _collectable.transform.rotation);
        }

        // Destroy GameObject
        Destroy(this.gameObject);
    }
}
