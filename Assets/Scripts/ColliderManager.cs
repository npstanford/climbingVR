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
    public float MaxSlope;
    public float CurrentPlayerHeight;
    public GameObject playerHead;
    public Vector3 WindVelocity;
    //public float OverheadColliderHeight;
    public bool PlayerIsTouchingGround;
    public bool PlayerIsStunned;
    public bool PlayerIsJumping;
    public float StunLength;
    public GripTool GripToolLeft;
    public GripTool GripToolRight;
    public Image InjuredMask;
    private Vector3 prevRoomPosition;
    private Vector3 curRoomPosition;
    public InputManager im;
    public BlurVisionInWalls Head;
    public PlayerLandingSounds playerLandingSounds;


    private IEnumerator StunCoroutine;


    //stun info
    public float ImpactDistance;

    public GameObject displayCube;
    public Teleporter teleporter;

    public Transform Room; // add this as an attempt to rewrite sticking to platforms
    private Rigidbody rb;

	// Use this for initialization
	void Start () {
        //Room = playerCollider.gameObject.transform;
        rb = Room.GetComponent<Rigidbody>();
        prevRoomPosition = Room.transform.position;
        curRoomPosition = Room.transform.position;
        //RealPlayerHeight = playerHead.transform.position.y;


    }

    // Update is called once per frame
    void Update()
    {

        prevRoomPosition = curRoomPosition;
        curRoomPosition = Room.transform.position;


        RaycastHit hit; //err I could and should pull this out of FixedUpdate and put it in start()
        LayerMask layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;

        bool PlayerWasTouchingGround = PlayerIsTouchingGround;

        PlayerIsTouchingGround = false; //so that if we don't hit on the raycast, it will remain false

        //raycast to determine if touching ground and to adjust y position if necessary
        if (Physics.Raycast(playerHead.transform.position, Vector3.down, out hit, CurrentPlayerHeight + .1f, layerMask))
        {
            InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();

            if (ia != null)
            {
                if (ia.IsGround && !(ia.CanPickUp && hit.collider.transform.IsChildOf(transform)))
                {
                    if (!PlayerWasTouchingGround)
                    {
                        playerLandingSounds.PlayerLanded();
                    }
                    PlayerIsTouchingGround = true;
                    float AmountPlayerUnderGround = CurrentPlayerHeight - hit.distance;
                    if (AmountPlayerUnderGround > .01)
                    {
                        // instead of not doing anything if below max slope... just raise only by max slope

                        float HeightAdjustment = Room.transform.position.y + Mathf.Min(CurrentPlayerHeight / 2, (CurrentPlayerHeight - hit.distance));

                        if (!Head.HeadInWall)
                        {
                            Room.transform.position = new Vector3(Room.transform.position.x, HeightAdjustment, Room.transform.position.z);
                            // below code is supposed to help players get over ledges... not sure if it will work
                            Vector3 PlayerDirection = playerHead.transform.forward;
                            Room.transform.position += new Vector3(PlayerDirection.x, 0.0f, PlayerDirection.z).normalized * .1f;
                        }
                    }
                }
            }

        }

        /*
        //Raycast to keep you from smashing into walls. Note, this will 100% prevent players from approaching walls. If 
        if (!im.climb.IsClimbing)
        {
            float BubbleSize = 3f;
            Debug.DrawRay(playerHead.transform.position, playerHead.transform.forward * BubbleSize, Color.red);
            if (Physics.Raycast(playerHead.transform.position, playerHead.transform.forward, out hit, BubbleSize, layerMask))
            {
                Debug.Log("backup raycast hit:" + hit.collider.gameObject.name);
                Room.transform.position = prevRoomPosition;


            }
        }
        */
    }

    void FixedUpdate ()
    {
        //assumption here is that in the local coordinates of the rig that the ground is a y=0
        //CurrentPlayerHeight = Mathf.Min(playerHead.transform.localPosition.y, RealPlayerHeight);
        Vector3 colliderCenter = playerHead.transform.localPosition;

        if (playerHead.transform.localPosition.y > RealPlayerHeight)
        {
            CurrentPlayerHeight = RealPlayerHeight;
            //colliderCenter.y = (colliderCenter.y + (playerHead.transform.localPosition.y / RealPlayerHeight)) / 2.0f;
            colliderCenter.y = playerHead.transform.localPosition.y - (RealPlayerHeight / 2.0f);
            PlayerIsJumping = true;
        } else
        {
            CurrentPlayerHeight = playerHead.transform.localPosition.y;
            colliderCenter.y = colliderCenter.y / 2.0f;
            PlayerIsJumping = false;
        }

        playerCollider.center = colliderCenter;

        GroundedCollider.transform.localPosition = new Vector3(playerCollider.center.x, playerCollider.center.y - CurrentPlayerHeight / 2,
            playerCollider.center.z);

        //playerCollider.size = new Vector3(0.2f, colliderCenter.y * 2f, 0.2f);
        playerCollider.size = new Vector3(0.2f, CurrentPlayerHeight, 0.2f);

        //update display cube for testing purposes
        displayCube.transform.localPosition = colliderCenter;
        displayCube.transform.localScale = playerCollider.size;



    }

 


    private void OnTriggerStay(Collider other)
    {
        Wind wind = other.GetComponent<Wind>();


        if (wind != null)
        {
            if (!other.transform.IsChildOf(transform))
            {
                // so wind blocks can't push you while gliding.

                WindVelocity = wind.WindVelocity;
            }
            else
            {
                WindVelocity = Vector3.zero;
            }
        }

        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
        if (ia != null)
        {
            if (ia.HurtsPlayer)
            {

                PlayerHit(other.gameObject);
            }
            else if (ia.PushesPlayer)
            {
                //rb.isKinematic = false;
            }
        }
    }

    public void PlayerHit(GameObject other)
    {
        Vector3 playerLocation = Room.transform.TransformPoint(playerCollider.center);
        Vector3 hitDirection = (playerLocation) - other.transform.position;
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (rb == null) { hitDirection = Vector3.zero; }
        else
        {
            hitDirection = rb.velocity;
        }
        if (PlayerIsTouchingGround)
        {
            hitDirection.y = 0.0f;
        }

        if (StunCoroutine != null)
        {
            StopCoroutine(StunCoroutine);
        }
        StunCoroutine = StunPlayerCoroutine(playerLocation, hitDirection);
        StartCoroutine(StunCoroutine);
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
            if (ia.PushesPlayer) // <--- HACKY
            {
                //rb.isKinematic = true;

               // PlayerIsTouchingGround = false;

            }
        }
    }


    IEnumerator StunPlayerCoroutine(Vector3 playerLocation, Vector3 hitDirection)
    {
        GripToolLeft.HideHook();
        GripToolRight.HideHook();
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
            
            InjuredMask.color = Color.Lerp(Color.red, Color.clear, ((Time.time - stunnedStart) / StunLength)*.4f);
            yield return null;
        }

        InjuredMask.color = Color.clear;

        PlayerIsStunned = false;
        GripToolLeft.ShowHook();
        GripToolRight.ShowHook();
    }




}
