using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public bool IsPickingUp;

    public void Start()
    {
        IsPickingUp = false;
    }

    public void Grab(ControllerState controller)
    {


        if (controller.canPickUp && controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {

            Rigidbody rb = controller.ObjectToPickUp;

            rb.transform.parent = controller.transform;
            controller.Holding = rb;
            rb.useGravity = false;
            rb.isKinematic = true;
            //FixedJoint j = controller.gameObject.AddComponent<FixedJoint>();
            // j.connectedBody = rb;
            IsPickingUp = true;
        }

        else if (controller.canPickUp && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(controller);
        }
    }

        public void Drop(ControllerState controller)
    {
        Rigidbody rb = controller.Holding;

        if (rb != null) { 
           controller.Holding.transform.parent = null;
            controller.Holding = null;
            rb.isKinematic = false;
           // FixedJoint j = controller.gameObject.GetComponent<FixedJoint>();
           // Destroy(j);
            rb.useGravity = true;
            IsPickingUp = false;
            Vector3 force = (controller.controller.transform.localPosition - controller.prevPos) / Time.deltaTime;
            rb.AddForce(force / rb.mass, ForceMode.VelocityChange);
            Debug.Log("Force: " + force);
        }
    }

}
