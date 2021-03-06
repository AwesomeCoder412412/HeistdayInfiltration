﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SimpleShoot : MonoBehaviour
{

    public GameObject bulletPrefab;
    public GameObject casingPrefab;
    public GameObject muzzleFlashPrefab;
    public Transform barrelLocation;
    public Transform casingExitLocation;
    public bool angered = false;
    private float timeToShoot = 0f;
    public float timeBetweenShots = 1f;
    public GuardAI guardAI;
    public float maxAngle, maxAngleLoose;
    public float maxDistance, maxDistanceLoose;
    public LookAtPlayer lookAtPlayer;
    public float shotPower = 100f;
  

    void Start()
    {
        if (barrelLocation == null)
            barrelLocation = transform;
    }

    void Update()
    {
        if (guardAI.isGuard)
        {
            Vector3 displacement = lookAtPlayer.target.position - transform.position;
            float angle = Vector3.Angle(transform.forward, displacement);
            float currentMaxAngle = (angered) ? maxAngleLoose : maxAngle;
            float currentMaxDistance = (angered) ? maxDistanceLoose : maxDistance;
            Debug.Log("Loggins " + " angered " + angered);
            Debug.Log("Loggins " + " angle " + Mathf.Abs(angle) + " and currentMaxAngle " + currentMaxAngle);
            Debug.Log("Loggins " + " distance " + Vector3.Distance(lookAtPlayer.target.position, transform.position) + " and currentMaxDistance " + currentMaxDistance);
            if (Mathf.Abs(angle) < currentMaxAngle && Vector3.Distance(lookAtPlayer.target.position, transform.position) < currentMaxDistance)
            {
                Debug.Log("Loggins " + " set angered to true, patrolling is false");
                angered = true;
                guardAI.SetPatrolling(false);
            }
            else
            {
                Debug.Log("Loggins " + " set angered to false, patrolling is true");
                angered = false;
                guardAI.SetPatrolling(true);
            }
            /*if (Input.GetButtonDown("Fire1"))
            {
                GetComponent<Animator>().SetTrigger("Fire");
            }*/
            if (angered == true && timeToShoot < 0)
            {
                GetComponent<Animator>().SetTrigger("Fire");
                timeToShoot = timeBetweenShots;
            }
        }
        else
        {
            if (lookAtPlayer.target != null && timeToShoot < 0)
            {
                GetComponent<Animator>().SetTrigger("Fire");
                timeToShoot = timeBetweenShots;
            }
        }
        timeToShoot -= Time.deltaTime;
    }
    void Shoot()
    {
        //  GameObject bullet;
        //  bullet = Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation);
        // bullet.GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);

        GameObject tempFlash;
       Instantiate(bulletPrefab, barrelLocation.position, barrelLocation.rotation).GetComponent<Rigidbody>().AddForce(barrelLocation.forward * shotPower);
       tempFlash = Instantiate(muzzleFlashPrefab, barrelLocation.position, barrelLocation.rotation);

       // Destroy(tempFlash, 0.5f);
        //  Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation).GetComponent<Rigidbody>().AddForce(casingExitLocation.right * 100f);
       
    }


    void CasingRelease()
    {
         GameObject casing;
        casing = Instantiate(casingPrefab, casingExitLocation.position, casingExitLocation.rotation) as GameObject;
        casing.GetComponent<Rigidbody>().AddExplosionForce(550f, (casingExitLocation.position - casingExitLocation.right * 0.3f - casingExitLocation.up * 0.6f), 1f);
        casing.GetComponent<Rigidbody>().AddTorque(new Vector3(0, Random.Range(100f, 500f), Random.Range(10f, 1000f)), ForceMode.Impulse);
    }


}
