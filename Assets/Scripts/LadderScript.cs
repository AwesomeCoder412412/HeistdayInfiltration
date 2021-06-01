using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LadderScript : MonoBehaviour
{
    public static LadderScript instance;
    public GameObject roof;
    public bool hasTreasure = false;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already a instance of the LadderScript script");
        }
        MirrorVariables.instance.c = true;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
