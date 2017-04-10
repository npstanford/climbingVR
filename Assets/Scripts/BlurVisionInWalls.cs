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


    void OnTriggerStay(Collider other)
    {
        Debug.Log("Head entered: " + other.gameObject.name);
        //inefficent hacky call just to see if thsi works.
        //it doesn't work... I think it is because probuilder uses fancy colliders that don't count the inside 
        if (other.bounds.Intersects(this.GetComponent<Collider>().bounds))
        {
            InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
            if (ia != null)
            {
                if (!ia.IsPartOfBody)
                {
                    BlurVision();
                }
            }

        }
    }
    void OnTriggerExit()
    {
        UnBlurVision();
    }

}
