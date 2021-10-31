using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAssignment : MonoBehaviour
{
    public GameObject room;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnTriggerEnter(Collider collision)
    {
        if (collision.gameObject.tag == "Enemy")
        {
            Debug.Log("yes2");
            collision.gameObject.GetComponent<GuardAI>().roomIndex = room.GetComponent<RoomEvents>().roomIndex;
        }
    }
}
