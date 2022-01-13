using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class BallCreator : NetworkBehaviour
{
    public GameObject ball;
    public GameObject[] ballPos;
    // Start is called before the first frame update
    void Start()
    {
        
    }
    public void spawnBalls()
    {
        int j = Random.Range(0, ballPos.Length - 1);
        for (int i = 0; i < ballPos.Length; i++)
        {
            GameObject ballInstant = Instantiate(ball, ballPos[i].transform.position, Quaternion.identity);
            Debug.Log("spawned ball " + ballInstant.name);
            if (i == j)
            {
                ballInstant.GetComponent<Renderer>().material.SetColor("_Color", Color.red);
                ballInstant.tag = "Fridge";
                ballInstant.GetComponent<BoingBoing>().addedVelocity = .25f;
                ballInstant.GetComponent<Rigidbody>().mass *= 10;

            }
            NetworkServer.Spawn(ballInstant);
        }
    }


    [ClientRpc]
    public void RpcDebug(string str)
    {
        Debug.Log(str);
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
