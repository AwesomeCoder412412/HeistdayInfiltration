using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Treasure : NetworkBehaviour
{
    public GameObject[] treasures;
    public Transform treasurePosition;
    private GameObject treasure;
    public static Treasure instance;
    public bool treasureSpawned;
    private void Start()
    {
        treasureSpawned = false;
        instance = this;
        //TODO: this is horrible, normal isServer should be here but it doesnt work how we think it does?

        if (MirrorVariables.instance.isServer)
        {
            if (!treasureSpawned)
            {
                SpawnTreasure();
            }
        }

    }
    private void Update()
    {
        if (MirrorVariables.instance.isServer)
        {
            if (!treasureSpawned)
            {
                SpawnTreasure();
            }
        }
    }
    public void SpawnTreasure()
    {
        GameObject treasurePrefab = treasures[Random.Range(0, treasures.Length)];
        treasure = Instantiate(treasurePrefab, treasurePosition.position, treasurePosition.rotation);
        NetworkServer.Spawn(treasure);
        treasureSpawned = true;
    }
    public void DestroyTreasure()
    {
        if (treasure)
        {
            NetworkServer.Destroy(treasure);
        }
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            NetworkServer.Destroy(treasure);
            ScoreManager.score += 100;
            LadderScript.instance.hasTreasure = true;
        }
    }
}
