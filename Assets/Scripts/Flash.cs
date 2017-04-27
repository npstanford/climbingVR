using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Flash : MonoBehaviour {

    public Color color1;
    public Color color2;
    public bool FlashEnabled;
    public float period;

    private Material mat;

    // Use this for initialization
    void Start()
    {
        mat = this.gameObject.GetComponent<MeshRenderer>().material;
    }

    // Update is called once per frame
    void Update()
    {
        if (FlashEnabled)
        {
            if (Time.time % period <= period / 2)
            {
                mat.color = Color.Lerp(color1, color2, (Time.time % period) / 2);
            }
            else
            {
                mat.color = Color.Lerp(color2, color1, (Time.time % period) / 2);
            }
        }
    }


}
