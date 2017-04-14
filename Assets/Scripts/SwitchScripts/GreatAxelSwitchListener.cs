using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GreatAxelSwitchListener : MonoBehaviour {

    public float AnglesPerHit;
    public float TimeToRotate;

    public SwitchControl[] scs;

    private int[] hits;

    // Use this for initialization
    void Start() {
        hits = new int[scs.Length];

        int i;
        for (i = 0; i < hits.Length; i++)
        {
            hits[i] = 0;
        }

        Activate();
	}
	
	// Update is called once per frame
	void Update () {
        int i;
        for (i = 0; i < hits.Length; i++)
        {
            if (scs[i].hits > hits[i])
            {

                hits[i] = scs[i].hits;
                StartCoroutine("Activate");
            }
        }

	}

    IEnumerator Activate()
    {
        float TimeElapsed = 0;
        while (TimeElapsed < TimeToRotate)
        {
            this.transform.Rotate(0.0f, (AnglesPerHit * Time.deltaTime) / TimeToRotate, 0.0f, Space.Self);
            TimeElapsed += Time.deltaTime;
            yield return null;
        }

    }
}
