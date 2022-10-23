using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigSpiderController : SpiderController {
    void Awake() {
        _rBody = GetComponent<Rigidbody>();
        _anim = GetComponent<Animator>();

        _layerMask = 1 << LayerMask.NameToLayer("Ground");
    }
    
    public void PlayEntry() {
        _rBody.AddForce(Vector3.left * 2, ForceMode.Impulse);
    }

    /** 
    Method to track when this object hits another collider tagged as an obstacle. Immediately
    changes the movement direction and direction the object is facing upon collision.
    **/
    override protected void OnTriggerEnter(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Obstacle")) {
            SwitchDirection();

            _anim.SetBool("isMoving", false);
        } else if (collider.gameObject.CompareTag("Block")) {
            Destroy(collider.gameObject);
        }
    }

    /** 
    Method to track when this object hits another collider tagged as the player. As long as
    the player is located within the bounds, this object will continuously attack the player.
    **/
    override protected void OnTriggerStay(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player")) {
            _canMove = false;
            _anim.SetTrigger("isAttacking");
            collider.gameObject.GetComponent<Status>().AdjustHealth(-10);
        }
    }
}
