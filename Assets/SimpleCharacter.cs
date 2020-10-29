using UnityEngine;
using System.Collections;

public class SimpleCharacter : MonoBehaviour {

	// Use this for initialization
	void Start () {
        inputManager = GameObject.FindObjectOfType<InputManager>();
	}

    bool doJump = false;
    InputManager inputManager;
	
	void FixedUpdate () {
        if( doJump )
        {
            doJump = false;
            GetComponent<Rigidbody2D>().velocity = new Vector2(0, 5);
        }
	}


    void Update()
    {
        if( inputManager.GetButtonDown( "Jump" ) )
        {
            doJump = true;
        }
    }
}
