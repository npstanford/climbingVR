using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Climb : MonoBehaviour {

    public float JumpDampFactor = 1.0f;
    public bool IsClimbing;
    public float GripDepletion;


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
            if (controller.GripObject.GetComponent<StickPlayerToPlatform>())
            {
                Room.transform.parent = controller.GripObject.transform.parent;
            }
            Cling(Room);

            //Room.transform.localPosition += (controller.prevPos - controller.controller.transform.localPosition);
            Room.transform.localPosition += (controller.prevPos - controller.curPos);

        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(Room);
            Room.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;
        }

        //controller.prevPos = controller.transform.localPosition;

    }

    public void Drop(Rigidbody Room)
    {
        Room.transform.parent = null;
        IsClimbing = false;
        //Room.useGravity = true;
       // Room.isKinematic = false;
    }

    public void Cling(Rigidbody Room)
    {
        IsClimbing = true;
        //Room.useGravity = false;
       // Room.isKinematic = true;
    }



}
