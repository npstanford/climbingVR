﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public enum PHTypes { Red, Blue };
    public PHTypes PHType;

    public bool Broken;
    public float FireRate;
    public float BulletVelocity;
    public float AttackRadius;
    public float ShootRadius;
    public float rotateSpeed;
    public float bobHeight;
    public float bobVelocity;
    public bool IsStunned;
    public float StunLength;
    public BoxCollider PlayerDetector;
    public Rigidbody MisslePrefab;
    public GameObject MissleOrigin;
    public GameObject Player;
    public Material stunnedMaterial;
    public GameObject EnemyBody;
    public float FlySpeed;
    public AudioSource PropellerNoise;
    public AudioSource SuckingNoise;
    public ParticleSystem SuckingParticles;
    public AudioSource PHDefeatedSound;
    public AudioSource ExplosionSound;

    private bool attack;
    public bool CanSeePlayer;
    private Vector3 startPosition;
    private MeshRenderer[] mrs;
    private Material[] mats;
    private InteractionAttributes[] ias;
    private Vector3 oldPlayerLocation;
    private Vector3 newPlayerLocation;
    private bool reset; //this is used to trigger special Update code to reorient the enemy after whatever the fuck the player did to him
    private Quaternion targetRot;
    private bool trackPlayerIsRunning = false;
    private Vector3 HomePosition; // this is where propeller heads will return to if you displace them
    private Rigidbody rb;
    private Vector3 startPositionGlobal;

    //for detecting when you smash them
    private int contactIndex;
    private Vector3[] deflectionPositions;
    private int numDeflections = 2;
    public float SmashVelocity;

    // Use this for initialization
    void Start() {
        IEnumerator shoot = Shoot();
        IEnumerator trackPlayer= TrackPlayer();
        StartCoroutine(shoot);
        /*
        if (PHType == Enemy.PHTypes.Red) {  }
        else if (PHType == Enemy.PHTypes.Blue) { }
        */
        StartCoroutine(trackPlayer);
        startPosition = EnemyBody.transform.localPosition;
        startPositionGlobal = this.transform.position;
        HomePosition = transform.position;
        mrs = GetComponentsInChildren<MeshRenderer>();
        ias = GetComponentsInChildren<InteractionAttributes>();
        mats = new Material[mrs.Length];
        for (int i = 0; i < mrs.Length; i++)
        {
            mats[i] = mrs[i].material;
        }
        newPlayerLocation = Player.transform.position;
        oldPlayerLocation = newPlayerLocation;
        targetRot = Quaternion.identity;
        CanSeePlayer = false;
        rb = GetComponent<Rigidbody>();
        if (Broken)
        {
            
            rb.useGravity = true;
            rb.isKinematic = false;
            PropellerNoise.Stop();
            GetComponentInChildren<RotatingPlatform>().RotationEnabled = false;
        }
    }

    // Update is called once per frame
    void Update() {
        Vector3 playerDirection = (Player.transform.position - this.transform.position);
        float playerDist = playerDirection.magnitude;
        attack = (playerDist < AttackRadius);
        oldPlayerLocation = newPlayerLocation;
        newPlayerLocation = Player.transform.position;

        if (attack && !IsStunned)
        {
            RaycastHit hit;
            LayerMask lm = (1 << 13);
            lm += (1 << 2);
            lm = ~lm;


            //note the below code may allow a player to cover their face in order to not be seen
            if (Physics.Raycast(transform.position, newPlayerLocation - transform.position, out hit, AttackRadius + 5, lm))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia != null)
                {
                    if (ia.IsPartOfBody || ia.CanPickUp)
                    {
                        trackPlayerIsRunning = true;
                        CanSeePlayer = true;
                        float step = rotateSpeed * Time.deltaTime;
                        transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
                    }
                    else
                    {
                        CanSeePlayer = false;
                    }
                }
            }
            else
            {
                CanSeePlayer = false;
            }


        }
        else
        {
            trackPlayerIsRunning = false;
        }



        if (PHType == Enemy.PHTypes.Red)
        {
           
            if ((transform.position - HomePosition).magnitude > .2f && !IsStunned)
            {
                Vector3 DirectionHome = (HomePosition - transform.position).normalized;
                transform.position += DirectionHome * FlySpeed * Time.deltaTime;
            }

        } 


        else if (PHType == Enemy.PHTypes.Blue && !IsStunned
            && ((this.transform.position + transform.forward * FlySpeed * Time.deltaTime) - startPositionGlobal).magnitude < AttackRadius)
            //)
        {
            //Debug.Log("roaming distance: " + ((this.transform.position + transform.forward * FlySpeed * Time.deltaTime) - startPositionGlobal).magnitude);

            RaycastHit hit;
                LayerMask lm = (1 << 13); //enemy
                lm += (1 << 2); //ignroe raycast
                lm += (1 << 9); //body
                lm += (1 << 8); //controller
                lm += (1 << 14); //bullet
                lm = ~lm;


                if (Physics.Raycast(transform.position, transform.forward, out hit, 2, lm))
                {
                    float step = rotateSpeed *2 * Time.deltaTime;
                    transform.Rotate(Vector3.up, step, Space.World);
                    //TODO: have it "look" 30 degrees up right down left and pick the first one where it doesn't hit the same object

                    if (!Physics.Raycast(transform.position, transform.forward, out hit, 1f, lm))
                    {
                        this.transform.position += transform.forward * FlySpeed / 3 * Time.deltaTime;
                    }
                }

            this.transform.position += transform.forward * FlySpeed * Time.deltaTime;


        }



        //bob up and down
        if (!IsStunned)
        {
            EnemyBody.transform.localPosition = new Vector3(startPosition.x, startPosition.y + Mathf.Sin(Time.time * bobVelocity) * bobHeight,
                startPosition.z);
        }

        if (reset)
        {
            reset = false;

            if (!Broken)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            //startPosition.y += 1.0f; //this fucks up aiming for some reason. 

        }




        if (Broken && !rb.useGravity && !IsStunned)
        {

            rb.useGravity = true;
            rb.isKinematic = false;
            transform.parent = null;
        }


    }

    /*
    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Grapple"))
        {
            StopCoroutine("StunCoroutine");
            StartCoroutine("StunCoroutine");
        }
    }
    */

    private IEnumerator Shoot()
    {
        while (true && !Broken)
        {



            if (attack && !IsStunned && CanSeePlayer)
            {
                if ((Player.transform.position - this.transform.position).magnitude < ShootRadius)
                {
                    SuckingNoise.Play();
                    SuckingParticles.Play();
                    yield return new WaitForSeconds(SuckingNoise.clip.length + .1f);
                    SuckingParticles.Stop();
                    if (attack && !IsStunned && CanSeePlayer)
                    {
                        Rigidbody bullet = Instantiate(MisslePrefab, MissleOrigin.transform.position, Quaternion.identity) as Rigidbody;
                        bullet.velocity = transform.forward * BulletVelocity;
                    }
                }
            }
            float timeToNextShot = Random.value;
            timeToNextShot = timeToNextShot * FireRate + FireRate * 0.5f;


            yield return new WaitForSeconds(timeToNextShot);
        }
    }


    public void Stun()
    {
        StopCoroutine("Staggered"); //not sure if this line works..
        StopCoroutine("StunCoroutine");
        StartCoroutine("StunCoroutine");
    }

    public void Explode()
    {
        
      

        StopCoroutine("Staggered"); //not sure if this line works..
        StopCoroutine("StunCoroutine");
        ExplosionSound.Play();

        Transform[] children = this.transform.GetComponentsInChildren<Transform>();
        foreach (Transform child in children)
        {
            if (child.CompareTag("PHDestructionNoise"))
            {
                child.transform.parent = null;
                break;
            }
            if (child.CompareTag("PHHead") || child.CompareTag("PHNose") || 
                child.CompareTag("PHPropeller") || child.CompareTag("PHWeakPoint"))
            {
                child.parent = null;
                Rigidbody rbChild;

                rbChild = child.gameObject.AddComponent<Rigidbody>();

                if (child.CompareTag("PHWeakPoint")) {
                    Destroy(child.gameObject);
                    break;
                }


                rbChild.isKinematic = false;
                rbChild.useGravity = true;
                rbChild.velocity = Random.onUnitSphere * 15;
                rbChild.angularVelocity = Random.onUnitSphere * 25;

                InteractionAttributes ia = child.GetComponent<InteractionAttributes>();
                if (ia!=null)
                {
                    ia.HurtsPlayer = false;
                    ia.ObjectToPickUp = rbChild;
                }

                PickUpCollisionSounds sound = child.GetComponent<PickUpCollisionSounds>();
                if (sound != null) { sound.enabled = true; }

                if (child.CompareTag("PHPropeller"))
                {
                    RotatingPlatform rp = child.GetComponent<RotatingPlatform>();
                    if (rp != null)
                    {
                        rp.RotationEnabled = false;
                    }
                }
            }
        }
        StartCoroutine("DelayedDeath");
        
    }

    private IEnumerator DelayedDeath()
    {
        yield return new WaitForSeconds(.5f);
        Destroy(this.gameObject);
    }

    private IEnumerator TrackPlayer()
    {
        while (true) { 
            if (trackPlayerIsRunning)
                {
                    UpdateTarget();
                    //Quaternion rot = Quaternion.LookRotation(playerDirection);
                }
            yield return new WaitForSeconds(.2f);
        }
    }

    private void UpdateTarget()
    {
        Vector3 playerVelocity = (newPlayerLocation - oldPlayerLocation) / Time.deltaTime;
        float t = (newPlayerLocation - transform.position).magnitude / BulletVelocity;
        Vector3 aimDirection = (playerVelocity * t + newPlayerLocation - transform.position) / (t * BulletVelocity);

        targetRot = Quaternion.LookRotation(aimDirection);
    }



    private IEnumerator StunCoroutine()
    {
        PHDefeatedSound.Play();
        float stunnedStart = Time.time;

        if (!IsStunned) // if enemy is already stunned and gets hit again, the player might be holding the enemy, in which case, we don't want to make iskinematic=false;
        {


            rb.useGravity = true;
            rb.isKinematic = false;
        }


        IsStunned = true;
        GetComponentInChildren<RotatingPlatform>().RotationEnabled = false;

        PropellerNoise.Stop();
        foreach (MeshRenderer mr in mrs)
        {
            mr.material = stunnedMaterial;
        }

        foreach (InteractionAttributes ia in ias)
        {
            if (ia != null)
            {
                ia.HurtsPlayer = false;
                //ia.CanClimb = true;
                ia.IsGround = true;
                ia.CanPickUp = true;
                ia.CanHookshot = true;
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0, StunLength - 2));

        //flicker the enemy the last few seconds before it is unstunned
        while ((Time.time - stunnedStart) < StunLength)
        {
            for (int i = 0; i<mrs.Length; i++)
            {
                mrs[i].material = mats[i];
            }
            yield return new WaitForSeconds(0.1f);
            for (int i = 0; i < mrs.Length; i++)
            {
                mrs[i].material = stunnedMaterial;
            }
            yield return new WaitForSeconds(0.3f);
        }

        for (int i = 0; i < mrs.Length; i++)
        {
            mrs[i].material = mats[i];
        }


        foreach (InteractionAttributes ia in ias)
        {
            if (ia != null)
            {
                ia.HurtsPlayer = true;
                //ia.CanClimb = false;
                ia.IsGround = false;
                ia.CanPickUp = true;
                ia.CanHookshot = false;
            }
        }


        if (!Broken)
        {
            GetComponentInChildren<RotatingPlatform>().RotationEnabled = true;
            rb.useGravity = false;
            if (PHType == Enemy.PHTypes.Red) { rb.isKinematic = true; }
            PropellerNoise.Play();
        }
        rb.velocity = Vector3.zero;
        IsStunned = false;
        reset = true; // I have a feeling this isn't actually used anymore...

        transform.parent = null; // to stop corner cases where the enemy on coming unstunned is still childed to the controller
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia!=null)
        {
            if(ia.CanPickUp)
            {
                //decided to try seeing what it is like if propeller heads get stunend when hit by rocks
                StopCoroutine("StunCoroutine");
                StopCoroutine("Staggered");
                StartCoroutine("StunCoroutine");


                /*
                if (!IsStunned)
                {
                    StopCoroutine("Battered");
                    StartCoroutine("Battered");
                }
                */
            }
        }

        if (other.CompareTag("Grapple") && !IsStunned)
        {
            Vector3 staggerDirection = (transform.position - other.transform.position).normalized;
            StartCoroutine(Staggered(staggerDirection));
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        InteractionAttributes ia = collision.collider.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanPickUp)
            {
                //decided to try seeing what it is like if propeller heads get stunend when hit by rocks
                StopCoroutine("StunCoroutine");
                StopCoroutine("Staggered");
                StartCoroutine("StunCoroutine");


                /*
                if (!IsStunned)
                {
                    StopCoroutine("Battered");
                    StartCoroutine("Battered");
                }
                */
            }

            if (ia.CanClimb || ia.IsGround)
            {
                //Debug.Log("PH collision velocity:" + rb.velocity.magnitude);
                if(rb.velocity.magnitude > SmashVelocity)
                {
                    Debug.Log("Explode the PH");
                    Explode();
                }
            }
        }

    }

    /*
    private IEnumerator Battered()
    {
        float batteredTime = 2.0f;

        Rigidbody rb = GetComponent<Rigidbody>();
        rb.isKinematic = false;
        rb.useGravity = true;
        IsStunned = true;

        yield return new WaitForSeconds(batteredTime);

        rb.isKinematic = true;
        rb.useGravity = false;
        IsStunned = false;

    }
    */

    private IEnumerator Staggered(Vector3 staggerDirection)
    {
        float staggeredTime = .6f;
        //ExplosionSound.Play();
        //ExplosionSound.volume = .98f;
        IsStunned = true;
        SuckingParticles.Stop();

        transform.position += staggerDirection * .5f;



        Vector3 HeadSnapBackAngle = new Vector3(-20f, 0f, 0f);
        transform.Rotate(HeadSnapBackAngle, Space.Self);
        yield return new WaitForSeconds(.3f);

        rb.isKinematic = false;
        rb.useGravity = true;

        /*TODO animation ideas
         *  ring blel
         *  head snaps back
         *  shakes head side to side
         */

        yield return new WaitForSeconds(staggeredTime);
        if (PHType == Enemy.PHTypes.Red) { rb.isKinematic = true; }

        rb.useGravity = false;
        IsStunned = false;
    }

    /*
    private void OnTriggerStay(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();


        if (ia != null && contactIndex < numDeflections)
        {
            if (ia.IsGround || ia.CanClimb)
            {
                //idea, if we swing fast enough, this bounces back toward the PH
                //rb.velocity = (this.transform.position - other.transform.position).normalized * rb.velocity.magnitude;
                deflectionPositions[contactIndex] = other.transform.position;
                contactIndex += 1;
            }

            if (!(contactIndex < numDeflections))
            {
                Vector3 deflectionVelocity = Vector3.zero;
                for (int i = 1; i < numDeflections; i++)
                {
                    deflectionVelocity += (deflectionPositions[i] - deflectionPositions[i - 1]) / Time.deltaTime;
                }

                deflectionVelocity = deflectionVelocity / (numDeflections - 1);

                
            }

        }
    }*/

}
