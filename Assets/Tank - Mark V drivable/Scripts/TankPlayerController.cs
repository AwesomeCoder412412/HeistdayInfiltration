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
    public GameObject player;
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
        Debug.Log("Length " + FindObjectsOfType<FpsControllerLPFP>().Length);
        foreach (FpsControllerLPFP fPS in FindObjectsOfType<FpsControllerLPFP>())
        {
            Debug.Log("isLocalPlayer " + fPS.IsLocalPlayer());
            if (fPS.IsLocalPlayer())
            {
                playerId = fPS.playerId;
            }
        }
    }
    private void Awake()
    {
        Debug.Log("testing");
        Physics.IgnoreLayerCollision(9, 10);
        currentPlayer = player;
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
        CmdFixedUpdateTheSecond(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical"), playerId);
    }
    InputManager inputManager;
    [Command]
    public void CmdFixedUpdateTheSecond(float horizontalInput, float verticalInput, int playerId1)
    {
        Debug.Log("playerId1 " + playerId1);
        Debug.Log("playerId2 " + playerId2);
        Debug.Log("tankMode " + tankMode);
        if (tankMode && playerId2 == playerId1)
        {
            Debug.Log("works");
            float turn = horizontalInput;
            float moveVertical = verticalInput;
            Vector3 movement = new Vector3(0.0f, 0.0f, moveVertical);

            if (currentSpeed < maxSpeed)
            {
                Rigid.AddRelativeForce(movement * speedPower);
            }

            Rigid.AddTorque(transform.up * torque * turn);
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
        if ((inputManager != null) && inputManager.GetButtonDown("Exit Vehicle") && tankMode)
        {
            tankMode = false;
            player.SetActive(true);
            player.transform.position = transform.position + tankOffset;
            tankCamera.SetActive(false);
            currentPlayer = player;
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Player") && isServer)
        {
            player = collision.gameObject;
            tankMode = true;
            RpcTurnOffPlayer(player);
            playerId2 = player.GetComponent<FpsControllerLPFP>().playerId;
            playersConnection1 = player.GetComponent<NetworkIdentity>().connectionToClient;
            TargetTurnOnTankCamera(playersConnection1);
            currentPlayer = gameObject;
        }
    }
    [TargetRpc]
    private void TargetTurnOnTankCamera(NetworkConnection target)
    {
        tankCamera.SetActive(true);
    }
    [ClientRpc]
    private void RpcTurnOffPlayer(GameObject player)
    {
        player.SetActive(false);
    }
}
