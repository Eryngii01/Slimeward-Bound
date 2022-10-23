using System;
using UnityEngine;
using UnityEngine.InputSystem;

[RequireComponent(typeof(Animator))]
[RequireComponent(typeof(Rigidbody))]
public class PlayerController : MonoBehaviour
{

    // -------------- Input System ---------------
    private PlayerInputActions _playerInputActions;
    private InputAction _movement;
    // -------------------------------------------

    // --------------- Components ----------------
    private Animator _anim;
    private Rigidbody _rBody;
    private Status _status;
    [SerializeField]
    private BoxCollider _deadCollider;
    // -------------------------------------------

    // ------------- Observer Events -------------
    public static event Action<GameObject> addFirstPlayerEvent;
    public static event Action<GameObject, GameObject> duplicateEvent;
    public static event Action<GameObject, GameObject> absorbEvent;
    public static event Action<int> switchCharacterEvent;
    public static event Action<GameObject> switchBulletEvent;
    public static event Action<int> unlockBulletEvent;
    public static event Action pauseEvent;
    // -------------------------------------------

    // ---------- Instantiating Objects ----------
    [SerializeField]
    private GameObject clone;
    [SerializeField]
    private Transform leftSpawnPos;
    [SerializeField]
    private Transform rightSpawnPos;

    [SerializeField]
    private GameObject bullet;
    [SerializeField]
    private Transform bulletSpawnPos;

    public int currentBulletIndex = 0;
    private static int _maxBulletIndex = 1;
    // -------------------------------------------

    // ---------- Bookkeeping Attribute ----------
    public float jumpForce, moveSpeed, knockBack, gravity;

    private Vector3 _mousePos;

    private bool _firstPlayer = false, _isGrounded = true;
    private float _canShoot = 0f;
    private static int _slimeCount = 0;

    private GameObject _absorbingPlayer;
    // -------------------------------------------

    void Awake() {
        _playerInputActions = new PlayerInputActions();
        _movement = _playerInputActions.Player.Movement;
        
        _anim = GetComponent<Animator>();
        _rBody = GetComponent<Rigidbody>();
        _status = GetComponent<Status>();
    }

    // Start is called before the first frame update
    void Start() {
        if (_slimeCount == 0) {
            _firstPlayer = true;

            // Add the original player to the list of current slimes
            addFirstPlayerEvent?.Invoke(this.gameObject);
            _status.SetHealth(1, _firstPlayer);

            _slimeCount++;
        } else {
            _status.SetHealth(20, _firstPlayer);
        }
    }

    void FixedUpdate() {
        if (!_isGrounded) {
            float velocityY = _rBody.velocity.y;
            velocityY += (gravity) * Time.deltaTime;
            _rBody.velocity = new Vector2(_rBody.velocity.x, velocityY);
        }

        MovePlayer();
        Aim();
        Shoot();
    }
    
    /** 
    Method to move the player along the x-axis, corresponding to the input from the A and D keys. 
    **/
    void MovePlayer() {
        Vector3 movementVector = new Vector3(_movement.ReadValue<float>(), 0, 0);

        if (movementVector.x != Vector3.zero.x) {
            _anim.SetBool("isMoving", true);
            _anim.SetFloat("inputX", movementVector.x);

            double sizeBoost = moveSpeed * ((double)_status.CurrentHP / _status.MaxHP);

            if (_isGrounded) {
                _rBody.MovePosition(_rBody.position + (movementVector * (moveSpeed + (float)sizeBoost) * Time.fixedDeltaTime));
            } else {
                _rBody.MovePosition(_rBody.position + (movementVector * moveSpeed * Time.fixedDeltaTime * 0.75f));
            }

            transform.position = _rBody.position;
        } else {
            _anim.SetBool("isMoving", false);
        }
    }

    /** 
    Method to retrieve the mouse position on the screen. 
    **/
    void Aim() {
        // _mousePos = Camera.main.ScreenToWorldPoint(_playerInputActions.Player.MousePosition.ReadValue<Vector2>());
        _mousePos = Camera.main.ScreenToWorldPoint(Mouse.current.position.ReadValue());
        _mousePos.z = 0;
    }

