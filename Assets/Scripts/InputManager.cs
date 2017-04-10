using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public bool RunInPlace = false;
    public ControllerState lController;
    public ControllerState rController;
    public Rigidbody Body; //note this should more accurately be understood as Room and not body
    public BoxCollider OverheadCollider;
    public BoxCollider RunningColliderTop;
    public BoxCollider RunningColliderBottom;

    //private OverheadCollider ohc;

    public float walkHookDiff = 1.0f;

    //capabilities
    private Walk walk;
    private Climb climb;
    private Hookshot hookshot;
    private Glide glide;
    private Run run;

    private float lTouchpadPressTime;
    private float rTouchpadPressTime;


    // Use this for initialization
    void Start () {
        walk = GetComponent<Walk>();
        climb = GetComponent<Climb>();
        hookshot = GetComponent<Hookshot>();
        glide = GetComponent<Glide>();
        run = GetComponent<Run>();

	}

    // Update is called once per frame
    void Update() {

        if (lController.device == null || rController.device == null) {
            Debug.Log("Device is null");
            return;
        }

        if (RunInPlace)
        {
            CheckRunning(lController);
        } else
        {
            CheckWalking(rController, ref rTouchpadPressTime);
            CheckWalking(lController, ref lTouchpadPressTime);
        }


        //shoooting
        CheckShooting(rController);
        //CheckShooting(lController, ref lTouchpadPressTime);

        //climbing
        CheckClimbing(rController);
        CheckClimbing(lController);

        //gliding
        CheckGliding(lController);

        

        //HACK -- problem is users could slip out of gripping blocks without letting up on the trigger
        if (!rController.canGrip && !lController.canGrip)
        {
            climb.Drop(Body);
        }


    }

    void CheckWalking(ControllerState controller, ref float touchpadPressTime)
    {
        if (controller.device == null) { return; }
        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {

            touchpadPressTime = Time.time; //aht he bug here is that I am pulling from Time.time and not taking the diff. I shoudl take the diff here
        }

        if (controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            
            // if the touch was quick enough, take a step
            if (Time.time - touchpadPressTime < walkHookDiff)
            {
                walk.Step(controller.controller);
            }

            touchpadPressTime = 0.0f;
        }
    }

    void CheckClimbing(ControllerState controller)
    {
        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger)
    || controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            climb.Grab(controller, Body);
        }
    }

    void CheckShooting(ControllerState controller)
    {
        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {

            hookshot.Scan(controller);
            if(controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                hookshot.Shoot(controller);
            }
        }
        if (controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            hookshot.StopScan();
        }
    }


    void CheckGliding(ControllerState lController)
    {
        //TODO change logic so that we are only doing this for left controller


        /*
            if (CheckOverheadHand(rController) && rController.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
            {
                glide.StartGliding(rController);
            } else 
            */

        /*
            
            if (CheckOverheadHand(lController) && lController.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
            {
                glide.StartGliding(lController);
            } else
            {
                glide.StopGliding();
            }

        */

        if (lController.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            glide.StartGliding(lController);
        }
        else if (lController.device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            glide.StopGliding();
        }

    }

    void CheckRunning(ControllerState controller)
    {
        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            run.Step(controller);
        }

        if (controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            run.Stop();
        }

    }


    bool CheckOverheadHand(ControllerState controller)
    {
        BoxCollider controllerCollider = controller.controller.GetComponent<BoxCollider>();
        return controllerCollider.bounds.Intersects(OverheadCollider.bounds);

    }

    /*

    bool CheckRunningTop(ControllerState controller)
    {
        BoxCollider controllerCollider = controller.controller.GetComponent<BoxCollider>();
        return controllerCollider.bounds.Intersects(RunningColliderTop.bounds);

    }


    bool CheckRunningBottom(ControllerState controller)
    {
        BoxCollider controllerCollider = controller.controller.GetComponent<BoxCollider>();
        return controllerCollider.bounds.Intersects(RunningColliderBottom.bounds);

    }

    */
}
