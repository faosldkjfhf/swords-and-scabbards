using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
    [Header("Player")]
    public static float playerHealth = 100.0f;

    [Header("Game Status")]
    public static bool running = true;

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

    // Update is called once per frame
    void Update() { }

    public void QuitGame()
    {
        Debug.Log("Quitting game");
        Application.Quit(0);
    }
}
