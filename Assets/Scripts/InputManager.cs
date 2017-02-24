using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour {
    public ControllerState lController;
    public ControllerState rController;
    public Rigidbody Body;

    public float walkHookDiff = 1.0f;

    //capabilities
    private Walk walk;
    private Climb climb;
    private Hookshot hookshot;

    private float lTouchpadPressTime;
    private float rTouchpadPressTime;

    // Use this for initialization
    void Start () {
        walk = GetComponent<Walk>();
        climb = GetComponent<Climb>();
        hookshot = GetComponent<Hookshot>();
	}

    // Update is called once per frame
    void Update() {

        if (lController.device == null || rController.device == null) {
            Debug.Log("Device is null");
            return;
        }

        //walking
        CheckWalking(rController, ref rTouchpadPressTime);
        CheckWalking(lController, ref lTouchpadPressTime);

        //shoooting
        CheckShooting(rController, ref rTouchpadPressTime);
        CheckShooting(lController, ref lTouchpadPressTime);

        //climbing
        CheckClimbing(rController);
        CheckClimbing(lController);

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

    void CheckShooting(ControllerState controller, ref float touchpadPressTime)
    {
        if (touchpadPressTime > walkHookDiff)
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
}
