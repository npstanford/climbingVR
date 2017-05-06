using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemy : MonoBehaviour {

    public float chargingVelocity;
    private Vector3 ChargingDirection;
    public float rotateSpeed;

    public bool IsStunned;
    public bool IsCharging;
    public bool attack;
    public GameObject Player;
    public GameObject LaserFinder;
    public AreaListener DetectPlayer;
    public SwitchControl WeakPoint;
    private Quaternion targetRot;
    private bool trackPlayerIsRunning = false;
    private Vector3 oldPlayerLocation;
    private Vector3 newPlayerLocation;
    private Vector3 PlayerDirection;
    private MeshRenderer[] mrs;
    private Material[] mats;
    private InteractionAttributes[] ias;
    public Quaternion rot;
    public Material stunnedMaterial;
    public float StunLength;
    public int hits = 0;
    IEnumerator StunCoroutine;
    private Vector3 startPosition;
    private Quaternion startRotation;
    private bool HeadingBackToStart = false;

    // Use this for initialization
    void Start () {
        IEnumerator trackPlayer = TrackPlayer();
        StunCoroutine = Stun();
        StartCoroutine(trackPlayer);
        mrs = GetComponentsInChildren<MeshRenderer>();
        ias = GetComponentsInChildren<InteractionAttributes>();
        mats = new Material[mrs.Length];
        for (int i = 0; i < mrs.Length; i++)
        {
            mats[i] = mrs[i].material;
        }
        startPosition = transform.position;
        startRotation = transform.rotation;
    }
	
	// Update is called once per frame
	void Update () {
        attack = DetectPlayer.Activated;
        if (hits < WeakPoint.hits)
        {
            Debug.Log("fewer hits");
            hits = WeakPoint.hits;
            StopCoroutine("Stun");
            StartCoroutine("Stun");
        }

        if (attack && !IsStunned && !IsCharging)
        {
            trackPlayerIsRunning = true;


            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, step);
            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, 0f, 0f),
                transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, 0f, 0f));
            // if playerDirection and forward are ever within delta, then charge
            //Debug.Log(Vector3.Angle(transform.forward, PlayerDirection));
            if (Vector3.Angle(transform.forward, PlayerDirection) < 5f)
            {
                IEnumerator ChargeCoroutine = Charge(PlayerDirection);
                StartCoroutine(ChargeCoroutine);
            }
        } else
        {
            trackPlayerIsRunning = false;

        }

        //after knocking player off, return back to starting position
        //note: I should rotate first, and then once rotated, start transltaing
        //i.e I should refactor so that I can track things other than the player
        if (!attack && !IsStunned && (transform.position - startPosition).magnitude > 1)
        {
            if (!HeadingBackToStart)
            {
                StartCoroutine("HeadBackToStart");
            }
        }


    }

    IEnumerator Charge(Vector3 playerDirection)
    {
        yield return new WaitForSeconds(1f);
        Debug.Log("CHARGING!");
        IsCharging = true;
 

        ChargingDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        ChargingDirection = ChargingDirection.normalized;

        //Rigidbody rb = GetComponent<Rigidbody>();
       // rb.AddForce(ChargingDirection * chargingVelocity);

        LayerMask layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;
        RaycastHit hit;

        while (true)
        {
            if (Physics.Raycast(LaserFinder.transform.position, -LaserFinder.transform.up, out hit, 3f, layerMask))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia != null)
                {
                    if (!ia.IsGround)
                    {
                        Debug.Log("Laser finder hit something otherthan ground: " + hit.collider.name);
                        IsCharging = false;
                        //rb.velocity = Vector3.zero;
                        yield break;
                    }
                }

            }
            else
            {
                Debug.Log("Laser finder hit nothing");

                IsCharging = false;
               // rb.velocity = Vector3.zero;
                yield break;

            }
            //rb.velocity = -ChargingDirection * chargingVelocity;
            transform.position += ChargingDirection * chargingVelocity * Time.deltaTime;
            yield return null;
            
        }

        
    }
    private IEnumerator TrackPlayer()
    {
        while (true)
        {
            if (trackPlayerIsRunning)
            {
                UpdateTarget();
                //Quaternion rot = Quaternion.LookRotation(playerDirection);
            }
            yield return new WaitForSeconds(.2f);
        }
    }

    private IEnumerator HeadBackToStart()
    {
        HeadingBackToStart = true;
        Vector3 DirectionToStart = startPosition - transform.position;

        Quaternion rotStart = Quaternion.LookRotation(DirectionToStart);
        Debug.Log("Direction to start: " + DirectionToStart);
        Debug.Log("Angle back to start: " + Vector3.Angle(transform.forward, DirectionToStart));
        while (!attack && Vector3.Angle(transform.forward, DirectionToStart) > 1f)
        {

            Debug.Log("rotating back to start angle: " + Vector3.Angle(transform.forward, DirectionToStart));

            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rotStart, step);
            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, 0f, 0f),
                transform.rotation.eulerAngles.y, Mathf.Clamp(transform.rotation.eulerAngles.z, 0f, 0f));
            yield return null;
        }

        while(!attack && (transform.position - startPosition).magnitude > 1)
        {
            Debug.Log("translating back to start");
            transform.position += transform.forward * 5.0f * Time.deltaTime;
            yield return null;
        }
        HeadingBackToStart = false;
    }

    private void UpdateTarget()
    {
        PlayerDirection = (Player.transform.position - this.transform.position);

        PlayerDirection.y = 0;

        rot = Quaternion.LookRotation(PlayerDirection);

    }

    private IEnumerator Stun()
    {
        Debug.Log("Stun coroutine started");
        StopCoroutine("Charge");
        float stunnedStart = Time.time;
        IsStunned = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        foreach (MeshRenderer mr in mrs)
        {
            mr.material = stunnedMaterial;
        }

        bool[] hurtsPlayer = new bool[ias.Length];

        for (int i=0; i< ias.Length; i++)
        {
  
            if (ias[i] != null)
            {
                hurtsPlayer[i] = ias[i].HurtsPlayer;
                ias[i].HurtsPlayer = false;
                
            }
        }

        yield return new WaitForSeconds(Mathf.Max(0, StunLength - 2));

        //flicker the enemy the last few seconds before it is unstunned
        while ((Time.time - stunnedStart) < StunLength)
        {
            for (int i = 0; i < mrs.Length; i++)
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


        for (int i = 0; i < ias.Length; i++)
        {
            if (ias[i] != null)
            {
                ias[i].HurtsPlayer = hurtsPlayer[i];
            }
        }


        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        IsStunned = false;

    }

}
