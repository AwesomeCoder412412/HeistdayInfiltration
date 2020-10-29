using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LookAtPlayer : MonoBehaviour
{
    public Transform target;
    private GuardAI guardAI;
    public float rotOffset = 60f;
    // Start is called before the first frame update
    void Start()
    {
        guardAI = GetComponent<GuardAI>();
        if (guardAI.isGuard)
        {
            target = TankPlayerController.currentPlayer.transform;
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (guardAI.isGuard)
        {
            target = TankPlayerController.currentPlayer.transform;
            if (target != null && guardAI.patrolling == false)
            {
               // target = TankPlayerController.currentPlayer.transform;
                transform.LookAt(target);
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y + rotOffset, 0);
            }
        }
        else
        {
            if (target != null)
            {
                transform.LookAt(target);
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y + rotOffset, 0);
            }
        }
    }
}

