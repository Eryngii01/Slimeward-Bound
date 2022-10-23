using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    private Camera _mainCamera;
    private GameObject _currentPlayer;
    private int _currentPlayerIndex;
    private float movementSpeed = 1f;
    private Vector3 _offset;

    private bool _isZooming = false;
    private float _zoomTarget;
    [SerializeField]
    private float _zoomSpeed;
    private float _distance;

    private Vector3 _minBounds, _maxBounds;
    private float _halfHeight, _halfWidth;

    private bool _isSeeking = false;
    [SerializeField]
    private float _seekSpeed;

    void Awake() {
        _mainCamera = GetComponent<Camera>();
        
        _offset = Vector3.down;
    }

    // Start is called before the first frame update
    void Start() {
        _halfHeight = GetComponent<Camera>().orthographicSize;
        _halfWidth = _halfHeight * Screen.width / Screen.height;

        _minBounds = GameManager.Instance.MapBounds.bounds.min;
        _maxBounds = GameManager.Instance.MapBounds.bounds.max;
    }

    // Update is called once per frame
    void Update() {
        if (!_isSeeking) {
            FollowPlayer();
        } else {
            MoveToPosition();
        }

        if (_isZooming) {
            Zoom();
        }
    }

    private void Zoom() {
        _mainCamera.orthographicSize = Mathf.MoveTowards(_mainCamera.orthographicSize, _zoomTarget, (_zoomSpeed + _distance) * Time.deltaTime);
        
        _halfHeight = _mainCamera.orthographicSize;
        _halfWidth = _halfHeight * Screen.width / Screen.height;

        if (Mathf.Abs(_mainCamera.orthographicSize - _zoomTarget) < 0.001f) {
            _isZooming = false;
        }
    }

    private void FollowPlayer() {
        Vector3 playerPos = new Vector3(_currentPlayer.transform.position.x, _currentPlayer.transform.position.y, transform.position.z);
        transform.position = (playerPos + _offset) * movementSpeed;

        float clampedX = Mathf.Clamp(transform.position.x, _minBounds.x + _halfWidth, _maxBounds.x - _halfWidth);
        float clampedY = Mathf.Clamp(transform.position.y, _minBounds.y + _halfHeight, _maxBounds.y - _halfHeight);
        transform.position = new Vector3(clampedX, clampedY, transform.position.z);
    }

    private void MoveToPosition() {
        var step = _seekSpeed * Time.deltaTime;

        float clampedX = Mathf.Clamp(_currentPlayer.transform.position.x, _minBounds.x + _halfWidth, _maxBounds.x - _halfWidth);
        float clampedY = Mathf.Clamp(_currentPlayer.transform.position.y, _minBounds.y + _halfHeight, _maxBounds.y - _halfHeight);
        Vector3 destination = new Vector3(clampedX, clampedY, transform.position.z);

        transform.position = Vector3.MoveTowards(transform.position, destination + _offset, step);

        if (Vector3.Distance(transform.position, destination) < 0.001f) {
            _isSeeking = false;
        }
    }

    private void ChangeDuplicatingPlayer(GameObject clone, int currentPlayerIndex) {
        _currentPlayerIndex = currentPlayerIndex;
        _currentPlayer = clone;
        _isSeeking = true;

        int HP = clone.GetComponent<Status>().CurrentHP;
        _zoomTarget = 1.2609f + (0.03f * HP);
        _isZooming = true;
    }

    private void ChangeRemovedPlayer(GameObject currentPlayer, int prevPlayerIndex, int currentPlayerIndex) {
        _currentPlayerIndex = currentPlayerIndex;
        _currentPlayer = currentPlayer;
        _isSeeking = true;

        int HP = currentPlayer.GetComponent<Status>().CurrentHP;
        _zoomTarget = 1.2609f + (0.03f * HP);
        _isZooming = true;
    }

    private void SeekPlayer(GameObject currentPlayer, int currentPlayerIndex) {
        _distance = Vector3.Distance(_currentPlayer.transform.position, currentPlayer.transform.position) % 5;

        _currentPlayerIndex = currentPlayerIndex;
        _currentPlayer = currentPlayer;
        _isSeeking = true;

        int HP = currentPlayer.GetComponent<Status>().CurrentHP;
        _zoomTarget = 1.2609f + (0.03f * HP);
        _isZooming = true;

        _offset = Vector3.down * (HP * 0.01f);
    }

    private void ZoomCamera(int HP, int currentPlayerIndex) {
        _distance = 0.1f;
        _currentPlayerIndex = currentPlayerIndex;
        
        _zoomTarget = 1.2609f + (0.03f * HP);
        _isZooming = true;

        _offset = Vector3.down * (HP * 0.01f);
    }

    private void OnEnable() {
        GameManager.addPlayerEvent += ChangeDuplicatingPlayer;
        GameManager.removePlayerEvent += ChangeRemovedPlayer;

        GameManager.switchPlayerEvent += SeekPlayer;

        Status.HPEvent += ZoomCamera;
    }

    private void OnDisable() {
        GameManager.addPlayerEvent -= ChangeDuplicatingPlayer;
        GameManager.removePlayerEvent -= ChangeRemovedPlayer;

        GameManager.switchPlayerEvent -= SeekPlayer;

        Status.HPEvent -= ZoomCamera;
    }
}
