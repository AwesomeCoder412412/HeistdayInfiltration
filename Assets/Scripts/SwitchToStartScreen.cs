using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class SwitchToStartScreen : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        SceneManager.LoadScene("StartScreen");
    }

    // Update is called once per frame
    void Update()
    {
        SceneManager.LoadScene("StartScreen");
    }
}
