using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;

public class RoomProcGen : MonoBehaviour
{
    public Prop[] props;
    public GameObject floor;
    public float roomZ, roomWidth;
    // Start is called before the first frame update
    void Start()
    {
        foreach (Prop prop in props)
        { 
            for (int i = 0; i < Random.Range(prop.min, prop.max); i++)
            { 
                float maxDimension = Mathf.Pow((Mathf.Pow(prop.prop.width, 2) + Mathf.Pow(prop.prop.height, 2)), 0.5f) / 2;
                Vector3 randomPos = floor.transform.position + new Vector3(roomWidth / 2, 2, roomZ / 2) + new Vector3(Random.Range((-(roomWidth) / 2) + maxDimension, (roomWidth / 2) - maxDimension), 0, Random.Range((-(roomZ) / 2) + maxDimension, (roomZ / 2) - maxDimension));
                Vector3 randomRot = new Vector3(0, Random.Range(0, 360), 0);
                GameObject boxInstant = Instantiate(prop.prop.gameObject, transform);
                //Instantiate(box.gameObject, randomPos, Quaternion.Euler(randomRot));
                boxInstant.transform.rotation = Quaternion.Euler(randomRot);
                boxInstant.transform.position = randomPos;
                Debug.Log("prop spawned");
            }
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
[System.Serializable]
public class Prop
{
    public int min, max;
    public PropProp prop;



}