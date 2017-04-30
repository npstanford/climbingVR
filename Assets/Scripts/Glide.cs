using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{

    public GameObject GliderModel;
    //public BoxCollider OverheadCollider;
    //public GameObject OverheadCollider; // the invisible box collider over the players head
    public Rigidbody Room;
    public float GlideSpeed;
    public float GlideFallSpeed;
    public Vector3 GliderOffset;
    public bool IsGliding;
    public float GripDepletion;
    public ColliderManager cm;


   // private MeshRenderer _gliderRenderer;
    private ColliderManager Body;


    // Use this for initialization
    void Start()
    {
        //_gliderRenderer = GliderModel.GetComponent<MeshRenderer>();
        //_gliderRenderer.enabled = false;
        GliderModel.SetActive(false);
        IsGliding = false;
        Body = Room.GetComponent<ColliderManager>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    /*
    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        GliderModel.SetActive(true);


        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead)
        {
            IsGliding = true;

            Vector3 glideVector = controller.transform.right;
            glideVector = glideVector.normalized; // glideVector is not yaw. It is the direction of the nose

            int forwardOrBackward = 1; //1 represents forward
            if (controller.transform.right.y > 0)
            {
                forwardOrBackward = -1;
            }

            //this is the idea to basically smooth the transition from forward to backward
            glideVector.y = Mathf.Pow(glideVector.y, 2);

            if(glideVector.y < .1f)
            {
                glideVector = Vector3.zero;
            }

            glideVector = glideVector.normalized;

            //float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z) , controller.transform.forward);
            Vector3 rollDirection = new Vector3(0.0f, 0.0f, controller.transform.up.z).normalized;
            //rollDirection.z = Mathf.Pow(rollDirection.z, 2);
            rollDirection = rollDirection.normalized;


            glideVector = -rollDirection + (glideVector * forwardOrBackward);
            //glideVector = glideVector * forwardOrBackward;
            glideVector = glideVector.normalized;


            Vector3 glideVelocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed;

            glideVelocity.y = Mathf.Min(GlideFallSpeed * -0.6f, glideVelocity.y);

            glideVelocity += Body.WindVelocity;


            Room.position += glideVelocity * Time.deltaTime;
        }
    }
    */
    
    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        GliderModel.SetActive(true);


        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead )
        {
            IsGliding = true;

            Vector3 glideVector = controller.transform.right;
            glideVector = glideVector.normalized; // glideVector is not yaw. It is the direction of the nose


            float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z)
                , controller.transform.forward);


            int upOrDownRoll = 1;

            if (controller.transform.forward.y > 0)
            {
                upOrDownRoll = -1;
            }


                glideVector = Quaternion.AngleAxis(rollAngle* upOrDownRoll, Vector3.up) * glideVector;



            Vector3 glideVelocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed;

            glideVelocity.y = Mathf.Min(GlideFallSpeed*-0.6f, glideVelocity.y);

            glideVelocity += Body.WindVelocity;

            RaycastHit hit;
            if(Physics.Raycast(cm.displayCube.transform.position, glideVelocity, out hit, .2f))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia!=null)
                {
                    if(ia.CanClimb || ia.IsGround)
                    {
                        glideVelocity = Vector3.down * GlideFallSpeed;
                    }
                }
            }

            Room.position += glideVelocity * Time.deltaTime;
           } else // note this is hacky... should probably refactor so there is a "show glider" method that input manager calls when buttons pressed, and a start gliding method
        {
            IsGliding = false;
        }
    }
  

    public void StopGliding()
    {
        //_gliderRenderer.enabled = false;
        GliderModel.SetActive(false);
        //Room.useGravity = true;
        Room.velocity = new Vector3(Room.velocity.x*0.2f, Mathf.Min(Room.velocity.y, 0), Room.velocity.z*0.2f);
        IsGliding = false;
    }




}
