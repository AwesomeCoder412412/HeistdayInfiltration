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
    public int heartCount;
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
        heartCount = hearts.Count;
    }

    // Update is called once per frame
    void Update()
    {

    }
    public void RestoreHearts(int i, bool restoreAll)
    {
        int staticHeartCount = heartCount;
        if (restoreAll)
        {
            for (int k = 0; k < hearts.Count - staticHeartCount; k++)
            {
                GameObject currentHeart = hearts[hearts.Count - k - 1];
                currentHeart.SetActive(true);
                heartCount++;
            }
        }
        else
        {
            for (int k = 0; k < i; k++)
            {
                GameObject currentHeart = hearts[hearts.Count - heartCount + k + 1];
                currentHeart.SetActive(true);
                heartCount++;
            }
        }

    }


    public void RemoveHeart()
    {
        if (heartCount > 0)
        {
            GameObject lastHeart = hearts[heartCount - 1];
            lastHeart.SetActive(false);
            heartCount--;
            ScoreManager.score += 50;

            if (heartCount <= 0)
            {
                death.SetActive(true);
                Cursor.lockState = CursorLockMode.None;
                Cursor.visible = true;

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
