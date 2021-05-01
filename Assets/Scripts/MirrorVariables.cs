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
    [SyncVar]
    public bool rpcNoWork = false;
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
       /*/ if (gameObject.scene.buildIndex != -1)
        {
            DontDestroyOnLoad(gameObject);
        }*/
        Debug.Log(rpcNoWork);
        if (isServer)
        {
            return;
        }
      /*  if (!isServer && rpcNoWork)
        {
            Debug.Log("staying alive");
            string ip = NetworkManager.singleton.networkAddress;
            NetworkManager.singleton.StopClient();
            Debug.Log(ip + "gummies");
            NetworkManager.singleton.networkAddress = ip;
            Debug.Log(NetworkManager.singleton.networkAddress + "gummies2");
            NetworkManager.singleton.StartClient();
            Debug.Log(NetworkManager.singleton.networkAddress + "gummies3");
        }*/
    }

    [Command (ignoreAuthority = true)]
    public void CmdRespawn()
    {
        RpcRespawn();
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        Debug.Log("I'm getting this RPC");
        if (isServer)
        {
            StartCoroutine(RespawnHost());
        }
        else
        {
            RespawnPain.instance.RespawnClient();
        }

    }
    public IEnumerator RespawnHost()
    {
        Debug.Log("yield return bug");
        yield return new WaitForSecondsRealtime(2);
        //rpcNoWork = true;
        Debug.Log("not stopped");
        NetworkManager.singleton.StopHost();
        Debug.Log("stopped");
        NetworkManager.singleton.StartHost();
        Debug.Log("host has been started");
        //rpcNoWork = false;
    }
    public IEnumerator RespawnClient()
    {

        Debug.Log("staying alive");
        string ip = NetworkManager.singleton.networkAddress;
        NetworkManager.singleton.StopClient();
        Debug.Log(ip + "gummies");
        NetworkManager.singleton.networkAddress = ip;
        Debug.Log(NetworkManager.singleton.networkAddress + "gummies2");
        yield return new WaitForSecondsRealtime(4);
        Debug.Log("gummmies2.5");
        NetworkManager.singleton.StartClient();
        Debug.Log(NetworkManager.singleton.networkAddress + "gummies3");
    }

    public void Respawn()
    {
        CmdRespawn();
    }

    public void SyncVarTest()
    {
        rpcNoWork = true;
    }


    /*public void SpawnPlayer()
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
    }*/
}
