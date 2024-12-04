using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    int index = 0;

    [SerializeField]
    List<GameObject> playerObjects;
    private PlayerInputManager playerInputManager;
    private HashSet<int> playerIndexSet = new HashSet<int>();
    static private List<PlayerController> players = new List<PlayerController>();

    static public List<PlayerController> Players() {
        return players;
    }

    static public void Register(PlayerController player) {
        players.Add(player);
    }

    static public void Unregister(PlayerController player) {
        players.Remove(player);
    }

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = this.GetComponent<PlayerInputManager>();

        SpawnPlayers();
    }

    // spawns 2 new players
    public void SpawnPlayers() {
        index = Random.Range(0, playerObjects.Count);
        playerIndexSet.Add(index);
        playerInputManager.playerPrefab = playerObjects[index];
        playerInputManager.JoinPlayer();

        index = Random.Range(0, playerObjects.Count);

        while (playerIndexSet.Contains(index))
        {
            index = Random.Range(0, playerObjects.Count);
        }

        playerInputManager.playerPrefab = playerObjects[index];
        playerInputManager.JoinPlayer();

        ResetIndexSet();
    }

    void ResetIndexSet() {
        playerIndexSet.Clear();
    }

    public void RespawnPlayers() {
        // destroy current players
        for (int i = players.Count - 1; i >= 0; i--) {
            players[i].Disconnect();
            Destroy(players[i].gameObject);
        }

        // clear the current players
        players.Clear();

        // recreate new players
        SpawnPlayers();
    }
}
