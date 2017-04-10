using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour {

    public float JumpDampFactor = 1.0f;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Grab(ControllerState controller, Rigidbody Room)
    {

        if (controller.canGrip)
        {
            //TODO gc.PlayerIsGripping = true;
        }
        else
        {
            //TODO gc.PlayerIsGripping = false;
        }


        /* TODO
        if (gc.PlayerState == GameController.PlayerStates.Injured)
        {
            Body.useGravity = true;
            Body.isKinematic = false;
            return;
        }
        */

        /* old functuioning code
        if (controller.canGrip && controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Cling(Room);

            Room.transform.position += (controller.prevPos - controller.controller.transform.localPosition);


        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(Room);

            Room.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;
        }

        controller.prevPos = controller.transform.localPosition;

        */


        if (controller.canGrip && controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Room.transform.parent = controller.GripObject.transform.parent;
            Cling(Room);

            Room.transform.localPosition += (controller.prevPos - controller.controller.transform.localPosition);


        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(Room);
            Room.transform.parent = null;
            Room.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;
        }

        controller.prevPos = controller.transform.localPosition;

    }

    public void Drop(Rigidbody Room)
    {
        Room.useGravity = true;
        Room.isKinematic = false;
    }

    public void Cling(Rigidbody Room)
    {
        Room.useGravity = false;
        Room.isKinematic = true;
    }



}
