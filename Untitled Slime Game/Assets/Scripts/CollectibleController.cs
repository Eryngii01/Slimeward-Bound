using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class CollectibleController : MonoBehaviour
{
    // --------------- Components ----------------
    private SpriteRenderer _sRenderer;
    // -------------------------------------------

    // ------------- Observer Events -------------
    // public static event Action<GameObject> addFirstPlayerEvent;
    // -------------------------------------------
    
    [SerializeField]
    private Sprite[] colors;
    // Serialization for adding the bullets to the scene, otherwise the collectible will automatically be water.
    [SerializeField]
    private int _currentColor = 4;

    private Vector3 _upperBound, _lowerBound;

    private bool _floatDirection = true;
    private float _step;

    void Awake() {
        _sRenderer = GetComponent<SpriteRenderer>();
        _sRenderer.sprite = colors[_currentColor];

        _upperBound = transform.position;
        _upperBound.y += 0.05f;
        _lowerBound = transform.position;

        _step = 0.018f * Time.deltaTime;
    }

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        if (_floatDirection) {
            FloatUp();
        } else {
            FloatDown();
        }
    }

    private void FloatUp() {
        transform.position = Vector3.MoveTowards(transform.position, _upperBound, _step);

        if (Mathf.Abs(transform.position.y - _upperBound.y) < 0.001f) {
            _floatDirection = !_floatDirection;
        }
    }

    private void FloatDown() {
        transform.position = Vector3.MoveTowards(transform.position, _lowerBound, _step);

        if (Mathf.Abs(transform.position.y - _lowerBound.y) < 0.001f) {
            _floatDirection = !_floatDirection;
        }
    }

    private void OnTriggerEnter(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Player")) {
            // Add collectible to player
            if (_currentColor == 4) {
                // This is water
                collider.gameObject.GetComponent<Status>().AdjustHealth(3);
                MusicManager.Instance.PlayPowerUp();
            } else {
                // Unlock a new bullet color!
                collider.gameObject.GetComponent<PlayerController>().UnlockBullet();
                MusicManager.Instance.PlayOpenMenu();
            }

            Destroy(this.gameObject);
        }
    }
}
