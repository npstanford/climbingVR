using UnityEngine;
using System.Collections;

public class StickPlayerToPlatform : MonoBehaviour {



	// Use this for initialization
	void Start () {
	
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    //will make top level box collider progammatically size and shape of moving platform and make it move with it
    void OnTriggerEnter(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))

        {
            /*
            Body.transform.parent = transform;
            Body.useGravity = false;
            Body.isKinematic = true;
            Body.constraints = RigidbodyConstraints.FreezePosition |
                RigidbodyConstraints.FreezeRotation;
*/
            collider.gameObject.transform.parent = transform;
            Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = false;
            rb.isKinematic = true;
            rb.constraints = RigidbodyConstraints.FreezePosition |
                RigidbodyConstraints.FreezeRotation;

        }


    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            collider.gameObject.transform.parent = null;
            Rigidbody rb = collider.gameObject.GetComponent<Rigidbody>();
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.constraints = RigidbodyConstraints.FreezeRotation;

        }
    }

}
