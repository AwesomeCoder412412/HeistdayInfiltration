using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;

public class RoomTrigger : MonoBehaviour
{
    private bool opened = false;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnTriggerEnter(Collider other)
    {
        if (!opened)
        {
            if (other.gameObject.CompareTag("Player") && other.gameObject.GetComponent<FpsControllerLPFP>().isLocalPlayer || other.gameObject.CompareTag("Tank") && other.gameObject.GetComponent<FpsControllerLPFP>().isLocalPlayer)
            {
                other.gameObject.GetComponent<PlayerController>().CmdIncrementRoomIndex();
                opened = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        //Debug.Log(gameObject.name + " hi");
    }
}
