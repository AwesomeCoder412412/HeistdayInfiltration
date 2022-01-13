using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GravityPlatform : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }
    private void OnCollisionExit(Collision collision)
    {
        ChangeGravity();
    }
    public void ChangeGravity()
    {
        int dir = Random.Range(0, 3);
        Vector2 curDir = new Vector2(0, 0);
        if (Physics.gravity.x != 0)
        {
            curDir = new Vector2(0, Physics.gravity.x > 0 ? 1 : -1);
        }
        else if (Physics.gravity.y != 0)
        {
            curDir = new Vector2(1, Physics.gravity.y > 0 ? 1 : -1);
        }
        else
        {
            curDir = new Vector2(2, Physics.gravity.z > 0 ? 1 : -1);
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
        Physics.gravity = gravity;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
