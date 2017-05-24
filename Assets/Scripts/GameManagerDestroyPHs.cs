using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameManagerDestroyPHs : MonoBehaviour {

    
    public int DestroyedEnemies;

	// Use this for initialization
	void Start () {
	}
	
	// Update is called once per frame
	void Update () {
        
	}

    private void OnTriggerEnter(Collider other)
    {
        Enemy e = other.gameObject.GetComponentInParent<Enemy>();
        if (e != null)
        {
            DestroyedEnemies += 1;
            GameObject.Destroy(e.gameObject);
        }
    }



}
