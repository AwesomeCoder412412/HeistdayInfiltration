using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardCreator : MonoBehaviour
{
    public int guards;
    private string guard = "guards";
    public GameObject[] guardArray;
    // Start is called before the first frame update
    void Start()
    {
        guards = PlayerPrefs.GetInt(guard);
        if (guards > guardArray.Length)
        {
            guards = guardArray.Length;
        }
        int guardsLeft = guards;
        for (int i = 0; i < guardArray.Length; i++)
        {
            if (guardsLeft > 0)
            {
                guardArray[i].SetActive(true);
            }
            guardsLeft--;
        }
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
