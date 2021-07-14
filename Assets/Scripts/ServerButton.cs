using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class ServerButton : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void Useless()
    {
        //PlayButton.instance.GetServers();
        FirebaseVariables.instance.refresh = true;
    }

    public void ButtonPressed()
    {
        NetworkManager.singleton.networkAddress = gameObject.GetComponentInChildren<Text>().text;
        NetworkManager.singleton.StartClient();
    }
}
