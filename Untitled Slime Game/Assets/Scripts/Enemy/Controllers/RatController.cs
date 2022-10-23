using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RatController : EnemyController {

    void Awake() {
        _rBody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();
    }

    /** 
    Method to move this type of enemy, consisting of constant movement along the x-axis with pausing intervals
    controlled by _waitTime.
    **/
    override protected void Move() {
        if (_waitTime <= 0 && _canMove) {
            _anim.SetBool("isMoving", true);
            _rBody.MovePosition(_rBody.position + (_moveDirection * _moveSpeed));
        } else {
            _anim.SetBool("isMoving", false);
            _waitTime -= Time.deltaTime;
        }
    }

    /** 
    Method to track when the ground or the player exits from the colllider bounds around this type of enemy.

    - When the player leaves the bounds, the enemy will immediately cease to attack.
    - When the ground (underneath) leaves the bounds, the enemy will immediately switch directions to 
        avoid falling off the platform.
    **/
    override protected void OnTriggerExit(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Ground")) {
            // Trigger the gameObject to move in the opposite direction
            _moveDirection *= -1;

            // Flip the sprite
            _rotation.y = (_rotation.y - 180) % 360;
            transform.rotation = Quaternion.Euler(_rotation);

            _waitTime = _timeBetweenSwitch;
        } else if (collider.gameObject.CompareTag("Player")) {
            _canMove = true;
        }
    }
}
