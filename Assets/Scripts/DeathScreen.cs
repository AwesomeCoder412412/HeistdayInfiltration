using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class DeathScreen : NetworkBehaviour
{
    public GameObject freecam;
    public GameObject self;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.SetActive(false);
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            if (pc.isLocalPlayer && !pc.isPaused)
            {
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
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
        if (isServer)
        {
            MirrorVariables.instance.Respawn();
            //foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
            //{
            //    pc.hasStarted = false;
            //}
            //gameObject.SetActive(false);
        }
        /*else
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
            foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
            {
                if (pc.isLocalPlayer)
                {
                    pc.gameObject.SetActive(false);
                    freecam.SetActive(true);
                }
            }
            self.SetActive(false);
        } */
        /* MirrorVariables.instance.spawnNewPlayer = true;
         SceneManager.LoadScene("Networking");
         Time.timeScale = 1.0f; */
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
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            if (pc.isLocalPlayer)
            {
                pc.isPaused = false;
            }
        }
        Time.timeScale = 1.0f;
        Cursor.visible = false;
        Cursor.lockState = CursorLockMode.Locked;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
