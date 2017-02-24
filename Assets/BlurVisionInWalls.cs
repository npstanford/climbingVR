using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlurVisionInWalls : MonoBehaviour {
    public Image HeadBlur;
    public Color flashColor = new Color(0f, 0f, 0f, 1f);


    // Use this for initialization
    void Start () {
    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void BlurVision()
    {
        HeadBlur.color = flashColor;
    }

    void UnBlurVision()
    {
        HeadBlur.color = Color.clear;
    }

    void OnTriggerEnter()
    {
        BlurVision();
    }

    void OnTriggerExit()
    {
        UnBlurVision();
    }
}
