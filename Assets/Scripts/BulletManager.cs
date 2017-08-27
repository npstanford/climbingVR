using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour
{

    public float lifetime;
    public AudioSource bulletEmitted;
    public AudioSource bulletHitsPlayer;

    private Vector3 originPosition;
    private Rigidbody rb;

    private int contactIndex;
    private Vector3[] deflectionPositions;
    private int numDeflections = 2;

    // Use this for initialization
    void Start()
    {
        IEnumerator bulletCoroutine = BulletLifeTime(lifetime);
        StartCoroutine(bulletCoroutine);
        bulletEmitted.Play();
        rb = this.GetComponent<Rigidbody>();
        originPosition = this.transform.position;
        contactIndex = 0;
        deflectionPositions = new Vector3[numDeflections];
    }

    // Update is called once per frame
    void Update()
    {

    }

    IEnumerator BulletLifeTime(float lifetime)
    {
        yield return new WaitForSeconds(lifetime);
        Destroy(this.gameObject);
    }


    private void OnTriggerEnter(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();


        if (ia != null)
        {

            if (ia.CanClimb || ia.IsGround || ia.IsPartOfBody) //I hate this code
            {
                if (ia.IsPartOfBody)
                {
                    ControllerState cs = other.GetComponent<ControllerState>();
                    if (cs !=null)
                    {
                        if (cs.Holding != null)
                        {
                            return;
                        }
                    }
                    bulletHitsPlayer.Play();
                }
                Destroy(this.gameObject);
            }



        }

    }

    private void OnTriggerStay(Collider other)
    {
        InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();


        if (ia != null && contactIndex < numDeflections)
        {
            if (ia.CanPickUp)
            {
                //idea, if we swing fast enough, this bounces back toward the PH
                //rb.velocity = (this.transform.position - other.transform.position).normalized * rb.velocity.magnitude;
                deflectionPositions[contactIndex] = other.transform.position;
                contactIndex += 1;
            }

            if (!(contactIndex < numDeflections))
            {
                Vector3 deflectionVelocity = Vector3.zero;
                for (int i = 1; i<numDeflections; i++)
                {
                    deflectionVelocity += (deflectionPositions[i] - deflectionPositions[i - 1]) / Time.deltaTime;
                }

                deflectionVelocity = deflectionVelocity / (numDeflections -1);

                //(1) confine deflected velocity within a range, (2) combine with a richoct velocity 
                deflectionVelocity = deflectionVelocity.normalized * Mathf.Clamp(deflectionVelocity.magnitude, 4f, 30f);

                float reflectionIndex;
                if (other.gameObject.CompareTag("Branch"))
                {
                    reflectionIndex= (deflectionVelocity.magnitude / 30f) * 7f;
                } else
                {
                    //if it is a stone or anything else not good for whacking
                    reflectionIndex = (deflectionVelocity.magnitude / 30f) * 2f;
                }
                    
                    
                InteractionAttributes iaBullet = this.GetComponent<InteractionAttributes>();
                iaBullet.CanPickUp = true;

                if (deflectionVelocity.magnitude < 10f)
                {
                    rb.useGravity = true;
                }


                
                ControllerState cs = other.GetComponentInParent<ControllerState>();
                if (cs!=null)
                {
                    cs.VibrateController((ushort) 1000);
                }


                rb.velocity = (deflectionVelocity.normalized + reflectionIndex*(originPosition - this.transform.position).normalized).normalized * deflectionVelocity.magnitude;
            } 

        }




    }
}
