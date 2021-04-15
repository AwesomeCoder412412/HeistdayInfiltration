using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TAAAAAAANK : MonoBehaviour
{
   
    public Toggle tankToggle;
    public static bool tankOrNoTank;
    public GameObject tankBarrier;
    // Start is called before the first frame update
    void Start()
    {
     GameObject tank = GameObject.FindGameObjectWithTag("Tank");
     if (tankOrNoTank)
        {
            tankBarrier.SetActive(false);
            
        }
     else
        {
            tank.SetActive(false);
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public void GimmeeTank()
    {
        tankOrNoTank = tankToggle.isOn;
    }
}
