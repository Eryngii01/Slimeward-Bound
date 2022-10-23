using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPipeController : MonoBehaviour {
    private bool _isBurning = false;

    // Update is called once per frame
    void Update() {
        
    }

    void OnCollisionStay(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            Status player = collision.gameObject.GetComponent<Status>();
            player.AdjustHealth(-5);

            if (!_isBurning) {
                player.Burn();
                MusicManager.Instance.PlaySizzle();

                _isBurning = true;
            }
        }
    } 

    void OnCollisionExit(Collision collision) {
        if (collision.gameObject.CompareTag("Player")) {
            collision.gameObject.GetComponent<Status>().CoolDown();
            _isBurning = false;
        }
    }
}
