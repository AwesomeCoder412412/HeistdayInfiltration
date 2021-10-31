using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomEvents : MonoBehaviour
{
    public GameObject leftWall;
    public GameObject rightWall;
    public GameObject topWall;
    public GameObject frontDoor;
    public GameObject backDoor;
    public float timeLeft;
    private float timeSoFar;
    public float sideWallSpeed;
    public float topWallSpeed;
    public int roomIndex;
    private bool hasImploded = false;
    // Start is called before the first frame update
    void Start()
    {
        timeSoFar = timeLeft;
    }

    // Update is called once per frame
    void Update()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        bool enemiesExist = false;
        foreach (GameObject enemy in enemies)
        {
            if (enemy.GetComponent<GuardAI>().roomIndex == roomIndex)
            {
                enemiesExist = true;
            }

        }
        timeSoFar += Time.deltaTime;
        if (timeSoFar < timeLeft)
        {
            leftWall.transform.position += Vector3.right * sideWallSpeed * Time.deltaTime;
            rightWall.transform.position += Vector3.left * sideWallSpeed * Time.deltaTime;
            topWall.transform.position += Vector3.down * sideWallSpeed * Time.deltaTime;
            frontDoor.transform.position += Vector3.forward * sideWallSpeed * Time.deltaTime;
            backDoor.transform.position += Vector3.back * sideWallSpeed * Time.deltaTime;
            
        }
        else if(!enemiesExist && timeSoFar - timeLeft > 5 && !hasImploded)
        {
            ImplodeRoom();
            hasImploded = true;
        }
    }
    public void ImplodeRoom()
    {
        timeSoFar = 0;
    }
}
