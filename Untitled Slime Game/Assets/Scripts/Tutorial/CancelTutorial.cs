using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CancelTutorial : MonoBehaviour {
    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player")) {
            TutorialController.Instance.ExitTutorial();
        }
    }
}
