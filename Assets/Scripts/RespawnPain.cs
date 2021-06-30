using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RespawnPain : MonoBehaviour
{
    public static RespawnPain instance;
    public bool counting = false;
    private float time = 0;
    
    // Start is called before the first frame update
    void Start()
    {
        instance = this;
        DontDestroyOnLoad(gameObject);
    }

    // Update is called once per frame
    void Update()
    {

        if (counting)
        {
            if (time > 1)
            {
                NetworkManager.singleton.StartClient();
                Debug.Log(NetworkManager.singleton.networkAddress + "gummies3");
            }
            else
            {
                time += Time.unscaledDeltaTime;
            }
        }
        Debug.Log("RespawnPain is still here");
    }
    public void RespawnClient()
    {
        Debug.Log("staying alive");
        string ip = NetworkManager.singleton.networkAddress;
        NetworkManager.singleton.StopClient();
        Debug.Log(ip + "gummies");
        NetworkManager.singleton.networkAddress = ip;
        Debug.Log(NetworkManager.singleton.networkAddress + "gummies2");
        Debug.Log("gummmies2.5");
        counting = true;
    }
}
