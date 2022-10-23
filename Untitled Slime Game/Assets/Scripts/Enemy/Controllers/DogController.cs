using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DogController : MonoBehaviour
{
    [SerializeField]
    private float _moveSpeed;

    // Start is called before the first frame update
    void Start() {
        
    }

    // Update is called once per frame
    void Update() {
        FollowPlayer();
    }

    /** 
    Method to retrieve the player positions on the map and follow the character with the least distance
    separating this enemy.

    The enemy can only move along the x-axis.
    **/
    private void FollowPlayer() {
        List<GameObject> playerList = GameManager.Instance.PlayerList;

        GameObject curPlayer = playerList[0];
        float distance = float.MaxValue;

        foreach (GameObject player in playerList) {
            float curDistance = Vector3.Distance(transform.position, player.transform.position);
            if (Mathf.Min(distance, curDistance) != distance) {
                distance = curDistance;
                curPlayer = player;
            }
        }

        Vector3 destination = new Vector3(curPlayer.transform.position.x, transform.position.y, transform.position.z);
        transform.position = Vector3.MoveTowards(transform.position, destination, _moveSpeed * Time.deltaTime);
    }
}
