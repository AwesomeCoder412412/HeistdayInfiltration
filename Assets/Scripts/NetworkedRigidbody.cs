using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class NetworkedRigidbody : NetworkBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        Rigidbody rigidbody = GetComponent<Rigidbody>();
        if (isServer)
        {
            rigidbody.isKinematic = false;
        }
        else
        {
            rigidbody.isKinematic = true;
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
