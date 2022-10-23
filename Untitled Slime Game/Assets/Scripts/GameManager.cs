using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    private static GameManager _instance;
    public static GameManager Instance {
        get {
            if (_instance == null)
                Debug.LogError("Game Manager is null!");

            return _instance;
        }
    }

    [SerializeField]
    private BoxCollider2D _mapBounds;
    public BoxCollider2D MapBounds {
        get { return _mapBounds; }
    }

    // ------------- Observer Events -------------
    public static event Action<GameObject, int> addPlayerEvent;
    public static event Action<GameObject, int, int> removePlayerEvent;
    public static event Action<GameObject, int> switchPlayerEvent;
    public static event Action pauseGameEvent, resumeGameEvent, gameOverEvent, winEvent;
    // -------------------------------------------

    // ---------- Instantiating Objects ----------
    [SerializeField]
    private GameObject _antEnemy;
    [SerializeField]
    private Transform[] _antSpawnPositions;
    [SerializeField]
    private BoxCollider _leftBorder, _rightBorder;

    private float _antSpawnTimer = -1, _timeToSpawnAnt = 7f;
    // -------------------------------------------

    // ------------- Size Colliders --------------
    [SerializeField]
    private GameObject _smallColliders, _largeColliders;
    // -------------------------------------------
    
    private List<GameObject> _playerList;
    public List<GameObject> PlayerList {
        get {return _playerList;}
    }

    public int currentPlayer, _bulletType;

    [SerializeField]
    private Texture2D _cursorSprite;

    [SerializeField]
    private GameObject _boss;

    private bool _beatBoss = false, _canSpawnBoss = true;

    void Awake() {
        if (_instance == null) {
            _instance = this;
        }
        
        _playerList = new List<GameObject>();
    }

    // Start is called before the first frame update
    void Start() {
        Cursor.SetCursor(_cursorSprite, Vector2.zero, CursorMode.Auto);
    }

    // Update is called once per frame
    void Update() {
        if (_antSpawnTimer < 0 && _playerList[currentPlayer].GetComponent<Status>().CurrentHP <= 50) {
            int spawnPos = UnityEngine.Random.Range(0, _antSpawnPositions.Length);
            GameObject antInstance = Instantiate(_antEnemy, _antSpawnPositions[spawnPos].position, _antEnemy.transform.rotation);
            antInstance.GetComponent<AntController>().SetBorders(_leftBorder, _rightBorder);

            _antSpawnTimer = _timeToSpawnAnt;
        } else {
            _antSpawnTimer -= Time.deltaTime;
        }
    }

    void UpdateColliders() {
        // Change colliders available depending on player size
        Status currentStatus = _playerList[currentPlayer].GetComponent<Status>();

        if (currentStatus.CurrentHP <= 50) {
            _smallColliders.SetActive(true);
            _largeColliders.SetActive(false);
        } else if (currentStatus.CurrentHP > 50) {
            _smallColliders.SetActive(false);
            _largeColliders.SetActive(true);
        }
    }

    void AddFirstPlayer(GameObject player) {
        _playerList.Add(player);
        currentPlayer = _playerList.Count - 1;

        // Notify other observers of the current player
        addPlayerEvent?.Invoke(player, currentPlayer);
    }

    void AddClone(GameObject player, GameObject clone) {
        _playerList.Add(clone);
        currentPlayer = _playerList.Count - 1;

        // Disable the other PlayerController;
        player.GetComponent<PlayerController>().enabled = false;

        // If the duplicating character is a clone, disable the clone script
        /** try {
            player.GetComponent<DuplicateController>().enabled = false;
        } catch (Exception e) {} // What to do about the 'e' warning
        **/

        // Notify other observers of the current player
        addPlayerEvent?.Invoke(clone, currentPlayer);

        // May want to keep tabs on the current bullet color for the clone and whatnot
    }

    void RemoveClone(GameObject player, GameObject absorbingPlayer) {
        _playerList.Remove(player);

        int prevPlayer = currentPlayer;
        currentPlayer = _playerList.IndexOf(absorbingPlayer.gameObject);

        // Store clone's HP and destroy the gameObject
        int absorbingHP = player.GetComponent<Status>().CurrentHP;
        Destroy(player);

        // Enable the script of the other gameObject
        absorbingPlayer.GetComponent<PlayerController>().enabled = true;
        absorbingPlayer.GetComponent<Status>().AdjustHealth(absorbingHP);
        /** try {
            absorbingPlayer.GetComponent<DuplicateController>().enabled = true;
        } catch (Exception e) {} **/

        removePlayerEvent?.Invoke(absorbingPlayer, prevPlayer, currentPlayer);
    }

    void UpdateHPGameStatus(int HP, int currentPlayerIndex) {
        UpdateColliders();
    }

    void SwitchPlayer(int direction) {
        GameObject prevPlayer = _playerList[currentPlayer];

        // Keep track of other gameObject and enable that script while disabling this script
        if (direction == 0) {
            currentPlayer--;
            currentPlayer = currentPlayer >= 0 ? currentPlayer : _playerList.Count - 1;
        } else {
            currentPlayer++;
            currentPlayer = currentPlayer % _playerList.Count;
        }

        prevPlayer.GetComponent<Animator>().SetBool("isMoving", false);
        prevPlayer.GetComponent<PlayerController>().enabled = false;

        _playerList[currentPlayer].GetComponent<PlayerController>().enabled = true;
        // Switch the colliders available accordingly
        UpdateColliders();
        
        switchPlayerEvent?.Invoke(_playerList[currentPlayer], currentPlayer);
    }

    void RemovePlayer(GameObject player) {
        int prevPlayer = currentPlayer;
        GameObject nextPlayer = player;

        if (_playerList.Count == 1) {
            // There is only one player object left to destroy, so it's game over
            EndGame();
        } else {
            if (prevPlayer == 0) {
                // Promote the next player in the list if the original is destroyed
                nextPlayer = _playerList[prevPlayer + 1];

                PlayerController nextPlayerController = nextPlayer.GetComponent<PlayerController>();
                nextPlayerController.enabled = true;
                nextPlayerController.Promote();
            } else {
                _playerList[prevPlayer].GetComponent<DuplicateController>().enabled = false;
                
                // Set the new current player to the original player
                nextPlayer = _playerList[0];

                // Enable the script of the other gameObject
                nextPlayer.GetComponent<PlayerController>().enabled = true;
            }

            currentPlayer = 0;
            _playerList.Remove(player);

            nextPlayer.GetComponent<Status>().ChangePlayerOrder(currentPlayer);
            removePlayerEvent?.Invoke(nextPlayer, prevPlayer, currentPlayer);
        }
    }

    void DefeatBoss() {
        _beatBoss = true;

        MusicManager.Instance.PlayBGM();
    }

    void EndGame() {
        gameOverEvent?.Invoke();
        MusicManager.Instance.PlayLose();
        MusicManager.Instance.PlayOpenMenu();

        currentPlayer = 0;
    }

    public void PauseGame() {
        // _playerList[currentPlayer].GetComponent<PlayerController>().Pause();
        pauseGameEvent?.Invoke();
    }

    public void ResumeGame() {
        _playerList[currentPlayer].GetComponent<PlayerController>().Resume();
        resumeGameEvent?.Invoke();
    }

    void OnTriggerEnter(Collider collider) {
        if (collider.gameObject.CompareTag("Player") && !_beatBoss && _canSpawnBoss) {
            _boss.SetActive(true);
            _boss.GetComponent<BigSpiderController>().PlayEntry();
            MusicManager.Instance.PlayBattle();

            _canSpawnBoss = false;
        } else if (collider.gameObject.CompareTag("Player") && _beatBoss) {
            _playerList[currentPlayer].SetActive(false);
            MusicManager.Instance.PlayWin();
            MusicManager.Instance.PlayOpenMenu();
            winEvent?.Invoke();
        }
    }

    void OnEnable() {
        PlayerController.addFirstPlayerEvent += AddFirstPlayer;
        PlayerController.duplicateEvent += AddClone;
        PlayerController.absorbEvent += RemoveClone;
        Status.HPEvent += UpdateHPGameStatus;
        
        PlayerController.switchCharacterEvent += SwitchPlayer;
        PlayerController.pauseEvent += PauseGame;

        DeathBehaviour.deathEvent += RemovePlayer;
        BigSpiderStatus.defeatBossEvent += DefeatBoss;
    }

    void OnDisable() {
        PlayerController.addFirstPlayerEvent -= AddFirstPlayer;
        PlayerController.duplicateEvent -= AddClone;
        PlayerController.absorbEvent -= RemoveClone;
        Status.HPEvent -= UpdateHPGameStatus;
        
        PlayerController.switchCharacterEvent -= SwitchPlayer;
        PlayerController.pauseEvent -= PauseGame;

        DeathBehaviour.deathEvent -= RemovePlayer;
        BigSpiderStatus.defeatBossEvent -= DefeatBoss;
    }
}
