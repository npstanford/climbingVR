using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour {

    public float JumpDampFactor = 1.0f;
    public bool IsClimbing;
    public float GripDepletion;
    public bool HasClimbed = false; // for use by the tutorial system

    // Use this for initialization
    void Start () {
        IsClimbing = false;
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void Grab(ControllerState controller, Rigidbody Room)
    {


        if (controller.canGrip && controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            HasClimbed = true;
            StickPlayerToPlatform sptp = controller.GripObject.GetComponent<StickPlayerToPlatform>();
            if (sptp != null)
            {
                //Room.transform.parent = controller.GripObject.transform.parent;
                Room.transform.parent = sptp.StickyObject.transform;
            }
            Cling(Room);

            //Room.transform.localPosition += (controller.prevPos - controller.controller.transform.localPosition);
            //Room.transform.localPosition += (controller.prevPos - controller.curPos);
            Room.transform.position += transform.TransformPoint(controller.prevPos) - transform.TransformPoint(controller.curPos);
        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            //Drop(controller, Room);
           //theoretically, input manager handles this now
        }

        //controller.prevPos = controller.transform.localPosition;

    }

    public void Drop( Rigidbody Room)
    {
        //purpose for the first controller state argument is to know whether to throw the player or not
        // a better way of doing this would be to remove it and create a new method called "throw" and have input manager call that on press up
        Room.transform.parent = null;
        IsClimbing = false;
        Room.useGravity = true;
        Room.isKinematic = false;
    }

    public void ThrowPlayer(ControllerState controller, Rigidbody Room)
    {
        Room.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;

    }

    public void Cling(Rigidbody Room)
    {
        IsClimbing = true;
        Room.useGravity = false;
        Room.isKinematic = true;
    }



}
