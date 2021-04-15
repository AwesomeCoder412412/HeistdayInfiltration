using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;
using Mirror;

public class PlayerController : MonoBehaviour
{
    public bool ladderTouched = false;
    public float ladderSpeed = 0.7f;
    public int roomIndex = 0;
    public static PlayerController instance;
    public GameObject knife;
    public GameObject bullet;
    public int retry;
    public string retry1 = "retry1";
    //public GameObject pauseMenu;
    public bool isPaused;
    private void Start()
    {
        inputManager = GameObject.FindObjectOfType<InputManager>();
        //Physics.IgnoreCollision(GetComponent<Collider>(), knife.GetComponent<Collider>());
        Physics.IgnoreCollision(GetComponent<Collider>(), bullet.GetComponent<Collider>());
        //Physics.IgnoreCollision(bullet.GetComponent<Collider>(), knife.GetComponent<Collider>());
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already an instance of the PlayerController class!");
        }
    }
    InputManager inputManager;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Laser"))
        {
            HealthManager.instance.RemoveHeart();
        }
        if (other.gameObject.CompareTag("Bullet"))
        {
            HealthManager.instance.RemoveHeart();
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
                SceneManager.LoadScene("VictoryScreen");
            }
        }
    }
    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Bullet"))
        {
            HealthManager.instance.RemoveHeart();
        }
    }
    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Ladder"))
        {
            ladderTouched = false;
            LadderScript.instance.roof.GetComponent<BoxCollider>().enabled = true;
        }
    }
    public void Update()
    {
        //DontDestroyOnLoad(gameObject);
        if (transform.position.y < 70)
        {
            SceneManager.LoadScene("DeathScreen");
        }
        //Debug.Log(instance != null);
        instance = this;
        //Cursor.visible = false;
        //Cursor.lockState = CursorLockMode.Locked;
        if (ladderTouched == true)
        {
            transform.position += new Vector3(0f, ladderSpeed, 0f);
        }
        
        if (inputManager.GetButtonDown("Pause"))
        {
            Time.timeScale = 0f;
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
                MirrorVariables.instance.spawnNewPlayer = true;
                MirrorVariables.instance.playersPain = NetworkManager.singleton.numPlayers;
                SceneManager.LoadScene("Networking");
                /*retry = 1;
                PlayerPrefs.SetInt(retry1, retry);
                SceneManager.LoadScene("StartScreen");*/
                //PlayButton.instance.PlayExpress();
                //SceneManager.LoadScene("Networking");
                Time.timeScale = 1.0f;
            }
            if (Input.GetKeyDown(KeyCode.M))
            {
                SceneManager.LoadScene("StartScreen");
                Cursor.visible = true;
                Cursor.lockState = CursorLockMode.None;
            }
        }
        /*if (Input.GetKeyDown(KeyCode.O))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        /*if (Input.GetKeyDown(KeyCode.I))
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }*/
    }
}
