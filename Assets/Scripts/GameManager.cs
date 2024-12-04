using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public static float playerHealth = 100.0f;

    [Header("Game Status")]
    public static bool running = true;
    public uint bestOf = 7;

    private List<PlayerController> players = new List<PlayerController>();

    private List<uint> scores = new List<uint>();

    public TextMeshProUGUI endText;



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

    void Restart(int id) {
        foreach(uint score in scores) {
            if (score > (int)(bestOf / 2)) {
                Debug.Log("game over!");
                endText.enabled = true;
            }
        }

        foreach(PlayerController player in players) {
            player.Reset();
        }
    }

    public void RegisterPlayer(PlayerController player)
    {
        players.Add(player);
        scores.Add(0);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerController player in players)
        {
            if (player.IsDead())
            {
                // Debug.Log(player.GetId() + " died!");
                player.OnDeath();
                scores[player.GetId() - 1]++;

                Restart(player.GetId() - 1);
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
