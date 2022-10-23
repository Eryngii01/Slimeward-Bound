using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AntController : EnemyController {
    [SerializeField]
    private GameObject _stolenBall;

    private bool _goingUp;
    public bool _hasSlime = false;

    [SerializeField]
    private float _distance = 0.01f, _runDistance = 2f; //_switchCooldown
    // private float _switchTime = -1;

    private BoxCollider _leftBorder, _rightBorder;

    private int _layerMask, _runLayerMask;

    void Awake() {
        _rBody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _layerMask = 1 << LayerMask.NameToLayer("Ground");
        _runLayerMask = 1 << LayerMask.NameToLayer("Player");

        SwitchDirection();
    }

    void FixedUpdate() {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.right, out hit, _distance, _layerMask) && !_goingUp) {
            // Rotate upwards
            _rotation.z = (_rotation.z - 90) % 360;
            _rBody.rotation = Quaternion.Euler(_rotation);
            _moveDirection = transform.up;

            _goingUp = true;
        } else {
            _goingUp = false;

            if (_hasSlime) {
                EscapePlayer();
            }
        }
    }

    public void SetBorders(BoxCollider leftBorder, BoxCollider rightBorder) {
        _leftBorder = leftBorder;
        _rightBorder = rightBorder;
    }

    void DestroyRebel() {
        Destroy(this);
    }

    override protected void Move() {
        _rBody.position = Vector3.MoveTowards(transform.position, transform.position + _moveDirection, _moveSpeed);
    }

    /** void FollowPlayer() {
        if (_switchTime < 0) {
            List<GameObject> playerList = GameManager.Instance.PlayerList;

            GameObject curPlayer = playerList[0];
            float distance = float.MaxValue;

            // Fetch information on the closest player gameObject
            foreach (GameObject player in playerList) {
                float curDistance = Vector3.Distance(transform.position, player.transform.position);
                if (Mathf.Min(distance, curDistance) != distance) {
                    distance = curDistance;
                    curPlayer = player;
                }
            }

            // Moving right
            if (_moveDirection.x > 0 && curPlayer.transform.position.x < transform.position.x) {
                if (Mathf.Abs(transform.position.x - curPlayer.transform.position.x) < Mathf.Abs(transform.position.x - _rightBorder.bounds.min.x)) {
                    SwitchDirection();
                    _switchTime = _switchCooldown;
                }
            // Moving left
            } else if (_moveDirection.x < 0 && curPlayer.transform.position.x > transform.position.x) {
                if (Mathf.Abs(transform.position.x - curPlayer.transform.position.x) < Mathf.Abs(transform.position.x - _leftBorder.bounds.max.x)) {
                    SwitchDirection();
                    _switchTime = _switchCooldown;
                }
            }
        } else {
            _switchTime -= Time.deltaTime;
        }
    }
    **/

    void EscapePlayer() {
        RaycastHit hit;

        if(Physics.Raycast(transform.position, -transform.right, out hit, _runDistance, _runLayerMask)) {
            SwitchDirection();
        }
    }

    override protected void OnTriggerEnter(Collider collider) {
        base.OnTriggerEnter(collider);

        if (collider.gameObject.CompareTag("Player") && !_hasSlime) {
            // Attack Player
            collider.gameObject.GetComponent<Status>().AdjustHealth(-3);

            // Spawn a slime ball
            int currentColor = collider.gameObject.GetComponent<PlayerController>().currentBulletIndex;
            _stolenBall.SetActive(true);
            _stolenBall.GetComponent<ColorSelector>().SetColor(currentColor);

            // Begin running away
            SwitchDirection();
            _hasSlime = true;
        } else if (collider.gameObject.CompareTag("Exit")) {
            Destroy(this.gameObject);
        }
    }

    override protected void OnTriggerStay(Collider collider) {
        return;
    }

    override protected void OnTriggerExit(Collider collider) {
        if (!enabled) {
            return;
        } else if (!_goingUp && collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Obstacle")) {
            // Rotate downwards
            _rotation.z = (_rotation.z + 90) % 360;
            _rBody.rotation = Quaternion.Euler(_rotation);
            _moveDirection = -transform.up;
        } else if (collider.gameObject.CompareTag("Player")) {
            _canMove = true;
        }
    }

    override protected void Pause() {
        _anim.SetBool("isMoving", false);
        this.enabled = false;
    }
}
