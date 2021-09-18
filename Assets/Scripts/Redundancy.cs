using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class Redundancy : NetworkBehaviour
{
    [SyncVar]
    public bool tankExists;
    public GameObject tank;
    // Start is called before the first frame update
    void Start()
    {
        if (isServer)
        {
            tankExists = PlayerPrefs.GetInt("tanktoggle") == 1; 
        }
        tank.SetActive(tankExists);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
