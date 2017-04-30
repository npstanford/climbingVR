using UnityEngine;
using System.Collections;

public class StickPlayerToPlatform : MonoBehaviour {

    public GameObject StickyObject;
    public Rigidbody rb;

    public bool OnPlatform;
    public GameObject ObjectToStick;

	// Use this for initialization
	void Start () {
        OnPlatform = false;
	}
	
	// Update is called once per frame
	void Update () {
	
	}

    private void FixedUpdate()
    {
        if (OnPlatform)
        {

        }
    }


    void OnTriggerStay(Collider collider)
    {
    if (collider.gameObject.CompareTag("Player"))
    {
        OnPlatform = true;
        ObjectToStick = collider.gameObject;

            if (rb != null)
            {
                if (!rb.gameObject.Equals(ObjectToStick))
                {
                    rb = ObjectToStick.GetComponent<Rigidbody>();
                }
            }
            else
            {
                rb = ObjectToStick.gameObject.GetComponent<Rigidbody>();
            }




            if (StickyObject != null)
            {
                if (ObjectToStick.gameObject.transform.parent != StickyObject.transform)
                {
                    ObjectToStick.gameObject.transform.parent = StickyObject.transform;

                }
            }
            else
            {
                if (ObjectToStick.gameObject.transform.parent != transform)
                {
                    ObjectToStick.gameObject.transform.parent = transform;


                }

            }



            if (rb.useGravity || !rb.isKinematic )
            {
                Debug.Log("sticky platform turned off gravity");
                //rb.useGravity = false;
               // rb.isKinematic = true;
            }
        }
    



    }

    void OnTriggerExit(Collider collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            OnPlatform = false;


            Debug.Log("Player off paltform");

            collider.gameObject.transform.parent = null;
 

            //rb.useGravity = true;
            //rb.isKinematic = false;

        }


    }


}
