using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PausedMenu : MonoBehaviour
{
    public static PausedMenu instance;
    public GameObject pauseMenu;
    // Start is called before the first frame update
    void Start()
    {
        MirrorVariables.instance.SpawnPlayer();
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already an instance of the PausedMenu class!");
        }
        //pauseMenu = gameObject;
    }

    // Update is called once per frame
    void Update()
    {
        //MirrorVariables.instance.SpawnPlayer();
    }
    public void MainMenu()
    {
        SceneManager.LoadScene("StartScreen");
    }
    public void Retry()
    {
        SceneManager.LoadScene("MainGame");
    }
}
