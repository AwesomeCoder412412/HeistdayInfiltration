using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using UnityEngine.UI;

public class JoinServer : MonoBehaviour
{
    public InputField address;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void ButtonPressed()
    {
        NetworkManager.singleton.networkAddress = address.text;
        NetworkManager.singleton.StartClient();
    }
}
