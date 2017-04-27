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


    // Use this for initialization
    void Start() {
        IEnumerator shoot = Shoot();
        StartCoroutine(shoot);
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
            // the three lines below attempts to guess where the player will be in the future and aim there
            Vector3 playerVelocity = (newPlayerLocation - oldPlayerLocation) / Time.deltaTime;
            float t = (newPlayerLocation - transform.position).magnitude / BulletVelocity;
            Vector3 aimDirection = (playerVelocity * t + newPlayerLocation - transform.position) / (t * BulletVelocity);

            Quaternion rot = Quaternion.LookRotation(aimDirection);
            //Quaternion rot = Quaternion.LookRotation(playerDirection);
            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, step);
            //transform.LookAt(Player.transform);
        }

        //bob up and down
        if (!IsStunned)
        {
            EnemyBody.transform.localPosition = new Vector3(startPosition.x, startPosition.y + Mathf.Sin(Time.time * bobVelocity) * bobHeight,
                startPosition.z);
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
        StopCoroutine("StunCoroutine");
        StartCoroutine("StunCoroutine");
    }


    private IEnumerator StunCoroutine()
    {
        float stunnedStart = Time.time;
        IsStunned = true;
        GetComponentInChildren<RotatingPlatform>().RotationEnabled = false;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
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
        rb.velocity = Vector3.zero;
        IsStunned = false;
    }
}
