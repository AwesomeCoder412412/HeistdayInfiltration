using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.SceneManagement;
using FPSControllerLPFP;

public class MirrorVariables : NetworkBehaviour
{
    public static MirrorVariables instance;
    public NetworkConnection conn;
    public int minRooms = 5;
    public GameObject victoryScreen;
    public int maxRooms = 10;
    public bool spawnNewPlayer = false;
    public int playersPain;
    public bool shouldOpen = false;
    public BoxCollider puzzleDoor;
    public GameObject buttonsGalore;
    public int n;
    public bool c = false;
    public GameObject treasureRoom;
    [SyncVar]
    public bool rpcNoWork = true;
    private string minRoom = "minRoom";
    private string maxRoom = "maxRoom";
    public GameObject tank;
    public GameObject teleportGoal;

    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        //DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {
        teleportGoal = GameObject.FindGameObjectWithTag("Spawn");
        treasureRoom = GameObject.FindGameObjectWithTag("Treasure");
        /*if (SceneManager.GetActiveScene() == SceneManager.GetSceneByName("Networking") && !c )
        {
            if  (n < 4)
            {
                n++;
            }
            minRooms = PlayerPrefs.GetInt(minRoom);
            maxRooms = PlayerPrefs.GetInt(maxRoom);
            int randomRooms = Random.Range(minRooms, maxRooms);
            int i = 0;
            Debug.Log(randomRooms + " random rooms");
            for (i = 0; i < randomRooms; i++)
            {
                Debug.Log("I'm in the loop " + i);
                GenerateRoomsLazy(i, Random.Range(0, GenerateRooms.instance.puzzleRooms.Length), Random.Range(0, GenerateRooms.instance.puzzles.Length));
            }
            TreasureAhoy(i);
            //n++;
            //c = true;

        }*/
        Debug.Log(GenerateRooms.instance.roomWidth + " pog");
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
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.gameObject.transform.position = GameObject.FindGameObjectWithTag("Spawn").transform.position;
        }

        foreach (GameObject guard in GameObject.FindGameObjectsWithTag("Enemy"))
        {
            NetworkServer.Destroy(guard);
        }
        foreach (GameObject teammate in GameObject.FindGameObjectsWithTag("Teammate"))
        {
            NetworkServer.Destroy(teammate);
        }
        if (GameObject.FindGameObjectWithTag("Tank") == null)
        {
            Debug.Log("tank is ded");
        }
        tank.transform.position = new Vector3(123.73f, 350, -208.94f);
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.hasStarted = false;
        }
        RpcRespawn();
    }
    [Command(ignoreAuthority = true)]
    public void CmdGenerateRooms(int i, int roomPrefabIndex, int puzzlePrefabIndex)
    {
        RpcGenerateRoomsLazy(i, roomPrefabIndex, puzzlePrefabIndex);
    }
    [Command(ignoreAuthority = true)]
    public void CmdGenerateTreasure(int i)
    {
        RpcTreasureRoomLazy(i);
    }
    [ClientRpc]
    public void RpcRespawn()
    {
        Debug.Log("I'm getting this RPC");
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Destroy"))
        {
            Destroy(room);
        }
        Destroy(GameObject.FindGameObjectWithTag("Treasure"));

      /*  if (isServer)
        {
            StartCoroutine(RespawnHost());
        }
        else
        {
            RespawnPain.instance.RespawnClient();
        }*/
        victoryScreen.SetActive(false);
    }
    public IEnumerator RespawnHost()
    {
        Debug.Log("yield return bug");
        yield return new WaitForSecondsRealtime(0.5f);
        //rpcNoWork = true;
        Debug.Log("not stopped");
        NetworkManager.singleton.StopHost();
        Debug.Log("stopped");
        AsyncOperation async = SceneManager.LoadSceneAsync("Inbetween");
        while (!async.isDone)
        {
            yield return 0;
        }
        Debug.Log("not done yet");
        //SceneManager.LoadScene("Networking");
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

    [ClientRpc]
    public void RpcGenerateRoomsLazy(int i, int roomPrefabIndex, int puzzlePrefabIndex)
    {
        Debug.Log("spawn room " + i);
        GameObject room = Instantiate(GenerateRooms.instance.puzzleRooms[roomPrefabIndex], Vector3.forward * GenerateRooms.instance.roomWidth * i, GenerateRooms.instance.puzzleRooms[roomPrefabIndex].transform.rotation);
        GameObject puzzle = Instantiate(GenerateRooms.instance.puzzles[puzzlePrefabIndex], Vector3.forward * GenerateRooms.instance.roomWidth * i, GenerateRooms.instance.puzzles[puzzlePrefabIndex].transform.rotation);
        room.GetComponentInChildren<OpenPuzzle>().puzzleCanvas = puzzle;
        if (isServer)
        {
            GuardAI[] enemies = room.GetComponentsInChildren<GuardAI>();
            foreach (GuardAI ai in enemies)
            {
                ai.roomIndex = i;
                NetworkServer.Spawn(ai.gameObject);
            }
        }
    }

    [ClientRpc]
    public void RpcTreasureRoomLazy(int i)
    {
        GameObject treasureRoomInstance = Instantiate(GenerateRooms.instance.treasureRoom, Vector3.forward * GenerateRooms.instance.roomWidth * i, GenerateRooms.instance.treasureRoom.transform.rotation);
    }

    

    [Command(ignoreAuthority = true)]
    public void CmdVictory()
    {
        RpcVictory();
    }
    [ClientRpc]
    public void RpcVictory()
    {
        victoryScreen.SetActive(true);
    }
    public void Respawn()
    {
        Debug.Log("I'm being called yay");
        CmdRespawn();
    }

    public void SyncVarTest()
    {
        rpcNoWork = true;
    }

    public void TreasureAhoy(int i)
    {
        CmdGenerateTreasure(i);
    }

    public void GenerateRoomsLazy(int i, int roomPrefabIndex, int puzzlePrefabIndex)
    {
        CmdGenerateRooms(i, roomPrefabIndex, puzzlePrefabIndex);
    }
    public void UnlockDoor()
    {
        Debug.Log("functionfruit");
        CmdUnlockDoor();
    }
    [Command(ignoreAuthority = true)]
    public void CmdUnlockDoor()
    {
        Debug.Log("functioncmd");
        //buttonsGalore.GetComponent<OpenPuzzle>().didPuzzle = true;
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.roomIndex++;
        }
        //RpcUnlockDoor();
    }
    [ClientRpc]
    public void RpcUnlockDoor()
    {

        Debug.Log("beforeunlock");
        shouldOpen = true;
        puzzleDoor.enabled = false;
        Debug.Log("after unlock");
    }
    public void RoomsGo()
    {
        if (n < 4)
        {
            n++;
        }
        minRooms = PlayerPrefs.GetInt(minRoom);
        maxRooms = PlayerPrefs.GetInt(maxRoom);
        int randomRooms = Random.Range(minRooms, maxRooms);
        int i = 0;
        Debug.Log(randomRooms + " random rooms");
        for (i = 0; i < randomRooms; i++)
        {
            Debug.Log("I'm in the loop " + i);
            GenerateRoomsLazy(i, Random.Range(0, GenerateRooms.instance.puzzleRooms.Length), Random.Range(0, GenerateRooms.instance.puzzles.Length));
        }
        TreasureAhoy(i);
        //n++;
        //c = true;
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
