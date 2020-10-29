using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class DeathScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        if (PlayerController.instance.isPaused == false)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void Retry()
    {
        SceneManager.LoadScene("MainGame");
    }
    public void Resume()
    {
        Time.timeScale = 1.0f;
        PlayerController.instance.isPaused = false;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
