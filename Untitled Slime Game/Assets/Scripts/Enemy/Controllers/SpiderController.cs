using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpiderController : EnemyController {
    [SerializeField]
    private Vector3 _jumpHeight;
    
    [SerializeField]
    private float _timeToMove;
    private float _moveTime = -1;

    [SerializeField]
    private float _distance = 0.5f;
    protected int _layerMask;
    
    void Awake() {
        _rBody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _layerMask = 1 << LayerMask.NameToLayer("Ground");
    }

    void FixedUpdate() {
        if (_moveTime > 0) {
            // Avoid getting the spider caught on ledges
            RaycastHit hit;

            if(Physics.Raycast(transform.position, -transform.right, out hit, _distance, _layerMask)) {
                SwitchDirection();
            }
        }
    }

    /** 
    Method to move this type of enemy, consisting of constant jumping movement with pausing intervals 
    controlled by _waitTime.
    **/
    override protected void Move() {
        if (_waitTime < 0 && _moveTime > 0 && _canMove) {
            _anim.SetBool("isMoving", true);
            _rBody.MovePosition(_rBody.position + (_moveDirection * _moveSpeed * Time.deltaTime));

            _moveTime -= Time.deltaTime;

        } else if (_waitTime < 0 && _canMove) {
            _anim.SetBool("isMoving", false);
            _anim.SetTrigger("isJumping");
            _rBody.AddForce((_moveDirection + _jumpHeight) * _moveSpeed, ForceMode.Impulse);

            _waitTime = _timeBetweenSwitch;
            _moveTime = _timeToMove;
        } else {
            _waitTime -= Time.deltaTime;
        }
    }
}
