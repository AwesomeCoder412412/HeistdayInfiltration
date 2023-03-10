using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;
using FPSControllerLPFP;

public class PlayerController : NetworkBehaviour
{
    [SyncVar]
    public bool isDead;
    public bool ladderTouched = false;
    public float ladderSpeed = 0.7f;
    public bool loading = false;
    [SyncVar]
    public int roomIndex = 0;
    public GameObject knife;
    public GameObject bullet;
    public int retry;
    public string retry1 = "retry1";
    public bool isPaused;
    public bool hasStarted = false;
    public Vector3 playerGrav;
    private void Start()
    {
        playerGrav = Physics.gravity;
        Time.timeScale = 1.0f;
        inputManager = GameObject.FindObjectOfType<InputManager>();
        //Physics.IgnoreCollision(GetComponent<Collider>(), knife.GetComponent<Collider>());
        //TODO: IF BULLET IS BREAKING IN ANY WAY UNCOMMENT THE LINE BELOW and only the 1 line directly below this comment
        //Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>());
        //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), knife.GetComponent<Collider>());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        PausedMenu.instance.pauseMenu.SetActive(false);
    }
    InputManager inputManager;
    private void OnTriggerEnter(Collider other)
    {
        if (!isLocalPlayer)
        {
            return;
        }
        if (other.gameObject.CompareTag("Laser"))
        {
            HealthManager.instance.RemoveHeart();
            if (isServer)
            {
               // RpcDebug("host hit by laser");
            }
            else
            {
               // Debug.Log("client hit by laser");
            }
        }
        if (other.gameObject.CompareTag("Wall") && playerGrav != new Vector3(0, -9.81f, 0))
        {
            HealthManager.instance.RemoveHeart();
            foreach (RoomEvents re in GameObject.FindObjectsOfType<RoomEvents>())
            {
                if (re.roomIndex == roomIndex)
                {
                    gameObject.transform.position = re.start.transform.position;
                    playerGrav = new Vector3(0, -9.81f, 0);
                }
            }
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            HealthManager.instance.RemoveHeart();
            Destroy(gameObject);
            if (isServer)
            {
                //RpcDebug("host hit by bullet");
            }
            else
            {
               // Debug.Log("client hit by bullet");
            }
        }
        if (other.gameObject.CompareTag("Ladder"))
        {
            ladderTouched = true;
            LadderScript.instance.roof.GetComponent<BoxCollider>().enabled = false;
        }
        if (other.gameObject.CompareTag("Helicopter"))
        {
            if (LadderScript.instance.hasTreasure == true)
            {
                if (PlayerPrefs.GetInt("teammates") == 0)
                {
                    PlayerPrefs.SetInt("Achievement" + AchievementType.GoingSolo, 1);
                }
                if (HealthManager.instance.hearts.Count == 1)
                {
                    PlayerPrefs.SetInt("Achievement" + AchievementType.LivingOnTheEdge, 1);
                }
                if (PlayerPrefs.GetInt("teammates") == 0 && PlayerPrefs.GetInt("guards") >= 5 && PlayerPrefs.GetInt("minRoom") >= 5)
                {
                    PlayerPrefs.SetInt("Achievement" + AchievementType.ArnoldSchwarzenegger, 1);
                }
                ScoreManager.score += 200;
                Cursor.lockState = CursorLockMode.None;
                MirrorVariables.instance.CmdVictory();
                //SceneManager.LoadScene("VictoryScreen");
            }
        }
    }
    public IEnumerator TeleText()
    {
        GameObject.FindGameObjectWithTag("Teletext").GetComponent<Canvas>().enabled = true;
        yield return new WaitForSeconds(3);
        GameObject.FindGameObjectWithTag("Teletext").GetComponent<Canvas>().enabled = false;
    }
    private void OnCollisionEnter(Collision collision)
    {
//        Debug.Log("colliding with " + collision.gameObject.name);
        if (!isLocalPlayer)
        {
            return;
        }
        if (collision.gameObject.CompareTag("Fridge"))
        {
            foreach (RoomEvents re in GameObject.FindObjectsOfType<RoomEvents>())
            {
                if (re.roomIndex == roomIndex + 1)
                {
                    gameObject.transform.position = re.start.transform.position;
                    playerGrav = new Vector3(0, -9.81f, 0);
                    CmdIncrementRoomIndex();
                    StartCoroutine(TeleText());
                }
            }
        }
        if (collision.gameObject.CompareTag("Ball"))
        {
            Vector3 velocity = collision.gameObject.GetComponent<Rigidbody>().velocity;
            transform.position += (velocity.normalized * velocity.magnitude) / 100;
        }
        if (collision.gameObject.CompareTag("Wall") && playerGrav != new Vector3(0, -9.81f, 0))
        {
            HealthManager.instance.RemoveHeart();
            foreach (RoomEvents re in GameObject.FindObjectsOfType<RoomEvents>())
            {
                if (re.roomIndex == roomIndex)
                {
                    gameObject.transform.position = re.start.transform.position;
                    playerGrav = new Vector3(0, -9.81f, 0);
                }
            }
        }
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HealthManager.instance.RemoveHeart();
            if (isServer)
            {
                //RpcDebug("host hit by bullet");
            }
            else
            {
                //Debug.Log("client hit by bullet");
            }
        }
    }
    public void RemoveHearts(int hearts)
    {
        for (int i = 0; i < hearts; i++) {
            HealthManager.instance.RemoveHeart();
        }
    }

    public void EnforceGravity()
    {
        gameObject.GetComponent<Rigidbody>().useGravity = false;
        gameObject.GetComponent<Rigidbody>().AddForce(playerGrav * (gameObject.GetComponent<Rigidbody>().mass));
        Debug.Log("player controller grav " + playerGrav + "object " + gameObject.name + "applied force " + playerGrav * (gameObject.GetComponent<Rigidbody>().mass));
    }

    [Command(requiresAuthority = false)]
    public void CmdIncrementRoomIndex()
    {
        roomIndex++;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            ladderTouched = false;
            transform.position += new Vector3(0f, ladderSpeed, 0f);
            LadderScript.instance.roof.GetComponent<BoxCollider>().enabled = true;
        }
    }
    public IEnumerator HoldUp()
    {
        yield return new WaitForSeconds(10);
    }

    public void Loading()
    {
        MirrorVariables.instance.RoomsGo();
        HealthManager.instance.RestoreHearts(0, true);
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.gameObject.transform.position = GameObject.FindGameObjectWithTag("Start").transform.position;
        }
        int max = 0;
        foreach (FpsControllerLPFP fPS in FindObjectsOfType<FpsControllerLPFP>())
        {
            if (fPS.playerId > max)
            {
                max = fPS.playerId;
            }
        }
        //            Debug.Log(max + " playeridmax");
        for (int i = 1; i <= max; i++)
        {
            TeamateSpawner.instance.SpawnTeamates(i);
            //                Debug.Log("spawnedteammates " + i);
        }
        //FindObjectOfType<TankPlayerController>().GetComponent<NetworkIdentity>().AssignClientAuthority(connectionToClient);
        //Debug.Log("spawnteammates");
        //TeamateSpawner.instance.SpawnTeamates(playerId);

        hasStarted = true;
        loading = false;
    }
    public void FixedUpdate()
    {
        EnforceGravity();
        if (!isServer)
        {
            return;
        }
        //TODO: if i comment out everything from this to line 242 it fixes the delay on client(not sensitiviy)
        Vector3 curRot = gameObject.transform.rotation.eulerAngles;
        Vector3 targetRot = Vector3.zero;
        if (playerGrav.x != 0)
        {
            targetRot.z = (playerGrav.x > 0 ? 90 : -90);
            //targetRot.y = (Physics.gravity.x > 0 ? -90 : 90);

        }
        else if (playerGrav.y != 0)
        {
            targetRot.z = (playerGrav.y > 0 ? 180 : 0);
        }
        else
        {
            targetRot.x = (playerGrav.z > 0 ? -90 : 90);
        }
        //transform.rotation = Quaternion.Euler(targetRot);
        transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.Euler(targetRot), .5f);
        //DontDestroyOnLoad(gameObject);
    }

    public void Update()
    {
        if (transform.position.y < 70)
        {
            SceneManager.LoadScene("DeathScreen");
        }
        if (ladderTouched == true)
        {
            transform.position += new Vector3(0f, ladderSpeed, 0f);
        }

        if (loading)
        {
            Loading();
        }

        if (inputManager.GetButtonDown("Start") && isServer && !hasStarted && isLocalPlayer)
        {
            MirrorVariables.instance.LoadingScreen();
            StartCoroutine(WaitASecond());
            
            
        }
        if (inputManager.GetButtonDown("Pause"))
        {
            if (NetworkManager.singleton.numPlayers < 2)
            {
                Time.timeScale = 0f;
            }
            PausedMenu.instance.pauseMenu.SetActive(true);
            isPaused = true;
        }

        if (isPaused)
        {
            if (Input.GetKeyDown(KeyCode.R))
            {
                Time.timeScale = 1.0f;
                PausedMenu.instance.pauseMenu.SetActive(false);
                isPaused = false;
            }
            if (Input.GetKeyDown(KeyCode.T))
            {
                MirrorVariables.instance.Respawn();
                Time.timeScale = 1.0f;
                PausedMenu.instance.pauseMenu.SetActive(false);
                isPaused = false;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                FindObjectOfType<TelepathyTransport>().Shutdown();
                if (NetworkClient.isConnected)
                {
                    NetworkManager.singleton.StopClient();
                    print("OnApplicationQuit: stopped client");
                }

                // stop server after stopping client (for proper host mode stopping)
                if (NetworkServer.active)
                {
                    NetworkManager.singleton.StopServer();
                    print("OnApplicationQuit: stopped server");
                }
                NetworkManager.singleton.StopHost();
                SceneManager.LoadScene("StartScreen");

                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
    }

    public IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(0.1f);
        loading = true;
    }

    [ClientRpc]
    public void RpcDebug(string str)
    {
        Debug.Log(str);
    }
    [Command]
    public void CmdTeleport(Vector3 vector3)
    {
        transform.position = vector3;
    }

    
}
