using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Resolution : MonoBehaviour
{
    public InputField xInput;
    public InputField yInput;
    public Toggle fullscreen;
    // Start is called before the first frame update
    void Start()
    {
        xInput.text = "" + Screen.width;
        yInput.text = "" + Screen.height;
        fullscreen.isOn = Screen.fullScreen;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void OnEndEdit()
    {
        Screen.SetResolution(int.Parse(xInput.text), int.Parse(yInput.text), (fullscreen.isOn)?FullScreenMode.FullScreenWindow:FullScreenMode.Windowed);
        Debug.Log("OnEndEdit");
        
    }
}
