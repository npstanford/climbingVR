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

    public void StartGliding(ControllerState controller, bool PlayerIsTouchingGround, bool HandIsOverHead)
    {
        GliderModel.SetActive(true);
        //_gliderRenderer.enabled = true;



        /* this is all the logic on where to put the glider model. I don't need while it is limited to just one hand. 
        bool usingRightController = false;
        if (controller.device.index == SteamVR_Controller.GetDeviceIndex(SteamVR_Controller.DeviceRelation.Rightmost))
        {
            usingRightController = true;
        }


        if (usingRightController) 
        {
            GliderModel.transform.localRotation = Quaternion.Euler(0.0f, 225.0f, 0.0f);
            GliderModel.transform.localPosition = new Vector3(GliderOffset.x * -1.0f, GliderOffset.y, GliderOffset.z * -1.0f);


        }
        else
        { 
            GliderModel.transform.localRotation = Quaternion.Euler(0.0f, 45.0f, 0.0f);
            GliderModel.transform.localPosition = GliderOffset;

        }


        GliderModel.transform.parent = controller.transform; //todo this may need to happen earlier

        */

        if ((!PlayerIsTouchingGround || Body.WindVelocity != Vector3.zero) && HandIsOverHead )
        {
            IsGliding = true;

            Vector3 glideVector = controller.transform.right;
            //glideVector.y = 0.0f;
            glideVector = glideVector.normalized; // glideVector is not yaw. It is the direction of the nose

            /*
            if (usingRightController)
            {
                glideVector = glideVector * -1.0f;
            }
            */

            Room.useGravity = false;

            //this is the part where we figure out how the tilt affects the bearing vector

            float rollAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z)
                , controller.transform.forward);

            //Debug.Log("roll: " + roll);

            int upOrDownRoll = 1;

            if (controller.transform.forward.y > 0)
            {
                upOrDownRoll = -1;
            }

            /*
            //this code is figure out if we should be going fast or slow by how much we tilt the glider up and down
            float pitch = Vector3.Angle(new Vector3(controller.transform.right.x, 0.0f, controller.transform.right.z), controller.transform.right);

            Debug.Log("pitch: " + pitch);

            int upOrDownPitch = 1;

            if (controller.transform.right.y > 0) { 
            
                upOrDownPitch = -1;
            }
            */

            /*
            if (usingRightController)
            {
                glideVector = Quaternion.AngleAxis(-tiltAngle * upOrDown, Vector3.up) * glideVector;
            } else */
            {
                glideVector = Quaternion.AngleAxis(rollAngle* upOrDownRoll, Vector3.up) * glideVector;

            }


            Vector3 glideVelocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed;

            glideVelocity.y = Mathf.Min(GlideFallSpeed*-0.6f, glideVelocity.y);

            glideVelocity += Body.WindVelocity;

            Room.velocity = glideVelocity;
                }
    }

    public void StopGliding()
    {
        //_gliderRenderer.enabled = false;
        GliderModel.SetActive(false);
        Room.useGravity = true;
        Room.velocity = new Vector3(Room.velocity.x*0.2f, Mathf.Min(Room.velocity.y, 0), Room.velocity.z*0.2f);
        IsGliding = false;
    }




}
