using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SpawnTank : MonoBehaviour
{
    public Toggle toggle;
    // Start is called before the first frame update
    void Start()
    {
        PlayerPrefs.SetInt("tanktoggle", 0);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnToggle()
    {
        PlayerPrefs.SetInt("tanktoggle", toggle.isOn? 1 : 0);

    }
}
