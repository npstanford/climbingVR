using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletManager : MonoBehaviour {

    public float lifetime;
    public AudioSource bulletEmitted;
    public AudioSource bulletHitsPlayer;



	// Use this for initialization
	void Start () {
        IEnumerator bulletCoroutine = BulletLifeTime(lifetime);
        StartCoroutine(bulletCoroutine);
        bulletEmitted.Play();
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

            if (ia.CanClimb || ia.IsGround || ia.IsPartOfBody) //I hate this code
            {
                if (ia.IsPartOfBody)
                {
                    bulletHitsPlayer.Play();
                }
                Destroy(this.gameObject);
            }


            if (ia.CanPickUp)
            {

            }
        }

    }



}
