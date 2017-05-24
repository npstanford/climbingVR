using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.ImageEffects;

public class InputManager : MonoBehaviour {
    public enum Capabilities { Climb, Hookshot, Glide};

    public bool EnableAllComponents = true;
    //public bool RunInPlace = false;
    public ControllerState lController;
    public ControllerState rController;
    public Rigidbody Body; //note this should more accurately be understood as Room and not body
    //public BoxCollider OverheadCollider;
    public GripTool GripToolLeft;
    public GripTool GripToolRight;
    public GameObject PlayerHead;
    public BlurOptimized bo;
    public int NotTouchingGroundCount; //for smoothing otu not touching ground during running


    public float FallAcceleration;
    private float FallVelocity;

    //booleans for capabilities
    private bool ClimbingEnabled = false;
    private bool HookshotEnabled= false;
    private bool GlidingEnabled = false;

    //stunning
    private bool PlayerIsStunned = false;

    //private OverheadCollider ohc;

    public float walkHookDiff = 1.0f;

    //capabilities
    private Walk walk;
    public Climb climb;
    public Hookshot hookshot;
    public Glide glide;
    private Run run;
    private GripMeter gm;
    private PickUp pu;

    //inputs
    private ColliderManager cm;
    private BodyMovementCollector bmc; 

    private float lTouchpadPressTime;
    private float rTouchpadPressTime;

    public bool PlayerIsTouchingGround;



    // Use this for initialization
    void Start () {
        walk = GetComponent<Walk>();
        climb = GetComponent<Climb>();
        hookshot = GetComponent<Hookshot>();
        glide = GetComponent<Glide>();
        run = GetComponent<Run>();
        gm = GetComponent<GripMeter>();
        cm = Body.GetComponent<ColliderManager>();
        pu = GetComponent<PickUp>();
        bmc = FindObjectOfType<BodyMovementCollector>();

        if (!EnableAllComponents)
        {

            GripToolLeft.HideHook();
            GripToolRight.HideHook();
        }

    }

    // Update is called once per frame
    void Update() {

        if (EnableAllComponents)
        {
            EnableCapability(Capabilities.Climb);
            EnableCapability(Capabilities.Hookshot);
            EnableCapability(Capabilities.Glide);
            EnableAllComponents = false;
        }

        PlayerIsTouchingGround = cm.PlayerIsTouchingGround;
        PlayerIsStunned = cm.PlayerIsStunned;

        //GripManager
        if (climb.IsClimbing)
        {
            gm.DepleteGrip(climb.GripDepletion);
        }
        else if (glide.IsGliding)
        {
            gm.DepleteGrip(glide.GripDepletion);
        } else if (PlayerIsTouchingGround && !PlayerIsStunned)
        {
            //TODO make this contingent on the player being on the ground
            gm.RestoreGrip();
        }

        if (gm.RemainingGrip > 0.0f && !PlayerIsStunned)
        {
            //shoooting
            if (HookshotEnabled && rController.Holding == null) { CheckShooting(rController); }


            //climbing
            if (ClimbingEnabled)
            {
                if (rController.Holding == null) { CheckClimbing(rController); };
                if (lController.Holding == null) { CheckClimbing(lController); };
            }
            //gliding
            if (GlidingEnabled && lController.Holding == null) { CheckGliding(lController); }
        }
        else {
            climb.Drop(Body);
            glide.StopGliding(PlayerIsStunned);
        }

        if (PlayerIsStunned)
        {
            DropEverything();
        }


        if (lController.device == null || rController.device == null) {
            Debug.Log("Device is null");
            return;
        }

        //I think this should allow players to either run in place or click button to step
        //if (RunInPlace)
        //{
            CheckRunning();
        //} else
        //{
            CheckWalking(rController, ref rTouchpadPressTime);
            CheckWalking(lController, ref lTouchpadPressTime);
        //}

        CheckPickUp(rController);
        CheckPickUp(lController);

        

        //HACK -- problem is users could slip out of gripping blocks without letting up on the trigger

        if ((!rController.canGrip && !lController.canGrip) && climb.IsClimbing)
        {


                climb.Drop(Body);

        }

        if (lController.device.GetPress(SteamVR_Controller.ButtonMask.Grip) && GlidingEnabled)
        {
            GripToolLeft.HideHook();
        } else if (ClimbingEnabled)
        {
            //GripToolLeft.ShowHook();
        }

        if (hookshot.Grapple.HookshotFired)
        {
            //GripToolRight.HideHook();
        } else if (ClimbingEnabled)
        {
            //GripToolRight.ShowHook();
        }

        if (!PlayerIsTouchingGround && !climb.IsClimbing && !glide.IsGliding)
        {
     
            Fall();
        } else
        {
            FallVelocity = 0;
            bo.blurSize = 0;
        }

    }

