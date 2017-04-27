using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ColliderManager : MonoBehaviour {
    /*
     * Originally, this class ONLY implemented logic to move the box collider around inside of the camera rig
     * I am extending it now to also detect when that collider is touching ground and to update the player's status as such
     * 
     */ 


    public BoxCollider playerCollider;
    //public GameObject OverheadCollider;
    public GameObject GroundedCollider;

    public GameObject playerHead;
    public Vector3 WindVelocity;
    //public float OverheadColliderHeight;
    public bool PlayerIsTouchingGround;
    public bool PlayerIsStunned;
    public float StunLength;
    public Image InjuredMask;

    private IEnumerator StunCoroutine;


    //stun info
    public float ImpactDistance;

    public GameObject displayCube;
    public GameController gc;

    public Teleporter teleporter;

    public Transform Room; // add this as an attempt to rewrite sticking to platforms

	// Use this for initialization
	void Start () {
        //Room = playerCollider.gameObject.transform;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    void FixedUpdate ()
    {
        //assumption here is that in the local coordinates of the rig that the ground is a y=0
        Vector3 colliderCenter = playerHead.transform.localPosition;
        colliderCenter.y = colliderCenter.y / 2.0f;
        playerCollider.center = colliderCenter;

        playerCollider.size = new Vector3(0.2f, colliderCenter.y * 2f, 0.2f);

        //OverheadCollider.transform.position = playerHead.transform.position + Vector3.up * OverheadColliderHeight;
        GroundedCollider.transform.localPosition = new Vector3(colliderCenter.x, 0f, colliderCenter.z);

        //update display cube for testing purposes
        displayCube.transform.localPosition = colliderCenter;
        displayCube.transform.localScale = playerCollider.size;
    }

 


    private void OnTriggerStay(Collider other)
    {
        Wind wind = other.GetComponent<Wind>();


        if (wind != null)
        {
            WindVelocity = wind.WindVelocity;
        }

        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.IsGround) // <--- HACKY... 
            {



                PlayerIsTouchingGround = true;
                /*
                TeleportLocation tl = other.collider.GetComponent<TeleportLocation>();
                if (tl != null)
                {
                    teleporter.LastTeleportLocation = tl;
                }
                */
            }
            else if (ia.HurtsPlayer)
            {
                Vector3 hitDirection = (playerCollider.center- other.transform.position).normalized;
                if (PlayerIsTouchingGround)
                {
                    hitDirection.y = 0.0f;
                }

                if (StunCoroutine != null) { 
                    StopCoroutine(StunCoroutine);
                }
                StunCoroutine = StunPlayerCoroutine(hitDirection);
                StartCoroutine(StunCoroutine);

            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        Wind wind = other.GetComponent<Wind>();


        if (wind != null)
        {
            WindVelocity = Vector3.zero;
        }

        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.IsGround) // <--- HACKY
            {


                PlayerIsTouchingGround = false;

            }
        }
    }


    IEnumerator StunPlayerCoroutine(Vector3 hitDirection)
    {
        float stunnedStart = Time.time;
        PlayerIsStunned = true;
        //make screen red or something

        InjuredMask.color = Color.red;
        //while player is blinded, hit them backwards from impact
        RaycastHit hit;
        if (Physics.Raycast(playerCollider.center, hitDirection, out hit, ImpactDistance))
        {

            Room.transform.position += hit.point - playerCollider.center;
        } else
        {
            Room.transform.position += hitDirection * ImpactDistance;
        }

            while ((Time.time - stunnedStart) < StunLength)
        {
            
            InjuredMask.color = Color.Lerp(Color.red, Color.clear, (Time.time - stunnedStart) / StunLength);
            yield return null;
        }

        PlayerIsStunned = false;
    }

}
