using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class HealthManager : MonoBehaviour
{
    public GameObject teleportGoal;
    public GameObject death;
    public static HealthManager instance;
    public List<GameObject> hearts;
    // Start is called before the first frame update
    void Start()
    {
        if (instance == null)
        {
            instance = this;
        }
        else
        {
            Debug.LogError("Already a instance of the HealthManager script");
        }
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RemoveHeart()
    {
        if (hearts.Count > 0)
        {
            GameObject lastHeart = hearts[hearts.Count - 1];
            Destroy(lastHeart);
            hearts.Remove(lastHeart);
            ScoreManager.score += 50;

            if (hearts.Count == 0)
            {
                death.SetActive(true);
                foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
                {
                    if (pc.isLocalPlayer)
                    {
                        pc.CmdTeleport(teleportGoal.transform.position);
                    }
                }
                //Cursor.lockState = CursorLockMode.None;
                //Cursor.visible = true;
                //death.SetActive(true);
                //SceneManager.LoadScene("DeathScreen");
            }
        }

    }
}
