using System.Collections.Generic;
using Microsoft.Unity.VisualStudio.Editor;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public static float playerHealth = 100.0f;
    public PlayerManager playerManager = default;

    [Header("Game Status")]
    public static bool running = true;
    public uint bestOf = 7;

    [Header("UI")]
    public TextMeshProUGUI scoreText;
    public GameObject pauseScreen;
    public TextMeshProUGUI endText;

    private List<uint> scores = new List<uint> { 0, 0 };

    // public TextMeshProUGUI endText;

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }

        pauseScreen.SetActive(false);
    }

    public void LockCursor()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    public void UnlockCursor()
    {
        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
    }

    // Start is called before the first frame update
    void Start()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            UnlockCursor();
        }
        else
        {
            LockCursor();
        }
    }

    void Restart() {
        // update score and check game state
        for(int i = 0; i < scores.Count; i++) {
            if (scores[i] > (int)Mathf.Ceil(bestOf / 2)) {
                Debug.Log("game over!");
                endText.text = "Player " + (i + 1) + " Won!";

                running = false;
                return;
            }
        }

        // respawn the players
        playerManager.RespawnPlayers();
    }

    // Update is called once per frame
    void Update()
    {
        if (!running) {
            pauseScreen.SetActive(true);
            return;
        }

        foreach(PlayerController player in PlayerManager.Players()) {
            if (player.IsDead()) {
                player.OnDeath();

                Debug.Log(player.GetId());

                scores[player.GetId() % 2]++;

                scoreText.text = scores[0] + " - " + scores[1];

                Restart();
                break;
            }
        }
    }
}
