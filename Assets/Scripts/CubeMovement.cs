using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class CubeMovement : NetworkBehaviour
{
    private int speed = 10;
 
    public void Update()
    {
        if (!isLocalPlayer)
        {
            return;
        }
        CmdMove();
    }
    [Command]
    public void CmdMove()
    {

        Vector3 Movement = new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical"));

        transform.position += Movement * speed * Time.deltaTime;

    }
}
