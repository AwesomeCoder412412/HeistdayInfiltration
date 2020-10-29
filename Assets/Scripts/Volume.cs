using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Volume : MonoBehaviour
{
    private Toggle toggle;
    private string audioOn = "audio on";
    // Start is called before the first frame update
    void Start()
    {
        toggle = GetComponent<Toggle>();
        PlayerPrefs.SetInt(audioOn, 0);
    }
    public void ToggleAudio()
    {
        int toggleValue = (toggle.isOn) ? 0 : 1;
        PlayerPrefs.SetInt(audioOn, toggleValue);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
