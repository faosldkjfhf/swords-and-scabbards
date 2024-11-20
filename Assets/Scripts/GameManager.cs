using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public static float playerHealth = 100.0f;

    [Header("Game Status")]
    public static bool running = true;

    private List<PlayerController> players = new List<PlayerController>();

    private void Awake()
    {
        if (SceneManager.GetActiveScene().name == "Main Menu")
        {
            UnlockCursor();
        }
        else
        {
            Debug.Log("locking cursor");
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

    public void RegisterPlayer(PlayerController player)
    {
        players.Add(player);
    }

    // Update is called once per frame
    void Update()
    {
        foreach (PlayerController player in players)
        {
            if (player.IsDead())
            {
                Debug.Log(player.GetId() + " died!");
                player.OnDeath();
                // QuitGame();
            }
        }
    }

    public void QuitGame()
    {
        Application.Quit(0);
    }
}
