using UnityEngine;
using System.Collections;

public class Teleporter : MonoBehaviour
{

    public TeleportLocation LastTeleportLocation;
    public GameObject Player;

    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Teleport()
    {
        if (LastTeleportLocation != null)
        {
            Debug.Log("player was teleported");
            Player.transform.position = LastTeleportLocation.transform.position;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
        {
            Debug.Log("this object is calling teleport: " + this.name);
            Teleport();
        }
    }


}
