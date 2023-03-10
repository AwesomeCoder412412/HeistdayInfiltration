using UnityEngine;
using System.Collections;
using UnityEngine.SceneManagement;
using Mirror;
using FPSControllerLPFP;

public class TankPlayerController : NetworkBehaviour {

    public Rigidbody Rigid;
    public float speedPower; // engine power
    public Transform centerOfmass;
    public float turnPower = 10000;
    private float torque = 100f;
    private Vector3 vel;
    public float currentSpeed; // actual tank speed
    public float maxSpeed = 2.5f; // maximal tank speed
    public AudioSource engineSound;
    public bool tankMode = false;
    //public GameObject player;
    public Vector3 tankOffset;
    public GameObject tankCamera;
    public static GameObject currentPlayer;
    public NetworkConnection playersConnection1;
    public int playerId;
    public int playerId2;
    public override void OnStartServer()
    {
        base.OnStartServer();
    }
    public override void OnStartClient()
    {
        base.OnStartClient();
        inputManager = GameObject.FindObjectOfType<InputManager>();
        //Debug.Log("onstartlocalplayer " + OnStartLocalPlayer)
        StartCoroutine(WaitToSetPlayerId());
    }
    private IEnumerator WaitToSetPlayerId()
    {
        yield return new WaitForSeconds(1);
       // Debug.Log("Length " + FindObjectsOfType<FpsControllerLPFP>().Length);
        foreach (FpsControllerLPFP fPS in FindObjectsOfType<FpsControllerLPFP>())
        {
           // Debug.Log("isLocalPlayer " + fPS.IsLocalPlayer());
            if (fPS.IsLocalPlayer())
            {
                playerId = fPS.playerId;
            }
        }
    }
    private void Awake()
    {
        //Debug.Log("testing");
        Physics.IgnoreLayerCollision(9, 10);
        //currentPlayer = player;
    }
    void Start () {
        // set centre of mass
        Rigid.centerOfMass = centerOfmass.localPosition;
        engineSound.pitch = 0.6f;
        // max rotation speed
        Rigid.maxAngularVelocity = 0.6f;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
    }
    public void FixedUpdate()
    {
/*       if(!connectionToClient.isReady)
        {
            return;
        } */
        CmdFixedUpdateTheSecond(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), inputManager.GetButtonDown("Exit Vehicle"), playerId);
    }
    InputManager inputManager;
    [Command(requiresAuthority = false)]
    public void CmdFixedUpdateTheSecond(float horizontalInput, float verticalInput, bool exitVehicle, int playerId1)
    {
      //  Debug.Log("playerId1 " + playerId1);
       // Debug.Log("playerId2 " + playerId2);
       // Debug.Log("tankMode " + tankMode);
        if (tankMode && playerId2 == playerId1)
        {
         //   Debug.Log("works");
            float turn = horizontalInput;
            float moveVertical = verticalInput;
            Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);

            if (currentSpeed < maxSpeed)
            {
                Rigid.AddRelativeForce(movement * speedPower);
            }

            Rigid.AddTorque(transform.up * torque * turn);
            if (exitVehicle)
            {
                tankMode = false;
                RpcTogglePlayer(currentPlayer, true);
                currentPlayer.transform.position = transform.position + tankOffset;
                TargetToggleTankCamera(playersConnection1,false);
                currentPlayer = null;
            }
        }
    }


    void Update () {
        if (inputManager == null)
        {
            inputManager = GameObject.FindObjectOfType<InputManager>();
        }
        if (transform.position.y < 70)
        {
            SceneManager.LoadScene("DeathScreen");
        }
        if (currentSpeed > 1.0f)
        {
            torque = turnPower / 2;
        }
        else
        {
            torque = turnPower;
        }
        vel = Rigid.velocity;
        currentSpeed = vel.magnitude;

        engineSound.pitch = 0.6f + currentSpeed / 10;
        Debug.Log(inputManager != null);
        //Debug.Log((inputManager != null) + " " + inputManager.GetButtonDown("Exit Vehicle") + " " + tankMode);
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isServer)
        {
            //player = collision.gameObject;
            tankMode = true;
            currentPlayer = collision.gameObject;
            RpcTogglePlayer(currentPlayer, false);
            playerId2 = currentPlayer.GetComponent<FpsControllerLPFP>().playerId;
            playersConnection1 = currentPlayer.GetComponent<NetworkIdentity>().connectionToClient;
            TargetToggleTankCamera(playersConnection1, true);
            //currentPlayer = gameObject;
        }
    }
    [TargetRpc]
    private void TargetToggleTankCamera(NetworkConnection target, bool waterBottle)
    {
        tankCamera.SetActive(waterBottle);
    }
    [ClientRpc]
    private void RpcTogglePlayer(GameObject player, bool waterBottle)
    {
        player.SetActive(waterBottle);
    }
}
