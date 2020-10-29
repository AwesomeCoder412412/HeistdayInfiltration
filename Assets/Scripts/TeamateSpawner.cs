using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TeamateSpawner : MonoBehaviour
{
    public int teammates;
    private string teammate = "teammates";
    public GameObject teammatePrefab;
    public Transform spawnLocation;
    // Start is called before the first frame update
    void Start()
    {
        teammates = PlayerPrefs.GetInt(teammate);
        Debug.Log(teammates);
        for (int i = 0; i < teammates; i++)
        {
            Instantiate(teammatePrefab, spawnLocation.position, spawnLocation.rotation);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
