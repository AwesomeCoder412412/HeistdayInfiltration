using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FPSControllerLPFP;

public class OpenPuzzle : NetworkBehaviour
{
    [SyncVar]
    public bool didPuzzle = false;
    public GameObject meMyselfAndI; 
    // Start is called before the first frame update
    void Start()
    {
        meMyselfAndI = gameObject;
    }
    private void OnTriggerEnter(Collider other)
    {
        if ((other.gameObject.CompareTag("Player") && !didPuzzle && other.gameObject.GetComponent<FpsControllerLPFP>().isLocalPlayer) || other.gameObject.CompareTag("Tank") && !didPuzzle)
        {
            Debug.Log(gameObject.name + " puzzle name");
            Cursor.visible = true;
            ScoreManager.score += 50;
            //boxCollider.enabled = false;
            puzzleCanvas.SetActive(true);
            Cursor.lockState = CursorLockMode.None;
            MirrorVariables.instance.buttonsGalore = meMyselfAndI;
            MirrorVariables.instance.puzzleDoor = boxCollider;
            MirrorVariables.instance.UnlockDoor();
            //CmdPuzzleSync();
            //PlayerController.instance.roomIndex++;
            //didPuzzle = true;
        }
        if ((other.gameObject.CompareTag("Player") && !didPuzzle && !other.gameObject.GetComponent<FpsControllerLPFP>().isLocalPlayer) || other.gameObject.CompareTag("Tank") && !didPuzzle)
        {
            Debug.Log("apples");
            MirrorVariables.instance.buttonsGalore = meMyselfAndI;
            MirrorVariables.instance.puzzleDoor = boxCollider;
        }
    }
        // Update is called once per frame
        void Update()
    {
        
    }
    [Command(ignoreAuthority = true)]
    public void CmdPuzzleSync()
    {
        Debug.Log("banana");
        didPuzzle = true;
        foreach (PlayerController pc in GameObject.FindObjectsOfType<PlayerController>())
        {
            pc.roomIndex++;
        }
        RpcPuzzleSync();
    }

    [ClientRpc]
    public void RpcPuzzleSync()
    {
        boxCollider.enabled = false;
    }


    public void ButtonClicked()
    {
        boxCollider.enabled = false;
        puzzleCanvas.SetActive(true);
    }
    public BoxCollider boxCollider;
    public GameObject puzzleCanvas;
    
}
