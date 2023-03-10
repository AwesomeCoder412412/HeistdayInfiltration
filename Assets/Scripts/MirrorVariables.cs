using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using Mirror.RemoteCalls;
using UnityEngine.SceneManagement;
using FPSControllerLPFP;

public class MirrorVariables : NetworkBehaviour
{
    public bool treasureSpawned;
    public bool doingPuzzle = false;
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
    private string minRoom = "minRoom";
    private string maxRoom = "maxRoom";
    public GameObject tank;
    public GameObject teleportGoal;
    public int puzzle, grav;
    [SyncVar]
    public bool toTankOrNotToTank;

    void Start()
    {
        instance = this;
    }

    void Update()
    {
        //TODO: this is terrible
        teleportGoal = GameObject.FindGameObjectWithTag("Spawn");
        treasureRoom = GameObject.FindGameObjectWithTag("Treasure");
        if (isServer)
        {
            return;
        }
    }

    public void Respawn()
    {
        CmdRespawn();
    }

    [Command (requiresAuthority = false)]
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
        tank.transform.position = new Vector3(123.73f, 350, -208.94f);
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.hasStarted = false;
        }
        RpcRespawn();
    }
    
    [ClientRpc]
    public void RpcRespawn()
    {
        Treasure.instance.DestroyTreasure();
        foreach (GameObject room in GameObject.FindGameObjectsWithTag("Destroy"))
        {
            Destroy(room);
        }
        Destroy(GameObject.FindGameObjectWithTag("Treasure"));

        victoryScreen.SetActive(false);
    }
    public IEnumerator RespawnHost()
    {
        yield return new WaitForSecondsRealtime(0.5f);
        NetworkManager.singleton.StopHost();
        AsyncOperation async = SceneManager.LoadSceneAsync("Inbetween");
        while (!async.isDone)
        {
            yield return 0;
        }
        NetworkManager.singleton.StartHost();
    }
    public IEnumerator RespawnClient()
    {
        string ip = NetworkManager.singleton.networkAddress;
        NetworkManager.singleton.StopClient();
        NetworkManager.singleton.networkAddress = ip;
        yield return new WaitForSecondsRealtime(4);
        NetworkManager.singleton.StartClient();
    }

    [Command(requiresAuthority = false)]
    public void CmdGenerateRooms(int i, int roomPrefabIndex, int puzzlePrefabIndex, RoomType type, float distance)
    {
        if (type == RoomType.TIMED_ESCAPE_GRAVITY)
        {
            RpcGenerateGravityEscapeRooms(i, type, distance);
        }
        if (type == RoomType.STANDARD)
        {
            RpcGenerateStandardPuzzleRooms(i, Random.Range(0, 2), Random.Range(0, 2), distance);
        }
    }

    [Command(requiresAuthority = false)]
    public void CmdGenerateTreasure(int i, float distance)
    {
        RpcTreasureRoomLazy(i, distance);
    }

    [ClientRpc]
    public void RpcGenerateGravityEscapeRooms(int i, RoomType type, float distance)
    {
        //TODO: Mouse sensitivity is buggy when first loading into game, but it's fixed a couple seconds after generating the rooms.
        //I've narrowed it down to this line of code that applies the fix and I have absolutely no clue how instantiating a gameobject would fix mouse issues.
        //I initially theorized that it was perhaps the game freezing to load the rooms that fixed it somehow, but I tried to artificially lag the game through other means and
        //it did nothing. I could have messed up the testing though. Also, the sensitivity is less fixed the lower amount of rooms are generated. At 10 rooms its perfect and at 1 its barely an improvement. 

        GameObject room = Instantiate(GenerateRooms.instance.gravityRoom, GenerateRooms.instance.startPos + Vector3.forward * distance, GenerateRooms.instance.gravityRoom.transform.rotation);
        if (isServer)
        {
            room.GetComponent<RoomEvents>().roomIndex = i;
            GuardAI[] enemies = room.GetComponentsInChildren<GuardAI>();
            foreach (GuardAI ai in enemies)
            {
                ai.roomIndex = i;
                NetworkServer.Spawn(ai.gameObject);
            }
        }
    }

    [ClientRpc]
    public void RpcGenerateStandardPuzzleRooms(int i, int roomPrefabIndex, int puzzlePrefabIndex, float distance)
    {
        
        //Vector3.forward * GenerateRooms.instance.roomWidth * i new Vector3(16.708f,0,-38.0785f))
        GameObject room = Instantiate(GenerateRooms.instance.puzzleRooms[roomPrefabIndex], Vector3.forward * (distance - GenerateRooms.instance.roomWidth), GenerateRooms.instance.puzzleRooms[roomPrefabIndex].transform.rotation);
        GameObject puzzle = Instantiate(GenerateRooms.instance.puzzles[puzzlePrefabIndex], Vector3.forward * (distance - GenerateRooms.instance.roomWidth), GenerateRooms.instance.puzzles[puzzlePrefabIndex].transform.rotation);
        room.GetComponentInChildren<OpenPuzzle>().puzzleCanvas = puzzle;
        if (isServer)
        {
            room.GetComponent<RoomEvents>().roomIndex = i;
            GuardAI[] enemies = room.GetComponentsInChildren<GuardAI>();
            foreach (GuardAI ai in enemies)
            {
                ai.roomIndex = i;
                NetworkServer.Spawn(ai.gameObject);
            }
        }
    }

    public void LoadingScreen()
    {
        CmdLoadingScreen();
    }

    [Command(requiresAuthority = false)]
    public void CmdLoadingScreen()
    {
        RpcLoadingScreen();
    }

    [ClientRpc]
    public void RpcLoadingScreen()
    {
        if (GameObject.FindGameObjectWithTag("Loading") != null)
        {
            GameObject.FindGameObjectWithTag("Loading").GetComponent<Canvas>().enabled = true;
        }
    }


    [ClientRpc]
    public void RpcTreasureRoomLazy(int i, float distance)
    {
        if (GameObject.FindGameObjectWithTag("Loading") != null)
        {
            GameObject.FindGameObjectWithTag("Loading").SetActive(false);
        }
        GameObject treasureRoomInstance = Instantiate(GenerateRooms.instance.treasureRoom, Vector3.forward * (distance - GenerateRooms.instance.roomWidth), GenerateRooms.instance.treasureRoom.transform.rotation);
        treasureRoomInstance.GetComponent<RoomEvents>().roomIndex = i;
    }

    public void TreasureAhoy(int i, float distance)
    {
        CmdGenerateTreasure(i, distance);
    }

    public void GenerateRoomsLazy(int i, int roomPrefabIndex, int puzzlePrefabIndex, RoomType type, float distance)
    {
        CmdGenerateRooms(i, roomPrefabIndex, puzzlePrefabIndex, type, distance);
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
        puzzle = 0;
        grav = 0;
        float distance = 0;
        for (i = 0; i < randomRooms; i++)
        {
            RoomType[] types = { RoomType.TIMED_ESCAPE_GRAVITY, RoomType.STANDARD };
            RoomType type = types[Random.Range(0, types.Length)];
            GenerateRoomsLazy(i, Random.Range(0, GenerateRooms.instance.puzzleRooms.Length), Random.Range(0, GenerateRooms.instance.puzzles.Length), type, distance);
            if (type == RoomType.TIMED_ESCAPE_GRAVITY)
            {
                grav++;
                distance += GenerateRooms.instance.gravityRoom.GetComponent<RoomProcGen>().roomWidth + 2.5f;

            }
            if (type == RoomType.STANDARD)
            {
                puzzle++;
                distance += GenerateRooms.instance.roomWidth + 2.5f;
            }


        }
        TreasureAhoy(i, distance);
    }


    public void UnlockDoor()
    {
        CmdUnlockDoor();
    }
    [Command(requiresAuthority = false)]
    public void CmdUnlockDoor()
    {
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.roomIndex++;
        }
    }
    [ClientRpc]
    public void RpcUnlockDoor()
    {
        shouldOpen = true;
        puzzleDoor.enabled = false;
    }

    [Command(requiresAuthority = false)]
    public void CmdVictory()
    {
        RpcVictory();
    }

    [ClientRpc]
    public void RpcVictory()
    {
        victoryScreen.SetActive(true);
    }
}
public enum RoomType
{
    STANDARD, STANDARD_BALL_ESCAPE, TREASURE, STANDARD_GRAVITY_PASSIVE, STANDARD_GRAVITY_AGGRESSIVE, TIMED_ESCAPE_GRAVITY
}
