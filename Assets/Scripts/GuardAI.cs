using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GuardAI : MonoBehaviour
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
    public GameObject player;
    public float playerRadius = 5f;
    public float walkingOffset = 55f;
    public int roomIndex;
    public bool walking = true;
    private LookAtPlayer lookAtPlayer;
    public float distanceInFrontOfPlayer = 50f;
    //tell my story
    // Start is called before the first frame update
    void Start()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        cc = GetComponent<CharacterController>();
        lookAtPlayer = GetComponent<LookAtPlayer>();
        anim = GetComponent<Animator>();
        startPos = transform.position;
        defaultRotation = transform.eulerAngles.y;
        if (isGuard)
        {
            anim.SetBool("Walking", true);
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
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
           
            if (Vector3.Distance(TankPlayerController.currentPlayer.transform.position + TankPlayerController.currentPlayer.transform.forward * distanceInFrontOfPlayer, transform.position) > playerRadius)
            {
                //Debug.Log("Walking");
                cc.Move(Vector3.Normalize(TankPlayerController.currentPlayer.transform.position + TankPlayerController.currentPlayer.transform.forward * distanceInFrontOfPlayer - transform.position) * speed);
                //transform.LookAt(player.transform);
                //transform.eulerAngles = new Vector3(0, transform.rotation.eulerAngles.y + walkingOffset, 0);
                transform.rotation = Quaternion.LookRotation(TankPlayerController.currentPlayer.transform.position + TankPlayerController.currentPlayer.transform.forward * distanceInFrontOfPlayer - transform.position, Vector3.up);
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
                        if (enemy.GetComponent<GuardAI>() != null && enemy.GetComponent<GuardAI>().roomIndex == PlayerController.instance.roomIndex)
                        {
                            enemiesList.Add(enemy);
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
