using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour {

    public float FireRate;
    public float BulletVelocity;
    public float AttackRadius;
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


    private bool attack;
    private Vector3 startPosition;
    private MeshRenderer[] mrs;
    private Material[] mats;
    private InteractionAttributes[] ias;
    private Vector3 oldPlayerLocation;
    private Vector3 newPlayerLocation;
    private bool reset; //this is used to trigger special Update code to reorient the enemy after whatever the fuck the player did to him
    private Quaternion targetRot;
    private bool trackPlayerIsRunning = false;

    // Use this for initialization
    void Start() {
        IEnumerator shoot = Shoot();
        IEnumerator trackPlayer= TrackPlayer();
        StartCoroutine(shoot);
        StartCoroutine(trackPlayer);
        startPosition = EnemyBody.transform.localPosition;
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
            trackPlayerIsRunning = true;
            
            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, targetRot, step);
        } else
        {
            trackPlayerIsRunning = false;
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
            Rigidbody rb = GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            //startPosition.y += 1.0f; //this fucks up aiming for some reason. 

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
        while (true)
        {
            if (attack && !IsStunned)
            {
                Rigidbody bullet = Instantiate(MisslePrefab, MissleOrigin.transform.position, Quaternion.identity) as Rigidbody;
                bullet.velocity = transform.forward * BulletVelocity;

            }
            float timeToNextShot = Random.value;
            timeToNextShot = timeToNextShot * FireRate + FireRate * 0.5f;


            yield return new WaitForSeconds(timeToNextShot);
        }
    }


    public void Stun()
    {
        StopCoroutine("Battered"); //not sure if this line works..
        StopCoroutine("StunCoroutine");
        StartCoroutine("StunCoroutine");
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
        float stunnedStart = Time.time;
        IsStunned = true;
        GetComponentInChildren<RotatingPlatform>().RotationEnabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
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
                //ia.CanHookshot = true;
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
                //ia.CanHookshot = false;
            }
        }

        GetComponentInChildren<RotatingPlatform>().RotationEnabled = true;
        rb.useGravity = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        IsStunned = false;
        reset = true;
        transform.parent = null; // to stop corner cases where the enemy on coming unstunned is still childed to the controller
    }

    private void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia!=null)
        {
            if(ia.CanPickUp)
            {
                if (!IsStunned)
                {
                    StopCoroutine("Battered");
                    StartCoroutine("Battered");
                }
            }
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        InteractionAttributes ia = collision.collider.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.CanPickUp)
            {
                if (!IsStunned)
                {
                    StopCoroutine("Battered");
                    StartCoroutine("Battered");
                }
            }
        }
    }

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
}
