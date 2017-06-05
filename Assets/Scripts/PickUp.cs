using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PickUp : MonoBehaviour {

    public bool IsPickingUp;
    public Collider PlayerCollider;

    public void Start()
    {
        IsPickingUp = false;
    }

    public void Grab(ControllerState controller)
    {

        if (controller.device.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
        {
            if (controller.canPickUp)
            {
                
                Rigidbody rb = controller.ObjectToPickUp;
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
                //FixedJoint j = controller.gameObject.AddComponent<FixedJoint>();
                // j.connectedBody = rb;
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
            /*
            IEnumerator DelayCoroutine = DelayReturnCollisionDetection(rb);
            // begin experimental code
            StartCoroutine(DelayCoroutine);
            //end experimental code
            */

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
           // FixedJoint j = controller.gameObject.GetComponent<FixedJoint>();
           // Destroy(j);
            rb.useGravity = true;
            IsPickingUp = false;
            Vector3 force = (controller.controller.transform.localPosition - controller.prevPos) / Time.deltaTime;
            rb.AddForce(force / rb.mass, ForceMode.VelocityChange);
        }
    }

    /*
    IEnumerator DelayReturnCollisionDetection(Rigidbody rb)
    {
        yield return new WaitForSeconds(.1f);
        Collider[] pickUpColliders = rb.GetComponentsInChildren<Collider>();
        foreach (Collider pickUpCollider in pickUpColliders)
        {
            if (pickUpColliders != null)
            {
                Physics.IgnoreCollision(pickUpCollider, PlayerCollider, false);
            }
        }
    }
    */
}
