using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class GripManager : MonoBehaviour {

    public Rigidbody Body;

    public ControllerState lController;
    public ControllerState rController;

    public GameController gc;

    public bool EnableGripMeter = true;
    public float GripStrengthMax;
    public float GripStrength;
    public float GripDepletionFactor;
    public float GripRecoveryFactor;

    public float JumpDampFactor;


    // Use this for initialization
    void Start() {
        Body.useGravity = true;
        Body.isKinematic = false;

        GripStrength = GripStrengthMax;
    }

    // Update is called once per frame
    void Update() {

    }



    void FixedUpdate()
    {

        if (lController.canGrip && lController.device.GetPress(SteamVR_Controller.ButtonMask.Trigger)
            || rController.canGrip && rController.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            GripStrength = Mathf.Max(GripStrength - GripDepletionFactor * Time.deltaTime, 0);
            gc.PlayerIsGripping = true;
        } else
        {
            GripStrength = Mathf.Min(GripStrength + GripRecoveryFactor * Time.deltaTime, GripStrengthMax);
            gc.PlayerIsGripping = false;
        }



        if (gc.PlayerState == GameController.PlayerStates.Injured) {
            Body.useGravity = true;
            Body.isKinematic = false;
            return;
        }

        if (GripStrength <= 0 && EnableGripMeter)
        {
            gc.PlayerInjured();
        }

        if (lController.canGrip)
        {
            movePlayer(lController);
        }
        if (rController.canGrip)
        {
            movePlayer(rController);
        }


        //don't like the below code
        if (!(lController.canGrip || rController.canGrip))
        {
            Body.useGravity = true;
            Body.isKinematic = false;


        }


        lController.prevPos = lController.transform.localPosition;
        rController.prevPos = rController.transform.localPosition;

    }

    void movePlayer(ControllerState controller)
    {

        if (controller.canGrip && controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Body.useGravity = false;
            Body.isKinematic = true;

            Body.transform.position += (controller.prevPos - controller.controller.transform.localPosition);


        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Body.useGravity = true;
            Body.isKinematic = false;

            Body.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;
        }



    }

}
