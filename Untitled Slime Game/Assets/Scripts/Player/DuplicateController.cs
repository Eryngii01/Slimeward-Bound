using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DuplicateController : MonoBehaviour
{
    // ------------- Observer Events -------------
    public static event Action<GameObject> canAbsorbEvent;
    public static event Action canDuplicateEvent;
    // -------------------------------------------
    
    private Animator _anim;
    private Status _status;

    void Awake() {
        _anim = GetComponent<Animator>();
        _anim.SetBool("isJumping", true);

        _status = GetComponent<Status>();
    }

    // Start is called before the first frame update
    void Start() {

    }

    // Update is called once per frame
    void Update() {

    }

    void OnTriggerEnter(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player") && collider.gameObject != this.gameObject) {
            // Disable duplicate functionality when approaching another player clone
            // and instead allow to join the character into a single object
            canAbsorbEvent?.Invoke(collider.gameObject);
        }
    }

    void OnTriggerExit(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player")) {
            // Disable duplicate functionality when approaching another player clone
            // and instead allow to join the character into a single object
            canDuplicateEvent?.Invoke();
        }
    }
}
