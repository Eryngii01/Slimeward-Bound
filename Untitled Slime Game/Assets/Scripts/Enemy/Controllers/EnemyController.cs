using System.Collections;
using System.Collections.Generic;
using UnityEngine;

abstract public class EnemyController : MonoBehaviour {
    [SerializeField]
    protected float _moveSpeed;
    protected Vector3 _moveDirection = Vector3.left;
    protected Vector3 _rotation = Vector3.zero;

    protected Rigidbody _rBody;
    protected Animator _anim; 

    protected float _waitTime;
    protected bool _canMove = true;
    [SerializeField]
    protected float _timeBetweenSwitch;

    // Update is called once per frame
    virtual protected void Update() {
        Move();
    }

    /** 
    Method to move different types of enemies. Each enemy will have a different movement behaviour.
    **/
    abstract protected void Move();

    /** 
    Method to track when this object hits another collider tagged as an obstacle. Immediately
    changes the movement direction and direction the object is facing upon collision.
    **/
    virtual protected void OnTriggerEnter(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Obstacle") || collider.gameObject.CompareTag("Block")) {
            SwitchDirection();

            _anim.SetBool("isMoving", false);
            _waitTime = _timeBetweenSwitch;
        }
    }

    virtual protected void SwitchDirection() {
        // Flip the sprite
        _rotation.y = (_rotation.y - 180) % 360;
        transform.rotation = Quaternion.Euler(_rotation);

        // Trigger the gameObject to move in the opposite direction
        _moveDirection *= -1;
    }

    /** 
    Method to track when this object hits another collider tagged as the player. As long as
    the player is located within the bounds, this object will continuously attack the player.
    **/
    virtual protected void OnTriggerStay(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player")) {
            _canMove = false;
            _anim.SetTrigger("isAttacking");
            collider.gameObject.GetComponent<Status>().AdjustHealth(-5);
        }
    }

    /** 
    Method to track when another collider tagged as the player leaves the bounds of its collider. 
    When the player leaves the bounds, this object ceases to attack the player.
    **/
    virtual protected void OnTriggerExit(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player")) {
            _canMove = true;
        }
    }

    virtual protected void Pause() {
        _anim.SetBool("isMoving", false);
        this.enabled = false;
    }

    void OnEnable() {
        GameManager.pauseGameEvent += Pause;
    }

    void OnDisable() {
        GameManager.pauseGameEvent -= Pause;
    }
}
