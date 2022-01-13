using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PropProp : MonoBehaviour
{
    public float width, height;
    public bool isFrozen = true;
    // Start is called before the first frame update
    void Start()
    {
        if (isFrozen)
        {
            gameObject.GetComponent<Rigidbody>().freezeRotation = true;
            StartCoroutine(Freeze());
        }
    }

    public IEnumerator Freeze()
    {
        yield return new WaitForSeconds(3);
        gameObject.GetComponent<Rigidbody>().freezeRotation = false;
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
