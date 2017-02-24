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

    public void Grab(ControllerState controller, Rigidbody Body)
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


        if (controller.canGrip && controller.device.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        {
            Cling(Body);

            Body.transform.position += (controller.prevPos - controller.controller.transform.localPosition);


        }

        else if (controller.canGrip && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(Body);

            Body.velocity = (controller.prevPos - controller.controller.transform.localPosition) / Time.deltaTime / JumpDampFactor;
        }

        controller.prevPos = controller.transform.localPosition;

    }

    public void Drop(Rigidbody Body)
    {
        Body.useGravity = true;
        Body.isKinematic = false;
    }

    public void Cling(Rigidbody Body)
    {
        Body.useGravity = false;
        Body.isKinematic = true;
    }



}
