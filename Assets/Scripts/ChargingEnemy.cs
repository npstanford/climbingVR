using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class ChargingEnemy : MonoBehaviour {
    public enum GTState { Stunned, RotatingTowardHome, HeadingHome, RotatingTowardPlayer, Charging}
    public GTState state;
    public float chargingVelocity;
    private Vector3 ChargingDirection;
    public float rotateSpeed;

    public bool IsStunned;
    //public bool IsCharging;
    public bool attack;
    public float AngleToPlayer;
    public float AngleToHome;
    public bool AtEdge;
    public GameObject Player;
    public GameObject LaserFinder;
    public AreaListener DetectPlayer;
    public SwitchControl WeakPoint;
    private Quaternion targetRot;
    private bool trackPlayerIsRunning = false;
    private Vector3 oldPlayerLocation;
    private Vector3 newPlayerLocation;
    private MeshRenderer[] mrs;
    private Material[] mats;
    private InteractionAttributes[] ias;
    public Quaternion rot;
    public Material stunnedMaterial;
    public float StunLength;
    public int hits = 0;
    IEnumerator StunCoroutine;
    public GameObject startPosition;
    private Quaternion startRotation;
    private bool HeadingBackToStart = false;
    private NavMeshAgent nma;
    public bool TryingToGoHome = false;
    private LayerMask layerMask;
    private IEnumerator RotateTowardsCoroutine;
    private IEnumerator ChargeCoroutine;
    public GameObject PickUpZone;

    // Use this for initialization
    void Start () {
        state = GTState.HeadingHome;
        //IEnumerator trackPlayer = TrackPlayer();
        StunCoroutine = Stun();
        //StartCoroutine(trackPlayer);
        nma = GetComponent<NavMeshAgent>();
        mrs = GetComponentsInChildren<MeshRenderer>();
        ias = GetComponentsInChildren<InteractionAttributes>();
        mats = new Material[mrs.Length];
        for (int i = 0; i < mrs.Length; i++)
        {
            mats[i] = mrs[i].material;
        }
        startPosition = new GameObject();
        startPosition.transform.position = transform.position;
        startPosition.transform.rotation = transform.rotation;
        RotateTowardsCoroutine = RotateTowards(startPosition);
        ChargeCoroutine = Charge(Vector3.zero);

        layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;


    }

    // Update is called once per frame
    void Update() {
        attack = DetectPlayer.Activated;

        if (hits < WeakPoint.hits) //stunned is enterable by all states. Stun exits after a fixed length of time into headinghome
        {
            hits = WeakPoint.hits;
            UpdateState(GTState.Stunned);
        }

        if (state == GTState.HeadingHome)
        {
            if (attack) { UpdateState(GTState.RotatingTowardPlayer); }
        }

        else if (state == GTState.RotatingTowardPlayer)
        {
            Vector3 PlayerDirection = Player.transform.position - transform.position;
            PlayerDirection.y = 0;

            Vector3 GTForward = transform.forward;
            GTForward.y = 0;

            AngleToPlayer = Vector3.Angle(GTForward, PlayerDirection);
            if (!attack) { UpdateState(GTState.RotatingTowardHome); }
            else if (AngleToPlayer < 3f)
            {
                UpdateState(GTState.Charging);
            }
        }

        else if (state == GTState.Charging)
        {
            RaycastHit hit;
            AtEdge = !(Physics.Raycast(LaserFinder.transform.position, -LaserFinder.transform.up, out hit, 3f, layerMask));

            //AtEdge ? laserFinderCount += 1 :

            if (AtEdge)
                {
                    if (attack) { UpdateState(GTState.RotatingTowardPlayer); }
                    else { UpdateState(GTState.RotatingTowardHome); }
                }
        }

        else if (state == GTState.RotatingTowardHome)
        {
            if (attack) { UpdateState(GTState.RotatingTowardPlayer); }
            else
            {
                Vector3 HomeDirection = startPosition.transform.position - transform.position;
                HomeDirection.y = 0;
                Vector3 GTForward = transform.forward;
                GTForward.y = 0;

                AngleToHome = Vector3.Angle(GTForward, HomeDirection);

                if (AngleToHome < 3f)
                {
                    UpdateState(GTState.HeadingHome);
                }
            }
        }

        /*
        if (state == GTState.RotatingTowardPlayer)
        {
            nma.SetDestination(Player.transform.position);
        } else if (state == GTState.RotatingTowardHome)
        {
            nma.ResetPath();
            nma.SetDestination(startPosition);
        }

        */

    }

    public void UpdateState(GTState newState)
    {
        state = newState;
        if (newState == GTState.HeadingHome)
        {
            nma.isStopped = false;
            nma.SetDestination(startPosition.transform.position);
            nma.speed = 3;
            nma.acceleration = 3;
            nma.updatePosition = true;
            nma.isStopped = false;
        }
        else if(newState == GTState.RotatingTowardHome)
        {
            nma.isStopped = false;
            StopCoroutine(RotateTowardsCoroutine);
            StopCoroutine(ChargeCoroutine);
            RotateTowardsCoroutine = RotateTowards(startPosition);
            StartCoroutine(RotateTowardsCoroutine);
        }

        else if (newState == GTState.RotatingTowardPlayer)
        {
            nma.isStopped = false;
            StopCoroutine(RotateTowardsCoroutine);
            StopCoroutine(ChargeCoroutine);
            RotateTowardsCoroutine = RotateTowards(Player);
            StartCoroutine(RotateTowardsCoroutine);
        }

        else if (newState == GTState.Charging)
        {
            nma.isStopped = false;
            StopCoroutine(RotateTowardsCoroutine);
            StopCoroutine(ChargeCoroutine);
            ChargeCoroutine = Charge(transform.forward);
            StartCoroutine(ChargeCoroutine);

        }

        else if (newState == GTState.Stunned)
        {
            nma.isStopped = true;
            StopCoroutine(RotateTowardsCoroutine);
            StopCoroutine(ChargeCoroutine);
            StopCoroutine(StunCoroutine);
            StunCoroutine = Stun();
            StartCoroutine(StunCoroutine);
        }
    }

    IEnumerator Charge(Vector3 direction)
    {
        while (true)
        {
            nma.Move(direction * chargingVelocity * Time.deltaTime);
            yield return null;
        }

    }


    IEnumerator RotateTowards(GameObject target)
    {
        float nextActionTime = Time.time + .2f;
        Vector3 TargetDirection = (target.transform.position - transform.position);
        Quaternion rot = Quaternion.LookRotation(TargetDirection);


        while (true)
        {
            if (Time.time > nextActionTime)
            {
                Debug.Log("target is updating");

                nextActionTime += .2f;
                Vector3 GroundPosition = new Vector3(transform.position.x, 0f, transform.position.y);



                TargetDirection = (target.transform.position - transform.position);
                TargetDirection.y = 0;
                Debug.DrawRay(GroundPosition, TargetDirection, Color.red, 1f);
                rot = Quaternion.LookRotation(TargetDirection);
            }

            float step = nma.angularSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, step);
            transform.eulerAngles = new Vector3(0f, transform.rotation.eulerAngles.y, 0f);
            //I should be removing the y component up above when I calculate rot;

            yield return null;
        }
    }


    private IEnumerator Stun()
    {
        nma.enabled = false;
        float stunnedStart = Time.time;
        IsStunned = true;
        Rigidbody rb = GetComponent<Rigidbody>();
        rb.useGravity = true;
        rb.isKinematic = false;
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

        PickUpZone.GetComponent<InteractionAttributes>().CanPickUp = true;

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
        PickUpZone.GetComponent<InteractionAttributes>().CanPickUp = false;

        rb.useGravity = false;
        rb.isKinematic = true;
        rb.velocity = Vector3.zero;
        IsStunned = false;
        UpdateState(GTState.RotatingTowardHome);
        nma.enabled = true;

    }

}
