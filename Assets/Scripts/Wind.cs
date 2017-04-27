using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Wind : MonoBehaviour {

    public Vector3 WindVelocity;
    public float WindSpeed;
    public bool WindEnabled = true;
    public Collider windCollider;
    public ParticleSystem ps;

    private List<GameObject> ObjectsBeingBlownByWind;
    //todo, at some time in the future... make a list of objects as they enter the wind and on updates add a force to them


	// Use this for initialization
	void Start () {
		if (WindEnabled)
        {
            TurnOnWind();
        } else
        {
            TurnOffWind();
        }
	}
	
	// Update is called once per frame
	void Update () {
        WindVelocity = transform.up * WindSpeed;
        
    }

    public void TurnOnWind()
    {
        WindEnabled = true;
        windCollider.enabled = true;
        if (!ps.isPlaying) { ps.Play(); }
    }

    public void TurnOffWind()
    {
        WindEnabled = false;
        windCollider.enabled = false;
        if (ps.isPlaying) { ps.Stop(); }
    }


    public void OnTriggerEnter(Collider other)
    {
        //Debug.Log("Wind blowing on: " + other.gameObject.name);
       InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (ia != null && rb!=null) {
            if (ia.AffectedByWind)
            {
                Debug.Log("Wind is blowing: " + other.gameObject.name);
                //other.transform.position += WindVelocity * Time.deltaTime;
                rb.AddForce(WindVelocity, ForceMode.Impulse);
            }

       } 
    }

    public void OnTriggerExit(Collider other)
    {
        InteractionAttributes ia = other.GetComponent<InteractionAttributes>();
        Rigidbody rb = other.GetComponent<Rigidbody>();
        if (ia != null && rb != null)
        {
            if (ia.AffectedByWind)
            {
                //other.transform.position += WindVelocity * Time.deltaTime;
                rb.velocity = Vector3.zero;
            }

        }
    }

}
