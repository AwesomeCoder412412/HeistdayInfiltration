using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Treasure : MonoBehaviour
{
    public GameObject[] treasures;
    public Transform treasurePosition;
    private GameObject treasure;
    private void Start()
    {
        GameObject treasurePrefab = treasures[Random.Range(0, treasures.Length)];
       treasure = Instantiate(treasurePrefab, treasurePosition.position, treasurePosition.rotation);
    }
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Destroy(treasure);
            LadderScript.instance.hasTreasure = true;
        }
    }
}
