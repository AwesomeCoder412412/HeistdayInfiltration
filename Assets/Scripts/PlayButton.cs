using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using Mirror;

public class PlayButton : MonoBehaviour
{
    public InputField minRooms;
    public InputField maxRooms;
    public InputField guard;
    public InputField teammate;
    public InputField players;
    public string minRoom = "minRoom";
    public string maxRoom = "maxRoom";
    public string guards = "guards";
    public string teammates = "teammates";
    public string players1 = "players";
    // Start is called before the first frame update
    void Start()
    {

    }
    public void Play()
    {
        PlayerPrefs.SetInt(minRoom, int.Parse(minRooms.text));
        PlayerPrefs.SetInt(maxRoom, int.Parse(maxRooms.text));
        PlayerPrefs.SetInt(guards, int.Parse(guard.text));
        PlayerPrefs.SetInt(teammates, int.Parse(teammate.text));
        PlayerPrefs.SetInt(players1, int.Parse(players.text));
        NetworkManager.singleton.maxConnections = int.Parse(players.text);
        NetworkManager.singleton.StartHost();
    }
    public void Quit()
    {
        Application.Quit();
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
