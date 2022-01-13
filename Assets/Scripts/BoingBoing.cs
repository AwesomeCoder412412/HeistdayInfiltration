using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BoingBoing : MonoBehaviour
{
    public Rigidbody rigidbody;
    public float addedVelocity = 1f;
    public float initVelocityRange = 5f;
    public float velocityOffset = 2f;
    // Start is called before the first frame update
    void Start()
    {
        rigidbody.velocity = new Vector3(Random.Range(-initVelocityRange, initVelocityRange), Random.Range(-initVelocityRange, initVelocityRange), Random.Range(-initVelocityRange, initVelocityRange));
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void OnCollisionExit(Collision collision)
    {
        //rigidbody.velocity += 1 * Vector3.one;
        Vector3 newVelocity = new Vector3(rigidbody.velocity.x + Random.Range(-velocityOffset, velocityOffset), rigidbody.velocity.y + Random.Range(-velocityOffset, velocityOffset), rigidbody.velocity.z + Random.Range(-velocityOffset, velocityOffset));
        rigidbody.velocity = (rigidbody.velocity.magnitude + addedVelocity) * newVelocity / newVelocity.magnitude;
    }
}
