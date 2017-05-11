using UnityEngine;
using System.Collections;

public class TeleportLocation : MonoBehaviour {

    private Teleporter teleporter;

	// Use this for initialization
	void Start () {
        GetComponent<MeshRenderer>().enabled = false;
        GameObject go = GameObject.FindGameObjectWithTag("Teleporter");
        teleporter = go.GetComponent<Teleporter>();
	}

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            teleporter.LastTeleportLocation = this;
        }
    }


}
