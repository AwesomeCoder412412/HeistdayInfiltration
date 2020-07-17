using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GenerateRooms : MonoBehaviour
{
    public GameObject[] puzzleRooms;
    public GameObject[] puzzles;
    public GameObject treasureRoom;
    public int minRooms = 5;
    public int maxRooms = 10;
    public float roomWidth;


    private void Start()
    {
        int randomRooms = Random.Range(minRooms, maxRooms);
        int i = 0;
        for (i = 0; i < randomRooms; i++)
        {
            GameObject roomPrefab = puzzleRooms[Random.Range(0, puzzleRooms.Length)];
            GameObject puzzlePrefab = puzzles[Random.Range(0, puzzles.Length)];
            GameObject room = Instantiate(roomPrefab, Vector3.forward * roomWidth * i, roomPrefab.transform.rotation);
            GameObject puzzle = Instantiate(puzzlePrefab, Vector3.forward * roomWidth * i, puzzlePrefab.transform.rotation);
            room.GetComponentInChildren<OpenPuzzle>().puzzleCanvas = puzzle;
        }
        GameObject treasureRoomInstance = Instantiate(treasureRoom, Vector3.forward * roomWidth * i, treasureRoom.transform.rotation);
    }


}
