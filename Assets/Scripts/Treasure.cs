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
    private void Start()
    {
        instance = this;
        if (isServer)
        {
            GameObject treasurePrefab = treasures[Random.Range(0, treasures.Length)];
            treasure = Instantiate(treasurePrefab, treasurePosition.position, treasurePosition.rotation);
            NetworkServer.Spawn(treasure);
        }

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
