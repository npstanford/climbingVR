using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ChargingEnemy : MonoBehaviour {

    public float chargingVelocity;
    private Vector3 ChargingDirection;
    public float rotateSpeed;
    public float AttackRadius;

    public bool IsStunned;
    public bool IsCharging;
    public bool attack;
    public GameObject Player;
    public GameObject LaserFinder;

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        Vector3 playerDirection = (Player.transform.position - this.transform.position);
        float playerDist = playerDirection.magnitude;
        attack = (playerDist < AttackRadius);
        if (attack && !IsStunned && !IsCharging)
        {

            Quaternion rot = Quaternion.LookRotation(playerDirection);
            float step = rotateSpeed * Time.deltaTime;
            transform.rotation = Quaternion.RotateTowards(transform.rotation, rot, step);
            transform.eulerAngles = new Vector3(Mathf.Clamp(transform.rotation.eulerAngles.x, -10f, 10f), 
                transform.rotation.eulerAngles.y, transform.rotation.eulerAngles.z);
   

            // if playerDirection and forward are ever within delta, then charge
            if(Vector3.Angle(transform.forward, playerDirection) < 5f)
            {
                IEnumerator ChargeCoroutine = Charge(playerDirection);
                StartCoroutine(ChargeCoroutine);
            }
        }
    }

    IEnumerator Charge(Vector3 playerDirection)
    {
        yield return new WaitForSeconds(1f);
        //Debug.Log("CHARGING!");
        IsCharging = true;
 

        ChargingDirection = new Vector3(playerDirection.x, 0, playerDirection.z);
        ChargingDirection = ChargingDirection.normalized;

        //Rigidbody rb = GetComponent<Rigidbody>();
       // rb.AddForce(ChargingDirection * chargingVelocity);

        LayerMask layerMask = (1 << 16); // layer mask against "grapple" layer
        layerMask += (1 << 2); //ignore raycast layer 
        layerMask += (1 << 9); //ignore the player's body
        layerMask += (1 << 8); //ignore the controllers
        layerMask = ~layerMask;
        RaycastHit hit;

        while (true)
        {
            if (Physics.Raycast(LaserFinder.transform.position, -LaserFinder.transform.up, out hit, 3f, layerMask))
            {
                InteractionAttributes ia = hit.collider.gameObject.GetComponent<InteractionAttributes>();
                if (ia != null)
                {
                    if (!ia.IsGround)
                    {
                        IsCharging = false;
                        //rb.velocity = Vector3.zero;
                        yield break;
                    }
                }

            }
            else
            {
                IsCharging = false;
               // rb.velocity = Vector3.zero;
                yield break;

            }
            //rb.velocity = -ChargingDirection * chargingVelocity;
            transform.position += ChargingDirection * chargingVelocity * Time.deltaTime;
            yield return null;
            
        }

        
    }


    private void OnTriggerEnter(Collider other)
    {

        if (other.gameObject.CompareTag("Player"))
        {
            //Debug.Log("GT hit: " + other.gameObject.name);
            ///other.transform.position += chargingVelocity * ChargingDirection * Time.deltaTime;
            Rigidbody rb = other.gameObject.GetComponent<Rigidbody>();


            IEnumerator StunPlayerCoroutine = StunPlayer(rb);

            StartCoroutine(StunPlayerCoroutine);
        }
    }

    IEnumerator StunPlayer(Rigidbody rb)
    {
        rb.isKinematic = false;
        yield return new WaitForSeconds(2.0f);
        rb.isKinematic = true;
    }


}