    private void FixedUpdate()
    {
 
    }


    void CheckWalking(ControllerState controller, ref float touchpadPressTime)
    {
        if (controller.device == null) { return; }
        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Touchpad))
        {

            touchpadPressTime = Time.time; //aht he bug here is that I am pulling from Time.time and not taking the diff. I shoudl take the diff here
        }

        if (controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && PlayerIsTouchingGround)
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

    void CheckPickUp(ControllerState controller)
    {
        if (controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger)
    || controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            pu.Grab(controller);
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


        if (lController.device.GetPress(SteamVR_Controller.ButtonMask.Grip))
        {
            glide.StartGliding(lController, PlayerIsTouchingGround, CheckOverheadHand(lController));
        }
        else if (lController.device.GetPressUp(SteamVR_Controller.ButtonMask.Grip))
        {
            glide.StopGliding();
        }

    }


    void CheckRunning()
    {
        /* I dont think this code does anything useful so I commented it out to see if anything breaks
        if (!PlayerIsTouchingGround)
        {
            NotTouchingGroundCount += 1;
            if (NotTouchingGroundCount > 20)
            {
                return;
            }
        } else
        {
            NotTouchingGroundCount = 0;
        }
        */


        if (bmc.Head.speed > run.minHeadSpeedToRun && !bmc.NonRunningHeadMovement)
        {
            Vector3 avgDirection = (bmc.Head.direction + bmc.RightArm.direction + bmc.LeftArm.direction) / 3;
            avgDirection = avgDirection.normalized;

            float avgSpeed = .5f * bmc.Head.speed + .25f *bmc.RightArm.speed + .25f * bmc.LeftArm.speed;

            run.Step(avgDirection, avgSpeed);
        } else
        {
            run.Stop();
        }

    }

    bool CheckOverheadHand(ControllerState controller)
    {
        //BoxCollider controllerCollider = controller.controller.GetComponent<BoxCollider>();
        //return controllerCollider.bounds.Intersects(OverheadCollider.bounds);

        return (controller.transform.position.y > PlayerHead.transform.position.y - .4);

    }


    public void EnableCapability(Capabilities cap)
    {
        if (cap == Capabilities.Climb)
        {
            ClimbingEnabled = true;
            GripToolLeft.DisplayClimbingComponents();
            GripToolRight.DisplayClimbingComponents();
        }

        if (cap == Capabilities.Hookshot)
        {
            HookshotEnabled = true;
            GripToolRight.DisplayHookshotComponents();
        }

        if (cap == Capabilities.Glide)
        {
            GlidingEnabled = true;
            GripToolLeft.DisplayGliderComponents();
        }

    }

    public void Fall()
    {
        //FallVelocity += FallAcceleration * Time.deltaTime;
        //FallVelocity = Mathf.Min(FallVelocity, 30);
        float FallVelocity = Body.velocity.y;
        bo.blurSize = FallVelocity / 3; // blursize ranges from 0 to 10. Fall Velocity from 0 to 20. So this scales linearly.
        //Body.transform.position += Vector3.down * FallVelocity * Time.deltaTime;
    }

    //I don't like this. I had to write this though so that I can grab the golden spheres out of the players hands
    public void DropEverything()
    {
        pu.Drop(lController);
        pu.Drop(rController);
    }
}
