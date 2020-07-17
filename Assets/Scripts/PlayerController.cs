using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviour
{
    public bool ladderTouched = false;
    public float ladderSpeed = 0.7f;
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Laser"))
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
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            }
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
    private void Update()
    {
        if (ladderTouched == true)
        {
            GetComponent<CharacterController>().Move(new Vector3(0f, ladderSpeed, 0f));
        }
    }
}
