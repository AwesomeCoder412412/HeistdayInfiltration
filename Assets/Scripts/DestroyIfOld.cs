using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class DestroyIfOld : NetworkBehaviour
{
    private float initializationTime;
    // Start is called before the first frame update
    void Start()
    {
        initializationTime = Time.timeSinceLevelLoad;
    }

    // Update is called once per frame
    void Update()
    {
        Debug.Log("is server test " + isServer);
        float timeSinceInitialization = Time.timeSinceLevelLoad - initializationTime;
        if (timeSinceInitialization > 7)
        {
            NetworkServer.Destroy(gameObject);
        }
    }
}
