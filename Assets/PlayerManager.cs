using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class PlayerManager : MonoBehaviour
{
    int index = 0;

    [SerializeField]
    List<GameObject> playerObjects;
    private PlayerInputManager playerInputManager;
    private HashSet<int> playerIndexSet = new HashSet<int>();

    // Start is called before the first frame update
    void Start()
    {
        playerInputManager = this.GetComponent<PlayerInputManager>();
        index = Random.Range(0, playerObjects.Count);
        playerIndexSet.Add(index);
        playerInputManager.playerPrefab = playerObjects[index];
    }

    public void SwitchNextSpawnCharacter(PlayerInput input)
    {
        index = Random.Range(0, playerObjects.Count);

        while (playerIndexSet.Contains(index))
        {
            index = Random.Range(0, playerObjects.Count);
        }

        playerInputManager.playerPrefab = playerObjects[index];
    }
}
