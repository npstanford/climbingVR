using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public bool IsPickingUp;
    public Collider PlayerCollider;
    public bool HasPickedUp = false;
    public float GripDepletionRate; // this is what is set
    public GameObject PlayerHead;
    public float throwingScalingFactor;

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

    public void Grab(ControllerState controller, bool grabAnyway=false, Rigidbody ToPickUp=null)
    {

        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) || grabAnyway)
        {
           // Debug.Log("Attemting to grab anyway. CanPickUp: " + controller.canPickUp);
            if (controller.canPickUp || ToPickUp!=null)
            {
                HasPickedUp = true;
                Rigidbody rb;
                if (ToPickUp != null)
                {
                    rb = ToPickUp;
                } else
                {
                    rb = controller.ObjectToPickUp;
                }
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

                if (rb.gameObject.CompareTag("Branch"))
                {
                    rb.transform.parent = controller.BranchAttachPoint.transform;
                    rb.transform.localPosition = Vector3.zero;
                    rb.transform.localRotation = Quaternion.identity;
                }

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

        //else if (controller.canPickUp && controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        else if (controller.device.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
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
            //Debug.Log("force: " + force.magnitude);
            float aimCoefficient = 0;
            if (force.magnitude > 10f)
            {
                if (!rb.gameObject.CompareTag("Branch"))
                {
                    aimCoefficient = force.magnitude / throwingScalingFactor;
                }
            }


            force = (force.normalized + PlayerHead.transform.forward * aimCoefficient).normalized * force.magnitude;
            //force = force / 4;
            rb.AddForce(force / rb.mass, ForceMode.VelocityChange);
            //idea, add extra force in the direction the player is looking
        }
    }

}
