using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class TeamateSpawner : MonoBehaviour
{
    public int teammates;
    public static TeamateSpawner instance;
    private string teammate = "teammates";
    public GameObject teammatePrefab;
    public Transform spawnLocation;
    // Start is called before the first frame update
    public void SpawnTeamates(int playerId)
    {
        teammates = PlayerPrefs.GetInt(teammate);
        Debug.Log(teammates);
        for (int i = 0; i < teammates; i++)
        {
            //Debug.Log("teammate45");
            GameObject teammateInstance = Instantiate(teammatePrefab, spawnLocation.position, spawnLocation.rotation);
            NetworkServer.Spawn(teammateInstance);
            teammateInstance.GetComponent<GuardAI>().SetPlayerId(playerId);
        }
    }

    public void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already an instance of the TeamateSpawner class!");
        }
    }

    // Update is called once per frame
    public void Update()
    {
        
    }
}
