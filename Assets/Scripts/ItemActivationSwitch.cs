using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemActivationSwitch : MonoBehaviour {



    public InputManager im;
    public InputManager.Capabilities ActivateWhat;
    public Color color1 = Color.white;
    public Color color2 = Color.red;

    private Material mat;

	// Use this for initialization
	void Start () {
        mat = this.gameObject.GetComponent<MeshRenderer>().material;
	}
	
	// Update is called once per frame
	void Update () {
        if (Time.time % 2 <= 1) {
            mat.color = Color.Lerp(color1, color2, (Time.time % 2) / 2);
        } else
        {
            mat.color = Color.Lerp(color2, color1, (Time.time % 2) / 2);
        }
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Controller"))
        {
            im.EnableCapability(ActivateWhat);
            this.gameObject.SetActive(false);

        }
    }


}
