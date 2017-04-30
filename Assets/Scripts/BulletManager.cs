using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    public float lifetime;




	// Use this for initialization
	void Start () {
        IEnumerator bulletCoroutine = BulletLifeTime(lifetime);
        StartCoroutine(bulletCoroutine);
	}
	
	// Update is called once per frame
	void Update () {

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

            if (ia.IsPartOfBody)
            {

            }

            if (ia.CanClimb || ia.IsGround || ia.IsPartOfBody || ia.CanPickUp) //I hate this code
            {
                Destroy(this.gameObject);
            }
        }

    }



}
