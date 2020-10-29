using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class PlayButton : MonoBehaviour
{
    public InputField minRooms;
    public InputField maxRooms;
    public InputField guard;
    public InputField teammate;
    public string minRoom = "minRoom";
    public string maxRoom = "maxRoom";
    public string guards = "guards";
    public string teammates = "teammates";
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
        SceneManager.LoadScene("MainGame");
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
