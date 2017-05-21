using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class BlurVisionInWalls : MonoBehaviour {


    public Image HeadBlur;
    [HideInInspector]
    public bool HeadInWall = false;
    public Color flashColor = new Color(0f, 0f, 0f, .5f);


    // Use this for initialization
    void Start () {

    }
	
	// Update is called once per frame
	void Update () {
		
	}

    void BlurVision()
    {
        if (HeadBlur != null)
        {
            HeadBlur.color = flashColor;
        }
    }

    void UnBlurVision()
    {
        if (HeadBlur != null)
        {
            HeadBlur.color = Color.clear;
        }
    }


    void OnTriggerStay(Collider other)
    {
        //inefficent hacky call just to see if thsi works.
        //it doesn't work... I think it is because probuilder uses fancy colliders that don't count the inside 
        if (other.bounds.Intersects(this.GetComponent<Collider>().bounds))
        {
            InteractionAttributes ia = other.gameObject.GetComponent<InteractionAttributes>();
            if (ia != null)
            {
                if (!ia.IsPartOfBody && !ia.HurtsPlayer)
                {
                    BlurVision();
                    HeadInWall = true;

                }
            }

        }
    }
    void OnTriggerExit()
    {
        UnBlurVision();
        HeadInWall = false;
    }

}
