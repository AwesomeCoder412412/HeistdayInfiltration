using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;

public class MirrorVariables : NetworkBehaviour
{
    public static MirrorVariables instance;
    public NetworkConnection conn;
    public bool spawnNewPlayer = false;
    public int playersPain;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void SpawnPlayer()
    {
        if (spawnNewPlayer && isServer)
        {
            RpcSpawnPlayer();
            Time.timeScale = 1.0f;
        }
    }
    [ClientRpc]
    public void RpcSpawnPlayer()
    {
        Debug.Log("still alive");
        GameObject player = Instantiate(NetworkManager.singleton.playerPrefab);
        Debug.Log("playerd " + (player != null));
        Debug.Log("networkplayer " + (NetworkManager.singleton.playerPrefab != null));
        Debug.Log("server1 " + (MirrorVariables.instance.conn != null));
        NetworkServer.AddPlayerForConnection(MirrorVariables.instance.conn, player);
    }
}
