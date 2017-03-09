using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Glide : MonoBehaviour
{

    public GameObject GliderModel;
    private GameController gc;
    //public GameObject OverheadCollider; // the invisible box collider over the players head
    public Rigidbody Room;
    public float GlideSpeed;
    public float GlideFallSpeed;
    public Vector3 GliderOffset;


    private MeshRenderer _gliderRenderer;


    // Use this for initialization
    void Start()
    {
        _gliderRenderer = GliderModel.GetComponent<MeshRenderer>();
        _gliderRenderer.enabled = false;
        GameObject go = GameObject.FindGameObjectWithTag("GameController");
        gc = go.GetComponent<GameController>();
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void StartGliding(ControllerState controller)
    {
        _gliderRenderer.enabled = true;


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

        if (!gc.PlayerIsTouchingGround)
        {
            gc.FallingState = GameController.FallingStates.Gliding;


            Vector3 glideVector = controller.transform.right;
            glideVector.y = 0.0f;
            glideVector = glideVector.normalized;

            /*
            if (usingRightController)
            {
                glideVector = glideVector * -1.0f;
            }
            */

            Room.useGravity = false;

            //this is the part where we figure out how the tilt affects the bearing vector

            float tiltAngle = Vector3.Angle(new Vector3(controller.transform.forward.x, 0.0f, controller.transform.forward.z)
                , controller.transform.forward);

            Debug.Log("tilt angle: " + tiltAngle);

            int upOrDown = 1;

            if (controller.transform.forward.y > 0)
            {
                upOrDown = -1;
            }

            /*
            if (usingRightController)
            {
                glideVector = Quaternion.AngleAxis(-tiltAngle * upOrDown, Vector3.up) * glideVector;
            } else */
            {
                glideVector = Quaternion.AngleAxis(tiltAngle * upOrDown, Vector3.up) * glideVector;

            }




            Room.velocity = glideVector * GlideSpeed + Room.transform.up * (-1.0f) * GlideFallSpeed; 
        }
    }

    public void StopGliding()
    {
        _gliderRenderer.enabled = false;
        Room.useGravity = true;
        //Room.velocity = new Vector3(0.0f, Room.velocity.y, 0.0f);
        //gc.FallingState = GameController.FallingStates.
    }


}
