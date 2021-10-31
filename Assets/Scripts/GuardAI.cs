using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using FPSControllerLPFP;
using Mirror;

public class GuardAI : NetworkBehaviour
{
    private CharacterController cc;
    private float downV = 0f;
    public float gravity = 10;
    public Transform forwardPos;
    private Animator anim;
    private Vector3 startPos;
    private int dir = 1;
    public bool patrolling = true;
    private float lerpValue = 0f;
    public float speed = 1f;
    private float defaultRotation;
    public bool isGuard = true;
    [SyncVar]
    public GameObject player;
    public float playerRadius = 5f;
    //public float tankRadius = 10f;
    public float walkingOffset = 55f;
    public int roomIndex;
    public bool walking = true;
    private LookAtPlayer lookAtPlayer;
    public float distanceInFrontOfPlayer = 50f;
    public float distanceInFrontOfTank = 50f;
    public int playerId;
    private TankPlayerController tank;
    public GameObject currentPlayer;
    public float footLoose = 6;
    //tell my story
    // Start is called before the first frame update
    void Start()
    {
        //player = GameObject.FindGameObjectWithTag("Player");
        tank = FindObjectOfType<TankPlayerController>();
        cc = GetComponent<CharacterController>();
        lookAtPlayer = GetComponent<LookAtPlayer>();
        anim = GetComponent<Animator>();
        startPos = transform.position;
        defaultRotation = transform.eulerAngles.y;
        if (isServer)
        {
            if (isGuard)
            {
                anim.SetBool("Walking", true);
            }
        }
    }

    public void SetPlayerId(int playerId)
    {
        this.playerId = playerId;
        foreach (FpsControllerLPFP player1 in FindObjectsOfType<FpsControllerLPFP>())
        {
            Debug.Log("teammate " + player1.playerId + " " + playerId);
            if(player1.playerId == playerId)
            {
                player = player1.gameObject;
                Debug.Log("setplayer " + player.gameObject.name);
            }
        }
    }

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.tag == "Floor")
        {
            Debug.Log("yes2");

            roomIndex = other.gameObject.GetComponent<GuardAssignment>().room.GetComponent<RoomEvents>().roomIndex;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (!isServer)
        {
            return;
        }
        if (isGuard)
        {
            if (collision.transform.tag == "Tank")
            {
                transform.localScale = new Vector3(transform.localScale.x, 0.01f, transform.localScale.z);
            }
            Debug.Log(collision.gameObject.name);
            if (collision.transform.tag == "SafeBullet")
            {
                Debug.Log("Collided");
                Destroy(collision.transform.gameObject);
                Destroy(gameObject);
                ScoreManager.score++;
            }
            if (collision.gameObject.tag == "Player" && collision.gameObject.GetComponentInChildren<AutomaticGunScriptLPFP>().isKnifing) 
            {
                Destroy(gameObject);
                ScoreManager.score++;
            }
            if (collision.gameObject.tag == "Floor")
            {
                roomIndex = collision.gameObject.GetComponent<GuardAssignment>().room.GetComponent<RoomEvents>().roomIndex;
                Debug.Log("yes2");
            }

        }
        else
        {
            if (collision.transform.tag == "Bullet")
            {
                Debug.Log("Collided");
                Destroy(collision.transform.gameObject);
                Destroy(gameObject);
            }
        }
    }
    // Update is called once per frame
    void Update()
    {
        if (gameObject.name == "Teammate(Clone)")
        {
            Debug.Log("isguard " + isGuard);
            Debug.Log("cc " + (cc == null));
        }
        if (!isServer)
        {
            Debug.Log("I'm not the server");
            return;
        }
        if (cc.isGrounded)
        {
            downV = 0;
        }
        else
        {
            downV -= gravity;
            cc.Move(Vector3.up * downV);
        }
        if (isGuard)
        {
            if (patrolling == true)
            {
                transform.position = Vector3.Lerp(startPos, forwardPos.position, lerpValue);
                if (lerpValue >= 1)
                {
                    dir = -1;
                    transform.rotation = Quaternion.Euler(0, defaultRotation + 180, 0);
                }
                else if (lerpValue <= 0)
                {
                    dir = 1;
                    transform.rotation = Quaternion.Euler(0, defaultRotation, 0);
                }
                lerpValue += Time.deltaTime * dir * speed;
            }
        }
        else
        {
            Debug.Log("player1 " + (player == null));
            Debug.Log("playerId " + playerId);
            //GameObject currentPlayer = null;
            if (player.activeSelf)
            {
                currentPlayer = player;
            }
            else if(tank.playerId == this.playerId)
            {
                currentPlayer = tank.gameObject;
            }
            /*Debug.Log("player " + player.gameObject.name);
            Debug.Log("TankId " + tank.playerId);
            Debug.Log("playerId " + this.playerId);
            Debug.Log("currentplayer " + currentPlayer.gameObject.name);*/
            //float radius = (currentPlayer == player) ? playerRadius : tankRadius
            float distanceInFront = (currentPlayer == player) ? distanceInFrontOfPlayer : distanceInFrontOfTank;
            float currentRadius = Vector3.Distance(currentPlayer.transform.position + currentPlayer.transform.forward * distanceInFront, transform.position);
            if (currentPlayer != null && ((currentRadius > playerRadius && walking) || (currentRadius > footLoose && !walking)))
            {
                //Debug.Log("Walking");
                cc.Move(Vector3.Normalize(currentPlayer.transform.position + currentPlayer.transform.forward * distanceInFront - transform.position) * speed);
                //transform.LookAt(player.transform);
                //transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y + walkingOffset, 0);
                transform.rotation = Quaternion.LookRotation(currentPlayer.transform.position + currentPlayer.transform.forward * distanceInFront - transform.position, Vector3.up);
                transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y, 0);
                anim.SetBool("Walking", true);
                walking = true;
                lookAtPlayer.target = null;
            }
            else
            {
                //Debug.Log("Shooting");
                anim.SetBool("Walking", false);
                if (walking)
                {
                    GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
                    List<GameObject> enemiesList = new List<GameObject>();
                    foreach (GameObject enemy in enemies)
                    {
                        if (enemy.GetComponent<GuardAI>() == null)
                        {
                            Debug.Log(enemy.transform.parent.gameObject.name);
                        }
                        //Debug.Log(enemy.GetComponent<GuardAI>() != null);
                       // Debug.Log(PlayerController.instance != null);
                       foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
                        {
                            if (pc.isServer)
                            {
                                if (enemy.GetComponent<GuardAI>() != null && enemy.GetComponent<GuardAI>().roomIndex == pc.roomIndex)
                                {
                                    enemiesList.Add(enemy);
                                }
                            }
                        }
                        
                    }
                    if (enemiesList.Count > 0)
                    {
                        GameObject randomEnemy = enemiesList[Random.Range(0, enemiesList.Count)];
                        lookAtPlayer.target = randomEnemy.transform;
                    }
                    
                }
                walking = false;
            }
        }
    }
    public void SetPatrolling(bool patrolling)
    {
        this.patrolling = patrolling;
        anim.SetBool("Walking", patrolling);
    }
}
