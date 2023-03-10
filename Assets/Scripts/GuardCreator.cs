using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Mirror;
using FPSControllerLPFP;

public class GuardCreator : NetworkBehaviour
{
    public GameObject guardPrefab;
    public GuardData[] guardData;
    public int guards;
    private string guard = "guards";
    public GameObject[] guardArray;
    // Start is called before the first frame update
    void Start()
    {
        StartCoroutine(WaitASecond());
    }
    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator WaitASecond()
    {
        yield return new WaitForSeconds(2);
        Debug.Log("ok then");
        if (!FindObjectOfType<FpsControllerLPFP>().isServer)
        {
            yield break;
        }
        Debug.Log("passed check");
        guards = PlayerPrefs.GetInt(guard);
        for (int i = 0; i < guards; i++)
        {
            GameObject guardInstant = Instantiate(guardPrefab, guardData[i].spawnPosition.position, Quaternion.identity);
            RpcDebug("spawned guard " + guardInstant.name);
            guardInstant.GetComponent<GuardAI>().forwardPos = guardData[i].forwardPosition;
            NetworkServer.Spawn(guardInstant);
        }
    }
    [ClientRpc]
    public void RpcDebug(string str)
    {
        Debug.Log(str);
    }
}
[System.Serializable]
public class GuardData
{
    public Transform spawnPosition, forwardPosition;
    
}