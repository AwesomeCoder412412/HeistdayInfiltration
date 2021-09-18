﻿using UnityEngine;
using System.Collections;
using Mirror;

public class RayCastShoot : NetworkBehaviour {

    public float weaponRange = 32000f;
    public Transform gunEnd;
    private float time =  0f;
    public Camera fpsCam;
    public Light lightEff ;
    public GameObject explosion; // drag your explosion prefab here
    private float bulletOffset ;
    private bool fire = true;
    public AudioSource firingAudio;

    void Start () {

        lightEff.intensity=0f;
        
	}

    [Command(requiresAuthority = false)]
    private void CmdExplosion(Vector3 hitPos)
    {
        
        GameObject expl = Instantiate(explosion, hitPos, Quaternion.identity) as GameObject;
        NetworkServer.Spawn(expl);
        StartCoroutine(ExplosionTime(expl));
    }

    public IEnumerator ExplosionTime(GameObject expl)
    {
        yield return new WaitForSeconds(3);
        NetworkServer.Destroy(expl); // delete the explosion after 3 seconds
    }

    IEnumerator FireReload()
    {
        fire = false;
        // Debug.Log("Before Waiting 4 seconds");
        yield return new WaitForSeconds(4);
        // Debug.Log("After Waiting 4 Seconds");
        fire = true;
    }

    void Update () {
	    
        if(Input.GetButton("Fire1"))
        {

            lightEff.intensity = 1f;
            Vector3 rayOrigin = fpsCam.ViewportToWorldPoint(new Vector3(0.5f, 0.5f, 0));
            RaycastHit hit ; 

            if (Physics.Raycast(rayOrigin, fpsCam.transform.forward, out hit, weaponRange))
            {

                bulletOffset = hit.distance / 10000f;
                if (Physics.Raycast(rayOrigin, fpsCam.transform.forward - new Vector3(0.0f,bulletOffset,0.0f), out hit, weaponRange))
                {
                    Debug.DrawRay(rayOrigin, (fpsCam.transform.forward - new Vector3(0.0f, bulletOffset, 0.0f)).normalized * hit.distance, Color.red, 2);
                    Debug.Log(hit.collider.gameObject.name);
                    if (fire)
                    {
                        CmdExplosion(hit.point);
                        StartCoroutine(FireReload());
                        time = 2f;
                        firingAudio.Play();
                        if (hit.collider.gameObject.tag == "Enemy")
                        {
                            Debug.Log("Hit");
                            Destroy(hit.collider.gameObject);
                        }
                    }
                    
                }

            }
   
        }
        if (time > 0)
            time--;
        if(time == 0f)
            lightEff.intensity = 0f;

 


    }
}

