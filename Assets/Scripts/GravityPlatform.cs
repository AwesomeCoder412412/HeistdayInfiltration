using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlatform : MonoBehaviour
{
    public Vector3? gravity = null;
    // Start is called before the first frame update
    void Start()
    {

    }

    private void OnCollisionExit(Collision collision)
    {
        if (!collision.gameObject.CompareTag("Player") || gameObject.CompareTag("Fridge"))
        {
            return;
        }
        Debug.Log("plat grav3 " + collision.gameObject.GetComponent<PlayerController>().playerGrav + " " + (gravity == null));
        if (gravity == null)
        {
            ChangeGravity(collision.gameObject);
        }
        else
        {
            collision.gameObject.GetComponent<PlayerController>().playerGrav = (Vector3) gravity;
            Debug.Log("plat grav2 " + collision.gameObject.GetComponent<PlayerController>().playerGrav);
        }
    }
    public void ChangeGravity(GameObject player)
    {
        int dir = Random.Range(0, 3);
        Vector2 curDir = new Vector2(0, 0);
        if (player.GetComponent<PlayerController>().playerGrav.x != 0)
        {
            curDir = new Vector2(0, player.GetComponent<PlayerController>().playerGrav.x > 0 ? 1 : -1);
        }
        else if (player.GetComponent<PlayerController>().playerGrav.y != 0)
        {
            curDir = new Vector2(1, player.GetComponent<PlayerController>().playerGrav.y > 0 ? 1 : -1);
        }
        else
        {
            curDir = new Vector2(2, player.GetComponent<PlayerController>().playerGrav.z > 0 ? 1 : -1);
        }
        Vector3 gravity = new Vector3(0, 0, 0);
        Vector3 gravDir = dir == 0 ? Vector3.right : dir == 1 ? Vector3.up : Vector3.forward;
        if (curDir.x == dir)
        {
            float amount = -1 * Random.Range(10, 16) * curDir.y;
            gravity = gravDir * amount;

        }
        else
        {
            int rand = Random.Range(0, 2);
            float pos = rand == 0 ? 1 : -1;
            gravity = pos * Random.Range(10, 16) * gravDir;
        }
        Debug.Log("Dir " + dir + " CurDir " + curDir + " gravity " + gravity + " Physics.gravity " + Physics.gravity);
        player.GetComponent<PlayerController>().playerGrav = gravity;
        Debug.Log("plat grav " + player.GetComponent<PlayerController>().playerGrav);
    }
    public void SetGravity(Vector3 gravity)
    {
        this.gravity = new Vector3(gravity.x * Random.Range(10, 16), gravity.y * Random.Range(10, 16), gravity.z * Random.Range(10, 16));
    }
    // Update is called once per frame
    void Update()
    {

    }
}
