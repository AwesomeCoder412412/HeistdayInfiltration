using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SetAudio : MonoBehaviour
{

    private AudioListener audio;
    private string audioOn = "audio on";
    // Start is called before the first frame update
    void Start()
    {
        audio = GetComponent<AudioListener>();
        AudioListener.pause = PlayerPrefs.GetInt(audioOn) != 0;
    }

    // Update is called once per frame
    void Update()
    {
        AudioListener.pause = PlayerPrefs.GetInt(audioOn) != 0;
    }
}
