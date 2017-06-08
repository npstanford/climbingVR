using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public bool IsPickingUp;
    public Collider PlayerCollider;
    public bool HasPickedUp = false;
    public float GripDepletionRate; // this is what is set

    //this is only needed to enforce a state
    public ControllerState rController;
    public ControllerState lController;

    [HideInInspector]
    public float GripDepletion; // this is a function of grip depletion rate and the weight of the object

    public void Start()
    {
        IsPickingUp = false;
        HasPickedUp = false;
    }

    public void Update()
    {
        // I hate this. Basically the problem is that when the gold ball is snatched out of our hands, we never drop it.
        // Thus IsPickingUp stays true. 
        if (rController.Holding == null && lController.Holding == null)
        {
            IsPickingUp = false;
        }
    }

    public void Grab(ControllerState controller)
    {

        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (controller.canPickUp)
            {
                HasPickedUp = true;
                Rigidbody rb = controller.ObjectToPickUp;
                GripDepletion = GripDepletionRate * rb.mass;
                //begin experimental coce
                Collider[] pickUpColliders = rb.GetComponentsInChildren<Collider>();
                foreach(Collider pickUpCollider in pickUpColliders)
                {
                    if (pickUpColliders != null)
                    {
                        Physics.IgnoreCollision(pickUpCollider, PlayerCollider, true);
                    }
                }


                //end experimental code
                rb.transform.parent = controller.transform;
                controller.Holding = rb;
                rb.useGravity = false;
                rb.isKinematic = true;
                IsPickingUp = true;
            }
            else
            {
                //todo
            }
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
            Collider[] pickUpColliders = rb.GetComponentsInChildren<Collider>();
            foreach (Collider pickUpCollider in pickUpColliders)
            {
                if (pickUpColliders != null)
                {
                    Physics.IgnoreCollision(pickUpCollider, PlayerCollider, false);
                }
            }
            controller.Holding.transform.parent = null;
            controller.Holding = null;
            rb.isKinematic = false;
            rb.useGravity = true;
            IsPickingUp = false;
            Vector3 force = (controller.controller.transform.localPosition - controller.prevPos) / Time.deltaTime;
            rb.AddForce(force / rb.mass, ForceMode.VelocityChange);
        }
    }

}
