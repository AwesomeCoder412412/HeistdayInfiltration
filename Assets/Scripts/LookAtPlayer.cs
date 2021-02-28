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
    }
    // Update is called once per frame
    void Update()
    {
        if (guardAI.isGuard)
        {
            target = FindTargets().transform;
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
    public GameObject FindTargets()
    {
        float distance = Mathf.Infinity;
        GameObject tempPlayer = null;
        foreach (GameObject player in GameObject.FindGameObjectsWithTag("Player"))
        {
            if (tempPlayer == null || Vector3.Distance(transform.position, player.transform.position) < distance)
            {
                tempPlayer = player;
                distance = Vector3.Distance(transform.position, player.transform.position);
            }
        }
        return tempPlayer;
    }
}

