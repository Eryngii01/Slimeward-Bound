using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletController : MonoBehaviour
{
    private Animator _anim;
    private Rigidbody _rBody;

    private float _moveSpeed = 10f;
    private Vector3 _targetDirection;

    private bool _isMoving = true;
    private int _bulletColor;

    void Awake() {
        _anim = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody>();
    }

    // Start is called before the first frame update
    void Start() {
    }

    // Update is called once per frame
    void Update() {
        if (_isMoving) {
            _rBody.MovePosition(_rBody.position + (_targetDirection * _moveSpeed * Time.deltaTime)); 
        }
    }

    public void SetDirection(Vector3 direction) {
        _targetDirection = direction;
    }

    public void SetColor(int bulletColor) {
        /** 
        Forcefully play the animation according to color since the bug in the Unity animator persists...
        (https://forum.unity.com/threads/entry-state-always-uses-default-transition-and-ignores-other-conditional-transitions-bug.394141/)
        **/
        _bulletColor = bulletColor;

        switch (bulletColor) {
            case 0:
                _anim.Play("Blue Fall");
                break;
            case 1:
                _anim.Play("Green Fall");
                break;
            case 2:
                _anim.Play("Red Fall");
                break;
            case 3:
                _anim.Play("Yellow Fall");
                break;
            case 4:
                _anim.Play("Water Fall");
                break;
        }
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Enemy")) {
            collider.gameObject.GetComponent<EnemyStatus>().TakeDamage(_bulletColor);
        
            _isMoving = false;
            _anim.SetTrigger("isSplattered");
        } else if (collider.gameObject.CompareTag("Block")) {
            collider.gameObject.GetComponent<BlockStatus>().TakeDamage(_bulletColor);
        
            _isMoving = false;
            _anim.SetTrigger("isSplattered");
        } else if (!collider.gameObject.CompareTag("Player") && !collider.gameObject.CompareTag("Bullet")) {
            _isMoving = false;
            _anim.SetTrigger("isSplattered");
        }
    }
}
