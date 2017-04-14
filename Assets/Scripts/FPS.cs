using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class FPS : MonoBehaviour {

    public Text FPSdisplay;

    public float[] Times;
    public int i;

	// Use this for initialization
	void Start () {
        Times = new float[100];
        i = 0;
        for (i=0; i<=99; i++)
        {
            Times[i] = 0.0f;
        }

        i = 0;
	}
	
	// Update is called once per frame
	void Update () {

        Times[i] = Time.deltaTime;

        if (i == 99)
        {
            i = 0;
        }

        if (i == 0)
        {
            float avg = 0;
            foreach (float t in Times)
            {
                avg += t;
            }

            avg = avg / 100;
            avg = 1 / avg;

            FPSdisplay.text = avg.ToString("F0");
        }

        i += 1;
    }
}
