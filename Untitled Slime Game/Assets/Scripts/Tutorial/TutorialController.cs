using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TutorialController : MonoBehaviour {

    private static TutorialController _instance;
    public static TutorialController Instance {
        get { return _instance; }
    }

    [SerializeField]
    private GameObject _tutorial;
    [SerializeField]
    private GameObject _tutorialBoundaries;

    void Awake() {
        _instance = this;
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player")) {
            _tutorialBoundaries.SetActive(false);
        }
    }

    public void ExitTutorial() {
        _tutorial.SetActive(false);
    }
}
