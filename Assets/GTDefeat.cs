using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;   

public class GTDefeat : MonoBehaviour {
    public Text winScreen;

	// Use this for initialization
	void Start () {
        winScreen.text = "";
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void OnTriggerEnter(Collider other)
    {
        ChargingEnemy ce = other.gameObject.GetComponent<ChargingEnemy>();
        if (ce!=null)
        {
            winScreen.text = "You Win..!";
        }
    }

}
