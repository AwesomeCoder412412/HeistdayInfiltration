using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
            if (other.gameObject.CompareTag("Player") || other.gameObject.CompareTag("Tank"))
            {
                PlayerController.instance.roomIndex++;
                opened = true;
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
