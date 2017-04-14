using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SwitchControl : MonoBehaviour {

    public enum SwitchType {OneHit, OnAndOff, MultiHit };

    public SwitchType st;


    public int hits;
    public bool activated;

    public Color OffColor;
    public Color OnColor;

    private Material SwitchSurface;

	// Use this for initialization
	void Start () {
        hits = 0;
        SwitchSurface = this.GetComponent<MeshRenderer>().material;
        SwitchSurface.color = OffColor;

	}
	
	// Update is called once per frame
	void Update () {
		if (hits == 0)
        {
            if (Time.time % 2 < 1)
            {
                SwitchSurface.color = OffColor;
            }
            else
            {
                SwitchSurface.color = new Color(OnColor.r, OnColor.g, OnColor.b, .6f);
            }
        }

	}

    public void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Controller") || other.gameObject.CompareTag("Grapple"))
        {
            hits += 1;
            if (st == SwitchType.OneHit)
            {
                activated = true;
            }
            else if (st == SwitchType.OnAndOff)
            {
                activated = !activated;
            }

            if (st != SwitchType.MultiHit)
            {
                if (activated)
                {
                    SwitchSurface.color = OnColor;


                }
                else
                {
                    SwitchSurface.color = OffColor;
                }
            } else
            {

                StopCoroutine("Animate");
                StartCoroutine("Animate");
            }
        }

    }

    IEnumerator Animate()
    {
        //maybe should have a "Flash" color to indicate it has been hit again.
        SwitchSurface.color = OnColor;
        yield return new WaitForSeconds(5.0f);
        SwitchSurface.color = OffColor;

    }

}