    /** 
    Method to shoot projectiles from the player in the direction of the mouse position, corresponding to
    when the left button of the mouse is held down.

    The shot projectile will produce a knockback effect on the player applied in the opposite direction
    under specific conditions and effects...
        (1) a knockback effect in the Y-axis will not apply if the player is grounded
        (2) the y attribute of the mouse position unit vector must be < -0.5 && >= 0.5 for the Y-axis
            knockback to apply
        (3) a knockback effect in the X-axis is generally reduced to prevent sliding far distances
    **/
    void Shoot() {
        if (Mouse.current.leftButton.IsPressed() && _canShoot < 0) {
            _mousePos -= bulletSpawnPos.position;

            // Get unit vector
            _mousePos /= _mousePos.magnitude;

            MusicManager.Instance.PlayShoot();

            GameObject bulletInstance = Instantiate(bullet, bulletSpawnPos.position, bullet.transform.rotation);
            bulletInstance.GetComponent<BulletController>().SetColor(currentBulletIndex);
            bulletInstance.GetComponent<BulletController>().SetDirection(_mousePos);

            Vector3 knockBackDirection = new Vector3(_mousePos.x, _mousePos.y, 0);
            // Get unit vector
            // knockBackDirection = knockBackDirection / knockBackDirection.magnitude;
            // Lessen impact in x-axis
            knockBackDirection.x = knockBackDirection.x / 7;

            // Don't allow knockback in the upwards direction if the player is grounded
            if (_isGrounded && knockBackDirection.y < 0) {
                knockBackDirection.y = 0;
            } else {
                // Set Y to same knockback regardless of mouse position
                knockBackDirection.y = Mathf.Round(knockBackDirection.y);
            }

            double sizeBoost = knockBack * ((double)_status.CurrentHP / _status.MaxHP);
            _rBody.AddRelativeForce((Vector3.zero - knockBackDirection) * (knockBack + (float)sizeBoost), ForceMode.Impulse);

            _canShoot = 0.25f;
        } else {
            _canShoot -= Time.deltaTime;
        }
    }

    /** 
    Method to move the player along the y-axis under the influence of gravity. 
    **/
    void Jump(InputAction.CallbackContext obj) {
        if (_isGrounded) {
            _anim.SetTrigger("isJumping");
            _isGrounded = false;

            MusicManager.Instance.PlayJump();

            double sizeBoost = jumpForce * ((double)_status.CurrentHP / _status.MaxHP);
            _rBody.AddRelativeForce(Vector3.up * (jumpForce + (float)sizeBoost), ForceMode.Impulse);
        }
    }

    /** 
    Method to duplicate the player and switch controls to this character. 
    **/
    void Duplicate(InputAction.CallbackContext obj) {
        // If enough resources are available, create gameObject
        if (_slimeCount < 2 && _status.CurrentHP > 20) {
            _anim.SetTrigger("isDuplicating");
            _slimeCount++;

            MusicManager.Instance.PlayDuplicate();
        } else {
            MusicManager.Instance.PlayDeniedMenu();
        }
    }

    public void CreateClone() {
        GameObject cloneInstance;
        Vector3 spawnForce;
        if (_anim.GetFloat("inputX") > 0) {
            spawnForce = new Vector3(0.5f, 1f, 0f);
            cloneInstance = Instantiate(clone, rightSpawnPos.position, clone.transform.rotation);
        } else {
            spawnForce = new Vector3(-0.5f, 1f, 0f);
            cloneInstance = Instantiate(clone, leftSpawnPos.position, clone.transform.rotation);
            cloneInstance.GetComponent<Animator>().SetFloat("inputX", -1);
        }

        cloneInstance.GetComponent<Rigidbody>().AddRelativeForce(spawnForce * 2, ForceMode.Impulse);

        // Subtract health from the duplicating player and set the clone's health
        _status.AdjustHealth(-20);

        // Notify observers another slime is created
        duplicateEvent?.Invoke(this.gameObject, cloneInstance);
    }

    /** 
    Method to promote the clone gameObject to the first player in the case that the original player
    dies when there are clones active.
    **/
    public void Promote() {
        _firstPlayer = true;
    }

    public void SubtractPlayer() {
        _slimeCount--;

        _deadCollider.enabled = true;
        GetComponent<SphereCollider>().enabled = false;
        MusicManager.Instance.PlayDie();

        this.enabled = false;
    }

    /** 
    Method to increase the maxBulletIndex, allowing for a new type of bullet to be selected.

    Upon adding a new bullet type, this method notifies observers to update the UI and game events 
    accordingly.
    **/
    public void UnlockBullet() {
        _maxBulletIndex++;

        unlockBulletEvent?.Invoke(_maxBulletIndex - 1);
    }

    /** 
    Method to absorb the player into the character detected within the surrounding collider.
    The other character reassumes controls.
    **/
    void Absorb(InputAction.CallbackContext obj) {
        if (!_firstPlayer) {
            _slimeCount--;

            MusicManager.Instance.PlayAbsorb();

            // Notify observers of the change -> GameManager to remove the gameObject reference and Camera to change focus
            absorbEvent?.Invoke(this.gameObject, _absorbingPlayer);
        }
    }

    /** 
    Method to switch between the player that is currently being controlled if a duplicate currently exists. 
    Controlled by the UpArrow key.
    **/
    void SwitchPlayerUp(InputAction.CallbackContext obj) {
        if (_slimeCount > 1) {
            switchCharacterEvent.Invoke(0);
        } else {
            MusicManager.Instance.PlayDeniedMenu();
        }
    }

    /** 
    Method to switch between the player that is currently being controlled if a duplicate currently exists. 
    Controlled by the UpArrow key.
    **/
    void SwitchPlayerDown(InputAction.CallbackContext obj) {
        if (_slimeCount > 1) {
            switchCharacterEvent.Invoke(1);
        } else {
            MusicManager.Instance.PlayDeniedMenu();
        }
    }

