using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public void Grab(ControllerState controller)
    {


        if (controller.canPickUp && controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {

            Rigidbody rb = controller.PickUpObject;

            rb.transform.parent = controller.transform;

            rb.useGravity = false;
            //rb.isKinematic = true;
            //FixedJoint j = controller.gameObject.AddComponent<FixedJoint>();
           // j.connectedBody = rb;

        }

        else if (controller.canPickUp && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            Drop(controller);
        }
    }

        public void Drop(ControllerState controller)
    {
        Rigidbody rb = controller.PickUpObject;

        if (rb != null) { 
           controller.PickUpObject.transform.parent = null;
            rb.isKinematic = false;
           // FixedJoint j = controller.gameObject.GetComponent<FixedJoint>();
           // Destroy(j);
            rb.useGravity = true;

        }
    }

}
