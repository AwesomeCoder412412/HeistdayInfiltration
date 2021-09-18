using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class GenerateRooms : NetworkBehaviour
{
    private string minRoom = "minRoom";
    private string maxRoom = "maxRoom";
    public GameObject[] puzzleRooms;
    public GameObject[] puzzles;
    public GameObject treasureRoom;
    public int minRooms = 5;
    public int maxRooms = 10;
    public static GenerateRooms instance;
    public GameObject storage;

    public float roomWidth;

    [ClientRpc]
    public void RpcTest()
    {
        Debug.Log("testrpc works");
    }

    /*[ClientRpc]
    public void RpcSpawnRoomAndPuzzle(int i, int roomPrefabIndex, int puzzlePrefabIndex)
    {
        Debug.Log("spawn room " + i);
        GameObject room = Instantiate(puzzleRooms[roomPrefabIndex], Vector3.forward * roomWidth * i, puzzleRooms[roomPrefabIndex].transform.rotation);
        GameObject puzzle = Instantiate(puzzles[puzzlePrefabIndex], Vector3.forward * roomWidth * i, puzzles[puzzlePrefabIndex].transform.rotation);
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
    public void RpcTreasureRoom(int i)
    {
        GameObject treasureRoomInstance = Instantiate(treasureRoom, Vector3.forward * roomWidth * i, treasureRoom.transform.rotation);
    }*/

    private void Start()
    {
        instance = this;
        if (!isServer)
        {
            return;
        }

        Debug.Log("Calling Start");
        minRooms = PlayerPrefs.GetInt(minRoom);
        maxRooms = PlayerPrefs.GetInt(maxRoom);
        int randomRooms = Random.Range(minRooms, maxRooms);
        int i = 0;
        Debug.Log(randomRooms + " random rooms");
        RpcTest();
        if (!storage.activeInHierarchy)
        {
            Debug.Log("inactive");
            storage.SetActive(true);
        }
        for (i = 0; i < randomRooms; i++)
        {
            Debug.Log("I'm in the loop " + i);
            //MirrorVariables.instance.GenerateRoomsLazy(i, Random.Range(0, puzzleRooms.Length), Random.Range(0, puzzles.Length));
        }
        //RpcTreasureRoom(i);
        //MirrorVariables.instance.c = false;
    }


}
