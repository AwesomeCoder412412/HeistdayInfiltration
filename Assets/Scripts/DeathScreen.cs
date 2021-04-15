using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class DeathScreen : NetworkBehaviour
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
        if (NetworkServer.active && NetworkClient.isConnected) {
            NetworkManager.singleton.StopHost();
        }
    }
    public void Retry()
    {
        MirrorVariables.instance.spawnNewPlayer = true;
        SceneManager.LoadScene("Networking");
        Time.timeScale = 1.0f;
    }
    public IEnumerator RetryCoroutine()
    {
        /*AsyncOperation ao = SceneManager.LoadSceneAsync("Networking");
        while (!ao.isDone)
        {
            Debug.Log("Waiting for scene to load");
            yield return null;
        }*/
        yield return null;
        Debug.Log("before instantiante");
        GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);
        Debug.Log("playerd " + (player != null));
        Debug.Log("networkplayer " + (NetworkManager.singleton.playerPrefab != null));
        Debug.Log("server1 " + (MirrorVariables.instance.conn != null));
        NetworkServer.AddPlayerForConnection(MirrorVariables.instance.conn, player);
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
