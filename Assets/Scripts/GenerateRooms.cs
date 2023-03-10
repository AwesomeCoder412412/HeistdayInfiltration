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
    public GameObject gravityRoom;
    public Vector3 startPos;

    public float roomWidth;

    private void Start()
    {
        instance = this;
        if (!isServer)
        {
            return;
        }

        minRooms = PlayerPrefs.GetInt(minRoom);
        maxRooms = PlayerPrefs.GetInt(maxRoom);
        int randomRooms = Random.Range(minRooms, maxRooms);
        int i = 0;
        if (!storage.activeInHierarchy)
        {
            storage.SetActive(true);
        }
    }


}
