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
    public float RealPlayerHeight;
    public float CurrentPlayerHeight;
    public GameObject playerHead;
    public Vector3 WindVelocity;
    //public float OverheadColliderHeight;
    public bool PlayerIsTouchingGround;
    public bool PlayerIsStunned;
    public bool PlayerIsJumping;
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
        RaycastHit hit; //err I could and should pull this out of FixedUpdate and put it in start()
        LayerMask layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;
        PlayerIsTouchingGround = false; //so that if we don't hit on the raycast, it will remain false
        if (Physics.Raycast(playerHead.transform.position, Vector3.down, out hit, CurrentPlayerHeight, layerMask))
        {
            InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();

            if (ia != null)
            {
                if (ia.IsGround && !ia.CanPickUp)
                {
                    PlayerIsTouchingGround = true;
                    if (CurrentPlayerHeight - hit.distance > .01)
                    {
                        float HeightAdjustment = Room.transform.position.y + (CurrentPlayerHeight - hit.distance);

                        Room.transform.position = new Vector3(Room.transform.position.x, HeightAdjustment, Room.transform.position.z);
                    }
                }
            }

        }
    }

    void FixedUpdate ()
    {
        //assumption here is that in the local coordinates of the rig that the ground is a y=0
        //CurrentPlayerHeight = Mathf.Min(playerHead.transform.localPosition.y, RealPlayerHeight);
        Vector3 colliderCenter = playerHead.transform.localPosition;

        if (playerHead.transform.localPosition.y > RealPlayerHeight)
        {
            CurrentPlayerHeight = RealPlayerHeight;
            colliderCenter.y = (colliderCenter.y + (playerHead.transform.localPosition.y / RealPlayerHeight)) / 2.0f;
            PlayerIsJumping = true;
        } else
        {
            CurrentPlayerHeight = playerHead.transform.localPosition.y;
            colliderCenter.y = colliderCenter.y / 2.0f;
            PlayerIsJumping = false;
        }

        playerCollider.center = colliderCenter;


        //playerCollider.size = new Vector3(0.2f, colliderCenter.y * 2f, 0.2f);
        playerCollider.size = new Vector3(0.2f, RealPlayerHeight, 0.2f);

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



                //PlayerIsTouchingGround = true;
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
                Vector3 playerLocation = Room.transform.TransformPoint(playerCollider.center);
                Vector3 hitDirection = (playerLocation) - other.transform.position;
                Rigidbody rb = other.GetComponent<Rigidbody>();
                if (rb == null) { hitDirection = Vector3.zero; }
                else {
                    hitDirection = rb.velocity;
                }
                if (PlayerIsTouchingGround)
                {
                    hitDirection.y = 0.0f;
                }

                if (StunCoroutine != null) { 
                    StopCoroutine(StunCoroutine);
                }
                StunCoroutine = StunPlayerCoroutine(playerLocation, hitDirection);
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


               // PlayerIsTouchingGround = false;

            }
        }
    }


    IEnumerator StunPlayerCoroutine(Vector3 playerLocation, Vector3 hitDirection)
    {
        hitDirection = hitDirection.normalized;
        float stunnedStart = Time.time;
        PlayerIsStunned = true;
        


        //make screen red or something
        //Debug.DrawRay(playerLocation, hitDirection, Color.black, 30f);
        InjuredMask.color = Color.red;


        if (hitDirection != Vector3.zero)
        {
            //while player is blinded, hit them backwards from impact
            RaycastHit hit;
            if (Physics.Raycast(playerLocation, hitDirection, out hit, ImpactDistance))
            {

                Room.transform.position += hit.point - playerLocation;
            }
            else
            {
                Room.transform.position += hitDirection * ImpactDistance;
            }
        }
            while ((Time.time - stunnedStart) < StunLength)
        {
            
            InjuredMask.color = Color.Lerp(Color.red, Color.clear, (Time.time - stunnedStart) / StunLength);
            yield return null;
        }

        PlayerIsStunned = false;
    }




}