    /** 
    Method to switch the type of bullets the player can shoot between blue, green, red, and yellow.
    The type of bullets selected effectively changes the player's appearance as well.
    **/
    void SwitchBullet(InputAction.CallbackContext obj) {
        if (_maxBulletIndex == 0) {
            MusicManager.Instance.PlayDeniedMenu();
        } else {
            currentBulletIndex = (currentBulletIndex + 1) % _maxBulletIndex;
            switchBulletEvent.Invoke(this.gameObject);
            MusicManager.Instance.PlayClick();

            switch (currentBulletIndex) {
                case 0:
                    _anim.Play("Blue Idle");
                    break;
                case 1:
                    _anim.Play("Green Idle");
                    break;
                case 2:
                    _anim.Play("Red Idle");
                    break;
                case 3:
                    _anim.Play("Yellow Idle");
                    break;
            }
        }
        
    }

    void PauseGame(InputAction.CallbackContext obj) {
        MusicManager.Instance.PlayOpenMenu();
        pauseEvent?.Invoke();
    }

    // -------------- Observer Event Methods ---------------
    void EnableAbsorb(GameObject player) {
        _playerInputActions.Player.Duplicate.Disable();
        _playerInputActions.Player.Absorb.Enable();

        _absorbingPlayer = player;
    }

    void EnableDuplicate() {
        _playerInputActions.Player.Duplicate.Enable();
        _playerInputActions.Player.Absorb.Disable();
    }

    private void Pause() {
        _anim.SetBool("isMoving", false);
        this.enabled = false;
    }

    public void Resume() {
        this.enabled = true;
    }
    // ------------------------------------------------------

    void OnTriggerEnter(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Ground") || collider.gameObject.CompareTag("Block")) {
            _anim.SetTrigger("isGrounded");
            _isGrounded = true;
        }
    }

    void OnTriggerExit(Collider collider) {
        if (!enabled) {
            return;
        } else if (collider.gameObject.CompareTag("Ground") && _isGrounded) {
            switch (currentBulletIndex) {
                case 0:
                    _anim.Play("Blue Idle");
                    break;
                case 1:
                    _anim.Play("Green Idle");
                    break;
                case 2:
                    _anim.Play("Red Idle");
                    break;
                case 3:
                    _anim.Play("Yellow Idle");
                    break;
            }
            
            _isGrounded = false;
        }
    }

    void OnEnable() {
        DuplicateController.canAbsorbEvent += EnableAbsorb;
        DuplicateController.canDuplicateEvent += EnableDuplicate;

        GameManager.pauseGameEvent += Pause;

        _movement = _playerInputActions.Player.Movement;
        _movement.Enable();

        _playerInputActions.Player.Jump.performed += Jump;
        _playerInputActions.Player.Jump.Enable();

        _playerInputActions.Player.Duplicate.performed += Duplicate;
        _playerInputActions.Player.Duplicate.Enable();

        _playerInputActions.Player.Absorb.performed += Absorb;
        
        _playerInputActions.Player.SwitchPlayerUp.performed += SwitchPlayerUp;
        _playerInputActions.Player.SwitchPlayerUp.Enable();

        _playerInputActions.Player.SwitchPlayerDown.performed += SwitchPlayerDown;
        _playerInputActions.Player.SwitchPlayerDown.Enable();

        _playerInputActions.Player.MousePosition.Enable();
        _playerInputActions.Player.Shoot.Enable();

        _playerInputActions.Player.SwitchBullet.performed += SwitchBullet;
        _playerInputActions.Player.SwitchBullet.Enable();

        _playerInputActions.UI.Pause.performed += PauseGame;
        _playerInputActions.UI.Pause.Enable();
    }

    void OnDisable() {
        DuplicateController.canAbsorbEvent -= EnableAbsorb;
        DuplicateController.canDuplicateEvent -= EnableDuplicate;

        GameManager.pauseGameEvent -= Pause;

        _movement.Disable();

        _playerInputActions.Player.Jump.performed -= Jump;
        _playerInputActions.Player.Jump.Disable();

        _playerInputActions.Player.Duplicate.performed -= Duplicate;
        _playerInputActions.Player.Duplicate.Disable();

        _playerInputActions.Player.Absorb.performed -= Absorb;
        _playerInputActions.Player.Absorb.Disable();

        _playerInputActions.Player.SwitchPlayerUp.performed -= SwitchPlayerUp;
        _playerInputActions.Player.SwitchPlayerUp.Disable();
        _playerInputActions.Player.SwitchPlayerDown.performed -= SwitchPlayerDown;
        _playerInputActions.Player.SwitchPlayerDown.Disable();

        _playerInputActions.Player.MousePosition.Disable();
        _playerInputActions.Player.Shoot.Disable();

        _playerInputActions.Player.SwitchBullet.performed -= SwitchBullet;
        _playerInputActions.Player.SwitchBullet.Disable();

        _playerInputActions.UI.Pause.performed -= PauseGame;
        _playerInputActions.UI.Pause.Disable();
    }

    void OnDestroy() {
        _maxBulletIndex = 1;
        _slimeCount = 0;
    }
}
